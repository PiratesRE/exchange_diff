using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class CheckBoxBulkEditorAdapter : ButtonBaseBulkEditorAdapter
	{
		public CheckBoxBulkEditorAdapter(AutoHeightCheckBox checkBox) : base(checkBox)
		{
		}

		protected override void OnOwnerDraw(Graphics g)
		{
			CheckBox checkBox = base.HostControl as CheckBox;
			System.Drawing.ContentAlignment checkAlign = checkBox.CheckAlign;
			CheckBoxState state = checkBox.Focused ? CheckBoxState.MixedHot : CheckBoxState.MixedNormal;
			Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, state);
			Rectangle rectangle = base.CalculateCheckBounds(checkAlign, glyphSize);
			if (LayoutHelper.IsRightToLeft(base.HostControl))
			{
				rectangle.Offset(-1, 0);
			}
			if (base["Checked"] == 3)
			{
				int num = LayoutHelper.IsRightToLeft(base.HostControl) ? 12 : 8;
				Rectangle targetRect = new Rectangle(rectangle.Left + rectangle.Width - num, rectangle.Top, 8, 8);
				Color color = checkBox.Enabled ? checkBox.BackColor : SystemColors.Control;
				using (new SolidBrush(color))
				{
					g.DrawIcon(Icons.LockIcon, targetRect);
				}
				checkBox.Enabled = false;
				return;
			}
			if (Application.RenderWithVisualStyles)
			{
				CheckBoxRenderer.DrawCheckBox(g, rectangle.Location, state);
				return;
			}
			ControlPaint.DrawMixedCheckBox(g, rectangle, ButtonState.Checked);
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			if (string.Equals(e.PropertyName, "Checked"))
			{
				AutoHeightCheckBox autoHeightCheckBox = base.HostControl as AutoHeightCheckBox;
				if (base["Checked"] != null && base["Checked"] != 3)
				{
					bool @checked = autoHeightCheckBox.Checked;
					this.forceAllowCheckedChangedEvent = true;
					autoHeightCheckBox.Checked = autoHeightCheckBox.BulkEditDefaultChecked;
					this.forceAllowCheckedChangedEvent = false;
					autoHeightCheckBox.Checked = @checked;
					Binding binding = autoHeightCheckBox.DataBindings["Checked"];
					if (binding != null)
					{
						binding.WriteValue();
					}
				}
				else if (base["Checked"] != 3)
				{
					autoHeightCheckBox.Checked = false;
				}
			}
			base.HostControl.Invalidate();
		}
	}
}
