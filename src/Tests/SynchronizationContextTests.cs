using System.Drawing.Imaging;
using System.Runtime.InteropServices;

[TestFixture]
public class SynchronizationContextTests
{
    [Test]
    public async Task DoesNotDeadlockUnderPumplessSynchronizationContext()
    {
        var original = SynchronizationContext.Current;

        // Render a large, high-entropy image so the received-image write performs genuinely async IO.
        // Small control images fit the write buffer and flush synchronously, which would not exercise
        // the bug (see the WinForms sample controls, whose snapshots are only a few KB).
        using var control = new Control
        {
            Width = 256,
            Height = 256
        };
        using var bitmap = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
        var data = bitmap.LockBits(new(0, 0, 256, 256), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        var bytes = new byte[data.Stride * data.Height];
        new Random(1).NextBytes(bytes);
        Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
        bitmap.UnlockBits(data);
        control.BackgroundImage = bitmap;

        // A UI SynchronizationContext with no running message pump, as exists during a test. If the
        // converter leaks the context it installs while rendering (or the pipeline captures the
        // caller's), the received-image write's continuation is posted to a pump that never runs and
        // the pipeline - and this test - would hang.
        SynchronizationContext.SetSynchronizationContext(new PumplessSynchronizationContext());
        Task task;
        try
        {
            task = Verify(control)
                .DisableDiff()
                .ToTask();
        }
        finally
        {
            // Restore so this test's own awaits below are not captured by the pumpless context.
            SynchronizationContext.SetSynchronizationContext(original);
        }

        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10)));

        Assert.That(completed, Is.SameAs(task), "WinForms Verify deadlocked under a SynchronizationContext with no message pump.");

        // The new snapshot should fault (VerifyException); its type is internal to Verify, so just observe the fault.
        Exception? exception = null;
        try
        {
            await task;
        }
        catch (Exception e)
        {
            exception = e;
        }

        Assert.That(exception, Is.Not.Null, "Expected the new snapshot to throw.");
    }

    class PumplessSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback callback, object? state)
        {
            // No message pump: the continuation is queued but never dispatched.
        }
    }
}
