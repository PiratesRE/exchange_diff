using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class QueueViewerPropertyPage : ExchangePropertyPageControl
	{
		public QueueViewerPropertyPage()
		{
			this.InitializeComponent();
			base.HelpTopic = HelpId.QueueViewerPropertyPage.ToString();
			this.refreshLabel.Text = Strings.RefreshLabelText;
			this.autoRefreshEnabledCheckBox.Text = Strings.AutoRefreshCheckboxLabelText;
			this.refreshIntervalLabeledTextBox.Caption = Strings.RefreshIntervalLabel;
			this.limitsLabel.Text = Strings.LimitsLabelText;
			this.numberDisplayItemsLabeledTextBox.Caption = Strings.NumberDisplayItemsLabelText;
			this.Text = Strings.QueueViewerOptionsPageText;
			base.Size = this.DefaultSize;
			this.refreshIntervalLabeledTextBox.DataBindings.Add("Enabled", this.autoRefreshEnabledCheckBox, "Checked");
			base.BindingSource.DataSource = typeof(QueueViewerOptions);
			this.autoRefreshEnabledCheckBox.DataBindings.Add("Checked", base.BindingSource, "AutoRefreshEnabled");
			this.refreshIntervalLabeledTextBox.FormatMode = 4;
			this.refreshIntervalLabeledTextBox.DataBindings.Add(new Binding("Text", base.BindingSource, "RefreshInterval"));
			this.numberDisplayItemsLabeledTextBox.DataBindings.Add(new Binding("Text", base.BindingSource, "PageSize"));
		}

		private void InitializeComponent()
		{
			this.refreshLabel = new AutoHeightLabel();
			this.autoRefreshEnabledCheckBox = new AutoHeightCheckBox();
			this.refreshIntervalLabeledTextBox = new LabeledTextBox();
			this.limitsLabel = new AutoHeightLabel();
			this.numberDisplayItemsLabeledTextBox = new LabeledTextBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.refreshLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.refreshLabel, 2);
			this.refreshLabel.Location = new Point(13, 12);
			this.refreshLabel.Margin = new Padding(0);
			this.refreshLabel.Name = "refreshLabel";
			this.refreshLabel.ShowDivider = true;
			this.refreshLabel.Size = new Size(302, 17);
			this.refreshLabel.TabIndex = 0;
			this.refreshLabel.Text = "refreshLabel";
			this.autoRefreshEnabledCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.autoRefreshEnabledCheckBox, 2);
			this.autoRefreshEnabledCheckBox.Location = new Point(16, 37);
			this.autoRefreshEnabledCheckBox.Margin = new Padding(3, 8, 0, 0);
			this.autoRefreshEnabledCheckBox.Name = "autoRefreshEnabledCheckBox";
			this.autoRefreshEnabledCheckBox.Size = new Size(299, 17);
			this.autoRefreshEnabledCheckBox.TabIndex = 0;
			this.autoRefreshEnabledCheckBox.Text = "autoRefreshEnabledCheckBox";
			this.refreshIntervalLabeledTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.refreshIntervalLabeledTextBox.AutoSize = true;
			this.refreshIntervalLabeledTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.refreshIntervalLabeledTextBox.Caption = "refreshIntervalLabeledTextBox";
			this.refreshIntervalLabeledTextBox.Location = new Point(29, 59);
			this.refreshIntervalLabeledTextBox.Margin = new Padding(0, 5, 0, 0);
			this.refreshIntervalLabeledTextBox.Name = "refreshIntervalLabeledTextBox";
			this.refreshIntervalLabeledTextBox.Size = new Size(286, 20);
			this.refreshIntervalLabeledTextBox.TabIndex = 2;
			this.limitsLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.SetColumnSpan(this.limitsLabel, 2);
			this.limitsLabel.Location = new Point(13, 91);
			this.limitsLabel.Margin = new Padding(0, 12, 0, 0);
			this.limitsLabel.Name = "limitsLabel";
			this.limitsLabel.ShowDivider = true;
			this.limitsLabel.Size = new Size(302, 17);
			this.limitsLabel.TabIndex = 3;
			this.limitsLabel.Text = "limitsLabel";
			this.numberDisplayItemsLabeledTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.numberDisplayItemsLabeledTextBox.AutoSize = true;
			this.numberDisplayItemsLabeledTextBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.numberDisplayItemsLabeledTextBox.Caption = "numberDisplayItemsLabeledTextBox";
			this.tableLayoutPanel.SetColumnSpan(this.numberDisplayItemsLabeledTextBox, 2);
			this.numberDisplayItemsLabeledTextBox.Location = new Point(13, 113);
			this.numberDisplayItemsLabeledTextBox.Margin = new Padding(0, 5, 0, 0);
			this.numberDisplayItemsLabeledTextBox.Name = "numberDisplayItemsLabeledTextBox";
			this.numberDisplayItemsLabeledTextBox.Size = new Size(302, 20);
			this.numberDisplayItemsLabeledTextBox.TabIndex = 4;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ContainerType = ContainerType.PropertyPage;
			this.tableLayoutPanel.Controls.Add(this.refreshLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.autoRefreshEnabledCheckBox, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.refreshIntervalLabeledTextBox, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.limitsLabel, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.numberDisplayItemsLabeledTextBox, 0, 4);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(331, 145);
			this.tableLayoutPanel.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			this.MinimumSize = new Size(331, 145);
			base.Name = "QueueViewerPropertyPage";
			base.Size = new Size(331, 145);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private AutoHeightLabel refreshLabel;

		private AutoHeightCheckBox autoRefreshEnabledCheckBox;

		private LabeledTextBox refreshIntervalLabeledTextBox;

		private AutoHeightLabel limitsLabel;

		private LabeledTextBox numberDisplayItemsLabeledTextBox;

		private AutoTableLayoutPanel tableLayoutPanel;
	}
}
