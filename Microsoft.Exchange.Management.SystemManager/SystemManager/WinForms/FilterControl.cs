using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FilterControl : ExchangeUserControl
	{
		public FilterControl()
		{
			this.InitializeComponent();
			base.Name = "FilterControl";
			this.progressTimer = new Timer();
			this.progressTimer.Interval = 500;
			this.progressTimer.Tick += this.ProgressTimer_Tick;
			this.propertiesToFilter = new BindingList<FilterablePropertyDescription>();
			this.propertiesToFilter.ListChanged += this.propertiesToFilter_ListChanged;
			this.filterNodes = new BindingList<FilterNode>();
			this.filterNodes.ListChanged += this.filterNodes_ListChanged;
			this.SupportsOrOperator = true;
			this.AutoValidate = AutoValidate.EnableAllowFocusChange;
			if (!FilterControl.ShowProgressIndicator)
			{
				this.buttons.Items.Remove(this.progressIndicator);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoValidate AutoValidate
		{
			get
			{
				return base.AutoValidate;
			}
			set
			{
				base.AutoValidate = value;
			}
		}

		private void propertiesToFilter_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				List<FilterablePropertyDescription> list = new List<FilterablePropertyDescription>(this.propertiesToFilter);
				list.Sort();
				this.propertiesToFilter.RaiseListChangedEvents = false;
				this.propertiesToFilter.Clear();
				foreach (FilterablePropertyDescription item in list)
				{
					this.propertiesToFilter.Add(item);
				}
				this.propertiesToFilter.RaiseListChangedEvents = true;
				this.UpdateVisibile();
			}
		}

		private void UpdateVisibile()
		{
			if (this.DataSource != null && this.DataSource.SupportsFiltering)
			{
				foreach (object obj in this.FilterItems)
				{
					FilterItem filterItem = (FilterItem)obj;
					filterItem.BeginInit();
				}
				base.Visible = (this.PropertiesToFilter.Count > 0);
				foreach (object obj2 in this.FilterItems)
				{
					FilterItem filterItem2 = (FilterItem)obj2;
					filterItem2.EndInit();
				}
			}
		}

		private void InitializeComponent()
		{
			this.incompleteItems = new ArrayList();
			this.items = new Panel();
			this.buttons = new TabbableToolStrip();
			this.createFilterButton = new ToolStripButton(Strings.CreateFilter.ToString());
			this.createFilterButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.CreateFilter);
			this.addNewFilterButton = new ToolStripButton(Strings.AddFilter.ToString());
			this.addNewFilterButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.AddFilter);
			this.removeFilterButton = new ToolStripButton(Strings.RemoveFilter.ToString());
			this.removeFilterButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.RemoveFilter);
			this.applyFilterButton = new ToolStripButton(Strings.ApplyFilter.ToString());
			this.applyFilterButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.ApplyFilter);
			this.stopFilterButton = new ToolStripButton(Strings.StopFilter.ToString());
			this.stopFilterButton.ToolTipText = ExchangeUserControl.RemoveAccelerator(Strings.StopFilter);
			this.progressIndicator = new ToolStripLabel();
			this.buttons.SuspendLayout();
			base.SuspendLayout();
			this.items.Name = "filterItemsPanel";
			this.items.AutoScroll = true;
			this.items.AutoSize = true;
			this.items.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.items.ControlRemoved += this.FilterItemRemoved;
			this.items.Dock = DockStyle.Top;
			this.items.Margin = new Padding(0);
			this.items.TabIndex = 0;
			this.items.TabStop = true;
			this.buttons.Items.AddRange(new ToolStripItem[]
			{
				this.createFilterButton,
				this.addNewFilterButton,
				this.removeFilterButton,
				this.applyFilterButton,
				this.stopFilterButton,
				this.progressIndicator
			});
			this.buttons.GripStyle = ToolStripGripStyle.Hidden;
			this.buttons.Name = "toolstripButtons";
			this.buttons.Dock = DockStyle.Bottom;
			this.buttons.TabStop = true;
			this.buttons.TabIndex = 1;
			this.iconLibrary = new IconLibrary();
			this.iconLibrary.Icons.Add("Create", Icons.CreateFilter);
			this.iconLibrary.Icons.Add("Add", Icons.Add);
			this.iconLibrary.Icons.Add("Remove", Icons.RemoveFilter);
			this.iconLibrary.Icons.Add("Move", Icons.Move);
			this.buttons.ImageList = this.iconLibrary.SmallImageList;
			this.createFilterButton.ImageIndex = 0;
			this.createFilterButton.Name = "createFilterButton";
			this.createFilterButton.Click += this.createFilterButton_Click;
			this.addNewFilterButton.ImageIndex = 1;
			this.addNewFilterButton.Name = "addNewFilterButton";
			this.addNewFilterButton.Click += this.addNewFilter_Click;
			this.removeFilterButton.ImageIndex = 2;
			this.removeFilterButton.Name = "removeFilterButton";
			this.removeFilterButton.Click += this.removeFilterButton_Click;
			this.applyFilterButton.ImageIndex = 3;
			this.applyFilterButton.Name = "applyFilterButton";
			this.applyFilterButton.Click += this.applyFilterButton_Click;
			this.applyFilterButton.Alignment = ToolStripItemAlignment.Right;
			this.stopFilterButton.Name = "stopFilterButton";
			this.stopFilterButton.Click += this.stopFilterButton_Click;
			this.stopFilterButton.Alignment = ToolStripItemAlignment.Right;
			this.progressIndicator.Name = "progressIndicator";
			this.progressIndicator.Alignment = ToolStripItemAlignment.Right;
			this.progressIndicator.Image = Icons.Progress;
			base.Controls.Add(this.buttons);
			base.Controls.Add(this.items);
			this.AutoSize = true;
			this.Dock = DockStyle.Top;
			base.DockPadding.Top = 2;
			base.DockPadding.Bottom = 6;
			this.TabStop = true;
			this.buttons.ResumeLayout(false);
			this.buttons.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.progressTimer.Dispose();
				this.DataSource = null;
			}
			this.IsFiltering = false;
			base.Dispose(disposing);
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

		[DefaultValue(DockStyle.Top)]
		public override DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				base.Dock = value;
			}
		}

		[DefaultValue(true)]
		public new bool TabStop
		{
			get
			{
				return base.TabStop;
			}
			set
			{
				base.TabStop = value;
			}
		}

		protected override Padding DefaultPadding
		{
			get
			{
				return new Padding(0, 2, 0, 6);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal IList<FilterablePropertyDescription> PropertiesToFilter
		{
			get
			{
				return this.propertiesToFilter;
			}
		}

		private Control.ControlCollection FilterItems
		{
			get
			{
				return this.items.Controls;
			}
		}

		public bool IsApplied
		{
			get
			{
				return this.isApplied;
			}
			protected set
			{
				if (this.isApplied != value)
				{
					this.isApplied = value;
					this.OnIsAppliedChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsAppliedChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[FilterControl.EventIsAppliedChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsAppliedChanged
		{
			add
			{
				base.Events.AddHandler(FilterControl.EventIsAppliedChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(FilterControl.EventIsAppliedChanged, value);
			}
		}

		protected QueryFilter FilterExpressionTree
		{
			get
			{
				if (this.isFilterTreeStale)
				{
					if (this.filterNodes.Count > 0)
					{
						Hashtable hashtable = new Hashtable();
						foreach (FilterNode filterNode in this.filterNodes)
						{
							List<QueryFilter> list = hashtable[filterNode.PropertyDefinition] as List<QueryFilter>;
							if (list != null)
							{
								list.Add(filterNode.QueryFilter);
							}
							else
							{
								list = new List<QueryFilter>();
								list.Add(filterNode.QueryFilter);
								hashtable.Add(filterNode.PropertyDefinition, list);
							}
						}
						List<QueryFilter> list2 = new List<QueryFilter>(hashtable.Keys.Count);
						foreach (object obj in hashtable.Values)
						{
							List<QueryFilter> list3 = obj as List<QueryFilter>;
							if (list3.Count > 1 && this.SupportsOrOperator)
							{
								list2.Add(new OrFilter(list3.ToArray()));
							}
							else
							{
								list2.AddRange(list3);
							}
						}
						this.filterExpressionTree = new AndFilter(list2.ToArray());
					}
					else
					{
						this.filterExpressionTree = null;
					}
					this.isFilterTreeStale = false;
				}
				return this.filterExpressionTree;
			}
		}

		[DefaultValue("")]
		public string Expression
		{
			get
			{
				if (this.isFilterStringStale)
				{
					if (this.FilterExpressionTree != null)
					{
						this.filterExpressionString = this.FilterExpressionTree.GenerateInfixString(FilterLanguage.Monad);
					}
					else
					{
						this.filterExpressionString = string.Empty;
					}
					this.isFilterStringStale = false;
				}
				return this.filterExpressionString;
			}
		}

		public event EventHandler ExpressionChanged;

		private void OnExpressionChanged()
		{
			this.isFilterTreeStale = true;
			if (this.isInitializing)
			{
				return;
			}
			try
			{
				this.isFilterStringStale = true;
				if (this.DataSource != null)
				{
					this.DataSource.ApplyFilter(this.FilterExpressionTree);
				}
				this.IsApplied = true;
				this.persistedExpression = ((this.FilterExpressionTree != null) ? WinformsHelper.Serialize(this.FilterExpressionTree) : null);
				this.OnExpressionChanged(EventArgs.Empty);
			}
			catch (InvalidExpressionException ex)
			{
				base.ShowError(ex.Message);
			}
		}

		protected virtual void OnExpressionChanged(EventArgs e)
		{
			if (this.ExpressionChanged != null && this.notificationsActive)
			{
				this.ExpressionChanged(this, e);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public byte[] PersistedExpression
		{
			get
			{
				return this.persistedExpression;
			}
			set
			{
				try
				{
					base.SuspendLayout();
					this.notificationsActive = false;
					this.isInitializing = true;
					Control[] array = new Control[this.items.Controls.Count];
					this.items.Controls.CopyTo(array, 0);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] is FilterItem)
						{
							array[i].Dispose();
						}
					}
					List<FilterNode> nodesFromSerializedQueryFilter = FilterNode.GetNodesFromSerializedQueryFilter(value, this.PropertiesToFilter, this.ObjectSchema);
					foreach (FilterNode item in nodesFromSerializedQueryFilter)
					{
						this.filterNodes.Add(item);
					}
				}
				finally
				{
					this.isInitializing = false;
					this.notificationsActive = true;
					this.OnExpressionChanged();
					base.ResumeLayout();
				}
			}
		}

		internal ObjectSchema ObjectSchema
		{
			get
			{
				return this.objectSchema;
			}
			set
			{
				this.objectSchema = value;
			}
		}

		[DefaultValue(null)]
		public IAdvancedBindingListView DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				if (value != this.DataSource)
				{
					if (this.DataSource != null)
					{
						this.DataSource.FilteringChanged -= this.DataSource_FilteringChanged;
						this.IsFiltering = false;
						this.useFilterWithProgress = false;
					}
					this.dataSource = value;
					if (this.DataSource != null)
					{
						this.DataSource.FilteringChanged += this.DataSource_FilteringChanged;
						this.useFilterWithProgress = this.DataSource.SupportCancelFiltering;
						this.IsFiltering = this.DataSource.Filtering;
					}
					this.UpdateVisibile();
					if (this.dataSource != null && this.PersistedExpression != null)
					{
						this.OnExpressionChanged();
					}
				}
			}
		}

		[DefaultValue(true)]
		public bool SupportsOrOperator
		{
			get
			{
				return this.supportsOrOperator;
			}
			set
			{
				this.supportsOrOperator = value;
			}
		}

		private void addNewFilter_Click(object sender, EventArgs e)
		{
			FilterNode filterNode = new FilterNode();
			filterNode.FilterablePropertyDescription = this.PropertiesToFilter[0];
			filterNode.Operator = (PropertyFilterOperator)this.PropertiesToFilter[0].SupportedOperators.Values.GetValue(0);
			this.incompleteItems.Add(filterNode);
			this.filterNodes.Add(filterNode);
			this.filterNode_PropertyChanged(filterNode, EventArgs.Empty);
		}

		private void filterNodes_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.BindToNodePropertyChanges(this.filterNodes[e.NewIndex]);
				this.InsertFilterItem(this.CreateItemForNode(this.filterNodes[e.NewIndex], this.propertiesToFilter));
			}
			this.IsApplied = false;
			this.SetButtonsVisibility();
		}

		internal virtual FilterItem CreateItemForNode(FilterNode node, IList<FilterablePropertyDescription> propertiesToFilter)
		{
			return new FilterItem(node, propertiesToFilter);
		}

		private void BindToNodePropertyChanges(FilterNode node)
		{
			node.FilterablePropertyDescriptionChanged += this.filterNode_PropertyChanged;
			node.OperatorChanged += this.filterNode_PropertyChanged;
			node.ValueChanged += this.filterNode_PropertyChanged;
		}

		private void UnbindFromNodePropertyChanges(FilterNode node)
		{
			node.FilterablePropertyDescriptionChanged -= this.filterNode_PropertyChanged;
			node.OperatorChanged -= this.filterNode_PropertyChanged;
			node.ValueChanged -= this.filterNode_PropertyChanged;
		}

		private void filterNode_PropertyChanged(object sender, EventArgs e)
		{
			FilterNode filterNode = sender as FilterNode;
			if (filterNode.IsComplete)
			{
				this.incompleteItems.Remove(filterNode);
			}
			else if (!this.incompleteItems.Contains(filterNode))
			{
				this.incompleteItems.Add(filterNode);
			}
			this.IsApplied = false;
			this.SetButtonsVisibility();
		}

		private void removeFilterButton_Click(object sender, EventArgs e)
		{
			while (this.FilterItems.Count > 0)
			{
				Control control = this.FilterItems[0];
				this.FilterItems.RemoveAt(0);
				control.Dispose();
			}
		}

		private void applyFilterButton_Click(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FilterNode filterNode in this.filterNodes)
			{
				string value = filterNode.Validate();
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.AppendLine(value);
				}
			}
			if (stringBuilder.Length == 0)
			{
				this.OnExpressionChanged();
				return;
			}
			base.ShowError(stringBuilder.ToString());
		}

		private void createFilterButton_Click(object sender, EventArgs e)
		{
			this.addNewFilter_Click(this.addNewFilterButton, EventArgs.Empty);
		}

		private void stopFilterButton_Click(object sender, EventArgs e)
		{
			if (this.DataSource.SupportCancelFiltering)
			{
				this.stopFilterButton.Enabled = false;
				this.DataSource.CancelFiltering();
			}
		}

		protected void InsertFilterItem(FilterItem item)
		{
			item.BeginInit();
			item.Name = string.Format(CultureInfo.InvariantCulture, "Expression{0}", new object[]
			{
				this.FilterItems.Count.ToString(CultureInfo.InvariantCulture)
			});
			this.FilterItems.Add(item);
			this.FilterItems.SetChildIndex(item, 0);
			item.EndInit();
			base.Focus();
			base.SelectNextControl(item, true, true, true, false);
		}

		private void FilterItemRemoved(object sender, ControlEventArgs e)
		{
			FilterItem filterItem = e.Control as FilterItem;
			if (filterItem != null)
			{
				this.UnbindFromNodePropertyChanges(filterItem.FilterNode);
				if (this.incompleteItems.Contains(filterItem.FilterNode))
				{
					this.incompleteItems.Remove(filterItem.FilterNode);
				}
				this.filterNodes.Remove(filterItem.FilterNode);
			}
			if (this.FilterItems.Count == 0)
			{
				this.OnExpressionChanged();
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			this.SetButtonsVisibility();
			this.items.MaximumSize = new Size(0, (this.buttons.Height + 2) * 5);
		}

		private void SetButtonsVisibility()
		{
			base.SuspendLayout();
			bool flag = this.FilterItems.Count > 0;
			this.createFilterButton.Visible = !flag;
			this.removeFilterButton.Visible = flag;
			this.removeFilterButton.Enabled = (!this.IsFiltering || (this.FilterItems.Count == 1 && this.incompleteItems.Count == 1));
			this.applyFilterButton.Visible = (flag && !this.IsFiltering);
			this.applyFilterButton.Enabled = (this.incompleteItems.Count == 0);
			this.addNewFilterButton.Visible = flag;
			this.addNewFilterButton.Enabled = (!this.IsFiltering && this.incompleteItems.Count == 0 && this.FilterItems.Count < 10);
			this.progressIndicator.Visible = this.IsFiltering;
			this.stopFilterButton.Visible = this.IsFiltering;
			this.items.Enabled = !this.IsFiltering;
			base.ResumeLayout();
		}

		public bool IsFiltering
		{
			get
			{
				return this.isFiltering;
			}
			private set
			{
				if (value != this.isFiltering)
				{
					this.isFiltering = value;
					if (this.useFilterWithProgress)
					{
						this.SetButtonsVisibility();
					}
				}
			}
		}

		private void DataSource_FilteringChanged(object sender, EventArgs e)
		{
			if (this.DataSource.Filtering)
			{
				this.stopFilterButton.Enabled = this.DataSource.SupportCancelFiltering;
				this.IsFiltering = true;
				this.progressTimer.Stop();
				return;
			}
			if (this.stopFilterButton.Enabled)
			{
				this.stopFilterButton.Enabled = false;
				this.progressTimer.Start();
				return;
			}
			this.IsFiltering = false;
		}

		private void ProgressTimer_Tick(object sender, EventArgs e)
		{
			this.progressTimer.Stop();
			this.IsFiltering = false;
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool result;
			if (keyData == Keys.Return)
			{
				this.applyFilterButton.PerformClick();
				result = true;
			}
			else
			{
				result = base.ProcessDialogKey(keyData);
			}
			return result;
		}

		private const int maxExpressionsBeforeScroll = 5;

		private const int maxExpressionsTotal = 10;

		private ToolStripButton addNewFilterButton;

		private ToolStripButton applyFilterButton;

		private ToolStripButton removeFilterButton;

		private ToolStripButton createFilterButton;

		private ToolStripButton stopFilterButton;

		private ToolStripLabel progressIndicator;

		private TabbableToolStrip buttons;

		private Panel items;

		private IconLibrary iconLibrary;

		private bool isInitializing;

		private bool useFilterWithProgress;

		private bool isFiltering;

		private BindingList<FilterNode> filterNodes;

		private Timer progressTimer;

		private ArrayList incompleteItems;

		internal static bool ShowProgressIndicator = true;

		private BindingList<FilterablePropertyDescription> propertiesToFilter;

		private bool isApplied = true;

		private static readonly object EventIsAppliedChanged = new object();

		private QueryFilter filterExpressionTree;

		private bool isFilterTreeStale = true;

		private bool isFilterStringStale = true;

		private string filterExpressionString = string.Empty;

		private byte[] persistedExpression;

		private ObjectSchema objectSchema;

		private bool notificationsActive = true;

		private IAdvancedBindingListView dataSource;

		private bool supportsOrOperator;
	}
}
