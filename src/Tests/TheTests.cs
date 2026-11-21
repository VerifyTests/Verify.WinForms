using WindowsFormsApp1;

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
        return Verify(menu);
    }

    #endregion

    #region ToolStripDropDownMenu

    [Test]
    public Task ToolStripDropDownMenu()
    {
        var button = new ToolStripDropDownButton("File");
        var items = button.DropDownItems;

        items.Add(new ToolStripMenuItem("New"));
        items.Add(
            new ToolStripMenuItem("Word Wrap")
            {
                Checked = true
            });
        items.Add(new ToolStripSeparator());
        items.Add(new ToolStripMenuItem("Exit"));
        return Verify(button.DropDown);
    }

    #endregion

    #region ToolStripDropDown

    [Test]
    public Task ToolStripDropDown()
    {
        var dropDown = new ToolStripDropDown();
        var items = dropDown.Items;

        items.Add(new ToolStripLabel("Zoom"));
        items.Add(new ToolStripButton("100%"));
        items.Add(new ToolStripButton("200%"));
        return Verify(dropDown);
    }

    #endregion

    // WinForms keeps a shown drop-down on screen, and places one that belongs to an item by that
    // item. It should still be off screen the whole time it is shown, so it never flashes on
    // screen and the mouse cannot be over it.
    [Test]
    public async Task DropDownIsShownOffScreen()
    {
        var button = new ToolStripDropDownButton("File");
        button.DropDownItems.Add(new ToolStripMenuItem("Open"));
        var dropDown = button.DropDown;
        var locations = new List<Point>();
        dropDown.VisibleChanged += (_, _) => locations.Add(dropDown.Location);
        dropDown.Paint += (_, _) => locations.Add(dropDown.Location);

        await Verify(dropDown);

        await Assert.That(locations).IsNotEmpty();
        await Assert.That(locations.Where(_ => _ != new Point(-20000, -20000))).IsEmpty();
    }

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