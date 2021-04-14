using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class NumericUpDownPage : ExchangePropertyPageControl
	{
		public NumericUpDownPage()
		{
			this.InitializeComponent();
		}

		public string LabelText
		{
			get
			{
				return this.numericUpDownLabel.Text;
			}
			set
			{
				this.numericUpDownLabel.Text = value;
			}
		}

		public int Minimum
		{
			get
			{
				return (int)this.numericUpDown.Minimum;
			}
			set
			{
				this.numericUpDown.Minimum = value;
			}
		}

		public int Maximum
		{
			get
			{
				return (int)this.numericUpDown.Maximum;
			}
			set
			{
				this.numericUpDown.Maximum = value;
			}
		}

		public int Value
		{
			get
			{
				return (int)this.numericUpDown.Value;
			}
			set
			{
				this.numericUpDown.Value = value;
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.mainTableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void InitializeComponent()
		{
			this.mainTableLayoutPanel = new AutoTableLayoutPanel();
			this.numericUpDownLabel = new Label();
			this.numericUpDown = new ExchangeNumericUpDown();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.mainTableLayoutPanel.SuspendLayout();
			((ISupportInitialize)this.numericUpDown).BeginInit();
			base.SuspendLayout();
			this.mainTableLayoutPanel.AutoLayout = true;
			this.mainTableLayoutPanel.AutoSize = true;
			this.mainTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.mainTableLayoutPanel.ColumnCount = 2;
			this.mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));
			this.mainTableLayoutPanel.ContainerType = ContainerType.PropertyPage;
			this.mainTableLayoutPanel.Controls.Add(this.numericUpDownLabel, 0, 0);
			this.mainTableLayoutPanel.Controls.Add(this.numericUpDown, 1, 0);
			this.mainTableLayoutPanel.Dock = DockStyle.Top;
			this.mainTableLayoutPanel.Location = new Point(0, 0);
			this.mainTableLayoutPanel.Margin = new Padding(0);
			this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
			this.mainTableLayoutPanel.Padding = new Padding(13, 12, 16, 12);
			this.mainTableLayoutPanel.RowCount = 1;
			this.mainTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.mainTableLayoutPanel.Size = new Size(250, 44);
			this.mainTableLayoutPanel.TabIndex = 0;
			this.numericUpDownLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.numericUpDownLabel.AutoSize = true;
			this.numericUpDownLabel.Location = new Point(13, 15);
			this.numericUpDownLabel.Margin = new Padding(0, 3, 0, 4);
			this.numericUpDownLabel.Name = "numericUpDownLabel";
			this.numericUpDownLabel.Size = new Size(146, 13);
			this.numericUpDownLabel.TabIndex = 0;
			this.numericUpDownLabel.Text = "numericUpDownLabel";
			this.numericUpDown.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.numericUpDown.Location = new Point(162, 12);
			this.numericUpDown.Margin = new Padding(3, 0, 0, 0);
			this.numericUpDown.Name = "numericUpDown";
			this.numericUpDown.Size = new Size(72, 20);
			this.numericUpDown.TabIndex = 1;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.mainTableLayoutPanel);
			this.MinimumSize = new Size(250, 0);
			base.Name = "NumericUpDownPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			this.mainTableLayoutPanel.ResumeLayout(false);
			this.mainTableLayoutPanel.PerformLayout();
			((ISupportInitialize)this.numericUpDown).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private AutoTableLayoutPanel mainTableLayoutPanel;

		private Label numericUpDownLabel;

		private ExchangeNumericUpDown numericUpDown;
	}
}
