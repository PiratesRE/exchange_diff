using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultPaneCaption : ExchangeUserControl
	{
		public ResultPaneCaption()
		{
			this.InitializeComponent();
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(387, 22);
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.UpdateFonts();
		}

		private void UpdateFonts()
		{
			base.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			if (this.BaseFont != null)
			{
				this.labelDescription.Font = new Font(this.BaseFont.FontFamily, this.BaseFont.Size * FontHelper.GetScaleFactor(), this.BaseFont.Style);
				this.labelStatus.Font = new Font(this.labelDescription.Font.FontFamily, this.labelDescription.Font.Size);
			}
			else
			{
				this.labelDescription.Font = null;
				this.labelStatus.Font = null;
			}
			this.tableLayoutPanel.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		[DefaultValue(null)]
		public Font BaseFont
		{
			get
			{
				return this.baseFont;
			}
			set
			{
				if (this.baseFont != value)
				{
					this.baseFont = value;
					this.UpdateFonts();
				}
			}
		}

		[Obsolete("Use BaseFont instead.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Font Font
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[DefaultValue(null)]
		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.Icon != value)
				{
					Bitmap image = IconLibrary.ToBitmap(value, this.pictureBox.Size);
					if (this.pictureBox.Image != null)
					{
						this.pictureBox.Image.Dispose();
					}
					this.pictureBox.Image = image;
					this.icon = value;
				}
			}
		}

		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				this.labelDescription.Text = value;
				base.Text = value;
			}
		}

		[DefaultValue("")]
		public string Status
		{
			get
			{
				return this.labelStatus.Text;
			}
			set
			{
				this.labelStatus.Text = value;
			}
		}

		private void InitializeComponent()
		{
			this.labelStatus = new Label();
			this.labelDescription = new Label();
			this.pictureBox = new ExchangePictureBox();
			this.tableLayoutPanel = new TableLayoutPanel();
			((ISupportInitialize)this.pictureBox).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.labelStatus.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.labelStatus.AutoSize = true;
			this.labelStatus.Location = new Point(384, 4);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new Size(1, 13);
			this.labelStatus.TabIndex = 2;
			this.labelStatus.TextAlign = ContentAlignment.MiddleRight;
			this.labelStatus.UseMnemonic = false;
			this.labelDescription.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.labelDescription.AutoEllipsis = true;
			this.labelDescription.ImageAlign = ContentAlignment.MiddleLeft;
			this.labelDescription.Location = new Point(22, 0);
			this.labelDescription.Margin = new Padding(0);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new Size(359, 22);
			this.labelDescription.TabIndex = 1;
			this.labelDescription.TextAlign = ContentAlignment.MiddleLeft;
			this.labelDescription.UseMnemonic = false;
			this.pictureBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.pictureBox.Location = new Point(3, 3);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new Size(16, 16);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.pictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.labelDescription, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.labelStatus, 2, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Size = new Size(387, 22);
			this.tableLayoutPanel.TabIndex = 3;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ResultPaneCaption";
			base.Size = new Size(387, 22);
			((ISupportInitialize)this.pictureBox).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
		}

		private Label labelStatus;

		private Label labelDescription;

		private ExchangePictureBox pictureBox;

		private TableLayoutPanel tableLayoutPanel;

		private Font baseFont;

		private Icon icon;
	}
}
