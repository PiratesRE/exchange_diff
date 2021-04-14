using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class LabeledTextBox : CaptionedTextBox
	{
		public LabeledTextBox()
		{
			this.InitializeComponent();
			base.Name = "LabeledTextBox";
		}

		private void InitializeComponent()
		{
			this.labelCaption = new Label();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.Controls.Add(this.labelCaption, 0, 0);
			this.labelCaption.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.labelCaption.AutoSize = true;
			this.labelCaption.Location = new Point(0, 0);
			this.labelCaption.Margin = new Padding(0, 3, 0, 4);
			this.labelCaption.Name = "labelCaption";
			this.labelCaption.TabIndex = 0;
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue("")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Caption
		{
			get
			{
				return this.labelCaption.Text;
			}
			set
			{
				this.labelCaption.Text = value;
			}
		}

		private Label labelCaption;
	}
}
