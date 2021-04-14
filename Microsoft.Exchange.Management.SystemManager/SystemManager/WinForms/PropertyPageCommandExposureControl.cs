using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PropertyPageCommandExposureControl : ExchangePropertyPageControl
	{
		public PropertyPageCommandExposureControl()
		{
			this.InitializeComponent();
			this.Text = Strings.PropertyPageLoggingDialogTitle;
			this.exposedCommandLabel.Text = Strings.PropertyPageLoggingDialogText;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string CommandToShow
		{
			get
			{
				return this.exposedCommandTextBox.Text;
			}
			set
			{
				this.exposedCommandTextBox.Text = value;
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		private bool ShouldSerializeText()
		{
			return this.Text != Strings.PropertyPageLoggingDialogTitle;
		}

		[DefaultValue(true)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
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
			this.exposedCommandLabel = new Label();
			this.outputPanel = new AutoSizePanel();
			this.exposedCommandTextBox = new TextBox();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.outputPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.ContainerType = ContainerType.PropertyPage;
			this.tableLayoutPanel1.Controls.Add(this.exposedCommandLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.outputPanel, 0, 1);
			this.tableLayoutPanel1.Dock = DockStyle.Top;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Margin = new Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(437, 240);
			this.tableLayoutPanel1.TabIndex = 0;
			this.exposedCommandLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exposedCommandLabel.AutoSize = true;
			this.exposedCommandLabel.Location = new Point(13, 12);
			this.exposedCommandLabel.Margin = new Padding(0);
			this.exposedCommandLabel.Name = "exposedCommandLabel";
			this.exposedCommandLabel.Size = new Size(408, 13);
			this.exposedCommandLabel.TabIndex = 0;
			this.exposedCommandLabel.Text = "exposedCommandLabel";
			this.outputPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.outputPanel.BackColor = SystemColors.Window;
			this.outputPanel.Controls.Add(this.exposedCommandTextBox);
			this.outputPanel.Location = new Point(16, 28);
			this.outputPanel.Margin = new Padding(3, 3, 0, 0);
			this.outputPanel.Name = "outputPanel";
			this.outputPanel.Size = new Size(405, 200);
			this.outputPanel.TabIndex = 1;
			this.exposedCommandTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exposedCommandTextBox.BackColor = SystemColors.Window;
			this.exposedCommandTextBox.Location = new Point(0, 0);
			this.exposedCommandTextBox.Margin = new Padding(0);
			this.exposedCommandTextBox.Multiline = true;
			this.exposedCommandTextBox.Name = "exposedCommandTextBox";
			this.exposedCommandTextBox.ReadOnly = true;
			this.exposedCommandTextBox.ScrollBars = ScrollBars.Vertical;
			this.exposedCommandTextBox.Size = new Size(405, 200);
			this.exposedCommandTextBox.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "PropertyPageCommandExposureControl";
			base.Size = new Size(437, 255);
			this.Text = "PropertyPageCommandExposureControl";
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.outputPanel.ResumeLayout(false);
			this.outputPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private AutoTableLayoutPanel tableLayoutPanel1;

		private Label exposedCommandLabel;

		private TextBox exposedCommandTextBox;

		private AutoSizePanel outputPanel;
	}
}
