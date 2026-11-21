using WindowsFormsApp1;

public class TheTests
{
    #region FormUsage

    [Test]
    public Task FormUsage() =>
        Verify(new MyForm());

    #endregion

    // A form is shown top-level, off screen and without being activated, so its Shown handlers
    // have run when it is drawn, and it is hidden again after.
    [Test]
    public async Task FormIsShownOffScreenWithoutActivation()
    {
        var form = new Form
        {
            Width = 200,
            Height = 100,
            Text = "Shown"
        };
        var label = new Label
        {
            Text = "Not shown",
            AutoSize = true
        };
        form.Controls.Add(label);
        var activated = false;
        var locations = new List<Point>();
        form.Activated += (_, _) => activated = true;
        form.Shown += (_, _) =>
        {
            label.Text = "Shown";
            locations.Add(form.Location);
        };
        form.Paint += (_, _) => locations.Add(form.Location);

        await Verify(form);

        await Assert.That(activated).IsFalse();
        await Assert.That(locations).IsNotEmpty();
        await Assert.That(locations.Where(_ => _ != new Point(-20000, -20000))).IsEmpty();
        await Assert.That(form.Visible).IsFalse();
        await Assert.That(form.TopLevel).IsTrue();
    }

    // A form the test has already shown is drawn where it is, and left shown.
    [Test]
    public async Task AlreadyShownForm()
    {
        using var form = new QuietForm
        {
            Width = 200,
            Height = 100,
            Text = "Visible",
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Location = new(-20000, -20000)
        };
        form.Show();

        await Verify(form);

        await Assert.That(form.Visible).IsTrue();
        await Assert.That(form.TopLevel).IsTrue();
    }

    class QuietForm : Form
    {
        protected override bool ShowWithoutActivation => true;
    }

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