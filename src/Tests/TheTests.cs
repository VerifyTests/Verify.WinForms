using WindowsFormsApp1;

[TestFixture]
public class TheTests
{
    #region FormUsage

    [Test]
    public Task FormUsage() =>
        Verify(new MyForm());

    #endregion

    #region UserControlUsage

    [Test]
    public Task UserControlUsage() =>
        Verify(new MyUserControl());

    #endregion

    #region ContextMenuStrip

    [Test]
    public Task ContextMenuStrip()
    {
        var menu = new ContextMenuStrip();
        var items = menu.Items;

        items.Add(new ToolStripMenuItem("About"));
        items.Add(new ToolStripMenuItem("Exit"));
        //AutoVerify CI renders differently
        var settings = new VerifySettings();
        settings.AutoVerify();
        return Verify(menu, settings);
    }

    #endregion

    #region ControlUsage

    [Test]
    public Task ControlUsage() =>
        Verify(
            new Button
            {
                BackColor = Color.LightBlue,
                Text = "Help"
            });

    #endregion

    static TheTests()
    {
        #region Enable

        VerifyWinForms.Enable();

        #endregion

        VerifierSettings.UniqueForRuntime();
        VerifyPhash.RegisterComparer("png", .99f);
    }
}