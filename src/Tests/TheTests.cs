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
        return Verify(menu)
                .AutoVerify();
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
}