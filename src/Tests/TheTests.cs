using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class TheTests :
    VerifyBase
{
    #region FormUsage
    [Fact]
    public Task FormUsage()
    {
        return Verify(new MyForm());
    }
    #endregion

    #region UserControlUsage
    [Fact]
    public Task UserControlUsage()
    {
        return Verify(new MyUserControl());
    }
    #endregion

    #region ControlUsage
    [Fact]
    public Task ControlUsage()
    {
        return Verify(
            new Button
            {
                BackColor = Color.LightBlue,
                Text = "Help"
            });
    }
    #endregion

    public TheTests(ITestOutputHelper output) :
        base(output)
    {
        SharedVerifySettings.UniqueForRuntime();
    }

    static TheTests()
    {
        VerifyPhash.RegisterComparer("png", .99f);
    }
}