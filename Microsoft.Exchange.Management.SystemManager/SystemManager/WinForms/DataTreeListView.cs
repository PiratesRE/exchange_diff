using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataTreeListView : DataListView
	{
		public DataTreeListView()
		{
			this.IconLibrary = this.emptyIconLibrary;
			this.topItems = new DataTreeListViewItemCollection(this);
			this.AutoExpandNewItem = false;
			this.DoubleBuffered = true;
			this.OwnerDraw = true;
			this.emptyImage = new Bitmap(1, 1);
			this.emptyImage.SetPixel(0, 0, this.BackColor);
			this.BackgroundImage = this.emptyImage;
			this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.ChildrenDataMembers.CollectionChanged += delegate(object param0, CollectionChangeEventArgs param1)
			{
				this.CreateItems();
			};
			base.AvailableColumns.ListChanged += delegate(object param0, ListChangedEventArgs param1)
			{
				if (base.AvailableColumns.Count > 0 && base.AvailableColumns[0].IsReorderable)
				{
					base.AvailableColumns[0].IsReorderable = false;
				}
				foreach (ExchangeColumnHeader exchangeColumnHeader in base.AvailableColumns)
				{
					exchangeColumnHeader.IsSortable = false;
				}
				base.HideSortArrow = true;
			};
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.UpdateTopItemsFontStyle();
		}

		private void UpdateTopItemsFontStyle()
		{
			Font font = new Font(this.Font, this.Font.Style | FontStyle.Bold);
			base.SuspendLayout();
			foreach (object obj in this.TopItems)
			{
				DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
				dataTreeListViewItem.Font = font;
			}
			base.ResumeLayout(false);
		}

		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			DataTreeListViewItem dataTreeListViewItem = e.Item as DataTreeListViewItem;
			if (e.State != (ListViewItemStates)0 && dataTreeListViewItem != null)
			{
				Rectangle bounds = dataTreeListViewItem.Bounds;
				bounds.Intersect(base.ClientRectangle);
				if (base.Enabled && !bounds.IsEmpty && !dataTreeListViewItem.Selected && !dataTreeListViewItem.BackColorBegin.IsEmpty && !dataTreeListViewItem.BackColorEnd.IsEmpty)
				{
					using (Brush brush = new LinearGradientBrush(e.Bounds, dataTreeListViewItem.BackColorBegin, dataTreeListViewItem.BackColorEnd, LinearGradientMode.Horizontal))
					{
						e.Graphics.FillRectangle(brush, e.Bounds);
						goto IL_BE;
					}
				}
				if (base.Enabled && this.BackgroundImage == this.emptyImage)
				{
					e.DrawBackground();
				}
				IL_BE:
				e.DrawDefault = true;
				base.OnDrawItem(e);
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg == 15)
			{
				base.WndProc(ref m);
				this.DrawPlusMinusButtons();
				return;
			}
			base.WndProc(ref m);
		}

		private void DrawPlusMinusButtons()
		{
			using (Graphics graphics = base.CreateGraphics())
			{
				foreach (object obj in base.Items)
				{
					DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
					Rectangle bounds = dataTreeListViewItem.Bounds;
					if (bounds.Bottom >= base.ClientRectangle.Top)
					{
						bounds.Width = base.Columns[0].Width;
						bounds.Intersect(base.ClientRectangle);
						if (!dataTreeListViewItem.IsLeaf && !bounds.IsEmpty && dataTreeListViewItem.ChildrenItems.Count > 0)
						{
							Region clip = graphics.Clip;
							graphics.SetClip(LayoutHelper.MirrorRectangle(bounds, this));
							DataTreeListView.DrawPlusMinusButton(graphics, LayoutHelper.MirrorRectangle(dataTreeListViewItem.GetPlusMinusButtonBound(), this), !dataTreeListViewItem.IsExpanded);
							graphics.Clip = clip;
						}
						else if (bounds.Top > base.ClientRectangle.Bottom)
						{
							break;
						}
					}
				}
			}
		}

		private static void DrawPlusMinusButton(Graphics graphics, Rectangle bound, bool isPlusButton)
		{
			int num = Math.Min(bound.Width, bound.Height);
			num /= 2;
			if (num % 2 == 0)
			{
				num++;
			}
			Rectangle rectangle = new Rectangle(bound.Location, new Size(num, num));
			rectangle.Offset((bound.Width - num) / 2, (bound.Height - num) / 2);
			graphics.DrawRectangle(SystemPens.GrayText, rectangle.X, rectangle.Y, num - 1, num - 1);
			int num2 = rectangle.Width - 4;
			if (num2 > 0)
			{
				int num3 = rectangle.Y + rectangle.Height / 2;
				graphics.DrawLine(SystemPens.WindowText, rectangle.X + 2, num3, rectangle.X + 2 + num2 - 1, num3);
			}
			if (isPlusButton && num2 > 0)
			{
				int num4 = rectangle.X + rectangle.Width / 2;
				graphics.DrawLine(SystemPens.WindowText, num4, rectangle.Y + 2, num4, rectangle.Y + 2 + num2 - 1);
			}
		}

		protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
		{
			base.OnColumnWidthChanged(e);
			base.Invalidate();
		}

		protected override void OnItemActivate(EventArgs e)
		{
			if (Control.MouseButtons == MouseButtons.Left)
			{
				Point pt = base.PointToClient(Control.MousePosition);
				DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)base.GetItemAt(pt.X, pt.Y);
				if (dataTreeListViewItem != null && dataTreeListViewItem.ChildrenItems.Count > 0 && !dataTreeListViewItem.IsLeaf && dataTreeListViewItem.GetPlusMinusButtonBound().Contains(pt))
				{
					return;
				}
			}
			base.OnItemActivate(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			DataTreeListViewItem dataTreeListViewItem = base.FocusedItem as DataTreeListViewItem;
			if (dataTreeListViewItem != null)
			{
				switch (e.KeyCode)
				{
				case Keys.Left:
					if (dataTreeListViewItem.IsExpanded)
					{
						dataTreeListViewItem.IsExpanded = false;
						e.Handled = true;
					}
					else if (dataTreeListViewItem.Parent != null)
					{
						base.SelectedIndices.Clear();
						dataTreeListViewItem.Parent.Selected = true;
						dataTreeListViewItem.Parent.Focused = true;
						e.Handled = true;
					}
					break;
				case Keys.Right:
					if (dataTreeListViewItem.IsExpanded)
					{
						if (dataTreeListViewItem.ChildrenItems.Count > 0)
						{
							base.SelectedIndices.Clear();
							dataTreeListViewItem.ChildrenItems[0].Selected = true;
							dataTreeListViewItem.ChildrenItems[0].Focused = true;
							e.Handled = true;
						}
					}
					else if (!dataTreeListViewItem.IsLeaf)
					{
						dataTreeListViewItem.IsExpanded = true;
						e.Handled = true;
					}
					break;
				}
			}
			if (!e.Handled)
			{
				base.OnKeyDown(e);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				DataTreeListViewItem dataTreeListViewItem = base.GetItemAt(e.X, e.Y) as DataTreeListViewItem;
				if (dataTreeListViewItem != null && !dataTreeListViewItem.IsLeaf && dataTreeListViewItem.ChildrenItems.Count > 0 && dataTreeListViewItem.GetPlusMinusButtonBound().Contains(e.X, e.Y))
				{
					dataTreeListViewItem.IsExpanded = !dataTreeListViewItem.IsExpanded;
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			bool flag = this.BackgroundImage == this.emptyImage;
			this.emptyImage = new Bitmap(1, 1);
			this.emptyImage.SetPixel(0, 0, this.BackColor);
			if (flag)
			{
				this.BackgroundImage = this.emptyImage;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.emptyImage.Dispose();
				this.emptyIconLibrary.Dispose();
				this.TopItems.Clear();
			}
			base.Dispose(disposing);
		}

		public void ExpandAll()
		{
			this.SetExpandedStatusOnRootItems(true, true);
		}

		public void ExpandRootItems()
		{
			this.SetExpandedStatusOnRootItems(true, false);
		}

		public void CollapseAll()
		{
			this.SetExpandedStatusOnRootItems(false, true);
		}

		public void CollapseRootItems()
		{
			this.SetExpandedStatusOnRootItems(false, false);
		}

		private void SetExpandedStatusOnRootItems(bool isExpanded, bool isRecursive)
		{
			foreach (object obj in this.TopItems)
			{
				DataTreeListViewItem item = (DataTreeListViewItem)obj;
				this.SetExpandedStatusOnSubItem(item, isExpanded, isRecursive);
			}
		}

		private void SetExpandedStatusOnSubItem(DataTreeListViewItem item, bool isExpanded, bool isRecursive)
		{
			item.IsExpanded = isExpanded;
			if (isRecursive)
			{
				foreach (object obj in item.ChildrenItems)
				{
					DataTreeListViewItem item2 = (DataTreeListViewItem)obj;
					this.SetExpandedStatusOnSubItem(item2, isExpanded, isRecursive);
				}
			}
		}

		internal void InternalOnExpandItem(DataTreeListViewItem item)
		{
			this.OnExpandItem(new ItemCheckedEventArgs(item));
			base.Invalidate(item.Bounds);
		}

		internal void InternalOnCollapseItem(DataTreeListViewItem item)
		{
			this.OnCollapseItem(new ItemCheckedEventArgs(item));
			base.Invalidate(item.Bounds);
		}

		internal bool HasChildDataMember(object row)
		{
			bool result = false;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(row);
			foreach (object obj in this.ChildrenDataMembers)
			{
				DataTreeListViewColumnMapping dataTreeListViewColumnMapping = (DataTreeListViewColumnMapping)obj;
				if (properties[dataTreeListViewColumnMapping.DataMember] != null)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		protected virtual void OnExpandItem(ItemCheckedEventArgs e)
		{
			if (this.ExpandingItem != null)
			{
				this.ExpandingItem(this, e);
			}
		}

		public event ItemCheckedEventHandler ExpandingItem;

		protected virtual void OnCollapseItem(ItemCheckedEventArgs e)
		{
			if (this.CollapsingItem != null)
			{
				this.CollapsingItem(this, e);
			}
		}

		public event ItemCheckedEventHandler CollapsingItem;

		protected override void OnListManagerListChanged(ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
			case ListChangedType.ItemAdded:
			case ListChangedType.ItemDeleted:
			case ListChangedType.ItemMoved:
				base.OnListManagerListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				return;
			case ListChangedType.ItemChanged:
			{
				DataTreeListViewItem dataTreeListViewItem = this.TopItems[e.NewIndex];
				if (dataTreeListViewItem != null)
				{
					this.InternalUpdateItem(dataTreeListViewItem);
					return;
				}
				break;
			}
			default:
				base.OnListManagerListChanged(e);
				break;
			}
		}

		protected override void OnItemsForRowsCreated(EventArgs e)
		{
			this.TopItems.Clear();
			base.RestoreItemsStates(false);
			foreach (object obj in base.Items)
			{
				DataTreeListViewItem item = (DataTreeListViewItem)obj;
				this.TopItems.Add(item);
			}
			this.UpdateTopItemsFontStyle();
			base.OnItemsForRowsCreated(e);
		}

		protected override ListViewItemStates GetItemStates(ListViewItem item)
		{
			ListViewItemStates listViewItemStates = base.GetItemStates(item);
			if ((item as DataTreeListViewItem).IsExpanded)
			{
				listViewItemStates |= ListViewItemStates.Marked;
			}
			return listViewItemStates;
		}

		protected override void SetItemStates(ListViewItem item, ListViewItemStates itemStates)
		{
			base.SetItemStates(item, itemStates);
			(item as DataTreeListViewItem).IsExpanded = ((itemStates & ListViewItemStates.Marked) != (ListViewItemStates)0);
		}

		protected override ListViewItem CreateNewListViewItem(object row)
		{
			DataTreeListViewItem dataTreeListViewItem = new DataTreeListViewItem(this, row);
			dataTreeListViewItem.ImageIndex = base.ImageIndex;
			dataTreeListViewItem.IndentCount = 1;
			dataTreeListViewItem.IsLeaf = !this.HasChildDataMember(row);
			if (!dataTreeListViewItem.IsLeaf)
			{
				dataTreeListViewItem.IsExpanded = this.AutoExpandNewItem;
			}
			return dataTreeListViewItem;
		}

		internal DataTreeListViewItem InternalCreateListViewItemForRow(object row)
		{
			return base.CreateListViewItemForRow(row) as DataTreeListViewItem;
		}

		internal void InternalUpdateItem(DataTreeListViewItem item)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new DataTreeListView.InternalUpdateItemInvoker(this.InternalUpdateItem), new object[]
				{
					item
				});
				return;
			}
			ItemCheckedEventArgs e = new ItemCheckedEventArgs(item);
			this.OnUpdateItem(e);
		}

		internal void RaiseItemsForRowsCreated(EventArgs e)
		{
			if (!base.IsCreatingItems)
			{
				base.OnItemsForRowsCreated(e);
			}
		}

		[DefaultValue(false)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AutoExpandNewItem
		{
			get
			{
				return this.autoExpandGroup;
			}
			set
			{
				this.autoExpandGroup = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTreeListViewColumnMappingCollection ChildrenDataMembers
		{
			get
			{
				return this.childrenDataMembers;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTreeListViewItemCollection TopItems
		{
			get
			{
				return this.topItems;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int TotalItemsCount
		{
			get
			{
				int num = 0;
				Stack stack = new Stack();
				stack.Push(this.TopItems);
				while (stack.Count > 0)
				{
					DataTreeListViewItemCollection dataTreeListViewItemCollection = (DataTreeListViewItemCollection)stack.Pop();
					num += dataTreeListViewItemCollection.Count;
					foreach (object obj in dataTreeListViewItemCollection)
					{
						DataTreeListViewItem dataTreeListViewItem = (DataTreeListViewItem)obj;
						stack.Push(dataTreeListViewItem.ChildrenItems);
					}
				}
				return num;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ColumnHeaderStyle HeaderStyle
		{
			get
			{
				return base.HeaderStyle;
			}
			set
			{
				if (value == ColumnHeaderStyle.Clickable)
				{
					throw new NotSupportedException();
				}
				base.HeaderStyle = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OwnerDraw
		{
			get
			{
				return base.OwnerDraw;
			}
			set
			{
				base.OwnerDraw = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IconLibrary IconLibrary
		{
			get
			{
				return base.IconLibrary;
			}
			set
			{
				base.IconLibrary = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ShowGroups
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new NotSupportedException();
				}
				base.ShowGroups = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				if (value == null)
				{
					base.BackgroundImage = this.emptyImage;
					return;
				}
				base.BackgroundImage = value;
			}
		}

		[Browsable(false)]
		public override bool SupportsVirtualMode
		{
			get
			{
				return false;
			}
		}

		private const int textPadding = 5;

		private const int iconPadding = 3;

		private bool autoExpandGroup;

		private DataTreeListViewColumnMappingCollection childrenDataMembers = new DataTreeListViewColumnMappingCollection();

		private DataTreeListViewItemCollection topItems;

		private IconLibrary emptyIconLibrary = new IconLibrary();

		private Bitmap emptyImage;

		private delegate void InternalUpdateItemInvoker(DataTreeListViewItem item);
	}
}
