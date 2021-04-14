using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Designer(typeof(AutoSizePanelDesigner))]
	[DefaultEvent("Layout")]
	[DefaultProperty("AutoSize")]
	public class AutoSizePanel : ExchangeUserControl
	{
		public AutoSizePanel()
		{
			base.SetStyle(ControlStyles.ContainerControl, true);
			base.SuspendLayout();
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.ResumeLayout(false);
			base.Name = "AutoSizePanel";
		}

		[DefaultValue(true)]
		public override bool AutoSize
		{
			get
			{
				return this.autoSize;
			}
			set
			{
				if (value != this.autoSize)
				{
					this.autoSize = value;
					this.OnAutoSizeChanged(EventArgs.Empty);
					base.PerformLayout(this, "AutoSize");
				}
			}
		}

		[DefaultValue(AutoSizeMode.GrowAndShrink)]
		public new AutoSizeMode AutoSizeMode
		{
			get
			{
				return base.AutoSizeMode;
			}
			set
			{
				base.AutoSizeMode = value;
			}
		}

		protected override void OnDockChanged(EventArgs e)
		{
			base.OnDockChanged(e);
			if (this.Dock == DockStyle.None)
			{
				base.PerformLayout(this, "Dock");
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			if (this.AutoSize)
			{
				AutoSizePanel.ApplyPreferredSize(this);
			}
			base.OnLayout(levent);
			if (this.AutoSize)
			{
				AutoSizePanel.ApplyPreferredSize(this);
			}
			if (this.AutoScroll)
			{
				base.AdjustFormScrollbars(this.AutoScroll);
			}
		}

		public static bool CanAutoSizeWidth(Control control)
		{
			if (control.Parent == null)
			{
				return true;
			}
			switch (control.Dock)
			{
			case DockStyle.Top:
			case DockStyle.Bottom:
			case DockStyle.Fill:
				return false;
			}
			return AnchorStyles.Right != (control.Anchor & AnchorStyles.Right);
		}

		public static bool CanAutoSizeHeight(Control control)
		{
			if (control.Parent == null)
			{
				return true;
			}
			switch (control.Dock)
			{
			case DockStyle.Left:
			case DockStyle.Right:
			case DockStyle.Fill:
				return false;
			default:
				return AnchorStyles.Bottom != (control.Anchor & AnchorStyles.Bottom);
			}
		}

		public static Size GetFitToContentsSize(Control control)
		{
			if (control.HasChildren)
			{
				int num = int.MinValue;
				int num2 = int.MinValue;
				int num3 = 0;
				int num4 = 0;
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
						{
							flag2 = false;
							flag3 = false;
							Size size = control2.Size;
							num = Math.Max(num, control2.Left + size.Width);
							num2 = Math.Max(num2, control2.Top + size.Height);
							break;
						}
						case DockStyle.Top:
						case DockStyle.Bottom:
							flag3 = false;
							num4 += control2.Height;
							break;
						case DockStyle.Left:
						case DockStyle.Right:
							flag2 = false;
							num3 += control2.Width;
							break;
						case DockStyle.Fill:
							num3 += control2.Width;
							num4 += control2.Height;
							break;
						}
					}
				}
				ScrollableControl scrollableControl = control as ScrollableControl;
				if (scrollableControl != null)
				{
					ScrollableControl.DockPaddingEdges dockPadding = scrollableControl.DockPadding;
					num += dockPadding.Right;
					num2 += dockPadding.Bottom;
					num3 += dockPadding.Left + dockPadding.Right;
					num4 += dockPadding.Top + dockPadding.Bottom;
				}
				num = Math.Max(num, num3);
				num2 = Math.Max(num2, num4);
				if (!flag || !AutoSizePanel.CanAutoSizeWidth(control) || flag2)
				{
					num = control.Size.Width;
				}
				if (!flag || !AutoSizePanel.CanAutoSizeHeight(control) || flag3)
				{
					num2 = control.Size.Height;
				}
				return AutoSizePanel.GetAutoSizeModeSize(control, new Size(num, num2));
			}
			return control.Size;
		}

		public static void ApplyPreferredSize(Control control)
		{
			if (AutoSizePanel.CanAutoSizeWidth(control) || AutoSizePanel.CanAutoSizeHeight(control))
			{
				Size fitToContentsSize = AutoSizePanel.GetFitToContentsSize(control);
				if (control.Size != fitToContentsSize)
				{
					control.Size = AutoSizePanel.GetAutoSizeModeSize(control, fitToContentsSize);
				}
			}
		}

		private static Size GetAutoSizeModeSize(Control control, Size suggestedSize)
		{
			int num = suggestedSize.Width;
			int num2 = suggestedSize.Height;
			UserControl userControl = control as UserControl;
			if (userControl != null && userControl.AutoSizeMode == AutoSizeMode.GrowOnly)
			{
				num = Math.Max(num, control.Size.Width);
				num2 = Math.Max(num2, control.Size.Height);
			}
			return new Size(num, num2);
		}

		private bool autoSize = true;
	}
}
