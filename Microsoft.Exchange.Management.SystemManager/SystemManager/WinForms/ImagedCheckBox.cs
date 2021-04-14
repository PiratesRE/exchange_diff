using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ImagedCheckBox : ExchangeUserControl
	{
		public ImagedCheckBox()
		{
			this.InitializeComponent();
			this.checkBox.CheckedChanged += delegate(object sender, EventArgs e)
			{
				this.OnCheckedChanged(e);
			};
		}

		private void InitializeComponent()
		{
			this.pictureBox = new ExchangePictureBox();
			this.checkBox = new AutoHeightCheckBox();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			((ISupportInitialize)this.pictureBox).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.pictureBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.pictureBox.Location = new Point(3, 0);
			this.pictureBox.Margin = new Padding(3, 0, 0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new Size(16, 16);
			this.pictureBox.TabIndex = 1;
			this.pictureBox.TabStop = false;
			this.checkBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.checkBox.CheckAlign = ContentAlignment.MiddleLeft;
			this.checkBox.Location = new Point(22, 1);
			this.checkBox.Margin = new Padding(3, 0, 0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new Size(300, 14);
			this.checkBox.TabIndex = 2;
			this.checkBox.TextAlign = ContentAlignment.MiddleLeft;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 19f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.pictureBox, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.checkBox, 1, 0);
			this.tableLayoutPanel1.Dock = DockStyle.Top;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Margin = new Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(322, 16);
			this.tableLayoutPanel1.TabIndex = 3;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "ImagedCheckBox";
			base.Size = new Size(322, 17);
			((ISupportInitialize)this.pictureBox).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected CheckBox CheckBoxControl
		{
			get
			{
				return this.checkBox;
			}
		}

		protected PictureBox PictureBoxControl
		{
			get
			{
				return this.pictureBox;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(null)]
		public Image Image
		{
			get
			{
				return this.PictureBoxControl.Image;
			}
			set
			{
				this.PictureBoxControl.Image = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue("")]
		[Browsable(true)]
		public string Description
		{
			get
			{
				return this.CheckBoxControl.Text;
			}
			set
			{
				if (value != this.Description)
				{
					this.CheckBoxControl.Text = (value ?? string.Empty);
				}
			}
		}

		[DefaultValue(false)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool Checked
		{
			get
			{
				return this.CheckBoxControl.Checked;
			}
			set
			{
				this.CheckBoxControl.Checked = value;
			}
		}

		public event EventHandler CheckedChanged;

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (this.CheckedChanged != null)
			{
				this.CheckedChanged(this, e);
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return new Size(proposedSize.Width, this.checkBox.Height);
		}

		private ExchangePictureBox pictureBox;

		private TableLayoutPanel tableLayoutPanel1;

		private AutoHeightCheckBox checkBox;
	}
}
