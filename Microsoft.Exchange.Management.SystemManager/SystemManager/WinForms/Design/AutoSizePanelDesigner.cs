using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms.Design
{
	public class AutoSizePanelDesigner : ScrollableControlDesigner
	{
		public AutoSizePanel AutoSizePanel
		{
			get
			{
				return this.Control as AutoSizePanel;
			}
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.Control.SizeChanged += this.Control_SizeChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.Control != null)
			{
				this.Control.SizeChanged -= this.Control_SizeChanged;
			}
			base.Dispose(disposing);
		}

		private void Control_SizeChanged(object sender, EventArgs e)
		{
			if (this.Control.Parent != null)
			{
				this.Control.Parent.Invalidate(true);
			}
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			Rectangle clientRectangle = this.Control.ClientRectangle;
			clientRectangle.Width--;
			clientRectangle.Height--;
			Color backColor = this.Control.BackColor;
			Color color = ((double)backColor.GetBrightness() < 0.5) ? ControlPaint.Light(backColor) : ControlPaint.Dark(backColor);
			using (Pen pen = new Pen(color))
			{
				pen.DashStyle = DashStyle.Dash;
				pe.Graphics.DrawRectangle(pen, clientRectangle);
			}
			base.OnPaintAdornments(pe);
		}

		public override SelectionRules SelectionRules
		{
			get
			{
				return AutoSizePanelDesigner.FilterContainerSelectionRules(base.SelectionRules, this.Control, this.AutoSizePanel.AutoSize);
			}
		}

		public static SelectionRules FilterContainerSelectionRules(SelectionRules baseRules, Control control, bool autoSize)
		{
			if (autoSize)
			{
				bool flag = false;
				bool flag2 = true;
				bool flag3 = true;
				for (int i = 0; i < control.Controls.Count; i++)
				{
					Control control2 = control.Controls[i];
					if (control2.Visible)
					{
						flag = true;
						switch (control2.Dock)
						{
						case DockStyle.None:
							flag2 = false;
							flag3 = false;
							break;
						case DockStyle.Top:
						case DockStyle.Bottom:
							flag3 = false;
							break;
						case DockStyle.Left:
						case DockStyle.Right:
							flag2 = false;
							break;
						}
					}
				}
				if (flag && !flag2 && AutoSizePanel.CanAutoSizeWidth(control))
				{
					baseRules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
				}
				if (flag && !flag3 && AutoSizePanel.CanAutoSizeHeight(control))
				{
					baseRules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
				}
			}
			return baseRules;
		}

		public static SelectionRules FilterSelectionRules(SelectionRules baseRules, Control control, bool autoSize)
		{
			if (autoSize)
			{
				if (AutoSizePanel.CanAutoSizeWidth(control))
				{
					baseRules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
				}
				if (AutoSizePanel.CanAutoSizeHeight(control))
				{
					baseRules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
				}
			}
			return baseRules;
		}
	}
}
