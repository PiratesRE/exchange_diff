using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class WorkUnitPanelItem : IDisposable
	{
		internal bool NeedToUpdateSize
		{
			get
			{
				return this.needToUpdateSize;
			}
			set
			{
				this.needToUpdateSize = value;
			}
		}

		public WorkUnitPanelItem(WorkUnitsPanel ownerControl, WorkUnit workUnit)
		{
			this.ownerControl = ownerControl;
			this.workUnit = workUnit;
			this.WorkUnit.PropertyChanged += this.WorkUnit_PropertyChanged;
			this.NeedToUpdateSize = true;
		}

		public void BindToControl(WorkUnitPanel control)
		{
			this.control = control;
			this.Control.SuspendLayout();
			this.Control.WorkUnitPanelItem = this;
			this.Control.FastSetIsMinimized(this.IsMinimized);
			this.Control.SizeChanged += this.Control_SizeChanged;
			this.Control.Enter += this.Control_Enter;
			this.Size = this.Control.Size;
			this.Control.IsMinimizedChanged += this.Control_IsMinimizedChanged;
			this.Control.ResumeLayout(true);
		}

		public WorkUnitPanel UnbindControl()
		{
			this.Control.SuspendLayout();
			this.Control.SizeChanged -= this.Control_SizeChanged;
			this.Control.Enter -= this.Control_Enter;
			this.Control.IsMinimizedChanged -= this.Control_IsMinimizedChanged;
			this.Control.WorkUnitPanelItem = null;
			this.Control.ResumeLayout(false);
			WorkUnitPanel result = this.Control;
			this.control = null;
			return result;
		}

		private void Control_Enter(object sender, EventArgs e)
		{
			this.ownerControl.SetFocusedPanel(this);
		}

		private void Control_IsMinimizedChanged(object sender, EventArgs e)
		{
			this.IsMinimized = this.Control.IsMinimized;
		}

		private void Control_SizeChanged(object sender, EventArgs e)
		{
			this.Size = this.Control.Size;
			this.ownerControl.NeedToUpdateLogicalTopofPanelItems = true;
		}

		private void WorkUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.NeedToUpdateSize = true;
			if (this.Control == null)
			{
				this.ownerControl.NeedToUpdateLogicalTopofPanelItems = true;
				return;
			}
			if (this.ownerControl.InvokeRequired)
			{
				this.ownerControl.Invoke(new PropertyChangedEventHandler(this.WorkUnit_PropertyChanged), new object[]
				{
					sender,
					e
				});
				return;
			}
			PropertyChangedEventHandler workUnitPropertyChanged = this.WorkUnitPropertyChanged;
			if (workUnitPropertyChanged != null)
			{
				workUnitPropertyChanged(sender, e);
			}
		}

		public void UpdatePanelItemSize()
		{
			if (this.NeedToUpdateSize)
			{
				this.NeedToUpdateSize = false;
				this.ownerControl.MeasureItem(this, out this.collapsedSize, out this.expandedSize);
				if (this.Control == null)
				{
					this.Size = (this.IsMinimized ? this.collapsedSize : this.expandedSize);
				}
			}
		}

		public WorkUnitPanel Control
		{
			get
			{
				return this.control;
			}
		}

		public WorkUnit WorkUnit
		{
			get
			{
				return this.workUnit;
			}
		}

		public bool IsMinimized
		{
			get
			{
				return this.isMinimized;
			}
			set
			{
				if (this.IsMinimized != value)
				{
					this.isMinimized = value;
					if (this.Control != null)
					{
						this.Control.FastSetIsMinimized(this.IsMinimized);
						return;
					}
					if (!this.NeedToUpdateSize)
					{
						this.Size = (this.IsMinimized ? this.collapsedSize : this.expandedSize);
					}
					this.ownerControl.NeedToUpdateLogicalTopofPanelItems = true;
				}
			}
		}

		public int LogicalTop
		{
			get
			{
				return this.logicalTop;
			}
			set
			{
				this.logicalTop = value;
			}
		}

		public Size Size
		{
			get
			{
				return this.size;
			}
			set
			{
				if (this.Size != value)
				{
					this.size = value;
					if (this.SizeChanged != null)
					{
						this.SizeChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public event PropertyChangedEventHandler WorkUnitPropertyChanged;

		public event EventHandler SizeChanged;

		public void Dispose()
		{
			if (this.WorkUnit != null)
			{
				this.WorkUnit.PropertyChanged -= this.WorkUnit_PropertyChanged;
				if (this.Control != null)
				{
					Control control = this.UnbindControl();
					control.Dispose();
				}
			}
		}

		private WorkUnitsPanel ownerControl;

		private Size collapsedSize;

		private Size expandedSize;

		internal volatile bool needToUpdateSize;

		private WorkUnitPanel control;

		private WorkUnit workUnit;

		private bool isMinimized;

		private int logicalTop;

		private Size size;
	}
}
