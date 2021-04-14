using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkUnitsPanel : CollapsiblePanelsPanel
	{
		internal bool NeedToUpdateLogicalTopofPanelItems
		{
			get
			{
				return this.needToUpdateLogicalTopofPanelItems;
			}
			set
			{
				this.needToUpdateLogicalTopofPanelItems = value;
			}
		}

		private int GetCountofPanelItemsUpdatedInOneTime()
		{
			int num = WorkUnitsPanel.countofPanelItemsUpdatedInOneTime;
			if (this.templatePanel != null)
			{
				Size size = WorkUnitPanel.MeasureCollapsedSizeForPanelItem(this.templatePanel, null);
				num = Math.Max(base.ClientSize.Height / size.Height + 1, num);
			}
			return num;
		}

		private bool AdjustPanelWidthToHideHorizontalScrollBar(int? totalHeight)
		{
			bool result = false;
			int num = base.Size.Width - base.Padding.Horizontal;
			if ((this.templatePanel.Width != num && totalHeight == null) || (totalHeight != null && totalHeight > base.ClientSize.Height))
			{
				num -= SystemInformation.VerticalScrollBarWidth;
			}
			if (this.templatePanel.Width != num)
			{
				this.templatePanel.Width = num;
				for (int i = 0; i < this.panelItems.Count; i++)
				{
					if (this.panelItems[i].Control != null)
					{
						this.panelItems[i].Control.Width = num;
					}
					this.panelItems[i].NeedToUpdateSize = true;
				}
				result = true;
			}
			return result;
		}

		private int UpdateLogicalTopForThisPanelItem(int index, int logicalTop)
		{
			if (this.panelItems[index].LogicalTop != logicalTop)
			{
				this.panelItems[index].LogicalTop = logicalTop;
				this.needToCreateItems = true;
			}
			this.panelItems[index].UpdatePanelItemSize();
			return logicalTop + this.panelItems[index].Size.Height + WorkUnitsPanel.defaultMargin.Vertical;
		}

		private int CalculateLogicalTopofPanelItems(int minCountofPanelItemsToUpdateSize, int minCountOfPanelItemsToUpdateLogicalTop)
		{
			this.NeedToUpdateLogicalTopofPanelItems = false;
			int num = 0;
			int num2 = base.Padding.Top;
			this.needToCreateItems = (this.needToCreateItems || this.panelItems[this.panelItems.Count - 1].NeedToUpdateSize);
			while (num < this.panelItems.Count && (minCountofPanelItemsToUpdateSize > 0 || minCountOfPanelItemsToUpdateLogicalTop > 0))
			{
				if (this.panelItems[num].NeedToUpdateSize)
				{
					minCountofPanelItemsToUpdateSize--;
				}
				num2 = this.UpdateLogicalTopForThisPanelItem(num, num2);
				minCountOfPanelItemsToUpdateLogicalTop--;
				num++;
			}
			if (num != this.panelItems.Count)
			{
				this.UpdateLogicalTopForThisPanelItem(num, num2);
				num = this.panelItems.Count - 1;
				this.UpdateLogicalTopForThisPanelItem(num, num2);
				this.NeedToUpdateLogicalTopofPanelItems = true;
			}
			return num2 - WorkUnitsPanel.defaultMargin.Vertical;
		}

		private int UpdateLogicalTopofPanelItems(int minCountofPanelItemsToUpdateSize, int minCountOfPanelItemsToUpdateLogicalTop)
		{
			int num = 0;
			if (!this.suspendUpdateLogicalTop && this.panelItems.Count > 0)
			{
				this.suspendUpdateLogicalTop = true;
				try
				{
					if (!this.NeedToUpdateLogicalTopofPanelItems)
					{
						num = this.panelItems[this.panelItems.Count - 1].LogicalTop + this.panelItems[this.panelItems.Count - 1].Size.Height;
					}
					else
					{
						base.SuspendLayout();
						try
						{
							this.AdjustPanelWidthToHideHorizontalScrollBar(null);
							num = this.CalculateLogicalTopofPanelItems(minCountofPanelItemsToUpdateSize, minCountOfPanelItemsToUpdateLogicalTop);
							if (this.AdjustPanelWidthToHideHorizontalScrollBar(new int?(num)))
							{
								num = this.CalculateLogicalTopofPanelItems(minCountofPanelItemsToUpdateSize, minCountOfPanelItemsToUpdateLogicalTop);
							}
						}
						finally
						{
							base.ResumeLayout(false);
						}
					}
				}
				finally
				{
					this.suspendUpdateLogicalTop = false;
				}
			}
			return num;
		}

		private int GetMaximumHeightofPanelItemsInBestTimes()
		{
			return this.UpdateLogicalTopofPanelItems(1, this.GetCountofPanelItemsUpdatedInOneTime());
		}

		private int UpdateLogicalTopForPartPanelItems()
		{
			int num = this.GetCountofPanelItemsUpdatedInOneTime();
			return this.UpdateLogicalTopofPanelItems(num, num);
		}

		public WorkUnitsPanel()
		{
			base.Name = "WorkUnitsPanel";
			Application.Idle += this.Application_Idle;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.WorkUnits = null;
				Application.Idle -= this.Application_Idle;
			}
			base.Dispose(disposing);
		}

		[DefaultValue(null)]
		internal IList<WorkUnit> WorkUnits
		{
			get
			{
				return this.workUnits;
			}
			set
			{
				if (value != this.WorkUnits)
				{
					if (this.WorkUnits is IBindingList)
					{
						((IBindingList)this.WorkUnits).ListChanged -= this.WorkUnits_ListChanged;
					}
					this.workUnits = value;
					if (this.WorkUnits is IBindingList)
					{
						((IBindingList)this.WorkUnits).ListChanged += this.WorkUnits_ListChanged;
					}
					if (base.IsHandleCreated)
					{
						this.WorkUnits_ListChanged(this.WorkUnits, new ListChangedEventArgs(ListChangedType.Reset, -1));
					}
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if (this.WorkUnits != null)
			{
				this.WorkUnits_ListChanged(this.WorkUnits, new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
			base.OnLoad(e);
		}

		private void WorkUnits_ListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
			case ListChangedType.Reset:
			case ListChangedType.ItemAdded:
			case ListChangedType.ItemDeleted:
			case ListChangedType.ItemMoved:
				if (base.InvokeRequired)
				{
					base.Invoke(new ListChangedEventHandler(this.WorkUnits_ListChanged), new object[]
					{
						sender,
						e
					});
					return;
				}
				this.CreateCollapsiblePanels();
				return;
			default:
				return;
			}
		}

		private void CreateCollapsiblePanels()
		{
			using (new ControlWaitCursor(this))
			{
				base.SuspendLayout();
				this.suspendUpdateItem = true;
				try
				{
					for (int i = base.CollapsiblePanels.Count - 1; i >= 0; i--)
					{
						base.CollapsiblePanels[i].Dispose();
					}
					base.CollapsiblePanels.Clear();
					this.templatePanel = null;
					for (int j = this.panelItems.Count - 1; j >= 0; j--)
					{
						this.panelItems[j].Dispose();
					}
					this.panelItems.Clear();
					if (this.WorkUnits != null)
					{
						base.TabStop = (this.WorkUnits.Count > 0);
						this.panelItems.Capacity = this.WorkUnits.Count;
						for (int k = 0; k < this.WorkUnits.Count; k++)
						{
							this.panelItems.Add(new WorkUnitPanelItem(this, this.WorkUnits[k]));
						}
						this.EnableVirtualMode();
						this.needToCreateItems = true;
					}
				}
				finally
				{
					this.suspendUpdateItem = false;
					base.ResumeLayout(false);
					base.PerformLayout(this, WorkUnitsPanel.CreatePanelItemLayout);
				}
			}
		}

		private void EnableVirtualMode()
		{
			this.firstReservedPanelItemIndex = 0;
			this.lastReservedPanelItemIndex = 0;
			this.createdItemIndices.Clear();
			base.AutoScrollPosition = new Point(base.Padding.Left, base.Padding.Top);
			this.lastVerticalScrollValue = base.Padding.Top;
			this.lastWorkUnitsPanelSize = base.Size;
			this.templatePanel = new WorkUnitPanel();
			this.templatePanel.TabStop = false;
			base.CollapsiblePanels.Add(this.templatePanel);
			this.templatePanel.SetBounds(base.Padding.Left, -32768, base.Size.Width - base.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth, this.templatePanel.Height);
			EventHandler value = new EventHandler(this.PanelItem_SizeChanged);
			for (int i = 0; i < this.panelItems.Count; i++)
			{
				this.panelItems[i].SizeChanged += value;
			}
			this.NeedToUpdateLogicalTopofPanelItems = true;
		}

		private void PanelItem_SizeChanged(object sender, EventArgs e)
		{
			WorkUnitPanelItem workUnitPanelItem = (WorkUnitPanelItem)sender;
			if (workUnitPanelItem.Control != null)
			{
				this.GetMaximumHeightofPanelItemsInBestTimes();
				this.UpdateTopofAffectedPanels(this.panelItems.IndexOf(workUnitPanelItem));
			}
		}

		private void UpdateTopofAffectedPanels(int startIndex)
		{
			while (startIndex < this.panelItems.Count - 1)
			{
				if (this.panelItems[startIndex].Control != null && this.panelItems[startIndex + 1].Control != null)
				{
					this.panelItems[startIndex + 1].Control.Top = this.panelItems[startIndex].Control.Top + this.panelItems[startIndex].Control.Height + WorkUnitsPanel.defaultMargin.Vertical;
				}
				startIndex++;
			}
		}

		private void UpdatePanelsLocation(int totalItemsHeight, int verticalScrollValue)
		{
			this.templatePanel.Top = -32768;
			int i = 0;
			bool flag = true;
			while (i < this.panelItems.Count)
			{
				if (this.panelItems[i].Control != null)
				{
					if (flag || i == this.panelItems.Count - 1)
					{
						this.panelItems[i].Control.Top = this.panelItems[i].LogicalTop - verticalScrollValue;
					}
					else
					{
						this.panelItems[i].Control.Top = -32768;
					}
					this.panelItems[i].Control.TabStop = flag;
				}
				flag = (flag && this.panelItems[i].LogicalTop < totalItemsHeight);
				i++;
			}
		}

		internal void MeasureItem(WorkUnitPanelItem panelItem, out Size collapsedSize, out Size expandSize)
		{
			base.SuspendLayout();
			this.templatePanel.Top = -32768;
			collapsedSize = WorkUnitPanel.MeasureCollapsedSizeForPanelItem(this.templatePanel, panelItem);
			expandSize = WorkUnitPanel.MeasureExpandedSizeForPanelItem(this.templatePanel, panelItem);
			base.ResumeLayout(false);
		}

		private void EnsurePanelsCreated()
		{
			if (this.suspendUpdateItem || this.panelItems.Count <= 0)
			{
				return;
			}
			this.suspendUpdateItem = true;
			try
			{
				int maximumHeightofPanelItemsInBestTimes = this.GetMaximumHeightofPanelItemsInBestTimes();
				this.needToCreateItems = false;
				int num = maximumHeightofPanelItemsInBestTimes + base.Padding.Bottom - base.ClientSize.Height;
				if (base.VerticalScroll.Value > num)
				{
					base.AutoScrollPosition = new Point(-base.AutoScrollPosition.X, num);
				}
				this.lastVerticalScrollValue = base.VerticalScroll.Value;
				int num2 = this.lastVerticalScrollValue - base.ClientSize.Height;
				int num3 = this.lastVerticalScrollValue + 2 * base.ClientSize.Height;
				if (num3 >= maximumHeightofPanelItemsInBestTimes)
				{
					num3 = maximumHeightofPanelItemsInBestTimes - 1;
				}
				if (num2 < base.Padding.Top)
				{
					num2 = base.Padding.Top;
				}
				this.firstReservedPanelItemIndex = this.GetPanelItemAtPoint(num2);
				this.lastReservedPanelItemIndex = this.GetPanelItemAtPoint(num3);
				for (int i = this.firstReservedPanelItemIndex; i <= this.lastReservedPanelItemIndex; i++)
				{
					this.CreatePanelForItem(i);
				}
				this.CreatePanelForItem(0);
				this.CreatePanelForItem(this.panelItems.Count - 1);
				this.UpdatePanelsLocation(maximumHeightofPanelItemsInBestTimes, this.lastVerticalScrollValue);
				base.AlignStatusLabel();
			}
			finally
			{
				this.suspendUpdateItem = false;
			}
		}

		private WorkUnitPanel CreatePanelForItem(int itemIndex)
		{
			if (this.panelItems[itemIndex].Control == null)
			{
				this.panelItems[itemIndex].BindToControl(this.CreateOrGetCachedWorkUnitPanel());
				this.panelItems[itemIndex].Control.TabIndex = itemIndex + 1;
				this.createdItemIndices.Add(itemIndex);
			}
			return this.panelItems[itemIndex].Control;
		}

		private WorkUnitPanel CreateOrGetCachedWorkUnitPanel()
		{
			WorkUnitPanel workUnitPanel = null;
			int num = -1;
			foreach (int num2 in this.createdItemIndices)
			{
				if ((num2 < this.firstReservedPanelItemIndex || num2 > this.lastReservedPanelItemIndex) && num2 != 0 && num2 != this.panelItems.Count - 1 && num2 != this.focusedPanelItemIndex && num2 != this.focusedPanelItemIndex - 1 && num2 != this.focusedPanelItemIndex + 1 && this.panelItems[num2].Control != null)
				{
					workUnitPanel = this.panelItems[num2].UnbindControl();
					num = num2;
					break;
				}
			}
			if (num != -1)
			{
				this.createdItemIndices.Remove(num);
			}
			if (workUnitPanel == null)
			{
				workUnitPanel = new WorkUnitPanel();
				workUnitPanel.Width = this.templatePanel.Width;
				base.CollapsiblePanels.Add(workUnitPanel);
			}
			return workUnitPanel;
		}

		internal void SetFocusedPanel(object panelItem)
		{
			this.focusedPanelItemIndex = this.panelItems.IndexOf((WorkUnitPanelItem)panelItem);
			if (this.focusedPanelItemIndex > 0)
			{
				this.CreatePanelForItem(this.focusedPanelItemIndex - 1);
			}
			if (this.focusedPanelItemIndex < this.panelItems.Count - 1)
			{
				this.CreatePanelForItem(this.focusedPanelItemIndex + 1);
			}
			base.ScrollControlIntoView(this.panelItems[this.focusedPanelItemIndex].Control);
			base.PerformLayout(null, WorkUnitsPanel.CreatePanelItemLayout);
		}

		private int GetPanelItemAtPoint(int logicalTopOfPanelItem)
		{
			int num = 0;
			while (num < this.panelItems.Count - 1 && this.panelItems[num + 1].LogicalTop <= logicalTopOfPanelItem)
			{
				num++;
			}
			return num;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			if (this.panelItems.Count > 0 && base.Visible)
			{
				this.UpdateLogicalTopForPartPanelItems();
				if (this.needToCreateItems || this.lastVerticalScrollValue != base.VerticalScroll.Value)
				{
					base.PerformLayout(null, WorkUnitsPanel.CreatePanelItemLayout);
				}
			}
		}

		protected override void AdjustLocationOfChildControls(LayoutEventArgs levent)
		{
			if (!this.lastWorkUnitsPanelSize.Equals(base.Size) || levent.AffectedProperty.Equals(WorkUnitsPanel.CreatePanelItemLayout))
			{
				if (!this.lastWorkUnitsPanelSize.Equals(base.Size))
				{
					this.NeedToUpdateLogicalTopofPanelItems = true;
					this.lastWorkUnitsPanelSize = base.Size;
				}
				this.EnsurePanelsCreated();
			}
			base.SetScrollState(8, true);
		}

		private void CreatePanelsIfScrollMoreThanOpenPage()
		{
			if (Math.Abs(this.lastVerticalScrollValue - base.VerticalScroll.Value) >= base.ClientSize.Height)
			{
				base.PerformLayout(null, WorkUnitsPanel.CreatePanelItemLayout);
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			this.CreatePanelsIfScrollMoreThanOpenPage();
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			this.CreatePanelsIfScrollMoreThanOpenPage();
		}

		protected override void GetPanelsState(out bool enableExpandAll, out bool enableCollapseAll)
		{
			enableExpandAll = false;
			enableCollapseAll = false;
			for (int i = 0; i < this.panelItems.Count; i++)
			{
				if (this.panelItems[i].IsMinimized)
				{
					enableExpandAll = true;
				}
				else
				{
					enableCollapseAll = true;
				}
				if (enableExpandAll && enableCollapseAll)
				{
					return;
				}
			}
		}

		protected override void SetIsMinimizeInAll(bool collapse)
		{
			using (new ControlWaitCursor(this))
			{
				base.SuspendLayout();
				this.suspendUpdateItem = true;
				try
				{
					for (int i = this.panelItems.Count - 1; i >= 0; i--)
					{
						this.panelItems[i].IsMinimized = collapse;
					}
					base.AutoScrollPosition = new Point(base.Padding.Left, base.Padding.Top);
				}
				finally
				{
					this.suspendUpdateItem = false;
					base.ResumeLayout(false);
					base.PerformLayout(null, WorkUnitsPanel.CreatePanelItemLayout);
				}
			}
		}

		[DefaultValue(0)]
		public TaskState TaskState
		{
			get
			{
				return this.taskState;
			}
			set
			{
				if (this.TaskState != value)
				{
					base.SuspendLayout();
					try
					{
						using (new ControlWaitCursor(this))
						{
							this.taskState = value;
							this.OnTaskStateChanged(EventArgs.Empty);
							for (int i = 0; i < this.panelItems.Count; i++)
							{
								if (this.panelItems[i].Control != null)
								{
									this.panelItems[i].Control.Refresh();
								}
								this.panelItems[i].NeedToUpdateSize = true;
							}
							this.NeedToUpdateLogicalTopofPanelItems = true;
						}
					}
					finally
					{
						base.ResumeLayout(false);
						base.AlignStatusLabel();
					}
				}
			}
		}

		protected virtual void OnTaskStateChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[WorkUnitsPanel.EventTaskStateChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler TaskStateChanged
		{
			add
			{
				base.Events.AddHandler(WorkUnitsPanel.EventTaskStateChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(WorkUnitsPanel.EventTaskStateChanged, value);
			}
		}

		public string GetSummaryText()
		{
			base.SuspendLayout();
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				stringBuilder.AppendLine(((WorkUnitCollection)this.WorkUnits).Description);
				stringBuilder.AppendLine(((WorkUnitCollection)this.WorkUnits).ElapsedTimeText);
				stringBuilder.AppendLine();
				for (int i = 0; i < this.panelItems.Count; i++)
				{
					WorkUnitPanel control = this.panelItems[i].Control;
					if (control == null)
					{
						control = this.templatePanel;
						control.WorkUnitPanelItem = this.panelItems[i];
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(control.GetSummaryText());
					stringBuilder.AppendLine();
				}
			}
			finally
			{
				base.ResumeLayout(false);
			}
			return stringBuilder.ToString();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.SuspendLayout();
			try
			{
				base.OnVisibleChanged(e);
			}
			finally
			{
				base.ResumeLayout();
			}
		}

		internal static readonly string CreatePanelItemLayout = "CreatePanelItem";

		private static readonly Padding defaultMargin = new Padding(0, 0, 0, 1);

		private IList<WorkUnit> workUnits;

		private List<WorkUnitPanelItem> panelItems = new List<WorkUnitPanelItem>();

		private List<int> createdItemIndices = new List<int>();

		private int firstReservedPanelItemIndex;

		private int lastReservedPanelItemIndex;

		private int focusedPanelItemIndex;

		private bool suspendUpdateItem;

		private bool needToCreateItems;

		private volatile bool needToUpdateLogicalTopofPanelItems;

		private bool suspendUpdateLogicalTop;

		private Size lastWorkUnitsPanelSize;

		private int lastVerticalScrollValue;

		private WorkUnitPanel templatePanel;

		private static int countofPanelItemsUpdatedInOneTime = 100;

		private TaskState taskState;

		private static readonly object EventTaskStateChanged = new object();
	}
}
