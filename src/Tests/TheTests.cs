using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;
using Verify;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class TheTests
{
    #region FormUsage
    [Test]
    public Task FormUsage()
    {
        return Verifier.Verify(new MyForm());
    }
    #endregion

    #region UserControlUsage
    [Test]
    public Task UserControlUsage()
    {
        return Verifier.Verify(new MyUserControl());
    }
    #endregion

    #region ControlUsage
    [Test]
    public Task ControlUsage()
    {
        return Verifier.Verify(
            new Button
            {
                BackColor = Color.LightBlue,
                Text = "Help"
            });
    }
    #endregion

    static TheTests()
    {
        #region Enable
        VerifyWinForms.Enable();
        #endregion
        SharedVerifySettings.UniqueForRuntime();
        VerifyPhash.RegisterComparer("png", .99f);
    }
}