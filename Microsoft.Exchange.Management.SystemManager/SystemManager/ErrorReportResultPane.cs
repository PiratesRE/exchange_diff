using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ErrorReportResultPane : ContentResultPane
	{
		public ErrorReportResultPane()
		{
			base.ViewModeCommands.Add(Theme.VisualEffectsCommands);
			base.EnableVisualEffects = true;
			this.InitializeComponent();
			this.LabelTitle = Strings.WelcomeToESM;
			this.UpdateTitleFont();
			this.titleImage.Image = IconLibrary.ToBitmap(Icons.Error, this.titleImage.Size);
			this.contentLabel.LinkClicked += this.contentLabel_LinkClicked;
		}

		private void contentLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string text = e.Link.LinkData as string;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					WinformsHelper.OpenUrl(new Uri(text));
				}
				catch (UrlHandlerNotFoundException ex)
				{
					base.ShowError(ex.Message);
				}
				catch (UriFormatException)
				{
				}
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.contentLabel.Text;
			}
			set
			{
				this.contentLabel.Text = value;
			}
		}

		private void UpdateContentLabelWithErrors()
		{
			string empty = string.Empty;
			this.contentLabel.Text = empty;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.UpdateTitleFont();
		}

		private void UpdateTitleFont()
		{
			if (this.labelTitle != null)
			{
				Font defaultFont = FontHelper.GetDefaultFont();
				this.labelTitle.Font = new Font(defaultFont.FontFamily, defaultFont.SizeInPoints + 2f, FontStyle.Bold);
			}
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.labelTitle = new Label();
			this.titleImage = new Label();
			this.contentTableLayoutPanel = new TableLayoutPanel();
			this.contentLabel = new LinkLabelCommand();
			this.tableLayoutPanel1.SuspendLayout();
			this.contentTableLayoutPanel.SuspendLayout();
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
			this.titleImage.Size = new Size(48, 48);
			this.titleImage.TabIndex = 2;
			this.contentTableLayoutPanel.AutoSize = true;
			this.contentTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.contentTableLayoutPanel.BackColor = Color.Transparent;
			this.contentTableLayoutPanel.Dock = DockStyle.Top;
			this.contentTableLayoutPanel.ColumnCount = 1;
			this.contentTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.contentTableLayoutPanel.Controls.Add(this.contentLabel, 0, 0);
			this.contentTableLayoutPanel.Padding = new Padding(0, 0, 0, 0);
			this.contentTableLayoutPanel.Location = new Point(16, 91);
			this.contentTableLayoutPanel.Name = "contentTableLayoutPanel";
			this.contentTableLayoutPanel.RowCount = 1;
			this.contentTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.contentTableLayoutPanel.Size = new Size(126, 50);
			this.contentTableLayoutPanel.TabIndex = 3;
			this.contentLabel.AutoSize = true;
			this.contentLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.contentLabel.Location = new Point(3, 0);
			this.contentLabel.Name = "contentLabel";
			this.contentLabel.Size = new Size(55, 13);
			this.contentLabel.TabIndex = 0;
			this.contentLabel.TabStop = true;
			this.contentLabel.Text = "linkLabel1";
			base.Controls.Add(this.contentTableLayoutPanel);
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "ErrorReportResultPane";
			base.Padding = new Padding(12, 5, 12, 5);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.contentTableLayoutPanel.ResumeLayout(false);
			this.contentTableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public string LabelTitle
		{
			get
			{
				return this.labelTitle.Text;
			}
			set
			{
				this.labelTitle.Text = value;
			}
		}

		protected override string GetContent()
		{
			return this.ErrorMessage;
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

		private TableLayoutPanel contentTableLayoutPanel;

		private LinkLabelCommand contentLabel;

		private Label labelTitle;
	}
}
