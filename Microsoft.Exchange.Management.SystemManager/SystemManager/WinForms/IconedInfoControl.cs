using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class IconedInfoControl : ExchangeUserControl
	{
		public IconedInfoControl()
		{
			this.InitializeComponent();
		}

		public IconedInfoControl(Icon icon, string description) : this()
		{
			this.pictureBox.Image = IconLibrary.ToBitmap(icon, this.pictureBox.Size);
			this.infoLabel.Text = description;
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		public override string Text
		{
			get
			{
				return this.infoLabel.Text;
			}
			set
			{
				this.infoLabel.Text = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue(null)]
		public Image IconImage
		{
			get
			{
				return this.pictureBox.Image;
			}
			set
			{
				this.pictureBox.Image = value;
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new TableLayoutPanel();
			this.pictureBox = new ExchangePictureBox();
			this.infoLabel = new Label();
			this.tableLayoutPanel.SuspendLayout();
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.infoLabel, 1, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(644, 22);
			this.tableLayoutPanel.TabIndex = 0;
			this.pictureBox.Dock = DockStyle.Top;
			this.pictureBox.Location = new Point(3, 3);
			this.pictureBox.MinimumSize = new Size(16, 16);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new Size(16, 16);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.infoLabel.AutoSize = true;
			this.infoLabel.Dock = DockStyle.Top;
			this.infoLabel.ImageAlign = ContentAlignment.TopLeft;
			this.infoLabel.Location = new Point(20, 3);
			this.infoLabel.Margin = new Padding(0, 3, 0, 0);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new Size(624, 13);
			this.infoLabel.TabIndex = 1;
			this.infoLabel.Text = "infoLabel";
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "IconedInfoControl";
			base.Size = new Size(644, 22);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private ExchangePictureBox pictureBox;

		private Label infoLabel;

		private TableLayoutPanel tableLayoutPanel;
	}
}
