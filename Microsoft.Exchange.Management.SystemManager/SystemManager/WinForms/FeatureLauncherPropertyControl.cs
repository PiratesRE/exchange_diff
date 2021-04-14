using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FeatureLauncherPropertyControl : ExchangeUserControl, IFeatureLauncherBulkEditSupport, IBulkEditSupport, IGetInnerObject, ISpecifyPropertyState
	{
		public FeatureLauncherPropertyControl()
		{
			this.InitializeComponent();
			this.iconLibrary = new IconLibrary();
			this.iconLibrary.Icons.Add(Strings.PropertiesButtonText, Icons.Properties);
			this.iconLibrary.Icons.Add(Strings.EnableButtonText, Icons.Enable);
			this.iconLibrary.Icons.Add(Strings.DisableButtonText, Icons.Disable);
			this.featureListView.IconLibrary = this.iconLibrary;
			this.enableButton.Text = Strings.EnableButtonText;
			this.enableButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.EnableButtonText);
			this.disableButton.Text = Strings.DisableButtonText;
			this.disableButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.DisableButtonText);
			this.propertiesButton.Text = Strings.PropertiesButtonText;
			this.propertiesButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.PropertiesButtonText);
			this.itemDescriptionDividerLabel.Text = Strings.DescriptionLabelText;
			this.featureColumnHeader.Text = Strings.FeatureColumnText;
			this.featureColumnHeader.Default = true;
			this.statusColumnHeader.Text = Strings.StatusColumnText;
			this.statusColumnHeader.Default = true;
			this.itemDescriptionLabel.Text = Strings.DefaultItemDescriptionText;
			this.InfoLabelText = string.Empty;
			this.buttonsToolStrip.ImageList = this.iconLibrary.SmallImageList;
			this.propertiesButton.ImageKey = Strings.PropertiesButtonText;
			this.propertiesButton.TextImageRelation = TextImageRelation.ImageBeforeText;
			this.enableButton.ImageKey = Strings.EnableButtonText;
			this.enableButton.TextImageRelation = TextImageRelation.ImageBeforeText;
			this.disableButton.ImageKey = Strings.DisableButtonText;
			this.disableButton.TextImageRelation = TextImageRelation.ImageBeforeText;
		}

		private ExchangePage ParentPage
		{
			get
			{
				for (Control parent = base.Parent; parent != null; parent = parent.Parent)
				{
					ExchangePage exchangePage = parent as ExchangePage;
					if (exchangePage != null)
					{
						return exchangePage;
					}
				}
				return null;
			}
		}

		public void SetBinding(BindingSource bindingSource)
		{
			if (!object.ReferenceEquals(this.dataSource, bindingSource))
			{
				if (this.dataSource != null)
				{
					foreach (object obj in this.FeatureListView.Items)
					{
						FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
						if (!string.IsNullOrEmpty(featureLauncherListViewItem.StatusPropertyName) && base.DataBindings[featureLauncherListViewItem.StatusBindingName] != null)
						{
							base.DataBindings[featureLauncherListViewItem.StatusBindingName].Format -= this.Binding_Format;
							base.DataBindings[featureLauncherListViewItem.StatusBindingName].Parse -= this.Binding_Parse;
							base.DataBindings.Remove(base.DataBindings[featureLauncherListViewItem.StatusBindingName]);
						}
					}
				}
				this.dataSource = bindingSource;
				if (this.dataSource != null)
				{
					foreach (object obj2 in this.FeatureListView.Items)
					{
						FeatureLauncherListViewItem featureLauncherListViewItem2 = (FeatureLauncherListViewItem)obj2;
						if (!string.IsNullOrEmpty(featureLauncherListViewItem2.StatusPropertyName))
						{
							Binding binding = new Binding(featureLauncherListViewItem2.StatusBindingName, bindingSource, featureLauncherListViewItem2.StatusPropertyName, true, featureLauncherListViewItem2.DataSourceUpdateMode);
							binding.Format += this.Binding_Format;
							binding.Parse += this.Binding_Parse;
							base.DataBindings.Add(binding);
						}
					}
				}
			}
		}

		private void Binding_Parse(object sender, ConvertEventArgs e)
		{
			switch ((FeatureStatus)e.Value)
			{
			case FeatureStatus.Enabled:
				e.Value = true;
				return;
			case FeatureStatus.Disabled:
				e.Value = false;
				return;
			default:
				e.Value = DBNull.Value;
				return;
			}
		}

		private void Binding_Format(object sender, ConvertEventArgs e)
		{
			FeatureStatus featureStatus = FeatureStatus.Unknown;
			if (e.Value != null && typeof(bool).IsAssignableFrom(e.Value.GetType()))
			{
				featureStatus = (((bool)e.Value) ? FeatureStatus.Enabled : FeatureStatus.Disabled);
			}
			e.Value = featureStatus;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.iconLibrary.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.featureListView = new FeatureLauncherDataListView();
			this.featureColumnHeader = new ExchangeColumnHeader();
			this.statusColumnHeader = new ExchangeColumnHeader();
			this.itemDescriptionDividerLabel = new AutoHeightLabel();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.itemDescriptionLabel = new Label();
			this.buttonsToolStrip = new TabbableToolStrip();
			this.propertiesButton = new ToolStripButton();
			this.enableButton = new ToolStripButton();
			this.disableButton = new ToolStripButton();
			this.infoLabel = new Label();
			this.tableLayoutPanel.SuspendLayout();
			this.buttonsToolStrip.SuspendLayout();
			base.SuspendLayout();
			this.featureListView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.featureListView.AvailableColumns.AddRange(new ExchangeColumnHeader[]
			{
				this.featureColumnHeader,
				this.statusColumnHeader
			});
			this.featureListView.HideSelection = false;
			this.featureListView.Location = new Point(3, 38);
			this.featureListView.Margin = new Padding(3, 3, 0, 0);
			this.featureListView.MultiSelect = false;
			this.featureListView.Name = "featureListView";
			this.featureListView.Size = new Size(315, 184);
			this.featureListView.TabIndex = 1;
			this.featureListView.UseCompatibleStateImageBehavior = false;
			this.featureListView.ItemActivate += this.featureListView_ItemActivate;
			this.featureListView.ColumnClick += this.featureListView_ColumnClick;
			this.featureListView.SelectedIndexChanged += delegate(object param0, EventArgs param1)
			{
				this.UpdateStatusWhenSelectionChanged();
			};
			this.itemDescriptionDividerLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.itemDescriptionDividerLabel.Location = new Point(0, 234);
			this.itemDescriptionDividerLabel.Margin = new Padding(0, 12, 0, 0);
			this.itemDescriptionDividerLabel.Name = "itemDescriptionDividerLabel";
			this.itemDescriptionDividerLabel.ShowDivider = true;
			this.itemDescriptionDividerLabel.Size = new Size(318, 16);
			this.itemDescriptionDividerLabel.TabIndex = 2;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.featureListView, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.itemDescriptionDividerLabel, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.itemDescriptionLabel, 0, 4);
			this.tableLayoutPanel.Controls.Add(this.buttonsToolStrip, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.infoLabel, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 5;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(318, 271);
			this.tableLayoutPanel.TabIndex = 0;
			this.itemDescriptionLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.itemDescriptionLabel.AutoSize = true;
			this.itemDescriptionLabel.Location = new Point(0, 258);
			this.itemDescriptionLabel.Margin = new Padding(0, 8, 0, 0);
			this.itemDescriptionLabel.Name = "itemDescriptionLabel";
			this.itemDescriptionLabel.Size = new Size(318, 13);
			this.itemDescriptionLabel.TabIndex = 3;
			this.itemDescriptionLabel.Text = "itemDescriptionLabel";
			this.itemDescriptionLabel.TextChanged += this.DescriptionLabelTextChanged;
			this.buttonsToolStrip.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.buttonsToolStrip.Dock = DockStyle.None;
			this.buttonsToolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.propertiesButton,
				this.enableButton,
				this.disableButton
			});
			this.buttonsToolStrip.BackColor = Color.Transparent;
			this.buttonsToolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
			this.buttonsToolStrip.Location = new Point(16, 28);
			this.buttonsToolStrip.Margin = new Padding(3, 3, 0, 0);
			this.buttonsToolStrip.Name = "buttonsToolStrip";
			this.buttonsToolStrip.Size = new Size(386, 20);
			this.buttonsToolStrip.Stretch = true;
			this.buttonsToolStrip.TabIndex = 0;
			this.buttonsToolStrip.TabStop = true;
			this.buttonsToolStrip.Text = "buttonsToolStrip";
			this.propertiesButton.Enabled = false;
			this.propertiesButton.ImageTransparentColor = Color.Magenta;
			this.propertiesButton.Name = "propertiesButton";
			this.propertiesButton.Size = new Size(64, 19);
			this.propertiesButton.Text = "properties";
			this.propertiesButton.Click += this.PropertiesButton_Click;
			this.enableButton.Enabled = false;
			this.enableButton.ImageTransparentColor = Color.Magenta;
			this.enableButton.Name = "enableButton";
			this.enableButton.Size = new Size(46, 19);
			this.enableButton.Text = "enable";
			this.enableButton.Click += this.EnableButton_Click;
			this.disableButton.Enabled = false;
			this.disableButton.ImageTransparentColor = Color.Magenta;
			this.disableButton.Name = "disableButton";
			this.disableButton.Size = new Size(48, 19);
			this.disableButton.Text = "disable";
			this.disableButton.Click += this.DisableButton_Click;
			this.infoLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.infoLabel.AutoSize = true;
			this.infoLabel.Location = new Point(0, 0);
			this.infoLabel.Margin = new Padding(0);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new Size(318, 13);
			this.infoLabel.TabIndex = 4;
			this.infoLabel.Text = "infoLabel";
			this.infoLabel.Visible = false;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "FeatureLauncherPropertyControl";
			base.Size = new Size(318, 279);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.buttonsToolStrip.ResumeLayout(false);
			this.buttonsToolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		[DefaultValue(true)]
		public bool EnablingButtonsVisible
		{
			get
			{
				return this.enablingButtonsVisible;
			}
			set
			{
				if (value != this.EnablingButtonsVisible)
				{
					this.enablingButtonsVisible = value;
					this.UpdateControlVisible();
				}
			}
		}

		[DefaultValue(true)]
		public bool PropertiesButtonVisible
		{
			get
			{
				return this.propertiesButtonVisible;
			}
			set
			{
				if (value != this.PropertiesButtonVisible)
				{
					this.propertiesButtonVisible = value;
					this.UpdateControlVisible();
				}
			}
		}

		[DefaultValue("")]
		public string InfoLabelText
		{
			get
			{
				return this.infoLabel.Text;
			}
			set
			{
				if (value != this.InfoLabelText)
				{
					this.infoLabel.Text = value;
					this.UpdateControlVisible();
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected FeatureLauncherListViewItem CurrentFeature
		{
			get
			{
				return this.currentFeature;
			}
			set
			{
				this.currentFeature = value;
			}
		}

		[DefaultValue(ColumnHeaderStyle.Clickable)]
		public ColumnHeaderStyle HeaderStyle
		{
			get
			{
				return this.featureListView.HeaderStyle;
			}
			set
			{
				this.featureListView.HeaderStyle = value;
			}
		}

		[DefaultValue(184)]
		public int FeatureListViewHeight
		{
			get
			{
				return this.featureListView.Height;
			}
			set
			{
				this.featureListView.Height = value;
			}
		}

		public FeatureLauncherListViewItem this[string featureName]
		{
			get
			{
				return (FeatureLauncherListViewItem)this.featureListView.Items[featureName];
			}
		}

		public void Add(FeatureLauncherListViewItem item)
		{
			this.featureListView.Items.Add(item);
			if (item.Icon != null)
			{
				this.iconLibrary.Icons.Add(item.Text, item.Icon);
				item.ImageIndex = this.iconLibrary.Icons.Count - 1;
			}
		}

		protected void RefreshList()
		{
			this.featureListView.BeginUpdate();
			this.featureListView.Sort();
			this.featureListView.Focus();
			if (this.CurrentFeature != null && this.CurrentFeature.Index >= 0)
			{
				this.featureListView.EnsureVisible(this.CurrentFeature.Index);
			}
			this.featureListView.EndUpdate();
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			this.RefreshList();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.UpdateControlVisible();
		}

		private void UpdateControlVisible()
		{
			if (base.IsHandleCreated)
			{
				base.SuspendLayout();
				this.infoLabel.Visible = !string.IsNullOrEmpty(this.InfoLabelText);
				this.propertiesButton.Visible = this.PropertiesButtonVisible;
				this.enableButton.Visible = this.EnablingButtonsVisible;
				this.disableButton.Visible = this.EnablingButtonsVisible;
				this.featureListView.View = (this.EnablingButtonsVisible ? View.Details : View.List);
				base.ResumeLayout(false);
				base.PerformLayout();
			}
		}

		protected virtual void UpdateStatusWhenSelectionChanged()
		{
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			if (this.featureListView.SelectedIndices.Count > 0)
			{
				this.CurrentFeature = (FeatureLauncherListViewItem)this.featureListView.FirstSelectedItem;
				string text = this.CurrentFeature.Description;
				bool flag = true;
				if (this.CurrentFeature.IsBanned)
				{
					flag = false;
					text = this.currentFeature.BannedMessage;
				}
				if (flag)
				{
					if (this.CurrentFeature.CanChangeStatus)
					{
						this.enableButton.Enabled = (FeatureStatus.Disabled == this.CurrentFeature.Status);
						this.disableButton.Enabled = (FeatureStatus.Enabled == this.CurrentFeature.Status);
					}
					this.SetPropertyButtonEnabled();
				}
				else
				{
					this.enableButton.Enabled = false;
					this.disableButton.Enabled = false;
					this.propertiesButton.Enabled = false;
				}
				this.itemDescriptionLabel.Text = text;
			}
			else
			{
				this.CurrentFeature = null;
				this.itemDescriptionLabel.Text = Strings.DefaultItemDescriptionText;
				this.enableButton.Enabled = false;
				this.disableButton.Enabled = false;
				this.propertiesButton.Enabled = false;
			}
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
			this.OnFeatureItemUpdated(EventArgs.Empty);
		}

		private void DescriptionLabelTextChanged(object sender, EventArgs args)
		{
			this.itemDescriptionLabel.Visible = !string.IsNullOrEmpty(this.itemDescriptionLabel.Text);
		}

		private void SetPropertyButtonEnabled()
		{
			this.propertiesButton.Enabled = (null != this.CurrentFeature.PropertyPageControl && (!this.CurrentFeature.EnablePropertiesButtonOnFeatureStatus || FeatureStatus.Enabled == this.CurrentFeature.Status));
		}

		private void EnableButton_Click(object sender, EventArgs e)
		{
			this.SetCurrentFeatureStatus(FeatureStatus.Enabled);
		}

		private void DisableButton_Click(object sender, EventArgs e)
		{
			this.SetCurrentFeatureStatus(FeatureStatus.Disabled);
		}

		private void SetCurrentFeatureStatus(FeatureStatus status)
		{
			this.CurrentFeature.Status = status;
			bool flag = FeatureStatus.Enabled == status;
			this.enableButton.Enabled = !flag;
			this.disableButton.Enabled = flag;
			this.SetPropertyButtonEnabled();
			this.featureListView.Sort();
			base.NotifyExposedPropertyIsModified(this.CurrentFeature.StatusBindingName);
		}

		private void PropertiesButton_Click(object sender, EventArgs e)
		{
			ExchangePropertyPageControl exchangePropertyPageControl = (ExchangePropertyPageControl)Activator.CreateInstance(this.CurrentFeature.PropertyPageControl);
			exchangePropertyPageControl.Context = new DataContext(AutomatedNestedDataHandler.CreateDataHandlerWithParentSchema(this.ParentPage.Context));
			this.ParentPage.ShowDialog(exchangePropertyPageControl);
		}

		private void featureListView_ItemActivate(object sender, EventArgs e)
		{
			if (this.propertiesButton.Enabled)
			{
				this.PropertiesButton_Click(sender, e);
			}
		}

		private void featureListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (this.featureListComparer == null)
			{
				this.featureListComparer = new FeatureLauncherPropertyControl.FeatureListItemComparer(e.Column, ListSortDirection.Ascending);
				this.featureListView.ListViewItemSorter = this.featureListComparer;
				return;
			}
			if (e.Column == this.featureListComparer.SortColumn)
			{
				if (this.featureListComparer.SortDirection == ListSortDirection.Ascending)
				{
					this.featureListComparer.SortDirection = ListSortDirection.Descending;
				}
				else
				{
					this.featureListComparer.SortDirection = ListSortDirection.Ascending;
				}
			}
			else
			{
				this.featureListComparer.SortColumn = e.Column;
			}
			this.featureListView.Sort();
		}

		internal DataListView FeatureListView
		{
			get
			{
				return this.featureListView;
			}
		}

		internal ToolStripButton PropertiesButton
		{
			get
			{
				return this.propertiesButton;
			}
		}

		internal ToolStripButton EnableButton
		{
			get
			{
				return this.enableButton;
			}
		}

		internal ToolStripButton DisableButton
		{
			get
			{
				return this.disableButton;
			}
		}

		public event EventHandler FeatureItemUpdated;

		private void OnFeatureItemUpdated(EventArgs e)
		{
			if (this.FeatureItemUpdated != null)
			{
				this.FeatureItemUpdated(this, e);
			}
		}

		protected override BulkEditorAdapter CreateBulkEditorAdapter()
		{
			return new FeatureLauncherBulkEditorAdapter(this);
		}

		private FeatureLauncherListViewItem GetFeatureListItemByPropertyName(string propertyName)
		{
			foreach (object obj in this.featureListView.Items)
			{
				FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
				if (featureLauncherListViewItem.StatusBindingName.Equals(propertyName))
				{
					return featureLauncherListViewItem;
				}
			}
			return null;
		}

		void ISpecifyPropertyState.SetPropertyState(string propertyName, PropertyState state, string message)
		{
			FeatureLauncherListViewItem featureListItemByPropertyName = this.GetFeatureListItemByPropertyName(propertyName);
			featureListItemByPropertyName.IsBanned = (state == PropertyState.UnsupportedVersion);
			featureListItemByPropertyName.BannedMessage = message;
			this.UpdateStatusWhenSelectionChanged();
		}

		object IGetInnerObject.GetObject(string identity)
		{
			return this.GetFeatureListItemByUniqueName(identity);
		}

		public override PropertyDescriptorCollection GetCustomProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			foreach (object obj in this.featureListView.Items)
			{
				FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(featureLauncherListViewItem)["Status"];
				propertyDescriptorCollection.Add(new DynamicPropertyDescriptor(base.GetType(), featureLauncherListViewItem.UniqueName, propertyDescriptor));
			}
			return propertyDescriptorCollection;
		}

		private FeatureLauncherListViewItem GetFeatureListItemByUniqueName(string identity)
		{
			foreach (object obj in this.featureListView.Items)
			{
				FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
				if (featureLauncherListViewItem.UniqueName.Equals(identity))
				{
					return featureLauncherListViewItem;
				}
			}
			return null;
		}

		public const bool DefaultEnablingButtonsVisible = true;

		public const bool DefaultPropertiesButtonVisible = true;

		private const int DefaultFeatureListViewHeight = 184;

		private DataListView featureListView;

		private AutoHeightLabel itemDescriptionDividerLabel;

		private ExchangeColumnHeader featureColumnHeader;

		private ExchangeColumnHeader statusColumnHeader;

		private FeatureLauncherPropertyControl.FeatureListItemComparer featureListComparer;

		private bool enablingButtonsVisible = true;

		private bool propertiesButtonVisible = true;

		private FeatureLauncherListViewItem currentFeature;

		private Label infoLabel;

		private AutoTableLayoutPanel tableLayoutPanel;

		private Label itemDescriptionLabel;

		private IconLibrary iconLibrary;

		private TabbableToolStrip buttonsToolStrip;

		private ToolStripButton propertiesButton;

		private ToolStripButton enableButton;

		private ToolStripButton disableButton;

		private BindingSource dataSource;

		private class FeatureListItemComparer : IComparer
		{
			public FeatureListItemComparer(int sortColumn, ListSortDirection sortDirection)
			{
				this.sortColumn = sortColumn;
				this.sortDirection = sortDirection;
			}

			public int SortColumn
			{
				get
				{
					return this.sortColumn;
				}
				set
				{
					this.sortColumn = value;
				}
			}

			public ListSortDirection SortDirection
			{
				get
				{
					return this.sortDirection;
				}
				set
				{
					this.sortDirection = value;
				}
			}

			public int Compare(object x, object y)
			{
				string text = ((ListViewItem)x).SubItems[this.SortColumn].Text;
				string text2 = ((ListViewItem)y).SubItems[this.SortColumn].Text;
				int result;
				if (this.SortDirection == ListSortDirection.Ascending)
				{
					result = string.Compare(text, text2, false, CultureInfo.CurrentCulture);
				}
				else
				{
					result = string.Compare(text2, text, false, CultureInfo.CurrentCulture);
				}
				return result;
			}

			private int sortColumn;

			private ListSortDirection sortDirection;
		}
	}
}
