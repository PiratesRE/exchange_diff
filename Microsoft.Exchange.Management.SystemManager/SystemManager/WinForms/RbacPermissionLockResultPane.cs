using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class RbacPermissionLockResultPane : ResultPane
	{
		public RbacPermissionLockResultPane()
		{
			base.ViewModeCommands.Add(Theme.VisualEffectsCommands);
			base.EnableVisualEffects = true;
			this.InitializeComponent();
			this.titleImage.Image = IconLibrary.ToBitmap(Icons.LockIcon, this.titleImage.Size);
			this.labelTitle.Text = Strings.NotPermittedByRbac;
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.labelTitle = new Label();
			this.titleImage = new Label();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.BackColor = Color.Transparent;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.labelTitle, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.titleImage, 0, 0);
			this.tableLayoutPanel1.Dock = DockStyle.Top;
			this.tableLayoutPanel1.Location = new Point(12, 5);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new Padding(0, 0, 0, 12);
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(126, 60);
			this.tableLayoutPanel1.TabIndex = 2;
			this.labelTitle.AutoSize = true;
			this.labelTitle.Dock = DockStyle.Fill;
			this.labelTitle.Location = new Point(57, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new Size(66, 48);
			this.labelTitle.TabIndex = 1;
			this.labelTitle.TextAlign = ContentAlignment.MiddleLeft;
			this.titleImage.Location = new Point(3, 0);
			this.titleImage.Name = "titleImage";
			this.titleImage.Size = new Size(32, 32);
			this.titleImage.TabIndex = 2;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "RbacPermissionLockResultPane";
			base.Padding = new Padding(12, 5, 12, 5);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public override string SelectionHelpTopic
		{
			get
			{
				return null;
			}
		}

		private TableLayoutPanel tableLayoutPanel1;

		private Label titleImage;

		private Label labelTitle;
	}
}
