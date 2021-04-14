using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.Tasks;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[ComplexBindingProperties("DataSource", "DataMember")]
	[Docking(DockingBehavior.AutoDock)]
	public class DataListView : ListView, IBulkEditor, IDataListViewBulkEditSupport, IBulkEditSupport
	{
		public DataListView()
		{
			this.AllowColumnReorder = true;
			this.DoubleBuffered = true;
			this.availableColumns = new ExchangeColumnHeaderCollection();
			this.availableColumns.ListChanged += this.availableColumns_ListChanged;
			this.availableColumns.ListChanging += this.availableColumns_ListChanging;
			this.CreateDataBoundContextMenu();
			this.filterValues = new FilterValueCollection(this);
			this.FullRowSelect = true;
			this.HideSelection = false;
			base.ShowGroups = false;
			this.ShowItemToolTips = true;
			this.View = View.Details;
			Application.Idle += this.Application_Idle;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		public new bool AllowColumnReorder
		{
			get
			{
				return base.AllowColumnReorder;
			}
			set
			{
				base.AllowColumnReorder = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		[Browsable(false)]
		public new bool FullRowSelect
		{
			get
			{
				return base.FullRowSelect;
			}
			set
			{
				base.FullRowSelect = value;
			}
		}

		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new bool HideSelection
		{
			get
			{
				return base.HideSelection;
			}
			set
			{
				base.HideSelection = value;
			}
		}

		[DefaultValue(false)]
		public new bool ShowGroups
		{
			get
			{
				return base.ShowGroups;
			}
			set
			{
				if (this.ShowGroups != value)
				{
					base.BeginUpdate();
					try
					{
						this.showInGroupsCommand.Checked = value;
						base.ShowGroups = value;
						if (this.SupportsGrouping)
						{
							if (value)
							{
								if (base.Columns[0].Name != this.SortProperty)
								{
									this.groupProperty = this.SortProperty;
									this.groupDirection = this.SortDirection;
								}
								base.VirtualMode = false;
							}
							this.ApplySort();
						}
					}
					finally
					{
						base.EndUpdate();
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		[Browsable(false)]
		public new bool ShowItemToolTips
		{
			get
			{
				return base.ShowItemToolTips;
			}
			set
			{
				base.ShowItemToolTips = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowSubitemIcon
		{
			get
			{
				return this.showSubitemIcon;
			}
			set
			{
				this.showSubitemIcon = value;
				if (base.IsHandleCreated)
				{
					this.UpdateShowSubitemIcon();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(View.Details)]
		public new View View
		{
			get
			{
				return base.View;
			}
			set
			{
				if (base.View != value)
				{
					if (value == View.LargeIcon || value == View.Tile)
					{
						base.LargeImageList = ((this.IconLibrary != null) ? this.IconLibrary.LargeImageList : null);
					}
					base.View = value;
					this.ApplySmartWidthOfColumns();
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(-1)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedColumnIndex
		{
			get
			{
				return this.selectedColumnIndex;
			}
			set
			{
				if (this.SelectedColumnIndex != value)
				{
					this.selectedColumnIndex = value;
					if (base.IsHandleCreated)
					{
						UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4236, (IntPtr)value, (IntPtr)0);
					}
				}
			}
		}

		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool ShowFilter
		{
			get
			{
				return this.showfilter;
			}
			set
			{
				if (this.ShowFilter != value)
				{
					this.showfilter = value;
					if (this.showFilterCommand != null)
					{
						this.showFilterCommand.Checked = value;
					}
					if (base.IsHandleCreated)
					{
						ListViewItem focusedItem = base.FocusedItem;
						if (base.VirtualMode)
						{
							this.BackupItemsStates();
						}
						base.RecreateHandle();
						if (base.VirtualMode)
						{
							this.RestoreItemsStates(true);
						}
						if (focusedItem != null)
						{
							focusedItem.Focused = true;
						}
						base.Invalidate();
						if (!value)
						{
							this.OnFilterChanged(EventArgs.Empty);
						}
					}
				}
			}
		}

		[Browsable(false)]
		public string BindingListViewFilter
		{
			get
			{
				if (!this.ShowFilter)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				for (int i = 0; i < base.Columns.Count; i++)
				{
					ExchangeColumnHeader exchangeColumnHeader = base.Columns[i] as ExchangeColumnHeader;
					if (exchangeColumnHeader != null)
					{
						string bindingListViewFilter = exchangeColumnHeader.BindingListViewFilter;
						if (!string.IsNullOrEmpty(bindingListViewFilter))
						{
							if (flag)
							{
								stringBuilder.Append(" AND ");
							}
							stringBuilder.Append(bindingListViewFilter);
							flag = true;
						}
					}
				}
				return stringBuilder.ToString();
			}
		}

		private void WmNotifyFilterButtonClick(ref Message m)
		{
			NativeMethods.NMHDFILTERBTNCLICK nmhdfilterbtnclick = (NativeMethods.NMHDFILTERBTNCLICK)m.GetLParam(typeof(NativeMethods.NMHDFILTERBTNCLICK));
			FilterButtonClickEventArgs filterButtonClickEventArgs = new FilterButtonClickEventArgs(base.Columns[nmhdfilterbtnclick.iItem], Rectangle.FromLTRB(nmhdfilterbtnclick.rc.left, nmhdfilterbtnclick.rc.top, nmhdfilterbtnclick.rc.right, nmhdfilterbtnclick.rc.bottom));
			this.OnFilterButtonClick(filterButtonClickEventArgs);
			m.Result = (filterButtonClickEventArgs.FilterChanged ? ((IntPtr)1) : IntPtr.Zero);
		}

		protected virtual void OnFilterButtonClick(FilterButtonClickEventArgs e)
		{
			ExchangeColumnHeader exchangeColumnHeader = e.ColumnHeader as ExchangeColumnHeader;
			if (exchangeColumnHeader != null)
			{
				FilterOperator filterOperator = exchangeColumnHeader.FilterOperator;
				NativeMethods.SCROLLINFO scrollinfo = new NativeMethods.SCROLLINFO(4, 0, 0, 0, 0);
				UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, base.Handle), 0, scrollinfo);
				Size sz = new Size(scrollinfo.nPos, 0);
				exchangeColumnHeader.ContextMenu.Show(this, e.FilterButtonBounds.Location + e.FilterButtonBounds.Size - sz, LeftRightAlignment.Left);
				Application.DoEvents();
				e.FilterChanged = (filterOperator != exchangeColumnHeader.FilterOperator);
			}
		}

		private void WmNotifyFilterChange(ref Message m)
		{
			this.OnFilterChanged(EventArgs.Empty);
		}

		protected virtual void OnFilterChanged(EventArgs e)
		{
			if (this.SupportsFiltering)
			{
				string bindingListViewFilter = this.BindingListViewFilter;
				if (bindingListViewFilter != this.bindingListView.Filter)
				{
					if (base.VirtualMode)
					{
						this.BackupItemsStates();
					}
					this.bindingListView.Filter = bindingListViewFilter;
				}
			}
		}

		internal HandleRef HeaderHandle
		{
			get
			{
				return this.headerHandle;
			}
		}

		private HandleRef TooltipHandle
		{
			get
			{
				return this.tooltipHandle;
			}
		}

		public void EditFilter(int columnIndex)
		{
			this.EditFilter(columnIndex, true);
		}

		public void EditFilter(int columnIndex, bool discardUserChanges)
		{
			if (base.IsHandleCreated)
			{
				UnsafeNativeMethods.SendMessage(this.HeaderHandle, 4631, (IntPtr)columnIndex, discardUserChanges);
			}
		}

		public void ClearAllFilters()
		{
			this.ClearFilter(-1);
		}

		public void ClearFilter(int columnIndex)
		{
			if (base.IsHandleCreated)
			{
				UnsafeNativeMethods.SendMessage(this.HeaderHandle, 4632, (IntPtr)columnIndex, (IntPtr)0);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			HandleRef handleRef = new HandleRef(this, base.Handle);
			IntPtr handle = UnsafeNativeMethods.SendMessage(handleRef, 4127, (IntPtr)0, (IntPtr)0);
			this.headerHandle = new HandleRef(this, handle);
			IntPtr handle2 = UnsafeNativeMethods.SendMessage(handleRef, 4174, (IntPtr)0, (IntPtr)0);
			this.tooltipHandle = new HandleRef(this, handle2);
			if (this.ShowFilter)
			{
				int num = UnsafeNativeMethods.GetWindowLong(this.HeaderHandle, -16);
				num |= 256;
				UnsafeNativeMethods.SetWindowLong(this.HeaderHandle, -16, num);
			}
			base.Invalidate();
			UnsafeNativeMethods.InvalidateRect(handleRef, IntPtr.Zero, false);
			base.OnHandleCreated(e);
			this.UpdateHeaderSortArrow();
			this.UpdateShowSubitemIcon();
		}

		private void UpdateShowSubitemIcon()
		{
			UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4150, (IntPtr)2, (IntPtr)(this.ShowSubitemIcon ? 2 : 0));
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public FilterValueCollection FilterValues
		{
			get
			{
				return this.filterValues;
			}
		}

		private void CreateDataBoundContextMenu()
		{
			this.contextMenuCommand = new Command();
			this.arrangeByCommand = new Command();
			this.showInGroupsCommand = new Command();
			this.arrangeByCommand.Name = "arrangeByCommand";
			this.arrangeByCommand.Text = Strings.ArrangeBy;
			this.showInGroupsCommand.Name = "showInGroupsCommand";
			this.showInGroupsCommand.Text = Strings.ShowInGroups;
			this.showInGroupsCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.ShowGroups = !this.ShowGroups;
			};
			this.contextMenuCommand.Commands.AddRange(new Command[]
			{
				this.ShowFilterCommand,
				this.arrangeByCommand
			});
			this.contextMenuCommand.Name = "contextMenu";
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command ContextMenuCommand
		{
			get
			{
				return this.contextMenuCommand;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Command DeleteSelectionCommand
		{
			get
			{
				return this.deleteSelectionCommand;
			}
			set
			{
				this.deleteSelectionCommand = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command RefreshCommand
		{
			get
			{
				return this.refreshCommand;
			}
			set
			{
				this.refreshCommand = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command ShowSelectionPropertiesCommand
		{
			get
			{
				return this.showSelectionPropertiesCommand;
			}
			set
			{
				this.showSelectionPropertiesCommand = value;
			}
		}

		protected override void OnItemActivate(EventArgs e)
		{
			this.Application_Idle(this, EventArgs.Empty);
			base.OnItemActivate(e);
			if (this.showSelectionPropertiesCommand != null && this.showSelectionPropertiesCommand.Enabled)
			{
				this.showSelectionPropertiesCommand.Invoke();
			}
		}

		[DefaultValue(-1)]
		public int ImageIndex
		{
			get
			{
				return this.imageIndex;
			}
			set
			{
				this.imageIndex = value;
			}
		}

		protected bool SupportsSearching
		{
			get
			{
				return this.bindingList != null && this.bindingList.SupportsSearching;
			}
		}

		protected bool SupportsSorting
		{
			get
			{
				return this.bindingList != null && this.bindingList.SupportsSorting;
			}
		}

		protected bool SupportsGrouping
		{
			get
			{
				return !base.VirtualMode && this.bindingListView != null && this.bindingListView.SupportsAdvancedSorting && base.Columns.Count > 1;
			}
		}

		protected bool SupportsFiltering
		{
			get
			{
				return this.bindingListView != null && this.bindingListView.SupportsFiltering;
			}
		}

		protected internal bool IsCreatingItems
		{
			get
			{
				return this.isCreatingItems;
			}
		}

		private bool IsShowingGroups
		{
			get
			{
				return this.SupportsGrouping && this.ShowGroups && null != this.GroupProperty;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DataSource = null;
				this.DataSourceRefresher = null;
				this.contextMenuCommand.Dispose();
				Application.Idle -= this.Application_Idle;
				this.IconLibrary = null;
			}
			base.Dispose(disposing);
		}

		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		[AttributeProvider(typeof(IListSource))]
		[Description("Indicates the source of data for the list view control.")]
		[Category("Data")]
		public object DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				if (this.DataSource != value)
				{
					if (value != null && !(value is IList) && !(value is IListSource))
					{
						throw new ArgumentException(Strings.BadDataSourceForComplexBinding, "value");
					}
					this.EnforceValidDataMember(value);
					if (this.DataSource != null && this.DataSource == this.DataSourceRefresher)
					{
						this.DataSourceRefresher = null;
					}
					this.SetListManager(value, this.dataMember);
					if (this.DataSource != null && this.DataSourceRefresher == null)
					{
						this.DataSourceRefresher = (this.DataSource as IRefreshableNotification);
					}
				}
			}
		}

		public IRefreshableNotification DataSourceRefresher
		{
			get
			{
				return this.dataSourceRefresher;
			}
			set
			{
				if (this.DataSourceRefresher != value)
				{
					bool isDataSourceRefreshing = this.IsDataSourceRefreshing;
					if (this.DataSourceRefresher != null)
					{
						this.DataSourceRefresher.RefreshingChanged -= this.DataSourceRefresher_RefreshingChanged;
					}
					this.dataSourceRefresher = value;
					if (this.DataSourceRefresher != null)
					{
						this.DataSourceRefresher.RefreshingChanged += this.DataSourceRefresher_RefreshingChanged;
						if (isDataSourceRefreshing != this.IsDataSourceRefreshing)
						{
							this.DataSourceRefresher_RefreshingChanged(this.DataSourceRefresher, EventArgs.Empty);
						}
					}
				}
			}
		}

		private bool IsDataSourceRefreshing
		{
			get
			{
				return this.DataSourceRefresher != null && this.DataSourceRefresher.Refreshing;
			}
		}

		private bool DataSourceRefreshed
		{
			get
			{
				return this.DataSourceRefresher != null && this.DataSourceRefresher.Refreshed;
			}
		}

		private void DataSourceRefresher_RefreshingChanged(object sender, EventArgs e)
		{
			bool refreshing = ((IRefreshableNotification)sender).Refreshing;
			if (refreshing)
			{
				this.Cursor = Cursors.AppStarting;
				this.BackupItemsStates();
				return;
			}
			this.Cursor = Cursors.Default;
			if (this.swCreateItems != null)
			{
				this.swCreateItems.Stop();
				this.swCreateItems = null;
			}
			if (base.IsHandleCreated)
			{
				if (this.needCreateItemsForRows)
				{
					this.EnsureItemsCreated();
				}
				else if (base.VirtualMode && !string.IsNullOrEmpty(this.SortProperty) && !(this.list is IPagedList))
				{
					this.CreateItems();
				}
				else
				{
					this.ApplySmartWidthOfColumns();
					this.TrySelectItemBySpecifiedIdentity();
				}
				this.Application_Idle(this, EventArgs.Empty);
				if (this.FocusedItemIndex < 0 && this.list.Count > 0)
				{
					if (base.SelectedIndices.Count > 0)
					{
						this.FirstSelectedItem.Focused = true;
					}
					else
					{
						this.ListManager.Position = 0;
						this.ListManager_PositionChanged(this.ListManager, EventArgs.Empty);
					}
					this.EnsureFocusedItemVisibleAsBefore();
				}
				this.ClearItemStatesBackup();
				this.selectedItemIdentity = null;
				this.UpdateNoResultsIndicator();
			}
		}

		private void BackupOffsetFocusedItem()
		{
			if (this.View == View.Details && this.focusedItemOffset < 0)
			{
				int focusedItemIndex = this.FocusedItemIndex;
				int topItemIndex = this.TopItemIndex;
				if (topItemIndex >= 0 && focusedItemIndex >= topItemIndex)
				{
					this.focusedItemOffset = focusedItemIndex - topItemIndex;
				}
			}
		}

		private void EnsureFocusedItemVisibleAsBefore()
		{
			if (this.View == View.Details && this.FocusedItemIndex >= 0 && base.FocusedItem != null && this.focusedItemOffset >= 0)
			{
				base.FocusedItem.EnsureVisible();
				int fromIndex = this.FocusedItemIndex - this.focusedItemOffset;
				if (base.TopItem != null && base.Items.Count > 0)
				{
					this.Scroll(fromIndex, base.TopItem.Index);
				}
			}
		}

		private void Scroll(int fromIndex, int toIndex)
		{
			if (fromIndex != toIndex && this.View == View.Details && base.Items.Count > 0)
			{
				int num;
				if (fromIndex > toIndex)
				{
					num = Math.Min(fromIndex - toIndex, base.Items.Count - 1 - this.TopItemIndex);
				}
				else
				{
					num = Math.Max(fromIndex - toIndex, -this.TopItemIndex);
				}
				int height = base.Items[0].Bounds.Height;
				UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4116, (IntPtr)0, (IntPtr)(height * num));
			}
		}

		private void EnforceValidDataMember(object newDataSource)
		{
			if (newDataSource != null && !string.IsNullOrEmpty(this.DataMember) && this.BindingContext != null)
			{
				try
				{
					BindingManagerBase bindingManagerBase = this.BindingContext[newDataSource, this.DataMember];
				}
				catch (ArgumentException)
				{
					this.dataMember = "";
				}
			}
		}

		[Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor))]
		[DefaultValue("")]
		[Description("Indicates a sub-list of the DataSource to show in the list view control.")]
		[Category("Data")]
		[RefreshProperties(RefreshProperties.Repaint)]
		public string DataMember
		{
			get
			{
				return this.dataMember;
			}
			set
			{
				if (value != this.dataMember)
				{
					this.SetListManager(this.DataSource, value);
				}
			}
		}

		public void SetDataBinding(object newDataSource, string newDataMember)
		{
			if (newDataSource != null && !(newDataSource is IList) && !(newDataSource is IListSource))
			{
				throw new ArgumentException(Strings.BadDataSourceForComplexBinding, "value");
			}
			this.SetListManager(newDataSource, newDataMember);
		}

		private void SetListManager(object newDataSource, string newDataMember)
		{
			this.inSetListManager = true;
			try
			{
				newDataMember = ((newDataMember == null) ? "" : newDataMember);
				if (newDataSource != null && this.BindingContext != null)
				{
					this.ListManager = (CurrencyManager)this.BindingContext[newDataSource, newDataMember];
				}
				else
				{
					this.ListManager = null;
				}
				this.dataSource = newDataSource;
				this.dataMember = newDataMember;
				this.ApplySort();
				this.OnDataSourceChanged(EventArgs.Empty);
			}
			finally
			{
				this.inSetListManager = false;
			}
		}

		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataListView.EventDataSourceChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler DataSourceChanged
		{
			add
			{
				base.Events.AddHandler(DataListView.EventDataSourceChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataListView.EventDataSourceChanged, value);
			}
		}

		private CurrencyManager ListManager
		{
			get
			{
				return this.listManager;
			}
			set
			{
				if (value != this.ListManager)
				{
					this.ClearItemStatesBackup();
					this.ClearItems(false);
					if (this.AutoGenerateColumns)
					{
						base.Columns.Clear();
					}
					this.ClearAllFilters();
					this.arrangeByCommand.Commands.Clear();
					if (this.ListManager != null)
					{
						this.ListManager.ListChanged -= this.ListManager_ListChanged;
						this.ListManager.PositionChanged -= this.ListManager_PositionChanged;
						this.list = null;
						this.bindingList = null;
						this.bindingListView = null;
						this.advancedBindingListView = null;
					}
					this.listManager = value;
					if (this.ListManager != null)
					{
						this.list = this.ListManager.List;
						this.bindingList = (this.list as IBindingList);
						this.bindingListView = (this.list as IBindingListView);
						this.advancedBindingListView = (this.list as IAdvancedBindingListView);
						this.ListManager.ListChanged += this.ListManager_ListChanged;
						this.ListManager.PositionChanged += this.ListManager_PositionChanged;
					}
					if (this.AutoGenerateColumns && base.HeaderStyle != ColumnHeaderStyle.None)
					{
						base.HeaderStyle = (this.SupportsSorting ? ColumnHeaderStyle.Clickable : ColumnHeaderStyle.Nonclickable);
					}
					this.ShowGroups = false;
					if (this.showFilterCommand != null)
					{
						this.showFilterCommand.Visible = this.SupportsFiltering;
					}
					this.arrangeByCommand.Visible = this.SupportsSorting;
					this.showInGroupsCommand.Visible = this.SupportsGrouping;
				}
			}
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			if (!this.inSetListManager)
			{
				this.SetListManager(this.DataSource, this.DataMember);
			}
			base.OnBindingContextChanged(e);
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			this.updateShell = true;
			base.OnSelectedIndexChanged(e);
			if (this.IsDataSourceRefreshing && !this.isCreatingItems)
			{
				if (!base.VirtualMode)
				{
					this.BackupOffsetFocusedItem();
					return;
				}
				if (base.SelectedIndices.Count > 0)
				{
					this.BackupItemsStates();
				}
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (base.IsHandleCreated && !this.isApplyingSmartWidthOfColumns)
			{
				base.BeginInvoke(new MethodInvoker(this.ApplySmartWidthOfColumns));
			}
		}

		[DefaultValue(true)]
		public bool AutoGenerateColumns
		{
			get
			{
				return this.autoGenerateColumns;
			}
			set
			{
				if (value != this.AutoGenerateColumns)
				{
					this.autoGenerateColumns = value;
					if (this.AutoGenerateColumns && this.DataSource != null)
					{
						base.Clear();
						this.CreateItems();
					}
				}
			}
		}

		private void CreateDataBoundColumns()
		{
			if (!this.AutoGenerateColumns)
			{
				return;
			}
			PropertyDescriptorCollection itemProperties = this.ListManager.GetItemProperties();
			if (itemProperties.Count == 0 || typeof(string) == itemProperties[0].ComponentType)
			{
				ExchangeColumnHeader exchangeColumnHeader = new ExchangeColumnHeader();
				exchangeColumnHeader.Text = string.Empty;
				exchangeColumnHeader.Width = base.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
				base.Columns.Add(exchangeColumnHeader);
				return;
			}
			this.arrangeByCommand.Commands.Clear();
			foreach (object obj in itemProperties)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				if (!typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType))
				{
					ExchangeColumnHeader exchangeColumnHeader2 = new ExchangeColumnHeader();
					exchangeColumnHeader2.Name = propertyDescriptor.Name;
					exchangeColumnHeader2.Text = propertyDescriptor.DisplayName;
					exchangeColumnHeader2.Width = 100;
					base.Columns.Add(exchangeColumnHeader2);
					this.AddArrangeByPropertyCommand(propertyDescriptor.Name, new LocalizedString(propertyDescriptor.DisplayName));
				}
			}
		}

		public void AddArrangeByPropertyCommand(string columnName, LocalizedString commandDescription)
		{
			Command command = new Command();
			command.Name = columnName;
			command.Text = commandDescription;
			command.Execute += delegate(object sender, EventArgs e)
			{
				Command command2 = (Command)sender;
				if (this.IsShowingGroups)
				{
					this.ApplyGroup(command2.Name);
					return;
				}
				this.ApplySort(command2.Name);
			};
			this.arrangeByCommand.Commands.Add(command);
		}

		public void RemoveArrangeByPropertyCommand(string columnName)
		{
			CommandCollection commands = this.arrangeByCommand.Commands;
			commands.Remove(commands[columnName]);
		}

		public void BackupItemsStates()
		{
			if (!base.VirtualMode)
			{
				using (IEnumerator enumerator = base.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						ListViewItem listViewItem = (ListViewItem)obj;
						object key = ((KeyValuePair<object, object>)this.item2RowMapping[listViewItem]).Key;
						if (!this.itemsStates.ContainsKey(key))
						{
							this.itemsStates[key] = this.GetItemStates(listViewItem);
						}
					}
					goto IL_D0;
				}
			}
			if (this.list != null && !this.needCreateItemsForRows && !this.isCreatingItems)
			{
				this.backupSelectedIdentities = this.GetPropertyValueFromSelectedObjects(this.IdentityProperty);
				if (base.FocusedItem != null)
				{
					this.focusedItemIdentity = this.GetRowIdentity(this.GetRowFromItem(base.FocusedItem));
				}
				this.BackupOffsetFocusedItem();
			}
			IL_D0:
			if (this.View == View.Details)
			{
				this.backupTopItemIndex = this.TopItemIndex;
			}
		}

		public void RestoreItemsStates(bool restoreViewPoint)
		{
			if (base.VirtualMode)
			{
				if (this.focusedItemIdentity != null)
				{
					base.FocusedItem = this.GetItemFromIdentity(this.focusedItemIdentity);
				}
				if (this.backupSelectedIdentities != null && this.backupSelectedIdentities.Count > 0)
				{
					base.SelectedIndices.Clear();
					bool flag = this.backupSelectedIdentities[0] is IComparable;
					if (flag)
					{
						this.backupSelectedIdentities.Sort();
					}
					int num = 0;
					for (int i = base.Items.Count - 1; i >= 0; i--)
					{
						if (num >= this.backupSelectedIdentities.Count)
						{
							break;
						}
						object rowIdentity = this.GetRowIdentity(this.list[i]);
						bool flag2 = flag ? (this.backupSelectedIdentities.BinarySearch(rowIdentity) >= 0) : (this.backupSelectedIdentities.IndexOf(rowIdentity) >= 0);
						if (flag2)
						{
							base.SelectedIndices.Add(i);
							num++;
						}
					}
				}
			}
			else
			{
				foreach (object obj in base.Items)
				{
					ListViewItem item = (ListViewItem)obj;
					this.RestoreItemStates(item);
				}
			}
			if (this.isShowingContextMenu)
			{
				UnsafeNativeMethods.EndMenu();
			}
			if (restoreViewPoint)
			{
				this.EnsureFocusedItemVisibleAsBefore();
			}
		}

		protected virtual ListViewItemStates GetItemStates(ListViewItem item)
		{
			ListViewItemStates listViewItemStates = ListViewItemStates.Default;
			if (item.Selected)
			{
				listViewItemStates |= ListViewItemStates.Selected;
			}
			if (item.Focused)
			{
				listViewItemStates |= ListViewItemStates.Focused;
			}
			if (item.Checked)
			{
				listViewItemStates |= ListViewItemStates.Checked;
			}
			return listViewItemStates;
		}

		protected virtual void SetItemStates(ListViewItem item, ListViewItemStates itemStates)
		{
			item.Selected = ((itemStates & ListViewItemStates.Selected) != (ListViewItemStates)0);
			item.Focused = ((itemStates & ListViewItemStates.Focused) != (ListViewItemStates)0);
			item.Checked = ((itemStates & ListViewItemStates.Checked) != (ListViewItemStates)0);
		}

		public bool RestoreItemStates(ListViewItem item)
		{
			bool result = false;
			object key = ((KeyValuePair<object, object>)this.item2RowMapping[item]).Key;
			object obj = this.itemsStates[key];
			if (obj != null)
			{
				this.SetItemStates(item, (ListViewItemStates)obj);
				result = true;
			}
			return result;
		}

		protected void ClearItemStatesBackup()
		{
			this.itemsStates.Clear();
			this.backupSelectedIdentities = null;
			this.focusedItemIdentity = null;
			this.focusedItemOffset = -1;
			this.backupTopItemIndex = -1;
		}

		public event EventHandler ItemsForRowsCreated;

		protected virtual void OnItemsForRowsCreated(EventArgs e)
		{
			if (this.ItemsForRowsCreated != null)
			{
				this.ItemsForRowsCreated(this, e);
			}
		}

		public event EventHandler CreatingItemsForRows;

		protected virtual void OnCreatingItemsForRows(EventArgs e)
		{
			if (this.CreatingItemsForRows != null)
			{
				this.CreatingItemsForRows(this, e);
			}
		}

		private void EnsureDataSourceSortedIfNeeded()
		{
			if (this.SupportsSorting && (!this.IsDataSourceRefreshing || !base.VirtualMode) && !string.IsNullOrEmpty(this.SortProperty) && this.list.Count > 1 && !this.bindingList.IsSorted)
			{
				this.ApplySort();
			}
		}

		private void ClearItems(bool backupStatesBeforeClear)
		{
			if (backupStatesBeforeClear)
			{
				this.BackupItemsStates();
			}
			if (base.VirtualMode)
			{
				base.VirtualListSize = 0;
				this.cachedItems = new ListViewItem[0];
				base.SelectedIndices.Clear();
			}
			else
			{
				base.SelectedIndices.Clear();
				base.Items.Clear();
				base.Groups.Clear();
			}
			this.id2ItemMapping = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			this.item2RowMapping = new Hashtable();
		}

		private void CreateItemsForRows()
		{
			if (base.VirtualMode)
			{
				int virtualListSize = base.VirtualListSize;
				int count = base.SelectedIndices.Count;
				int count2 = this.list.Count;
				bool flag = base.IsHandleCreated && base.VirtualMode && this.View == View.Details && !base.DesignMode;
				if (this.IsDataSourceRefreshing && !string.IsNullOrEmpty(this.SortProperty) && this.bindingList != null && !(this.list is IPagedList) && this.bindingList.SupportsSorting && this.bindingList.IsSorted)
				{
					this.bindingList.RemoveSort();
				}
				int num = Math.Min(this.TopItemIndex, count2 - 1);
				if ((virtualListSize > count2 || virtualListSize == 0) && this.IsDataSourceRefreshing)
				{
					this.ClearItems(false);
					num = ((count2 > 0) ? 0 : -1);
				}
				else if (flag && count2 > 0 && count2 < virtualListSize && this.FocusedItemIndex >= count2)
				{
					base.FocusedItem = base.Items[count2 - 1];
				}
				this.OnCreatingItemsForRows(EventArgs.Empty);
				try
				{
					base.VirtualListSize = count2;
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				catch (NullReferenceException)
				{
				}
				if (flag && num >= 0)
				{
					this.Scroll(num, this.TopItemIndex);
				}
				if (virtualListSize == 0 || base.VirtualListSize < virtualListSize)
				{
					this.id2ItemMapping = new Hashtable(base.VirtualListSize, StringComparer.InvariantCultureIgnoreCase);
					this.item2RowMapping = new Hashtable(base.VirtualListSize);
					this.cachedItems = new ListViewItem[base.VirtualListSize];
				}
				else
				{
					this.id2ItemMapping.Clear();
					this.item2RowMapping.Clear();
					Array.Clear(this.cachedItems, 0, this.cachedItems.Length);
				}
				if (base.IsHandleCreated && virtualListSize == base.VirtualListSize)
				{
					base.Invalidate();
				}
				if (base.SelectedIndices.Count > 0 || count != base.SelectedIndices.Count)
				{
					this.OnSelectedIndexChanged(EventArgs.Empty);
					return;
				}
			}
			else
			{
				using (new ControlWaitCursor(this))
				{
					this.ClearItems(true);
					this.id2ItemMapping = new Hashtable(this.list.Count, StringComparer.InvariantCultureIgnoreCase);
					this.item2RowMapping = new Hashtable(this.list.Count);
					this.OnCreatingItemsForRows(EventArgs.Empty);
					ListViewItem[] array = new ListViewItem[this.list.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = this.CreateListViewItemForRow(this.list[i]);
					}
					base.Items.AddRange(array);
					this.GroupListViewItems();
				}
			}
		}

		public void EnsureItemsCreated()
		{
			if (this.needCreateItemsForRows)
			{
				this.needCreateItemsForRows = false;
				this.CreateItems();
			}
		}

		protected virtual void CreateItems()
		{
			if (base.InvokeRequired)
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "DataListView.CreateItems called from background thread.");
				base.Invoke(new MethodInvoker(this.CreateItems));
				return;
			}
			ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "-->DataListView.CreateItems:");
			bool flag = base.Items.Count == 0;
			if (this.list != null)
			{
				NativeMethods.SCROLLINFO scrollinfo = new NativeMethods.SCROLLINFO(4, 0, 0, 0, 0);
				base.BeginUpdate();
				this.isCreatingItems = true;
				this.EnsureDataSourceSortedIfNeeded();
				try
				{
					if (base.Columns.Count == 0)
					{
						this.CreateDataBoundColumns();
					}
					if (base.Columns.Count > 0)
					{
						UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, base.Handle), 0, scrollinfo);
						this.CreateItemsForRows();
					}
					this.OnItemsForRowsCreated(EventArgs.Empty);
					if (!base.VirtualMode || !this.IsDataSourceRefreshing)
					{
						this.RestoreItemsStates(false);
					}
					if (base.FocusedItem == null && this.list != null && this.list.Count > 0)
					{
						this.ListManager.Position = 0;
						this.ListManager_PositionChanged(this.ListManager, EventArgs.Empty);
					}
				}
				finally
				{
					this.isCreatingItems = false;
					base.EndUpdate();
				}
				NativeMethods.SCROLLINFO scrollinfo2 = new NativeMethods.SCROLLINFO(4, 0, 0, 0, 0);
				UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, base.Handle), 0, scrollinfo2);
				if (scrollinfo2.nPos != scrollinfo.nPos)
				{
					int value = scrollinfo.nPos - scrollinfo2.nPos;
					UnsafeNativeMethods.SendMessage(new HandleRef(null, base.Handle), 4116, (IntPtr)value, IntPtr.Zero);
				}
				if ((!base.VirtualMode || !this.IsDataSourceRefreshing) && this.View == View.Details && (this.TopItemIndex == 0 || this.TopItemIndex == this.backupTopItemIndex) && base.FocusedItem != null)
				{
					this.EnsureFocusedItemVisibleAsBefore();
				}
				if (!this.IsDataSourceRefreshing)
				{
					this.TrySelectItemBySpecifiedIdentity();
					this.ClearItemStatesBackup();
				}
			}
			if (flag || !this.IsDataSourceRefreshing)
			{
				this.ApplySmartWidthOfColumns();
			}
			this.UpdateNoResultsIndicator();
			ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "<--DataListView.CreateItems:");
		}

		protected ListViewItem CreateListViewItemForRow(object row)
		{
			ListViewItem listViewItem = this.CreateNewListViewItem(row);
			listViewItem.ImageIndex = this.ImageIndex;
			object rowIdentity = this.GetRowIdentity(row);
			this.id2ItemMapping[rowIdentity] = listViewItem;
			this.item2RowMapping[listViewItem] = new KeyValuePair<object, object>(rowIdentity, row);
			listViewItem.Tag = row;
			while (listViewItem.SubItems.Count < base.Columns.Count)
			{
				listViewItem.SubItems.Add(string.Empty);
			}
			ItemCheckedEventArgs e = new ItemCheckedEventArgs(listViewItem);
			this.OnUpdateItem(e);
			return listViewItem;
		}

		public object GetRowIdentity(object row)
		{
			object result = row;
			if (row != null && !string.IsNullOrEmpty(this.IdentityProperty))
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(row);
				if (properties[this.IdentityProperty] != null)
				{
					result = DataListView.GetPropertyValue(row, properties[this.IdentityProperty]);
				}
			}
			return result;
		}

		protected virtual ListViewItem CreateNewListViewItem(object row)
		{
			return new ListViewItem();
		}

		protected virtual void OnUpdateItem(ItemCheckedEventArgs e)
		{
			ListViewItem item = e.Item;
			object rowFromItem = this.GetRowFromItem(item);
			if (rowFromItem != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(rowFromItem);
				if (rowFromItem is Enum || properties.Count == 0 || typeof(string) == properties[0].ComponentType)
				{
					item.Text = this.GetSubItemTextForRow(rowFromItem, 0, properties);
					Color subItemForColorForRow = this.GetSubItemForColorForRow(rowFromItem, 0, properties);
					if (subItemForColorForRow != Color.Empty)
					{
						item.ForeColor = subItemForColorForRow;
					}
				}
				else
				{
					for (int i = 0; i < base.Columns.Count; i++)
					{
						item.SubItems[i].Text = this.GetSubItemTextForRow(rowFromItem, i, properties);
						Color subItemForColorForRow2 = this.GetSubItemForColorForRow(rowFromItem, i, properties);
						if (subItemForColorForRow2 != Color.Empty)
						{
							item.UseItemStyleForSubItems = false;
							item.SubItems[i].ForeColor = subItemForColorForRow2;
						}
					}
					string text = string.Empty;
					if (!string.IsNullOrEmpty(this.StatusPropertyName))
					{
						int num = 1;
						if (base.Columns.Count > num)
						{
							PropertyDescriptor propertyDescriptor = properties[this.StatusPropertyName];
							if (propertyDescriptor != null)
							{
								string text2 = DataListView.GetPropertyValue(rowFromItem, propertyDescriptor).ToString();
								if (!string.IsNullOrEmpty(text2))
								{
									text = text2;
									item.SubItems[num].Text = this.GetSubItemTextFromRow(rowFromItem, this.StatusPropertyName, properties);
								}
							}
						}
					}
					if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(this.ImagePropertyName))
					{
						PropertyDescriptor propertyDescriptor2 = properties[this.ImagePropertyName];
						if (propertyDescriptor2 != null)
						{
							text = DataListView.GetPropertyValue(rowFromItem, propertyDescriptor2).ToString();
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						item.ImageKey = text;
						if (base.VirtualMode && this.IconLibrary != null)
						{
							item.ImageIndex = this.IconLibrary.Icons.IndexOf(item.ImageKey);
						}
					}
				}
			}
			this.RaiseUpdateItem(e);
		}

		public string StatusPropertyName { get; set; }

		private string GetSubItemTextForRow(object row, int columnIndex, PropertyDescriptorCollection properties)
		{
			ColumnHeader columnHeader = base.Columns[columnIndex];
			return this.GetSubItemTextFromRow(row, columnHeader, properties);
		}

		private string GetSubItemTextFromRow(object row, string columnName, PropertyDescriptorCollection properties)
		{
			ExchangeColumnHeader columnHeader = this.AvailableColumns.FirstOrDefault((ExchangeColumnHeader header) => header.Name == columnName);
			return this.FormatRawValueByColumn(this.GetRawValueFromRow(row, columnName, properties), columnHeader);
		}

		private string GetSubItemTextFromRow(object row, ColumnHeader columnHeader, PropertyDescriptorCollection properties)
		{
			return this.FormatRawValueByColumn(this.GetRawValueFromRow(row, columnHeader.Name, properties), columnHeader);
		}

		internal string FormatRawValueByColumn(object rawValue, ColumnHeader columnHeader)
		{
			ExchangeColumnHeader exchangeColumnHeader = columnHeader as ExchangeColumnHeader;
			string text = string.Empty;
			if (exchangeColumnHeader == null)
			{
				text = TextConverter.DefaultConverter.Format(null, rawValue, null);
			}
			else
			{
				ICustomFormatter customFormatter = exchangeColumnHeader.CustomFormatter ?? TextConverter.DefaultConverter;
				text = customFormatter.Format(exchangeColumnHeader.FormatString, rawValue, exchangeColumnHeader.FormatProvider);
				if (string.IsNullOrEmpty(text))
				{
					text = exchangeColumnHeader.DefaultEmptyText;
				}
			}
			return text;
		}

		private object GetRawValueFromRow(object row, int columnIndex, PropertyDescriptorCollection properties)
		{
			return this.GetRawValueFromRow(row, base.Columns[columnIndex].Name, properties);
		}

		private object GetRawValueFromRow(object row, string columnName, PropertyDescriptorCollection properties)
		{
			object obj = null;
			if (properties != null && properties.Count > 0 && !(row is Enum) && !(row is string) && !(row is EnumObject))
			{
				PropertyDescriptor propertyDescriptor = (columnName == "ToString()") ? ToStringPropertyDescriptor.DefaultPropertyDescriptor : properties[columnName];
				if (propertyDescriptor != null)
				{
					obj = DataListView.GetPropertyValue(row, propertyDescriptor);
				}
			}
			else
			{
				obj = row;
			}
			EnumObject enumObject = obj as EnumObject;
			if (enumObject != null)
			{
				obj = enumObject.Value;
			}
			return obj;
		}

		private Color GetSubItemForColorForRow(object row, int columnIndex, PropertyDescriptorCollection properties)
		{
			object rawValueFromRow = this.GetRawValueFromRow(row, columnIndex, properties);
			ExchangeColumnHeader exchangeColumnHeader = base.Columns[columnIndex] as ExchangeColumnHeader;
			if (exchangeColumnHeader != null && exchangeColumnHeader.ToColorFormatter != null)
			{
				return exchangeColumnHeader.ToColorFormatter.Format(rawValueFromRow);
			}
			return Color.Empty;
		}

		protected void RaiseUpdateItem(ItemCheckedEventArgs e)
		{
			ItemCheckedEventHandler itemCheckedEventHandler = (ItemCheckedEventHandler)base.Events[DataListView.EventUpdateItem];
			if (itemCheckedEventHandler != null)
			{
				itemCheckedEventHandler(this, e);
			}
		}

		public event ItemCheckedEventHandler UpdateItem
		{
			add
			{
				base.Events.AddHandler(DataListView.EventUpdateItem, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataListView.EventUpdateItem, value);
			}
		}

		[Category("Data")]
		[DefaultValue("")]
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		public string ImagePropertyName
		{
			get
			{
				return this.imagePropertyName;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				if (this.ImagePropertyName != value)
				{
					this.imagePropertyName = value;
					this.CreateItems();
				}
			}
		}

		internal static object GetPropertyValue(object row, PropertyDescriptor propertyDescriptor)
		{
			object obj;
			try
			{
				obj = WinformsHelper.GetPropertyValue(row, propertyDescriptor);
				if (obj == null)
				{
					obj = "";
				}
			}
			catch (TargetInvocationException ex)
			{
				obj = ex.InnerException.Message;
			}
			return obj;
		}

		public bool HasSelection
		{
			get
			{
				return this.SelectedObjects.Count > 0;
			}
		}

		public bool HasSingleSelection
		{
			get
			{
				return this.SelectedObjects.Count == 1;
			}
		}

		public bool HasMultiSelection
		{
			get
			{
				return this.SelectedObjects.Count > 1;
			}
		}

		public object SelectedObject
		{
			get
			{
				if (this.SelectedObjects.Count == 0)
				{
					return null;
				}
				return this.SelectedObjects[0];
			}
		}

		public IList SelectedObjects
		{
			get
			{
				if (this.selectedObjects == null)
				{
					ArrayList arrayList = new ArrayList(base.SelectedIndices.Count);
					foreach (object obj in base.SelectedIndices)
					{
						int index = (int)obj;
						object obj2 = base.VirtualMode ? this.list[index] : this.GetRowFromItem(base.Items[index]);
						if (obj2 != null)
						{
							arrayList.Add(obj2);
						}
					}
					if (this.SelectedObjectsSorter != null)
					{
						arrayList.Sort(this.SelectedObjectsSorter);
					}
					this.selectedObjects = ArrayList.ReadOnly(arrayList);
				}
				return this.selectedObjects;
			}
		}

		[DefaultValue(null)]
		public IComparer SelectedObjectsSorter
		{
			get
			{
				return this.selectedObjectsSorter;
			}
			set
			{
				this.selectedObjectsSorter = value;
			}
		}

		public object SelectedIdentity
		{
			get
			{
				IList list = this.SelectedIdentities;
				if (list.Count > 0)
				{
					return list[0];
				}
				return null;
			}
		}

		public IList SelectedIdentities
		{
			get
			{
				if (this.selectedIdentities == null)
				{
					this.selectedIdentities = ArrayList.ReadOnly(this.GetPropertyValueFromSelectedObjects(this.IdentityProperty));
				}
				return this.selectedIdentities;
			}
		}

		private ArrayList GetPropertyValueFromSelectedObjects(string propertyName)
		{
			ArrayList arrayList = new ArrayList(this.SelectedObjects.Count);
			if (this.SelectedObjects.Count > 0 && !string.IsNullOrEmpty(propertyName))
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this.SelectedObject)[propertyName];
				foreach (object obj in this.SelectedObjects)
				{
					if (propertyDescriptor != null)
					{
						arrayList.Add(DataListView.GetPropertyValue(obj, propertyDescriptor));
					}
					else
					{
						arrayList.Add(obj);
					}
				}
			}
			return arrayList;
		}

		internal WorkUnit[] GetSelectedWorkUnits()
		{
			return this.GetSelectedWorkUnits(this.IdentityProperty, null);
		}

		internal WorkUnit[] GetSelectedWorkUnits(string targetPropertyName, Type targetType)
		{
			if (string.IsNullOrEmpty(targetPropertyName))
			{
				throw new ArgumentNullException(targetPropertyName);
			}
			if (string.IsNullOrEmpty(this.SelectionNameProperty))
			{
				throw new InvalidOperationException("SelectionNameProperty must be set in DataListView before GetSelectedWorkUnits is called.");
			}
			if (this.IconLibrary == null)
			{
				throw new InvalidOperationException("IconLibrary must be set in DataListView before GetSelectedWorkUnits is called.");
			}
			WorkUnit[] result;
			using (new ControlWaitCursor(this))
			{
				IList list = this.SelectedObjects;
				WorkUnit[] array = new WorkUnit[list.Count];
				if (list.Count > 0)
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(list[0]);
					PropertyDescriptor propertyDescriptor = properties[this.SelectionNameProperty];
					PropertyDescriptor propertyDescriptor2 = string.IsNullOrEmpty(this.ImagePropertyName) ? null : properties[this.ImagePropertyName];
					PropertyDescriptor propertyDescriptor3 = properties[targetPropertyName];
					if (propertyDescriptor3 == null)
					{
						throw new InvalidOperationException(targetPropertyName + " must be valid in DataListView before GetSelectedWorkUnits is called.");
					}
					if (propertyDescriptor == null)
					{
						throw new InvalidOperationException("SelectionNameProperty must be valid in DataListView before GetSelectedWorkUnits is called.");
					}
					for (int i = 0; i < list.Count; i++)
					{
						object row = list[i];
						string text = (DataListView.GetPropertyValue(row, propertyDescriptor) ?? "").ToString();
						object obj = DataListView.GetPropertyValue(row, propertyDescriptor3);
						if (null != targetType)
						{
							ExTraceGlobals.DataFlowTracer.TraceDebug<object, Type>((long)this.GetHashCode(), "DataListView.GetSelectedWorkUnits is converting value {0} to {1}", obj, targetType);
							obj = LanguagePrimitives.ConvertTo(obj, targetType);
						}
						string name = null;
						if (propertyDescriptor2 != null)
						{
							name = (DataListView.GetPropertyValue(row, propertyDescriptor2) ?? "").ToString();
						}
						else
						{
							ListViewItem itemFromRow = this.GetItemFromRow(row);
							if (itemFromRow != null && !string.IsNullOrEmpty(itemFromRow.ImageKey))
							{
								name = itemFromRow.ImageKey;
							}
							else if (itemFromRow != null && itemFromRow.ImageIndex >= 0)
							{
								name = this.IconLibrary.Icons[itemFromRow.ImageIndex].Name;
							}
							else if (this.ImageIndex >= 0)
							{
								name = this.IconLibrary.Icons[this.ImageIndex].Name;
							}
						}
						Icon icon = (this.IconLibrary.Icons[name] != null) ? this.IconLibrary.Icons[name].Icon : null;
						array[i] = new WorkUnit(text, icon, obj);
					}
				}
				result = array;
			}
			return result;
		}

		public void SelectItemBySpecifiedIdentity(object identity, bool delay)
		{
			this.selectedItemIdentity = identity;
			if (!delay && identity != null)
			{
				this.TrySelectItemBySpecifiedIdentity();
				this.Application_Idle(this, EventArgs.Empty);
			}
		}

		internal void TrySelectItemBySpecifiedIdentity()
		{
			if (this.selectedItemIdentity == null)
			{
				return;
			}
			ListViewItem listViewItem = null;
			if (this.selectedItemIdentity is object[])
			{
				foreach (object identity in (object[])this.selectedItemIdentity)
				{
					listViewItem = this.GetItemFromIdentity(identity);
					if (listViewItem != null)
					{
						break;
					}
				}
			}
			else
			{
				listViewItem = this.GetItemFromIdentity(this.selectedItemIdentity);
			}
			if (listViewItem != null)
			{
				base.SelectedIndices.Clear();
				listViewItem.Selected = true;
				listViewItem.Focused = true;
				this.selectedItemIdentity = null;
				this.EnsureFocusedItemVisibleAsBefore();
				if (base.VirtualMode && this.IsDataSourceRefreshing)
				{
					this.BackupItemsStates();
				}
			}
		}

		[DefaultValue(false)]
		public bool HideSortArrow
		{
			get
			{
				return this.hideSortArrow;
			}
			set
			{
				if (value != this.hideSortArrow)
				{
					this.hideSortArrow = value;
					if (base.IsHandleCreated)
					{
						base.Invalidate();
					}
				}
			}
		}

		[DefaultValue("")]
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		[Category("Data")]
		public string IdentityProperty
		{
			get
			{
				return this.identityProperty;
			}
			set
			{
				if (value == null)
				{
					this.identityProperty = string.Empty;
				}
				else
				{
					this.identityProperty = value;
				}
				if (this.DataSource != null)
				{
					this.CreateItems();
				}
			}
		}

		[DefaultValue("")]
		[Category("Data")]
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		public string SelectionNameProperty
		{
			get
			{
				return this.selectionNameProperty;
			}
			set
			{
				if (value == null)
				{
					this.selectionNameProperty = string.Empty;
					return;
				}
				this.selectionNameProperty = value;
				foreach (ExchangeColumnHeader exchangeColumnHeader in this.AvailableColumns)
				{
					if (exchangeColumnHeader.Name == this.SelectionNameProperty)
					{
						exchangeColumnHeader.Visible = true;
					}
				}
			}
		}

		public string SelectionName
		{
			get
			{
				string result;
				switch (this.SelectedObjects.Count)
				{
				case 0:
					result = " ";
					break;
				case 1:
					if (!string.IsNullOrEmpty(this.SelectionNameProperty))
					{
						result = DataListView.GetPropertyValue(this.SelectedObject, TypeDescriptor.GetProperties(this.SelectedObject)[this.SelectionNameProperty]).ToString();
					}
					else
					{
						result = this.SelectedObject.ToString();
					}
					break;
				default:
					result = Strings.ItemsSelected(this.SelectedObjects.Count);
					break;
				}
				return result;
			}
		}

		public string SelectedObjectDetailsType
		{
			get
			{
				string result = string.Empty;
				if (this.SelectedObjects.Count > 0 && !string.IsNullOrEmpty(this.SelectedObjectDetailsTypeProperty))
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.SelectedObject);
					result = this.GetSubItemTextFromRow(this.SelectedObject, this.SelectedObjectDetailsTypeProperty, properties);
				}
				return result;
			}
		}

		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		[Category("Data")]
		[DefaultValue("")]
		public string SelectedObjectDetailsTypeProperty
		{
			get
			{
				return this.selectedObjectDetailsTypeProperty;
			}
			set
			{
				if (value == null)
				{
					this.selectedObjectDetailsTypeProperty = string.Empty;
					return;
				}
				this.selectedObjectDetailsTypeProperty = value;
				foreach (ExchangeColumnHeader exchangeColumnHeader in this.AvailableColumns)
				{
					if (exchangeColumnHeader.Name == this.SelectedObjectDetailsTypeProperty)
					{
						exchangeColumnHeader.Visible = true;
					}
				}
			}
		}

		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		[Category("Data")]
		[DefaultValue("")]
		public string SortProperty
		{
			get
			{
				return this.sortProperty;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				if (this.SortProperty != value)
				{
					this.sortProperty = value;
					this.ApplySort();
				}
			}
		}

		[Category("Data")]
		[DefaultValue(ListSortDirection.Ascending)]
		public ListSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				if (this.SortDirection != value)
				{
					this.sortDirection = value;
					this.ApplySort();
				}
			}
		}

		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
		[Category("Data")]
		[DefaultValue("")]
		public string GroupProperty
		{
			get
			{
				return this.groupProperty;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				if (this.GroupProperty != value)
				{
					this.groupProperty = value;
					this.ApplySort();
				}
			}
		}

		[DefaultValue(ListSortDirection.Ascending)]
		[Category("Data")]
		public ListSortDirection GroupDirection
		{
			get
			{
				return this.groupDirection;
			}
			set
			{
				if (this.GroupDirection != value)
				{
					this.groupDirection = value;
					this.ApplySort();
				}
			}
		}

		private void ApplySort(string property)
		{
			if (!(property == this.SortProperty))
			{
				this.SortProperty = property;
				return;
			}
			if (this.SortDirection == ListSortDirection.Ascending)
			{
				this.SortDirection = ListSortDirection.Descending;
				return;
			}
			this.SortDirection = ListSortDirection.Ascending;
		}

		private void ApplyGroup(string property)
		{
			if (!(property == this.GroupProperty))
			{
				this.GroupProperty = property;
				return;
			}
			if (this.GroupDirection == ListSortDirection.Ascending)
			{
				this.GroupDirection = ListSortDirection.Descending;
				return;
			}
			this.GroupDirection = ListSortDirection.Ascending;
		}

		private void ApplySort()
		{
			if (this.SupportsSorting && !string.IsNullOrEmpty(this.SortProperty))
			{
				using (new ControlWaitCursor(this))
				{
					if (base.VirtualMode && !this.IsDataSourceRefreshing && !this.isCreatingItems)
					{
						this.BackupItemsStates();
					}
					PropertyDescriptorCollection itemProperties = this.ListManager.GetItemProperties();
					PropertyDescriptor property;
					if (this.SortProperty == "ToString()")
					{
						property = ToStringPropertyDescriptor.DefaultPropertyDescriptor;
					}
					else
					{
						property = itemProperties[this.SortProperty];
					}
					if (this.IsShowingGroups && !string.IsNullOrEmpty(this.GroupProperty))
					{
						PropertyDescriptor property2 = itemProperties[this.GroupProperty];
						this.bindingListView.ApplySort(new ListSortDescriptionCollection(new ListSortDescription[]
						{
							new ListSortDescription(property2, this.GroupDirection),
							new ListSortDescription(property, this.SortDirection)
						}));
					}
					else if (!base.VirtualMode || !this.IsDataSourceRefreshing)
					{
						this.bindingList.ApplySort(property, this.SortDirection);
					}
					string b = this.IsShowingGroups ? this.GroupProperty : this.SortProperty;
					CommandCollection commands = this.arrangeByCommand.Commands;
					for (int i = 0; i < commands.Count - 2; i++)
					{
						commands[i].Checked = (commands[i].Name == b);
					}
					this.UpdateHeaderSortArrow();
					return;
				}
			}
			this.ListManager_ListChanged(this.ListManager, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		private void UpdateHeaderSortArrow()
		{
			if (base.IsHandleCreated && !this.HideSortArrow && base.HeaderStyle == ColumnHeaderStyle.Clickable)
			{
				int num = -1;
				int num2 = -1;
				InternalNativeMethods.HDITEM hditem = default(InternalNativeMethods.HDITEM);
				hditem.mask = 4U;
				for (int i = 0; i < base.Columns.Count; i++)
				{
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_GETITEM, (IntPtr)i, ref hditem);
					hditem.fmt &= -1537;
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_SETITEM, (IntPtr)i, ref hditem);
					if (!string.IsNullOrEmpty(this.SortProperty) && this.SortProperty == base.Columns[i].Name)
					{
						num = i;
					}
					if (!string.IsNullOrEmpty(this.GroupProperty) && this.GroupProperty == base.Columns[i].Name)
					{
						num2 = i;
					}
				}
				this.SelectedColumnIndex = num;
				if (-1 != num)
				{
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_GETITEM, (IntPtr)num, ref hditem);
					hditem.fmt &= -1537;
					hditem.fmt |= ((this.SortDirection == ListSortDirection.Ascending) ? 1024 : 512);
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_SETITEM, (IntPtr)num, ref hditem);
				}
				if (this.ShowGroups && -1 != num2)
				{
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_GETITEM, (IntPtr)num2, ref hditem);
					hditem.fmt &= -1537;
					hditem.fmt |= ((this.GroupDirection == ListSortDirection.Ascending) ? 1024 : 512);
					InternalUnsafeNativeMethods.SendMessage(this.HeaderHandle, NativeMethods.HDM_SETITEM, (IntPtr)num2, ref hditem);
				}
			}
		}

		private void GroupListViewItems()
		{
			if (this.IsShowingGroups)
			{
				PropertyDescriptor propertyDescriptor = this.ListManager.GetItemProperties()[this.GroupProperty];
				if (propertyDescriptor != null)
				{
					ControlWaitCursor controlWaitCursor;
					controlWaitCursor..ctor(this);
					try
					{
						base.BeginUpdate();
						try
						{
							base.Groups.Clear();
							ListViewGroup listViewGroup = new ListViewGroup();
							for (int i = 0; i < base.Items.Count; i++)
							{
								object rowFromItem = this.GetRowFromItem(base.Items[i]);
								if (rowFromItem != null)
								{
									object propertyValue = DataListView.GetPropertyValue(rowFromItem, propertyDescriptor);
									if (!object.Equals(listViewGroup.Tag, propertyValue))
									{
										string text = propertyValue.ToString();
										string headerText = text;
										listViewGroup = new ListViewGroup(text, headerText);
										listViewGroup.Tag = propertyValue;
										base.Groups.Add(listViewGroup);
									}
									base.Items[i].Group = listViewGroup;
								}
							}
							foreach (object obj in base.Groups)
							{
								ListViewGroup listViewGroup2 = (ListViewGroup)obj;
								listViewGroup2.Header = Strings.GroupHeader(listViewGroup2.Header, listViewGroup2.Items.Count);
							}
						}
						finally
						{
							base.EndUpdate();
						}
					}
					finally
					{
						controlWaitCursor.Dispose();
					}
				}
			}
		}

		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			ExchangeColumnHeader exchangeColumnHeader = base.Columns[e.Column] as ExchangeColumnHeader;
			bool flag = true;
			if (exchangeColumnHeader != null)
			{
				flag = exchangeColumnHeader.IsSortable;
			}
			if (this.SupportsSorting && flag)
			{
				PropertyDescriptorCollection itemProperties = this.ListManager.GetItemProperties();
				PropertyDescriptor propertyDescriptor = itemProperties[base.Columns[e.Column].Name];
				if (propertyDescriptor != null)
				{
					bool flag2;
					if (this.advancedBindingListView != null)
					{
						flag2 = this.advancedBindingListView.IsSortSupported(propertyDescriptor.Name);
					}
					else
					{
						flag2 = typeof(IComparable).IsAssignableFrom(propertyDescriptor.PropertyType);
					}
					if (flag2)
					{
						if (this.IsShowingGroups && this.GroupProperty == propertyDescriptor.Name)
						{
							this.ApplyGroup(propertyDescriptor.Name);
						}
						else
						{
							this.ApplySort(propertyDescriptor.Name);
						}
					}
				}
			}
			base.OnColumnClick(e);
		}

		protected override void OnColumnReordered(ColumnReorderedEventArgs e)
		{
			ExchangeColumnHeader exchangeColumnHeader = e.Header as ExchangeColumnHeader;
			ExchangeColumnHeader exchangeColumnHeader2 = base.Columns[e.NewDisplayIndex] as ExchangeColumnHeader;
			if ((exchangeColumnHeader != null && !exchangeColumnHeader.IsReorderable) || (exchangeColumnHeader2 != null && !exchangeColumnHeader2.IsReorderable))
			{
				e.Cancel = true;
			}
			base.OnColumnReordered(e);
		}

		private void WmNotifyBeginDrag(ref Message m)
		{
			bool flag = false;
			if (!this.isDraggingNonReorderableColumn)
			{
				NativeMethods.NMHEADER nmheader = (NativeMethods.NMHEADER)m.GetLParam(typeof(NativeMethods.NMHEADER));
				if (nmheader.iItem >= 0)
				{
					ExchangeColumnHeader exchangeColumnHeader = base.Columns[nmheader.iItem] as ExchangeColumnHeader;
					if (exchangeColumnHeader != null && !exchangeColumnHeader.IsReorderable)
					{
						this.isDraggingNonReorderableColumn = true;
						m.Result = (IntPtr)1;
						flag = true;
					}
				}
			}
			else
			{
				m.Result = (IntPtr)1;
				flag = true;
			}
			if (!flag)
			{
				base.WndProc(ref m);
			}
		}

		private void WmNotify(ref Message m)
		{
			int code = ((NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR))).code;
			if (code != -327)
			{
				switch (code)
				{
				case -313:
					this.WmNotifyFilterButtonClick(ref m);
					return;
				case -312:
					this.WmNotifyFilterChange(ref m);
					return;
				case -311:
					break;
				case -310:
					this.WmNotifyBeginDrag(ref m);
					return;
				case -309:
				case -308:
					goto IL_84;
				case -307:
					goto IL_55;
				default:
					if (code != -16)
					{
						goto IL_84;
					}
					break;
				}
				this.isDraggingNonReorderableColumn = false;
				base.WndProc(ref m);
				return;
				IL_84:
				base.WndProc(ref m);
				return;
			}
			IL_55:
			this.WmNotifyEndTrack(ref m);
		}

		private void WmReflectNotify(ref Message m)
		{
			base.WndProc(ref m);
			if (((NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR))).code == NativeMethods.LVN_ENDLABELEDIT)
			{
				this.Application_Idle(this, EventArgs.Empty);
			}
		}

		private void WmContextMenu(ref Message m)
		{
			this.isShowingContextMenu = true;
			try
			{
				if ((this.ContextMenu != null || this.ContextMenuStrip != null) && base.SelectedIndices.Count > 0 && (int)((long)m.LParam) == -1)
				{
					ListViewItem firstSelectedItem = this.FirstSelectedItem;
					Rectangle bounds = firstSelectedItem.GetBounds(ItemBoundsPortion.Icon);
					Point location = bounds.Location;
					location.X = bounds.X + bounds.Width / 2;
					location.Y = bounds.Y + bounds.Height / 2;
					if (location.X < base.ClientRectangle.Left)
					{
						location.X = base.ClientRectangle.Left;
					}
					if (location.X > base.ClientRectangle.Right)
					{
						location.X = base.ClientRectangle.Right;
					}
					if (location.Y < base.ClientRectangle.Top)
					{
						location.Y = base.ClientRectangle.Top;
					}
					if (location.Y > base.ClientRectangle.Bottom)
					{
						location.Y = base.ClientRectangle.Bottom;
					}
					if (this.ContextMenu != null)
					{
						this.ContextMenu.Show(this, location);
					}
					else if (this.ContextMenuStrip != null)
					{
						this.ContextMenuStrip.Show(this, location);
					}
				}
				else
				{
					int x = NativeMethods.LOWORD(m.LParam);
					int y = NativeMethods.HIWORD(m.LParam);
					NativeMethods.RECT rect = default(NativeMethods.RECT);
					UnsafeNativeMethods.GetWindowRect(this.HeaderHandle, ref rect);
					if (!Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom).Contains(x, y))
					{
						base.WndProc(ref m);
					}
				}
			}
			finally
			{
				this.isShowingContextMenu = false;
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg <= 78)
			{
				if (msg == 15)
				{
					base.WndProc(ref m);
					this.UpdateIndicator();
					return;
				}
				if (msg == 78)
				{
					this.WmNotify(ref m);
					return;
				}
			}
			else
			{
				if (msg == 123)
				{
					this.Application_Idle(this, EventArgs.Empty);
					this.WmContextMenu(ref m);
					return;
				}
				if (msg == 8270)
				{
					this.WmReflectNotify(ref m);
					return;
				}
			}
			base.WndProc(ref m);
		}

		private void UpdateIndicator()
		{
			if (this.DrawLockedIcon || this.DrawLockedString)
			{
				this.DrawLockedState();
				return;
			}
			if (this.ShowNoResultsIndicator || this.ShowBulkEditIndicator)
			{
				this.DrawIndicator(this.ShowNoResultsIndicator ? this.NoResultsLabelText : this.BulkEditingIndicatorText);
			}
		}

		public bool DrawLockedIcon { get; set; }

		public bool DrawLockedString { get; set; }

		protected virtual void DrawLockedState()
		{
			base.Enabled = false;
			if (this.DrawLockedString)
			{
				int num = 16;
				string text = Strings.NotPermittedByRbac;
				Rectangle indicatorBounds = this.GetIndicatorBounds(text, num, num / 2);
				this.DrawIndicator(indicatorBounds, text);
				Rectangle bounds = new Rectangle(indicatorBounds.X - num, indicatorBounds.Y + (this.Font.Height - num) / 2, num, num);
				this.DrawIcon(bounds, Icons.LockIcon);
				return;
			}
			int num2 = 8;
			int num3 = (base.HeaderStyle != ColumnHeaderStyle.None) ? 16 : 0;
			Rectangle bounds2 = new Rectangle(base.ClientRectangle.Right - num2, base.ClientRectangle.Top + num3, num2, num2);
			this.DrawIcon(bounds2, Icons.LockIcon);
		}

		private void DrawIcon(Rectangle bounds, Icon icon)
		{
			using (Graphics graphics = base.CreateGraphics())
			{
				Color color = base.Enabled ? this.BackColor : SystemColors.Control;
				using (new SolidBrush(color))
				{
					graphics.DrawIcon(icon, bounds);
				}
			}
		}

		private void DrawIndicator(string text)
		{
			this.DrawIndicator(this.GetIndicatorBounds(text), text);
		}

		private Rectangle GetIndicatorBounds(string text)
		{
			return this.GetIndicatorBounds(text, 0, 0);
		}

		private Rectangle GetIndicatorBounds(string text, int minXBorder, int minYPosition)
		{
			int num = this.ShowFilter ? (this.Font.Height * 4) : (this.Font.Height * 2);
			if (base.HeaderStyle == ColumnHeaderStyle.None)
			{
				num = 10;
			}
			num = Math.Max(num, minYPosition);
			int num2 = Math.Max(minXBorder, this.Font.Height);
			Size size = TextRenderer.MeasureText(text, this.Font, new Size(base.Width - num2 * 2, base.Height - num), this.GetIndicatorFormatFlags());
			Point location = new Point((base.Width - size.Width) / 2, num);
			return new Rectangle(location, size);
		}

		private void DrawIndicator(Rectangle bounds, string text)
		{
			using (Graphics graphics = base.CreateGraphics())
			{
				TextRenderer.DrawText(graphics, text, this.Font, bounds, base.Enabled ? SystemColors.ControlText : SystemColors.GrayText, this.GetIndicatorFormatFlags());
			}
		}

		private TextFormatFlags GetIndicatorFormatFlags()
		{
			return TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			if (base.IsHandleCreated && !this.isShowingContextMenu)
			{
				if (this.needCreateItemsForRows && (this.swCreateItems == null || this.swCreateItems.ElapsedMilliseconds >= 100L))
				{
					this.needCreateItemsForRows = false;
					this.CreateItems();
					if (this.swCreateItems != null)
					{
						this.swCreateItems.Reset();
						this.swCreateItems.Start();
					}
				}
				if (!this.needCreateItemsForRows)
				{
					if (this.ListManager != null && base.FocusedItem != null && this.ListManager.Position >= 0 && base.FocusedItem.Index < this.ListManager.Count && this.ListManager.Position < this.ListManager.Count)
					{
						object rowFromItem = this.GetRowFromItem(base.FocusedItem);
						object rowIdentity = this.GetRowIdentity(this.ListManager.Current);
						object rowIdentity2 = this.GetRowIdentity(rowFromItem);
						if (!StringComparer.InvariantCultureIgnoreCase.Equals(rowIdentity, rowIdentity2))
						{
							int num = this.ListManager.List.IndexOf(rowFromItem);
							if (-1 != num)
							{
								this.ListManager.Position = num;
							}
						}
					}
					this.RaiseSelectionChanged();
				}
			}
		}

		public event EventHandler SelectionChanged;

		protected virtual void OnSelectionChanged(EventArgs e)
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, e);
			}
		}

		private void WmNotifyEndTrack(ref Message m)
		{
			base.WndProc(ref m);
			this.IsColumnsWidthDirty = true;
		}

		internal bool IsColumnsWidthDirty
		{
			get
			{
				return this.isColumnsWidthDirty;
			}
			set
			{
				this.isColumnsWidthDirty = value;
			}
		}

		protected void ApplySmartWidthOfColumns()
		{
			if (!base.IsHandleCreated || this.IsColumnsWidthDirty || this.View != View.Details || base.Columns.Count < 1 || this.isApplyingSmartWidthOfColumns)
			{
				return;
			}
			this.isApplyingSmartWidthOfColumns = true;
			try
			{
				if (base.HeaderStyle == ColumnHeaderStyle.None)
				{
					base.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				}
			}
			finally
			{
				this.isApplyingSmartWidthOfColumns = false;
			}
			if (this.ShowFilter)
			{
				base.Focus();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Command ShowFilterCommand
		{
			get
			{
				if (this.showFilterCommand == null)
				{
					this.showFilterCommand = new Command();
					this.showFilterCommand.Name = "showFilterCommand";
					this.showFilterCommand.Text = Strings.ShowFilter;
					this.showFilterCommand.Execute += delegate(object param0, EventArgs param1)
					{
						this.ShowFilter = !this.ShowFilter;
						if (this.ShowFilter && base.Columns.Count > 0)
						{
							this.EditFilter(0);
						}
					};
					this.showFilterCommand.Checked = this.ShowFilter;
					this.showFilterCommand.Icon = Icons.CreateFilter;
					this.showFilterCommand.Visible = this.SupportsFiltering;
				}
				return this.showFilterCommand;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command InlineEditCommand
		{
			get
			{
				if (this.inlineEditCommand == null)
				{
					this.inlineEditCommand = new Command();
					this.inlineEditCommand.Enabled = this.CanInlineEdit;
					this.inlineEditCommand.Name = "inlineEditCommand";
					this.inlineEditCommand.Text = Strings.ListEditEdit;
					this.inlineEditCommand.Description = Strings.ListEditEditDescription;
					this.inlineEditCommand.Icon = Icons.Edit;
					this.inlineEditCommand.Execute += delegate(object param0, EventArgs param1)
					{
						this.ProcessCmdKeyRename();
					};
					this.SelectionChanged += delegate(object param0, EventArgs param1)
					{
						this.inlineEditCommand.Enabled = this.CanInlineEdit;
					};
				}
				return this.inlineEditCommand;
			}
		}

		private bool CanInlineEdit
		{
			get
			{
				return base.LabelEdit && base.FocusedItem != null && base.SelectedIndices.Count == 1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Command RemoveCommand
		{
			get
			{
				if (this.removeCommand == null)
				{
					this.removeCommand = new Command();
					this.removeCommand.Enabled = this.CanRemove;
					this.removeCommand.Name = "removeCommand";
					this.removeCommand.Text = Strings.ListEditRemove;
					this.removeCommand.Style = 8;
					this.removeCommand.Description = Strings.ListEditRemoveDescription;
					this.removeCommand.Icon = Icons.Remove;
					this.removeCommand.Execute += delegate(object param0, EventArgs param1)
					{
						try
						{
							this.RemoveSelectedItems();
						}
						catch (InvalidOperationException ex)
						{
							this.UIService.ShowMessage(ex.Message);
						}
					};
					this.SelectionChanged += delegate(object param0, EventArgs param1)
					{
						this.removeCommand.Enabled = this.CanRemove;
					};
				}
				return this.removeCommand;
			}
		}

		private IUIService UIService
		{
			get
			{
				return ((IUIService)this.GetService(typeof(IUIService))) ?? new UIService(this);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command MoveUpCommand
		{
			get
			{
				if (this.moveUpCommand == null)
				{
					this.moveUpCommand = new Command();
					this.moveUpCommand.Enabled = this.CanMove(-1);
					this.moveUpCommand.Name = "moveUpCommand";
					this.moveUpCommand.Text = Strings.ListEditMoveUp;
					this.moveUpCommand.Description = Strings.ListEditMoveUpDescription;
					this.moveUpCommand.Icon = Icons.MoveUp;
					this.moveUpCommand.Execute += delegate(object param0, EventArgs param1)
					{
						this.MoveFocusedItem(-1);
					};
					this.SelectionChanged += delegate(object param0, EventArgs param1)
					{
						this.moveUpCommand.Enabled = this.CanMove(-1);
					};
				}
				return this.moveUpCommand;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Command MoveDownCommand
		{
			get
			{
				if (this.moveDownCommand == null)
				{
					this.moveDownCommand = new Command();
					this.moveDownCommand.Enabled = this.CanMove(1);
					this.moveDownCommand.Name = "moveDownCommand";
					this.moveDownCommand.Text = Strings.ListEditMoveDown;
					this.moveDownCommand.Description = Strings.ListEditMoveDownDescription;
					this.moveDownCommand.Icon = Icons.MoveDown;
					this.moveDownCommand.Execute += delegate(object param0, EventArgs param1)
					{
						this.MoveFocusedItem(1);
					};
					this.SelectionChanged += delegate(object param0, EventArgs param1)
					{
						this.moveDownCommand.Enabled = this.CanMove(1);
					};
				}
				return this.moveDownCommand;
			}
		}

		private bool CanMove(int delta)
		{
			bool result = false;
			if (base.FocusedItem != null && this.ListManager != null && this.View == View.Details)
			{
				object rowFromItem = this.GetRowFromItem(base.FocusedItem);
				if (rowFromItem != null)
				{
					int num = this.ListManager.List.IndexOf(rowFromItem);
					int num2 = num + delta;
					result = (num2 >= 0 && num2 < this.ListManager.Count);
				}
			}
			return result;
		}

		private void MoveFocusedItem(int delta)
		{
			if (base.FocusedItem != null && this.ListManager != null)
			{
				object rowFromItem = this.GetRowFromItem(base.FocusedItem);
				if (rowFromItem != null)
				{
					if (base.VirtualMode)
					{
						this.BackupItemsStates();
					}
					int num = this.ListManager.List.IndexOf(rowFromItem);
					int index = num + delta;
					object value = this.ListManager.List[index];
					this.ListManager.List[index] = rowFromItem;
					this.ListManager.List[num] = value;
					if (this.bindingList == null)
					{
						this.CreateItems();
					}
					this.FocusOnDataRow(rowFromItem);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExchangeColumnHeaderCollection AvailableColumns
		{
			get
			{
				return this.availableColumns;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Command ShowColumnPickerCommand
		{
			get
			{
				if (this.showColumnPickerCommand == null)
				{
					this.showColumnPickerCommand = new Command();
					this.showColumnPickerCommand.Name = "showColumnPickerCommand";
					this.showColumnPickerCommand.Text = Strings.ColumnPickerCommandText;
					this.showColumnPickerCommand.Execute += delegate(object param0, EventArgs param1)
					{
						ColumnPickerDialog columnPickerDialog = new ColumnPickerDialog(this);
						columnPickerDialog.Text = Strings.ColumnPickerDialogTitle;
						using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(columnPickerDialog))
						{
							propertyPageDialog.MinimumSize = columnPickerDialog.MinimumSize;
							propertyPageDialog.ClientSize = columnPickerDialog.ClientSize;
							if (DialogResult.OK == this.UIService.ShowDialog(propertyPageDialog))
							{
								Hashtable hashtable = null;
								if (this.ShowFilter)
								{
									base.Focus();
									hashtable = new Hashtable(base.Columns.Count, StringComparer.InvariantCultureIgnoreCase);
									foreach (object obj in base.Columns)
									{
										ExchangeColumnHeader exchangeColumnHeader = (ExchangeColumnHeader)obj;
										hashtable.Add(exchangeColumnHeader.Name, new object[]
										{
											exchangeColumnHeader.FilterOperator,
											exchangeColumnHeader.FilterValue
										});
									}
								}
								base.BeginUpdate();
								foreach (object obj2 in base.Columns)
								{
									ExchangeColumnHeader exchangeColumnHeader2 = (ExchangeColumnHeader)obj2;
									if (exchangeColumnHeader2.Name != this.SelectionNameProperty)
									{
										exchangeColumnHeader2.Visible = false;
									}
								}
								for (int i = 0; i < columnPickerDialog.DisplayedColumnNames.Count; i++)
								{
									ExchangeColumnHeader exchangeColumnHeader3 = this.AvailableColumns[columnPickerDialog.DisplayedColumnNames[i]];
									if (exchangeColumnHeader3.Name != this.SelectionNameProperty)
									{
										exchangeColumnHeader3.Visible = true;
										if (this.ShowFilter)
										{
											object[] array = (object[])hashtable[exchangeColumnHeader3.Name];
											if (array != null)
											{
												exchangeColumnHeader3.FilterOperator = (FilterOperator)array[0];
												if (!string.IsNullOrEmpty((string)array[1]))
												{
													exchangeColumnHeader3.FilterValue = (string)array[1];
												}
											}
										}
									}
									else
									{
										exchangeColumnHeader3.DisplayIndex = i;
									}
									if (this.ShowFilter)
									{
										this.OnFilterChanged(EventArgs.Empty);
									}
									this.UpdateHeaderSortArrow();
								}
								this.OnColumnsChanged(EventArgs.Empty);
								this.CreateItems();
								base.EndUpdate();
							}
						}
					};
				}
				return this.showColumnPickerCommand;
			}
		}

		protected virtual void OnColumnsChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataListView.EventColumnsChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ColumnsChanged
		{
			add
			{
				base.Events.AddHandler(DataListView.EventColumnsChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataListView.EventColumnsChanged, value);
			}
		}

		private void availableColumns_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				ExchangeColumnHeader exchangeColumnHeader = this.availableColumns[e.NewIndex];
				exchangeColumnHeader.VisibleChanged += this.column_VisibleChanged;
				this.column_VisibleChanged(exchangeColumnHeader, EventArgs.Empty);
				exchangeColumnHeader.SetDefaultDisplayIndex();
				if (exchangeColumnHeader.Name == this.SelectionNameProperty || exchangeColumnHeader.Default)
				{
					exchangeColumnHeader.Visible = true;
				}
			}
		}

		private void availableColumns_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				ExchangeColumnHeader exchangeColumnHeader = this.availableColumns[e.NewIndex];
				exchangeColumnHeader.VisibleChanged -= this.column_VisibleChanged;
				if (base.Columns.Contains(exchangeColumnHeader))
				{
					base.Columns.Remove(exchangeColumnHeader);
				}
			}
		}

		private void column_VisibleChanged(object sender, EventArgs e)
		{
			ExchangeColumnHeader exchangeColumnHeader = (ExchangeColumnHeader)sender;
			if (exchangeColumnHeader.Visible && !base.Columns.Contains(exchangeColumnHeader))
			{
				base.Columns.Add(exchangeColumnHeader);
				return;
			}
			if (!exchangeColumnHeader.Visible)
			{
				int width = exchangeColumnHeader.Width;
				base.Columns.Remove(exchangeColumnHeader);
				exchangeColumnHeader.Width = width;
			}
		}

		public override bool RightToLeftLayout
		{
			get
			{
				return LayoutHelper.CultureInfoIsRightToLeft;
			}
			set
			{
			}
		}

		[DefaultValue(false)]
		public bool AllowRemove
		{
			get
			{
				return this.allowRemove;
			}
			set
			{
				if (this.allowRemove != value)
				{
					this.allowRemove = value;
					this.RemoveCommand.Enabled = this.CanRemove;
				}
			}
		}

		[Browsable(false)]
		public bool CanRemove
		{
			get
			{
				bool flag = null == this.DataSource;
				bool flag2 = (this.bindingList != null) ? this.bindingList.AllowRemove : (null != this.list);
				return this.AllowRemove && base.SelectedIndices.Count > 0 && (flag || flag2);
			}
		}

		private void RemoveSelectedItems()
		{
			if (this.CanRemove)
			{
				int lastIndex = base.SelectedIndices[base.SelectedIndices.Count - 1];
				if (this.list != null)
				{
					this.ListManager.SuspendBinding();
					try
					{
						IList list = this.SelectedObjects;
						for (int i = list.Count - 1; i >= 0; i--)
						{
							int index = this.list.IndexOf(list[i]);
							this.list.RemoveAt(index);
						}
						goto IL_C7;
					}
					finally
					{
						this.ListManager.ResumeBinding();
						this.EnsureItemsCreated();
					}
				}
				base.BeginUpdate();
				try
				{
					for (int j = base.SelectedIndices.Count - 1; j >= 0; j--)
					{
						base.Items.RemoveAt(base.SelectedIndices[j]);
					}
				}
				finally
				{
					base.EndUpdate();
				}
				IL_C7:
				this.UpdateSelectionAfterRemove(lastIndex);
			}
		}

		public void UpdateSelectionAfterRemove(int lastIndex)
		{
			if (base.Items.Count > lastIndex)
			{
				base.FocusedItem = base.Items[lastIndex];
			}
			else if (base.Items.Count > 0)
			{
				base.FocusedItem = base.Items[base.Items.Count - 1];
			}
			base.SelectedIndices.Clear();
			if (base.FocusedItem != null)
			{
				base.FocusedItem.Selected = true;
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			if (this.ShowFilter && UnsafeNativeMethods.GetKeyState(9) < 0 && (Control.ModifierKeys & Keys.Shift) == Keys.None)
			{
				this.EditFilter(0);
			}
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool result = false;
			if (this.Focused)
			{
				if (this.ShowFilter && keyData == (Keys.LButton | Keys.Back | Keys.Shift))
				{
					this.EditFilter(0);
					result = true;
				}
				else
				{
					result = base.ProcessDialogKey(keyData);
				}
			}
			else if (keyData == Keys.Tab)
			{
				base.Focus();
				result = true;
			}
			else if (keyData == (Keys.LButton | Keys.Back | Keys.Shift))
			{
				result = base.Parent.SelectNextControl(this, false, true, true, true);
			}
			return result;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool flag = false;
			if (this.Focused)
			{
				if (keyData <= Keys.F2)
				{
					if (keyData == Keys.Delete)
					{
						flag = this.ProcessCmdKeyDelete();
						goto IL_50;
					}
					if (keyData == Keys.F2)
					{
						flag = this.ProcessCmdKeyRename();
						goto IL_50;
					}
				}
				else
				{
					if (keyData == Keys.F5)
					{
						flag = this.ProcessCmdKeyRefresh();
						goto IL_50;
					}
					if (keyData == (Keys)131137)
					{
						flag = this.ProcessCmdKeySelectAll();
						goto IL_50;
					}
				}
				flag = false;
				IL_50:
				if (!flag)
				{
					flag = base.ProcessCmdKey(ref msg, keyData);
				}
			}
			return flag;
		}

		private bool ProcessCmdKeySelectAll()
		{
			if (base.MultiSelect && base.Items.Count > 0)
			{
				using (new ControlWaitCursor(this))
				{
					base.BeginUpdate();
					try
					{
						NativeMethods.LVITEM lvitem = default(NativeMethods.LVITEM);
						lvitem.mask = 8;
						lvitem.state = 2;
						lvitem.stateMask = 2;
						UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4139, -1, ref lvitem);
					}
					finally
					{
						base.EndUpdate();
					}
				}
				return true;
			}
			return false;
		}

		private bool ProcessCmdKeyRename()
		{
			if (base.LabelEdit && base.FocusedItem != null)
			{
				base.FocusedItem.BeginEdit();
				return true;
			}
			return false;
		}

		private bool ProcessCmdKeyDelete()
		{
			this.Application_Idle(this, EventArgs.Empty);
			bool result = false;
			if (this.DeleteSelectionCommand != null)
			{
				if (this.DeleteSelectionCommand.Enabled && this.HasSelection)
				{
					this.DeleteSelectionCommand.Invoke();
					result = true;
				}
			}
			else if (this.CanRemove)
			{
				this.RemoveCommand.Invoke();
				result = true;
			}
			return result;
		}

		private bool ProcessCmdKeyRefresh()
		{
			bool result = false;
			if (this.RefreshCommand != null && this.refreshCommand.Enabled)
			{
				this.RefreshCommand.Invoke();
				result = true;
			}
			return result;
		}

		private void ListManager_PositionChanged(object sender, EventArgs e)
		{
			this.OnListManagerPositionChanged(e);
		}

		protected virtual void OnListManagerPositionChanged(EventArgs e)
		{
			if (this.ListManager.Position >= 0 && this.ListManager.Position < base.Items.Count && !this.needCreateItemsForRows && !this.IsDataSourceRefreshing && (!base.VirtualMode || this.ListManager.Count == base.Items.Count))
			{
				this.FocusOnDataRow(this.ListManager.Current);
			}
		}

		private void ListManager_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new ListChangedEventHandler(this.ListManager_ListChanged), new object[]
				{
					sender,
					e
				});
				return;
			}
			if (!this.isCreatingItems)
			{
				this.OnListManagerListChanged(e);
			}
		}

		protected virtual void OnListManagerListChanged(ListChangedEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceDebug<ListChangedType, int, int>((long)this.GetHashCode(), "-->DataListView.OnListManagerListChanged:(ChangeType:{0}, NewIndex:{1}, OldIndex:{2})", e.ListChangedType, e.NewIndex, e.OldIndex);
			if (this.AutoGenerateColumns && (e.ListChangedType == ListChangedType.PropertyDescriptorAdded || e.ListChangedType == ListChangedType.PropertyDescriptorChanged || e.ListChangedType == ListChangedType.PropertyDescriptorDeleted))
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "*--DataListView.OnListManagerListChanged:SchemaChanged");
				base.Columns.Clear();
			}
			if (!base.VirtualMode && e.ListChangedType != ListChangedType.ItemAdded)
			{
				this.BackupOffsetFocusedItem();
			}
			if (e.ListChangedType == ListChangedType.Reset || e.ListChangedType == ListChangedType.ItemDeleted)
			{
				base.SelectedIndices.Clear();
				this.RaiseSelectionChanged();
			}
			this.needCreateItemsForRows = true;
			if (this.swCreateItems == null && this.IsDataSourceRefreshing)
			{
				this.swCreateItems = new Stopwatch();
				this.swCreateItems.Start();
			}
			ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "<--DataListView.OnListManagerListChanged:");
		}

		internal void RaiseSelectionChanged()
		{
			if (base.IsHandleCreated && this.updateShell)
			{
				if (this.isShowingContextMenu)
				{
					UnsafeNativeMethods.EndMenu();
				}
				this.selectedObjects = null;
				this.selectedIdentities = null;
				this.OnSelectionChanged(EventArgs.Empty);
				this.updateShell = false;
			}
		}

		private void UpdateNoResultsIndicator()
		{
			if (!string.IsNullOrEmpty(this.NoResultsLabelText))
			{
				if (base.InvokeRequired)
				{
					base.BeginInvoke(new MethodInvoker(this.UpdateNoResultsIndicator));
					return;
				}
				if (this.list != null && this.list.Count > 0)
				{
					this.ShowNoResultsIndicator = false;
				}
				else if (this.DataSourceRefreshed && !this.IsDataSourceRefreshing)
				{
					this.ShowNoResultsIndicator = true;
				}
				else if (this.ShowIndicatorOnceNoResults)
				{
					this.ShowNoResultsIndicator = true;
				}
				base.Invalidate();
			}
		}

		public void FocusOnDataRow(object row)
		{
			ListViewItem itemFromRow = this.GetItemFromRow(row);
			if (itemFromRow != null && itemFromRow != base.FocusedItem)
			{
				base.FocusedItem = itemFromRow;
				if (!itemFromRow.Selected)
				{
					base.SelectedIndices.Clear();
					itemFromRow.Selected = true;
				}
				itemFromRow.EnsureVisible();
				this.Application_Idle(this, EventArgs.Empty);
				return;
			}
			if (itemFromRow == null)
			{
				this.SelectItemBySpecifiedIdentity(this.GetRowIdentity(row), false);
			}
		}

		public object GetRowFromItem(ListViewItem item)
		{
			if (this.item2RowMapping == null)
			{
				return null;
			}
			return ((KeyValuePair<object, object>)this.item2RowMapping[item]).Value;
		}

		public ListViewItem GetItemFromRow(object row)
		{
			object rowIdentity = this.GetRowIdentity(row);
			ListViewItem listViewItem = (rowIdentity != null && this.id2ItemMapping != null) ? ((ListViewItem)this.id2ItemMapping[rowIdentity]) : null;
			if (listViewItem == null && base.VirtualMode && this.list != null)
			{
				int num = this.list.IndexOf(row);
				if (num >= 0 && num < base.Items.Count)
				{
					listViewItem = base.Items[num];
				}
			}
			return listViewItem;
		}

		public ListViewItem GetItemFromIdentity(object identity)
		{
			int itemIndexFromIdentity = this.GetItemIndexFromIdentity(identity);
			if (itemIndexFromIdentity < 0)
			{
				return null;
			}
			return base.Items[itemIndexFromIdentity];
		}

		public int GetItemIndexFromIdentity(object identity)
		{
			int i = -1;
			if (identity != null)
			{
				if (base.VirtualMode && this.list != null)
				{
					int num = Math.Min(this.list.Count, base.Items.Count);
					for (i = num - 1; i >= 0; i--)
					{
						if (identity.Equals(this.GetRowIdentity(this.list[i])))
						{
							break;
						}
					}
				}
				else
				{
					ListViewItem listViewItem = (this.id2ItemMapping != null) ? ((ListViewItem)this.id2ItemMapping[identity]) : null;
					i = ((listViewItem != null) ? listViewItem.Index : -1);
				}
			}
			return i;
		}

		[DefaultValue("")]
		public string NoResultsLabelText
		{
			get
			{
				return this.noResultsIndicatorText;
			}
			set
			{
				this.noResultsIndicatorText = value;
				if (string.IsNullOrEmpty(value) && this.ShowNoResultsIndicator)
				{
					this.ShowNoResultsIndicator = false;
				}
				this.UpdateNoResultsIndicator();
			}
		}

		[DefaultValue(false)]
		public bool ShowIndicatorOnceNoResults
		{
			get
			{
				return this.showIndicatorOnceNoResults;
			}
			set
			{
				if (this.ShowIndicatorOnceNoResults != value)
				{
					this.showIndicatorOnceNoResults = value;
					this.UpdateNoResultsIndicator();
				}
			}
		}

		[DefaultValue(false)]
		private bool ShowNoResultsIndicator { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(null)]
		public IconLibrary IconLibrary
		{
			get
			{
				return this.iconLibrary;
			}
			set
			{
				this.iconLibrary = value;
				ImageList imageList = base.SmallImageList = ((value != null) ? value.SmallImageList : null);
				if (this.View == View.LargeIcon || this.View == View.Tile)
				{
					imageList = (base.LargeImageList = ((value != null) ? value.LargeImageList : null));
				}
				if (imageList == null)
				{
					this.ImageIndex = -1;
					return;
				}
				if (this.ImageIndex == -1 && imageList.Images.Count > 0)
				{
					this.ImageIndex = 0;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use IconLibrary instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new ImageList SmallImageList
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use IconLibrary instead.")]
		[Browsable(false)]
		public new ImageList LargeImageList
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[Browsable(false)]
		public Icon SelectedItemIcon
		{
			get
			{
				Icon result = null;
				if (this.IconLibrary != null && base.SelectedIndices.Count > 0)
				{
					ListViewItem firstSelectedItem = this.FirstSelectedItem;
					result = this.IconLibrary.GetIcon(firstSelectedItem.ImageKey, firstSelectedItem.ImageIndex);
				}
				return result;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ListViewItem FirstSelectedItem
		{
			get
			{
				if (base.SelectedIndices.Count <= 0)
				{
					return null;
				}
				return base.Items[base.SelectedIndices[0]];
			}
		}

		private int TopItemIndex
		{
			get
			{
				if (!base.IsHandleCreated)
				{
					return -1;
				}
				return (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4135, 0, 0U);
			}
		}

		private int FocusedItemIndex
		{
			get
			{
				if (!base.IsHandleCreated)
				{
					return -1;
				}
				return (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, base.Handle), 4108, -1, 1U);
			}
		}

		[Obsolete("Use SelectedIndices instead")]
		public new ListView.SelectedListViewItemCollection SelectedItems
		{
			get
			{
				return null;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int TotalItemsCount
		{
			get
			{
				return base.Items.Count;
			}
		}

		[Browsable(false)]
		public virtual bool SupportsVirtualMode
		{
			get
			{
				return !this.ShowGroups && this.View != View.Tile;
			}
		}

		protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
		{
			base.OnRetrieveVirtualItem(e);
			if (e.Item == null && this.cachedItems != null && this.list != null)
			{
				if (e.ItemIndex >= this.cachedItems.Length && this.cachedItems.Length < base.VirtualListSize)
				{
					Array.Resize<ListViewItem>(ref this.cachedItems, base.VirtualListSize);
				}
				if (this.cachedItems[e.ItemIndex] == null || this.cachedItems[e.ItemIndex].SubItems.Count != base.Columns.Count)
				{
					if (e.ItemIndex < this.list.Count)
					{
						e.Item = (this.cachedItems[e.ItemIndex] = this.CreateListViewItemForRow(this.list[e.ItemIndex]));
						return;
					}
					e.Item = (this.cachedItems[e.ItemIndex] = this.CreateNewListViewItem(null));
					for (int i = base.Columns.Count - e.Item.SubItems.Count; i > 0; i--)
					{
						e.Item.SubItems.Add(string.Empty);
					}
					return;
				}
				else
				{
					e.Item = this.cachedItems[e.ItemIndex];
				}
			}
		}

		protected override void OnSearchForVirtualItem(SearchForVirtualItemEventArgs e)
		{
			this.EnsureItemsCreated();
			base.OnSearchForVirtualItem(e);
			if (e.Index < 0 && this.list != null && base.Items.Count > 0)
			{
				int num = e.StartIndex;
				if (num >= base.Items.Count)
				{
					num = 0;
				}
				int num2 = num;
				do
				{
					object obj = this.list[num2];
					if (obj != null)
					{
						PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
						for (int i = 0; i < base.Columns.Count; i++)
						{
							string subItemTextForRow = this.GetSubItemTextForRow(obj, i, properties);
							if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(subItemTextForRow, e.Text, CompareOptions.IgnoreCase))
							{
								e.Index = num2;
							}
							if (e.Index >= 0 || !e.IncludeSubItemsInSearch)
							{
								break;
							}
						}
					}
					num2++;
					if (num2 >= base.Items.Count)
					{
						num2 = 0;
					}
				}
				while (num2 != num && e.Index < 0);
			}
		}

		protected override void OnVirtualItemsSelectionRangeChanged(ListViewVirtualItemsSelectionRangeChangedEventArgs e)
		{
			base.OnVirtualItemsSelectionRangeChanged(e);
			if (e.IsSelected)
			{
				this.OnSelectedIndexChanged(EventArgs.Empty);
			}
		}

		internal void ExportListToFile(string fileName, Encoding fileEncoding, char separator)
		{
			this.EnsureItemsCreated();
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, fileEncoding))
			{
				int[] array = new int[base.Columns.Count];
				for (int i = 0; i < base.Columns.Count; i++)
				{
					ColumnHeader columnHeader = base.Columns[i];
					array[columnHeader.DisplayIndex] = i;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (int index in array)
				{
					ColumnHeader columnHeader2 = base.Columns[index];
					stringBuilder.Append(DataListView.EscapeText(columnHeader2.Text));
					stringBuilder.Append(separator);
				}
				if (stringBuilder.Length > 0)
				{
					streamWriter.WriteLine(stringBuilder.ToString(0, stringBuilder.Length - 1));
				}
				else
				{
					streamWriter.WriteLine();
				}
				for (int k = 0; k < base.Items.Count; k++)
				{
					ListViewItem listViewItem = base.Items[k];
					stringBuilder.Length = 0;
					foreach (int index2 in array)
					{
						ListViewItem.ListViewSubItem listViewSubItem = listViewItem.SubItems[index2];
						stringBuilder.Append(DataListView.EscapeText(listViewSubItem.Text));
						stringBuilder.Append(separator);
					}
					if (stringBuilder.Length > 0)
					{
						streamWriter.WriteLine(stringBuilder.ToString(0, stringBuilder.Length - 1));
					}
					else
					{
						streamWriter.WriteLine();
					}
				}
			}
		}

		private static string EscapeText(string cellText)
		{
			string text = cellText;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace("\"", "\"\"");
				string text2 = ", \v\f\t\r\n\"";
				if (text.IndexOfAny(text2.ToCharArray()) >= 0)
				{
					text = "\"" + text + "\"";
				}
			}
			return text;
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = new DataListViewBulkEditorAdapter(this);
				}
				return this.bulkEditorAdapter;
			}
		}

		private bool ShowBulkEditIndicator { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public string BulkEditingIndicatorText
		{
			get
			{
				return this.bulkEditingIndicatorText;
			}
			set
			{
				this.bulkEditingIndicatorText = value;
				this.ShowBulkEditIndicator = !string.IsNullOrEmpty(this.BulkEditingIndicatorText);
				base.Invalidate();
			}
		}

		public void FireDataSourceChanged()
		{
			this.OnDataSourceChanged(EventArgs.Empty);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		private void OnUserModified(EventArgs e)
		{
			if (this.UserModified != null)
			{
				this.UserModified(this, new PropertyChangedEventArgs("DataSource"));
			}
		}

		private const int TimeIntervalForCreateItems = 100;

		private const int DefaultSelectedColumnIndex = -1;

		private const bool DefaultShowFilter = false;

		private const int moveUpDelta = -1;

		private const int moveDownDelta = 1;

		private bool needCreateItemsForRows;

		private bool isCreatingItems;

		private ListViewItem[] cachedItems;

		private object focusedItemIdentity;

		private int backupTopItemIndex = -1;

		private ArrayList backupSelectedIdentities;

		private Stopwatch swCreateItems;

		private bool showSubitemIcon;

		private int selectedColumnIndex = -1;

		private bool showfilter;

		private HandleRef headerHandle;

		private HandleRef tooltipHandle;

		private FilterValueCollection filterValues;

		private Command contextMenuCommand;

		private Command arrangeByCommand;

		private Command showInGroupsCommand;

		private Command deleteSelectionCommand;

		private Command refreshCommand;

		private Command showSelectionPropertiesCommand;

		private int imageIndex = -1;

		private object dataSource;

		private IRefreshableNotification dataSourceRefresher;

		private int focusedItemOffset = -1;

		private string dataMember = string.Empty;

		private bool inSetListManager;

		private static readonly object EventDataSourceChanged = new object();

		private CurrencyManager listManager;

		private IList list;

		private IBindingList bindingList;

		private IBindingListView bindingListView;

		private IAdvancedBindingListView advancedBindingListView;

		private bool autoGenerateColumns = true;

		private static readonly object EventUpdateItem = new object();

		private string imagePropertyName = "";

		private IList selectedObjects;

		private IComparer selectedObjectsSorter;

		private IList selectedIdentities;

		private object selectedItemIdentity;

		private bool hideSortArrow;

		private string identityProperty = string.Empty;

		private string selectionNameProperty = string.Empty;

		private string selectedObjectDetailsTypeProperty = string.Empty;

		private string sortProperty = "";

		private ListSortDirection sortDirection;

		private string groupProperty = "";

		private ListSortDirection groupDirection;

		private bool isDraggingNonReorderableColumn;

		private bool isShowingContextMenu;

		private bool updateShell;

		private bool isColumnsWidthDirty;

		private bool isApplyingSmartWidthOfColumns;

		private Command showFilterCommand;

		private Command inlineEditCommand;

		private Command removeCommand;

		private Command moveUpCommand;

		private Command moveDownCommand;

		private ExchangeColumnHeaderCollection availableColumns;

		private Command showColumnPickerCommand;

		private static readonly object EventColumnsChanged = new object();

		private bool allowRemove;

		private string noResultsIndicatorText;

		private bool showIndicatorOnceNoResults;

		private Hashtable id2ItemMapping;

		private Hashtable item2RowMapping;

		private Hashtable itemsStates = new Hashtable();

		private IconLibrary iconLibrary;

		private BulkEditorAdapter bulkEditorAdapter;

		private string bulkEditingIndicatorText;
	}
}
