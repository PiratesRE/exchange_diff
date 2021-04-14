using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class BulkEditSummaryPage : ExchangePropertyPageControl
	{
		public BulkEditSummaryPage()
		{
			this.InitializeComponent();
			this.Text = Strings.BulkEditingSummaryDialogTitle;
			this.summaryDescriptionLabel.Text = Strings.BulkEditingSummaryDescription;
			GeneralPageSummaryInfo generalPageSummaryInfo = new GeneralPageSummaryInfo();
			GeneralPageSummaryInfo generalPageSummaryInfo2 = new GeneralPageSummaryInfo();
			generalPageSummaryInfo.Text = Strings.BulkEditingSelectedItemTypeLabel;
			base.BindingSource.DataSource = typeof(DataContext);
			generalPageSummaryInfo.BindingSource = base.BindingSource;
			generalPageSummaryInfo.PropertyName = "SelectedObjectDetailsType";
			generalPageSummaryInfo2.Text = Strings.BulkEditingSelectedItemNumberLabel;
			generalPageSummaryInfo2.BindingSource = base.BindingSource;
			generalPageSummaryInfo2.PropertyName = "SelectedObjectsCount";
			this.modifiedParametersLabel.Text = Strings.BulkEditingModifiedParametersLabel;
			this.summaryHintLabel.Text = Strings.BulkEditingSummaryHintLabel;
			this.modifiedParametersTextBox.DataBindings.Add("Text", base.BindingSource, "ModifiedParametersDescription", true, DataSourceUpdateMode.Never);
			this.exchangeSummaryControl1.SummaryInfoCollection.Add(generalPageSummaryInfo);
			this.exchangeSummaryControl1.SummaryInfoCollection.Add(generalPageSummaryInfo2);
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
			return this.Text != Strings.BulkEditingSummaryDialogTitle;
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

		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new AutoTableLayoutPanel();
			this.summaryDescriptionLabel = new Label();
			this.modifiedParametersLabel = new Label();
			this.autoSizePanel1 = new AutoSizePanel();
			this.modifiedParametersTextBox = new ExchangeTextBox();
			this.summaryHintLabel = new Label();
			this.exchangeSummaryControl1 = new ExchangeSummaryControl();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.autoSizePanel1.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel1.ContainerType = ContainerType.PropertyPage;
			this.tableLayoutPanel1.Controls.Add(this.summaryDescriptionLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.modifiedParametersLabel, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.autoSizePanel1, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.summaryHintLabel, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.exchangeSummaryControl1, 0, 2);
			this.tableLayoutPanel1.Dock = DockStyle.Top;
			this.tableLayoutPanel1.Location = new Point(0, 0);
			this.tableLayoutPanel1.Margin = new Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(437, 331);
			this.tableLayoutPanel1.TabIndex = 2;
			this.summaryDescriptionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.summaryDescriptionLabel.AutoSize = true;
			this.summaryDescriptionLabel.Location = new Point(13, 12);
			this.summaryDescriptionLabel.Margin = new Padding(0);
			this.summaryDescriptionLabel.Name = "summaryDescriptionLabel";
			this.summaryDescriptionLabel.Size = new Size(408, 13);
			this.summaryDescriptionLabel.TabIndex = 0;
			this.summaryDescriptionLabel.Text = "summaryDescriptionLabel";
			this.modifiedParametersLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.modifiedParametersLabel.AutoSize = true;
			this.modifiedParametersLabel.Location = new Point(13, 87);
			this.modifiedParametersLabel.Margin = new Padding(0, 12, 0, 0);
			this.modifiedParametersLabel.Name = "modifiedParametersLabel";
			this.modifiedParametersLabel.Size = new Size(408, 13);
			this.modifiedParametersLabel.TabIndex = 2;
			this.modifiedParametersLabel.Text = "modifiedParametersLabel";
			this.autoSizePanel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.autoSizePanel1.BackColor = SystemColors.Window;
			this.autoSizePanel1.Controls.Add(this.modifiedParametersTextBox);
			this.autoSizePanel1.Location = new Point(16, 103);
			this.autoSizePanel1.Margin = new Padding(3, 3, 0, 0);
			this.autoSizePanel1.Name = "autoSizePanel1";
			this.autoSizePanel1.Size = new Size(405, 200);
			this.autoSizePanel1.TabIndex = 3;
			this.modifiedParametersTextBox.AcceptsReturn = true;
			this.modifiedParametersTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.modifiedParametersTextBox.BackColor = SystemColors.Window;
			this.modifiedParametersTextBox.Location = new Point(0, 0);
			this.modifiedParametersTextBox.Margin = new Padding(0);
			this.modifiedParametersTextBox.Multiline = true;
			this.modifiedParametersTextBox.Name = "modifiedParametersTextBox";
			this.modifiedParametersTextBox.ReadOnly = true;
			this.modifiedParametersTextBox.ScrollBars = ScrollBars.Vertical;
			this.modifiedParametersTextBox.Size = new Size(405, 200);
			this.modifiedParametersTextBox.TabIndex = 0;
			this.summaryHintLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.summaryHintLabel.AutoSize = true;
			this.summaryHintLabel.Location = new Point(13, 306);
			this.summaryHintLabel.Margin = new Padding(0, 3, 0, 0);
			this.summaryHintLabel.Name = "summaryHintLabel";
			this.summaryHintLabel.Size = new Size(408, 13);
			this.summaryHintLabel.TabIndex = 4;
			this.summaryHintLabel.Text = "summaryHintLabel";
			this.exchangeSummaryControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeSummaryControl1.AutoSize = true;
			this.exchangeSummaryControl1.Location = new Point(13, 37);
			this.exchangeSummaryControl1.Margin = new Padding(0, 12, 0, 0);
			this.exchangeSummaryControl1.Name = "exchangeSummaryControl1";
			this.exchangeSummaryControl1.Size = new Size(408, 38);
			this.exchangeSummaryControl1.TabIndex = 1;
			this.AutoSize = true;
			base.Controls.Add(this.tableLayoutPanel1);
			base.Name = "BulkEditSummaryPage";
			base.Size = new Size(437, 363);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.autoSizePanel1.ResumeLayout(false);
			this.autoSizePanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel1.GetPreferredSize(proposedSize);
		}

		private AutoTableLayoutPanel tableLayoutPanel1;

		private Label summaryDescriptionLabel;

		private Label modifiedParametersLabel;

		private ExchangeTextBox modifiedParametersTextBox;

		private Label summaryHintLabel;

		private ExchangeSummaryControl exchangeSummaryControl1;

		private AutoSizePanel autoSizePanel1;
	}
}
