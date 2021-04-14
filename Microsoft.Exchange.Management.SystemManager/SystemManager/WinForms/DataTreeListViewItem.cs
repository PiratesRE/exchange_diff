using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataTreeListViewItem : ListViewItem
	{
		public DataTreeListViewItem() : this(null, null)
		{
		}

		public DataTreeListViewItem(DataTreeListView listView, object row) : this(listView, row, null)
		{
		}

		public DataTreeListViewItem(DataTreeListView listView, object row, DataTreeListViewItem parentItem)
		{
			this.dataSource = row;
			this.listView = listView;
			this.IsLeaf = true;
			this.Parent = parentItem;
			if (this.Parent != null)
			{
				this.Parent.ChildrenItems.Add(this);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTreeListViewItemCollection ChildrenItems
		{
			get
			{
				if (this.childrenItems == null)
				{
					this.childrenItems = new DataTreeListViewItemCollection(this);
				}
				return this.childrenItems;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLeaf
		{
			get
			{
				return this.isLeaf;
			}
			set
			{
				if (value != this.IsLeaf)
				{
					if (value && this.ChildrenItems.Count > 0)
					{
						throw new InvalidOperationException();
					}
					this.isLeaf = value;
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(null)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTreeListViewItem Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (value != this.Parent)
				{
					if (this.Parent != null)
					{
						this.Parent.ChildrenItems.Remove(this);
					}
					this.parent = value;
					if (this.Parent == null)
					{
						if (this.ListView != null && !this.ListView.TopItems.Contains(this))
						{
							this.ListView.TopItems.Add(this);
							return;
						}
					}
					else if (!this.Parent.ChildrenItems.Contains(value))
					{
						this.Parent.ChildrenItems.Add(this);
					}
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		[Browsable(false)]
		public bool IsExpanded
		{
			get
			{
				return this.isExpanded;
			}
			set
			{
				if (this.IsExpanded != value)
				{
					this.isExpanded = value;
					if (this.ListView != null && this.ChildrenItems.Count > 0 && this.isInListView)
					{
						this.ListView.BeginUpdate();
						try
						{
							if (value)
							{
								this.Expand();
							}
							else
							{
								this.Collapse();
							}
						}
						finally
						{
							this.ListView.EndUpdate();
						}
						this.ListView.Invalidate(base.Bounds);
					}
				}
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataTreeListView ListView
		{
			get
			{
				return (base.ListView as DataTreeListView) ?? this.listView;
			}
		}

		[Browsable(true)]
		public Color BackColorBegin
		{
			get
			{
				return this.backColorBegin;
			}
			set
			{
				this.backColorBegin = value;
				if (this.ListView != null)
				{
					this.ListView.Invalidate(base.Bounds);
				}
			}
		}

		[Browsable(true)]
		public Color BackColorEnd
		{
			get
			{
				return this.backColorEnd;
			}
			set
			{
				this.backColorEnd = value;
				if (this.ListView != null)
				{
					this.ListView.Invalidate(base.Bounds);
				}
			}
		}

		private DataTreeListViewColumnMapping ColumnMapping
		{
			get
			{
				return this.columnMapping;
			}
			set
			{
				this.columnMapping = value;
			}
		}

		internal bool IsInListView
		{
			get
			{
				return this.isInListView;
			}
		}

		internal Rectangle GetPlusMinusButtonBound()
		{
			Rectangle bounds = base.GetBounds(ItemBoundsPortion.Icon);
			bounds.Offset(-SystemInformation.SmallIconSize.Width, 0);
			return bounds;
		}

		public override void Remove()
		{
			if (this.Parent != null)
			{
				this.Parent.ChildrenItems.Remove(this);
				return;
			}
			if (this.ListView != null)
			{
				this.ListView.TopItems.Remove(this);
			}
		}

		internal int GetNextSiblingListViewIndex()
		{
			int result = -1;
			if (this.IsInListView)
			{
				if (this.IsLeaf || this.ChildrenItems.Count == 0 || !this.IsExpanded)
				{
					result = base.Index + 1;
				}
				else
				{
					result = this.ChildrenItems[this.ChildrenItems.Count - 1].GetNextSiblingListViewIndex();
				}
			}
			return result;
		}

		internal void AddToListView(DataTreeListView listView)
		{
			this.isInListView = true;
			if (base.Index < 0 && listView != null)
			{
				int num;
				if (this.Parent == null)
				{
					num = listView.Items.Count;
				}
				else
				{
					int num2 = this.Parent.ChildrenItems.IndexOf(this);
					if (num2 == 0)
					{
						num = this.Parent.Index + 1;
					}
					else if (this.Parent.ChildrenItems[num2 - 1].Index > 0)
					{
						num = this.Parent.ChildrenItems[num2 - 1].GetNextSiblingListViewIndex();
					}
					else
					{
						num = -1;
					}
				}
				if (num >= 0)
				{
					listView.Items.Insert(num, this);
				}
			}
			this.BindToChildDataSources();
			this.RecreateChildItems();
			listView.RestoreItemStates(this);
		}

		internal void RemoveFromListView()
		{
			this.isInListView = false;
			if (this.IsExpanded)
			{
				foreach (object obj in this.ChildrenItems)
				{
					DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
					dataTreeListViewItem.RemoveFromListView();
				}
			}
			if (base.ListView != null)
			{
				base.ListView.Items.Remove(this);
			}
		}

		private void ApplySort()
		{
			int num = -1;
			if (!string.IsNullOrEmpty(this.ListView.SortProperty))
			{
				foreach (object obj in this.ListView.Columns)
				{
					ColumnHeader columnHeader = (ColumnHeader)obj;
					if (this.ListView.SortProperty == columnHeader.Name)
					{
						num = columnHeader.Index;
						break;
					}
				}
			}
			foreach (object obj2 in this.childDataSources)
			{
				BindingSource bindingSource = (BindingSource)obj2;
				if (bindingSource.SupportsSorting && bindingSource.Count > 1)
				{
					string text = this.ListView.SortProperty;
					ListSortDirection sortDirection = this.ListView.SortDirection;
					DataTreeListViewColumnMapping dataTreeListViewColumnMapping = (DataTreeListViewColumnMapping)this.dataSource2ColumnMapping[bindingSource];
					if (num >= 0 && dataTreeListViewColumnMapping.PropertyNames.Count > num && !string.IsNullOrEmpty(dataTreeListViewColumnMapping.PropertyNames[num]))
					{
						text = dataTreeListViewColumnMapping.PropertyNames[num];
					}
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(bindingSource[0]);
					if (string.IsNullOrEmpty(text) || properties[text] == null)
					{
						text = dataTreeListViewColumnMapping.SortProperty;
						sortDirection = dataTreeListViewColumnMapping.SortDirection;
					}
					if (!string.IsNullOrEmpty(text) && properties[text] != null && (!bindingSource.IsSorted || bindingSource.SortProperty.Name != text || bindingSource.SortDirection != sortDirection))
					{
						bindingSource.ApplySort(properties[text], sortDirection);
					}
				}
			}
		}

		private void RecreateChildItems()
		{
			if (this.ListView != null && this.DataSource != null && this.childDataSources.Count > 0)
			{
				this.ApplySort();
				if (this.ListView.InvokeRequired)
				{
					this.ListView.Invoke(new MethodInvoker(this.RecreateChildItems));
					return;
				}
				this.ListView.BeginUpdate();
				try
				{
					if (this.ChildrenItems.Count > 0)
					{
						if (this.ChildrenItems[0].IsInListView)
						{
							this.ListView.BackupItemsStates();
						}
						this.ChildrenItems.Clear();
					}
					foreach (object obj in this.childDataSources)
					{
						BindingSource bindingSource = (BindingSource)obj;
						foreach (object obj2 in bindingSource)
						{
							DataTreeListViewItem dataTreeListViewItem = this.ListView.InternalCreateListViewItemForRow(obj2);
							dataTreeListViewItem.ColumnMapping = (DataTreeListViewColumnMapping)this.dataSource2ColumnMapping[bindingSource];
							dataTreeListViewItem.UpdateItem();
							if (dataTreeListViewItem.ColumnMapping.ImageIndex < 0)
							{
								if (!string.IsNullOrEmpty(this.ListView.ImagePropertyName))
								{
									PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj2);
									PropertyDescriptor propertyDescriptor = properties[this.ListView.ImagePropertyName];
									if (propertyDescriptor != null)
									{
										dataTreeListViewItem.ImageKey = DataListView.GetPropertyValue(obj2, propertyDescriptor).ToString();
									}
								}
							}
							else
							{
								dataTreeListViewItem.ImageIndex = dataTreeListViewItem.ColumnMapping.ImageIndex;
							}
							this.ChildrenItems.Add(dataTreeListViewItem);
						}
					}
					this.ListView.RaiseItemsForRowsCreated(EventArgs.Empty);
				}
				finally
				{
					this.ListView.EndUpdate();
				}
				this.ListView.TrySelectItemBySpecifiedIdentity();
			}
		}

		private void UpdateItem()
		{
			if (this.ListView != null)
			{
				this.ListView.InternalUpdateItem(this);
			}
			DataTreeListViewColumnMapping dataTreeListViewColumnMapping = this.ColumnMapping;
			if (dataTreeListViewColumnMapping != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.DataSource);
				for (int i = 0; i < dataTreeListViewColumnMapping.PropertyNames.Count; i++)
				{
					PropertyDescriptor propertyDescriptor;
					if (!string.IsNullOrEmpty(dataTreeListViewColumnMapping.PropertyNames[i]) && (propertyDescriptor = properties[dataTreeListViewColumnMapping.PropertyNames[i]]) != null && i < base.SubItems.Count)
					{
						base.SubItems[i].Text = this.ListView.FormatRawValueByColumn(DataListView.GetPropertyValue(this.DataSource, propertyDescriptor), this.ListView.Columns[i]);
					}
				}
			}
		}

		private void BindToChildDataSources()
		{
			if (this.ListView != null && this.DataSource != null)
			{
				this.ClearChildrenDataSources();
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.DataSource);
				foreach (object obj in this.ListView.ChildrenDataMembers)
				{
					DataTreeListViewColumnMapping dataTreeListViewColumnMapping = (DataTreeListViewColumnMapping)obj;
					if (properties[dataTreeListViewColumnMapping.DataMember] != null)
					{
						BindingSource bindingSource = new BindingSource(this.DataSource, dataTreeListViewColumnMapping.DataMember);
						this.childDataSources.Add(bindingSource);
						bindingSource.ListChanged += this.ChildList_ListChanged;
						this.dataSource2ColumnMapping[bindingSource] = dataTreeListViewColumnMapping;
						this.ColumnMapping = dataTreeListViewColumnMapping;
					}
				}
			}
		}

		internal void ClearChildrenDataSources()
		{
			foreach (object obj in this.childDataSources)
			{
				BindingSource bindingSource = (BindingSource)obj;
				bindingSource.ListChanged -= this.ChildList_ListChanged;
			}
			this.childDataSources.Clear();
			foreach (object obj2 in this.ChildrenItems)
			{
				DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj2;
				dataTreeListViewItem.ClearChildrenDataSources();
			}
		}

		private void ChildList_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (this.ListView == null)
			{
				this.ChildrenItems.Clear();
				this.IsExpanded = false;
				this.childDataSources.Clear();
				return;
			}
			IList list = sender as IList;
			if (list != null)
			{
				switch (e.ListChangedType)
				{
				case ListChangedType.Reset:
				case ListChangedType.ItemDeleted:
					this.ListView.SelectedIndices.Clear();
					this.ListView.RaiseSelectionChanged();
					this.RecreateChildItems();
					return;
				case ListChangedType.ItemChanged:
				{
					object row = list[e.NewIndex];
					DataTreeListViewItem dataTreeListViewItem = this.ListView.GetItemFromRow(row) as DataTreeListViewItem;
					if (dataTreeListViewItem == null)
					{
						return;
					}
					if (this.ListView.InvokeRequired)
					{
						this.ListView.Invoke(new MethodInvoker(dataTreeListViewItem.UpdateItem));
						return;
					}
					dataTreeListViewItem.UpdateItem();
					return;
				}
				}
				this.RecreateChildItems();
			}
		}

		private void Expand()
		{
			if (!this.IsLeaf && this.IsExpanded && this.ListView != null)
			{
				this.ListView.InternalOnExpandItem(this);
				int index = base.Index;
				foreach (object obj in this.ChildrenItems)
				{
					DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
					dataTreeListViewItem.AddToListView(this.listView);
				}
				foreach (object obj2 in this.ChildrenItems)
				{
					DataTreeListViewItem dataTreeListViewItem2 = (DataTreeListViewItem)obj2;
					if (dataTreeListViewItem2.IsExpanded)
					{
						dataTreeListViewItem2.Expand();
					}
				}
			}
		}

		private void Collapse()
		{
			if (!this.IsLeaf && !this.IsExpanded && this.ListView != null)
			{
				this.ListView.InternalOnCollapseItem(this);
				foreach (object obj in this.ChildrenItems)
				{
					DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
					dataTreeListViewItem.RemoveFromListView();
				}
			}
		}

		private DataTreeListViewItemCollection childrenItems;

		private bool isLeaf;

		private DataTreeListViewItem parent;

		private bool isExpanded;

		private object dataSource;

		private DataTreeListView listView;

		private Color backColorBegin = Color.Empty;

		private Color backColorEnd = Color.Empty;

		private DataTreeListViewColumnMapping columnMapping;

		private ArrayList childDataSources = new ArrayList();

		private bool isInListView;

		private Hashtable dataSource2ColumnMapping = new Hashtable();
	}
}
