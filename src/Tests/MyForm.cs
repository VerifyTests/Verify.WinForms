// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantNameQualifier
// ReSharper disable ArrangeThisQualifier
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable PartialTypeWithSinglePart
#pragma warning disable 8618

namespace WindowsFormsApp1;

public partial class MyForm : Form
{
    private Button button1;

    public MyForm() =>
        InitializeComponent();

    private void InitializeComponent()
    {
        this.button1 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // button1
        //
        this.button1.Location = new System.Drawing.Point(82, 106);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(75, 23);
        this.button1.TabIndex = 0;
        this.button1.Text = "button1";
        this.button1.UseVisualStyleBackColor = true;
        //
        // MyForm
        //
        this.ClientSize = new System.Drawing.Size(278, 244);
        this.Controls.Add(this.button1);
        this.Name = "MyForm";
        this.ResumeLayout(false);
    }
}