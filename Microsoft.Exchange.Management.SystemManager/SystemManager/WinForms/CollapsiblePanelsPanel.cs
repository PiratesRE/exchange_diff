using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultProperty("CollapsiblePanels")]
	public class CollapsiblePanelsPanel : FlowPanel
	{
		public CollapsiblePanelsPanel()
		{
			this.collapsiblePanels = (TypedControlCollection<CollapsiblePanel>)this.Controls;
			base.SetStyle(Theme.UserPaintStyle, true);
			this.contextMenu = new ContextMenu();
			this.expandAll = new MenuItem();
			this.collapseAll = new MenuItem();
			base.SuspendLayout();
			this.contextMenu.MenuItems.AddRange(new MenuItem[]
			{
				this.expandAll,
				this.collapseAll
			});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Popup += this.contextMenu_Popup;
			this.expandAll.Name = "expandAll";
			this.expandAll.Text = Strings.ExpandAll;
			this.expandAll.Click += delegate(object param0, EventArgs param1)
			{
				this.ExpandAll();
			};
			this.collapseAll.Name = "collapseAll";
			this.collapseAll.Text = Strings.CollapseAll;
			this.collapseAll.Click += delegate(object param0, EventArgs param1)
			{
				this.CollapseAll();
			};
			this.ContextMenu = this.contextMenu;
			base.Name = "CollapsiblePanelsPanel";
			this.ForeColor = SystemColors.WindowText;
			this.BackColor = SystemColors.Window;
			base.ResumeLayout(false);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ContextMenu ContextMenu
		{
			get
			{
				return base.ContextMenu;
			}
			set
			{
				base.ContextMenu = value;
			}
		}

		private void contextMenu_Popup(object sender, EventArgs e)
		{
			bool enabled;
			bool enabled2;
			this.GetPanelsState(out enabled, out enabled2);
			this.expandAll.Enabled = enabled;
			this.collapseAll.Enabled = enabled2;
		}

		protected virtual void GetPanelsState(out bool enableExpandAll, out bool enableCollapseAll)
		{
			enableExpandAll = false;
			enableCollapseAll = false;
			for (int i = 0; i < this.CollapsiblePanels.Count; i++)
			{
				if (this.CollapsiblePanels[i].Visible)
				{
					if (this.CollapsiblePanels[i].IsMinimized)
					{
						enableExpandAll = true;
					}
					else
					{
						enableCollapseAll = true;
					}
					if (enableCollapseAll && enableExpandAll)
					{
						return;
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.contextMenu.Dispose();
			}
			base.Dispose(disposing);
		}

		public void ExpandAll()
		{
			this.SetIsMinimizeInAll(false);
		}

		public void CollapseAll()
		{
			this.SetIsMinimizeInAll(true);
		}

		protected virtual void SetIsMinimizeInAll(bool collapse)
		{
			Control activeControl = this.GetChildAtPointIfHandleCreated(new Point(base.Padding.Left, base.Padding.Top));
			CollapsiblePanel.Animate = false;
			base.SuspendLayout();
			using (new ControlWaitCursor(this))
			{
				try
				{
					foreach (object obj in this.CollapsiblePanels)
					{
						CollapsiblePanel collapsiblePanel = (CollapsiblePanel)obj;
						if (collapsiblePanel.ContainsFocus)
						{
							activeControl = collapsiblePanel;
						}
						collapsiblePanel.IsMinimized = collapse;
					}
				}
				finally
				{
					CollapsiblePanel.Animate = true;
					base.ResumeLayout();
				}
			}
			base.ScrollControlIntoView(activeControl);
		}

		private Control GetChildAtPointIfHandleCreated(Point pt)
		{
			if (!base.IsHandleCreated)
			{
				return null;
			}
			return base.GetChildAtPoint(pt);
		}

		[DefaultValue(typeof(Color), "WindowText")]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		[DefaultValue(typeof(Color), "Window")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new TypedControlCollection<CollapsiblePanel>(this);
		}

		[Category("Behavior")]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[MergableProperty(false)]
		public TypedControlCollection<CollapsiblePanel> CollapsiblePanels
		{
			get
			{
				return this.collapsiblePanels;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new Control.ControlCollection Controls
		{
			get
			{
				return base.Controls;
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			bool flag = levent.AffectedProperty == CollapsiblePanel.AlignLayout;
			if (!flag)
			{
				base.OnLayout(levent);
			}
			bool flag2 = levent.AffectedProperty == "Parent";
			if (flag || flag2)
			{
				this.AlignStatusLabel();
			}
		}

		internal void AlignStatusLabel()
		{
			int num = 0;
			for (int i = 0; i < this.CollapsiblePanels.Count; i++)
			{
				num = Math.Max(num, this.CollapsiblePanels[i].GetStatusWidth());
			}
			for (int j = 0; j < this.CollapsiblePanels.Count; j++)
			{
				this.CollapsiblePanels[j].SetStatusWidth(num);
			}
		}

		public override Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				if (base.MaximumSize != value)
				{
					base.MaximumSize = value;
					if (base.Height < this.MaximumSize.Height && base.VScroll)
					{
						base.Height = this.MaximumSize.Height;
					}
				}
			}
		}

		private ContextMenu contextMenu;

		private MenuItem expandAll;

		private MenuItem collapseAll;

		private TypedControlCollection<CollapsiblePanel> collapsiblePanels;
	}
}
