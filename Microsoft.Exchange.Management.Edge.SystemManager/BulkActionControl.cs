using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class BulkActionControl : ExchangePropertyPageControl
	{
		public BulkActionControl()
		{
			this.InitializeComponent();
			this.Text = Strings.BulkActionTitleText;
			this.warningIconPictureBox.Image = IconLibrary.ToBitmap(Icons.Warning, this.warningIconPictureBox.Size);
			this.bulkActionLabel.Text = Strings.BulkActionLabelText;
			this.expandScopeCheckBox.Text = Strings.BulkActionCheckBoxText;
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void InitializeComponent()
		{
			this.warningIconPictureBox = new ExchangePictureBox();
			this.bulkActionLabel = new Label();
			this.expandScopeCheckBox = new AutoHeightCheckBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			((ISupportInitialize)base.BindingSource).BeginInit();
			((ISupportInitialize)this.warningIconPictureBox).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.warningIconPictureBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.warningIconPictureBox.Location = new Point(16, 12);
			this.warningIconPictureBox.Margin = new Padding(3, 0, 0, 0);
			this.warningIconPictureBox.Name = "warningIconPictureBox";
			this.warningIconPictureBox.Size = new Size(32, 32);
			this.warningIconPictureBox.TabIndex = 0;
			this.warningIconPictureBox.TabStop = false;
			this.bulkActionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.bulkActionLabel.Location = new Point(60, 12);
			this.bulkActionLabel.Margin = new Padding(0);
			this.bulkActionLabel.Name = "bulkActionLabel";
			this.bulkActionLabel.Size = new Size(147, 16);
			this.bulkActionLabel.TabIndex = 1;
			this.bulkActionLabel.Text = "bulkActionLabel";
			this.expandScopeCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.expandScopeCheckBox.Location = new Point(63, 52);
			this.expandScopeCheckBox.Margin = new Padding(3, 8, 0, 0);
			this.expandScopeCheckBox.Name = "expandScopeCheckBox";
			this.expandScopeCheckBox.Size = new Size(144, 17);
			this.expandScopeCheckBox.TabIndex = 0;
			this.expandScopeCheckBox.Text = "expandScopeCheckBox";
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 12f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ContainerType = ContainerType.Control;
			this.tableLayoutPanel.Controls.Add(this.expandScopeCheckBox, 2, 1);
			this.tableLayoutPanel.Controls.Add(this.bulkActionLabel, 2, 0);
			this.tableLayoutPanel.Controls.Add(this.warningIconPictureBox, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(223, 81);
			this.tableLayoutPanel.TabIndex = 2;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "BulkActionControl";
			base.Size = new Size(418, 96);
			((ISupportInitialize)base.BindingSource).EndInit();
			((ISupportInitialize)this.warningIconPictureBox).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue("false")]
		public bool IsExpandScopeSelected
		{
			get
			{
				return this.expandScopeCheckBox.Checked;
			}
		}

		private ExchangePictureBox warningIconPictureBox;

		private Label bulkActionLabel;

		private AutoTableLayoutPanel tableLayoutPanel;

		private AutoHeightCheckBox expandScopeCheckBox;
	}
}
