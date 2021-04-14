using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class MessagePropertyPage : ExchangePropertyPageControl
	{
		public MessagePropertyPage()
		{
			this.InitializeComponent();
			this.Text = Strings.GeneralProperties;
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			ExtensibleMessageInfo extensibleMessageInfo = null;
			if (base.Context != null && base.Context.DataHandler.DataSource != null)
			{
				extensibleMessageInfo = (base.Context.DataHandler.DataSource as ExtensibleMessageInfo);
			}
			if (extensibleMessageInfo == null)
			{
				this.FormatMissingMessageError();
				return;
			}
			this.FormatMessagePreview(extensibleMessageInfo);
		}

		private void FormatMessagePreview(ExtensibleMessageInfo message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.IdColumn, message.Identity));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.SubjectColumn, message.Subject));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.InternetMessageIdColumn, message.InternetMessageId));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.FromAddressColumn, message.FromAddress));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.StatusColumn, message.Status));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.SizeColumn, Math.Max(1UL, message.Size.ToKB())));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.MessageSourceNameColumn, message.MessageSourceName));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.SourceIPColumn, message.SourceIP));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.SCLColumn, message.SCL));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.DateReceivedColumn, message.DateReceived.ToString()));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.ExpirationTimeColumn, message.ExpirationTime));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.LastErrorColumn, message.LastError));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.QueueIdColumn, message.Queue));
			stringBuilder.AppendLine(this.FormatPropertyParagraph(Strings.Recipients, message.Recipients));
			this.exchangeTextBox1.Text = stringBuilder.ToString();
		}

		private string FormatPropertyParagraph(string propName, object value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(propName + ": ");
			StringBuilder stringBuilder2 = new StringBuilder();
			if (value == null)
			{
				stringBuilder2.Append(string.Empty);
			}
			else if (value is Enum)
			{
				stringBuilder2.Append(LocalizedDescriptionAttribute.FromEnum(value.GetType(), value));
			}
			else
			{
				if (value is Array)
				{
					using (IEnumerator enumerator = ((Array)value).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							stringBuilder2.Append(" " + obj.ToString());
						}
						goto IL_AB;
					}
				}
				stringBuilder2.Append(value.ToString());
			}
			IL_AB:
			stringBuilder.Append(stringBuilder2.ToString());
			return stringBuilder.ToString();
		}

		private void FormatMissingMessageError()
		{
			this.exchangeTextBox1.Text = Strings.MissingMessageError;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new AutoTableLayoutPanel();
			this.exchangeTextBox1 = new ExchangeTextBox();
			this.textBoxPanel = new AutoSizePanel();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.ContainerType = ContainerType.PropertyPage;
			this.tableLayoutPanel1.Controls.Add(this.textBoxPanel, 0, 0);
			this.tableLayoutPanel1.Dock = DockStyle.Top;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Margin = new Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(418, 396);
			this.tableLayoutPanel1.TabIndex = 0;
			this.exchangeTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeTextBox1.Location = new Point(0, 0);
			this.exchangeTextBox1.Margin = new Padding(0);
			this.exchangeTextBox1.Multiline = true;
			this.exchangeTextBox1.Name = "exchangeTextBox1";
			this.exchangeTextBox1.ReadOnly = true;
			this.exchangeTextBox1.ScrollBars = ScrollBars.Vertical;
			this.exchangeTextBox1.Size = new Size(386, 372);
			this.exchangeTextBox1.TabIndex = 0;
			this.textBoxPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxPanel.BackColor = SystemColors.Window;
			this.textBoxPanel.Location = new Point(16, 12);
			this.textBoxPanel.Margin = new Padding(3, 0, 0, 0);
			this.textBoxPanel.Name = "textBoxPanel";
			this.textBoxPanel.Size = new Size(386, 372);
			this.textBoxPanel.TabIndex = 0;
			this.textBoxPanel.Controls.Add(this.exchangeTextBox1);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "MessagePropertyPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private AutoTableLayoutPanel tableLayoutPanel1;

		private ExchangeTextBox exchangeTextBox1;

		private AutoSizePanel textBoxPanel;
	}
}
