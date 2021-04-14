using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class GeneralPropertyPageControl : ExchangePropertyPageControl
	{
		public GeneralPropertyPageControl()
		{
			this.InitializeComponent();
			this.Text = Strings.GeneralPropertyPageText;
			this.descriptionDividerLabel.Text = Strings.GeneralPropertyPageDescriptionDividerLabelText;
			this.summaryPanel.SummaryInfoCollection.ListChanged += this.objectInfoCollection_ListChanged;
			this.summaryPanel.SummaryInfoCollection.ListChanging += this.objectInfoCollection_ListChanging;
			this.modifiedInfo = new GeneralPageSummaryInfo();
			this.modifiedInfo.BindingSource = base.BindingSource;
			this.modifiedInfo.Text = Strings.GeneralPropertyPageModifiedLabelText;
			this.modifiedInfo.PropertyName = "WhenChanged";
			this.modifiedInfo.FormatMode = 0;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.modifiedInfo != null && !this.GeneralPageSummaryInfoCollection.Contains(this.modifiedInfo))
			{
				base.SuspendLayout();
				this.summaryPanel.SuspendLayout();
				this.GeneralPageSummaryInfoCollection.Add(this.modifiedInfo);
				this.summaryPanel.ResumeLayout(false);
				base.ResumeLayout(false);
				this.summaryPanel.PerformLayout();
				base.PerformLayout();
			}
			if (base.Controls.Count > 1 && this.GeneralPageSummaryInfoCollection.Count > 0 && !this.generalPageTableLayoutPanel.Controls.Contains(this.configurationDividerLabel))
			{
				this.generalPageTableLayoutPanel.Controls.Add(this.configurationDividerLabel, 0, 4);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Text
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Margin
		{
			get
			{
				return base.Margin;
			}
			set
			{
				base.Margin = value;
			}
		}

		private void objectInfoCollection_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.SetMarginOnSummaryLastRow();
			}
		}

		private void objectInfoCollection_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				if (this.modifiedInfo != null && this.GeneralPageSummaryInfoCollection.Contains(this.modifiedInfo) && this.GeneralPageSummaryInfoCollection[this.GeneralPageSummaryInfoCollection.Count - 1] != this.modifiedInfo)
				{
					this.GeneralPageSummaryInfoCollection.Remove(this.modifiedInfo);
					this.GeneralPageSummaryInfoCollection.Add(this.modifiedInfo);
				}
				this.SetMarginOnSummaryLastRow();
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				this.SetMarginOnSummaryLastRow();
			}
		}

		private void SetMarginOnSummaryLastRow()
		{
			if (this.summaryPanel.RowCount != 0)
			{
				Control controlFromPosition = this.summaryPanel.GetControlFromPosition(0, this.summaryPanel.RowCount - 1);
				Control controlFromPosition2 = this.summaryPanel.GetControlFromPosition(1, this.summaryPanel.RowCount - 1);
				controlFromPosition.Margin = new Padding(0, 3, 12, 6);
				controlFromPosition2.Margin = new Padding(0, 3, 0, 3);
				if (this.summaryPanel.RowCount - 2 >= 0)
				{
					controlFromPosition = this.summaryPanel.GetControlFromPosition(0, this.summaryPanel.RowCount - 2);
					controlFromPosition2 = this.summaryPanel.GetControlFromPosition(1, this.summaryPanel.RowCount - 2);
					controlFromPosition.Margin = new Padding(0, 3, 12, 9);
					controlFromPosition2.Margin = new Padding(0, 3, 0, 3);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChangeNotifyingCollection<GeneralPageSummaryInfo> GeneralPageSummaryInfoCollection
		{
			get
			{
				return this.summaryPanel.SummaryInfoCollection;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GeneralPageHeader Header
		{
			get
			{
				return this.namePanel;
			}
		}

		[DefaultValue(null)]
		public Icon ObjectIcon
		{
			get
			{
				return this.Header.Icon;
			}
			set
			{
				this.Header.Icon = value;
			}
		}

		[DefaultValue(true)]
		public bool CanChangeHeaderText
		{
			get
			{
				return this.Header.CanChangeHeaderText;
			}
			set
			{
				this.Header.CanChangeHeaderText = value;
			}
		}

		[DefaultValue("")]
		public string Description
		{
			get
			{
				return this.descriptionLabel.Text;
			}
			set
			{
				this.descriptionLabel.Text = value;
				if (string.IsNullOrEmpty(this.descriptionLabel.Text))
				{
					if (this.generalPageTableLayoutPanel.Controls.Contains(this.descriptionPanel))
					{
						this.generalPageTableLayoutPanel.Controls.Remove(this.descriptionPanel);
						return;
					}
				}
				else if (!this.generalPageTableLayoutPanel.Controls.Contains(this.descriptionPanel))
				{
					this.generalPageTableLayoutPanel.Controls.Add(this.descriptionPanel, 0, 3);
				}
			}
		}

		private void InitializeComponent()
		{
			this.namePanel = new GeneralPageHeader();
			this.namePanelDividerLabel = new AutoHeightLabel();
			this.summaryPanel = new ExchangeSummaryControl();
			this.configurationDividerLabel = new AutoHeightLabel();
			this.descriptionPanel = new TableLayoutPanel();
			this.descriptionDividerLabel = new AutoHeightLabel();
			this.descriptionLabel = new Label();
			this.generalPageTableLayoutPanel = new TableLayoutPanel();
			this.descriptionPanel.SuspendLayout();
			this.generalPageTableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.namePanel.AutoSize = true;
			this.namePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.namePanel.Dock = DockStyle.Top;
			this.namePanel.Location = new Point(16, 12);
			this.namePanel.Margin = new Padding(0);
			this.namePanel.Name = "namePanel";
			this.namePanel.Size = new Size(386, 34);
			this.namePanel.TabIndex = 0;
			this.namePanelDividerLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.namePanelDividerLabel.Location = new Point(16, 46);
			this.namePanelDividerLabel.Margin = new Padding(0);
			this.namePanelDividerLabel.Name = "namePanelDividerLabel";
			this.namePanelDividerLabel.ShowDivider = true;
			this.namePanelDividerLabel.Size = new Size(386, 16);
			this.namePanelDividerLabel.TabIndex = 3;
			this.summaryPanel.AutoSize = true;
			this.summaryPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.summaryPanel.Dock = DockStyle.Top;
			this.summaryPanel.Location = new Point(16, 62);
			this.summaryPanel.Margin = new Padding(0);
			this.summaryPanel.MinimumSize = new Size(386, 0);
			this.summaryPanel.Name = "summaryPanel";
			this.summaryPanel.TabIndex = 4;
			this.configurationDividerLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.configurationDividerLabel.Location = new Point(0, 0);
			this.configurationDividerLabel.Margin = new Padding(0, 0, 0, 3);
			this.configurationDividerLabel.Name = "configurationDividerLabel";
			this.configurationDividerLabel.ShowDivider = true;
			this.configurationDividerLabel.Size = new Size(386, 16);
			this.configurationDividerLabel.TabIndex = 0;
			this.descriptionPanel.AutoSize = true;
			this.descriptionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.descriptionPanel.ColumnCount = 1;
			this.descriptionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.descriptionPanel.Controls.Add(this.descriptionDividerLabel, 0, 0);
			this.descriptionPanel.Controls.Add(this.descriptionLabel, 0, 1);
			this.descriptionPanel.Dock = DockStyle.Top;
			this.descriptionPanel.Location = new Point(16, 80);
			this.descriptionPanel.Margin = new Padding(0);
			this.descriptionPanel.MinimumSize = new Size(386, 0);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.Padding = new Padding(0, 0, 0, 12);
			this.descriptionPanel.RowCount = 2;
			this.descriptionPanel.RowStyles.Add(new RowStyle());
			this.descriptionPanel.RowStyles.Add(new RowStyle());
			this.descriptionPanel.Size = new Size(386, 50);
			this.descriptionPanel.TabIndex = 3;
			this.descriptionDividerLabel.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.descriptionDividerLabel.Location = new Point(0, 3);
			this.descriptionDividerLabel.Margin = new Padding(0, 3, 0, 6);
			this.descriptionDividerLabel.Name = "descriptionDividerLabel";
			this.descriptionDividerLabel.ShowDivider = true;
			this.descriptionDividerLabel.Size = new Size(386, 17);
			this.descriptionDividerLabel.TabIndex = 4;
			this.descriptionDividerLabel.Text = "descriptionDividerLabel";
			this.descriptionDividerLabel.UseMnemonic = false;
			this.descriptionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.descriptionLabel.AutoSize = true;
			this.descriptionLabel.Location = new Point(0, 26);
			this.descriptionLabel.Margin = new Padding(0);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Size = new Size(386, 13);
			this.descriptionLabel.TabIndex = 5;
			this.generalPageTableLayoutPanel.AutoSize = true;
			this.generalPageTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.generalPageTableLayoutPanel.ColumnCount = 1;
			this.generalPageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.generalPageTableLayoutPanel.Controls.Add(this.namePanel, 0, 0);
			this.generalPageTableLayoutPanel.Controls.Add(this.namePanelDividerLabel, 0, 1);
			this.generalPageTableLayoutPanel.Controls.Add(this.summaryPanel, 0, 2);
			this.generalPageTableLayoutPanel.Dock = DockStyle.Top;
			this.generalPageTableLayoutPanel.Location = new Point(0, 0);
			this.generalPageTableLayoutPanel.Margin = new Padding(0);
			this.generalPageTableLayoutPanel.Name = "generalPageTableLayoutPanel";
			this.generalPageTableLayoutPanel.Padding = new Padding(16, 12, 16, 0);
			this.generalPageTableLayoutPanel.RowCount = 5;
			this.generalPageTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.generalPageTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.generalPageTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.generalPageTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.generalPageTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.generalPageTableLayoutPanel.Size = new Size(418, 62);
			this.generalPageTableLayoutPanel.TabIndex = 5;
			base.Controls.Add(this.generalPageTableLayoutPanel);
			this.Margin = new Padding(0);
			base.Name = "GeneralPropertyPageControl";
			this.descriptionPanel.ResumeLayout(false);
			this.descriptionPanel.PerformLayout();
			this.generalPageTableLayoutPanel.ResumeLayout(false);
			this.generalPageTableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private GeneralPageHeader namePanel;

		private ExchangeSummaryControl summaryPanel;

		private AutoHeightLabel namePanelDividerLabel;

		private TableLayoutPanel descriptionPanel;

		private AutoHeightLabel descriptionDividerLabel;

		private Label descriptionLabel;

		private AutoHeightLabel configurationDividerLabel;

		private TableLayoutPanel generalPageTableLayoutPanel;

		protected GeneralPageSummaryInfo modifiedInfo;
	}
}
