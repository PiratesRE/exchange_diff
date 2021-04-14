using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class DateTimePickerBulkEditorAdapter : BulkEditorAdapter
	{
		public DateTimePickerBulkEditorAdapter(ExtendedDateTimePicker dateTimePicker) : base(dateTimePicker)
		{
			dateTimePicker.Painted += this.OnAppearancePainted;
			dateTimePicker.FocusSetted += this.OnAppearancePainted;
			dateTimePicker.FocusKilled += this.OnAppearancePainted;
		}

		private void OnAppearancePainted(object sender, EventArgs e)
		{
			if (base["Value"] == 3)
			{
				this.DrawBulkEditorLockedState(Icons.LockIcon);
				return;
			}
			if (base["Value"] != null)
			{
				this.DrawBulkEditorInitialState(base.BulkEditingIndicatorText);
			}
		}

		private void DrawBulkEditorLockedState(Icon icon)
		{
			using (Graphics graphics = base.HostControl.CreateGraphics())
			{
				Rectangle targetRect = new Rectangle(base.HostControl.ClientRectangle.Right - 8, base.HostControl.ClientRectangle.Top, 8, 8);
				Color color = base.HostControl.Enabled ? base.HostControl.BackColor : SystemColors.Control;
				using (new SolidBrush(color))
				{
					graphics.DrawIcon(icon, targetRect);
				}
			}
			base.HostControl.Enabled = false;
		}

		private void DrawBulkEditorInitialState(string cueBanner)
		{
			if (!string.IsNullOrEmpty(cueBanner))
			{
				ExtendedDateTimePicker extendedDateTimePicker = base.HostControl as ExtendedDateTimePicker;
				using (Graphics graphics = extendedDateTimePicker.CreateGraphics())
				{
					TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
					Rectangle clientRectangle = extendedDateTimePicker.ClientRectangle;
					clientRectangle.Width -= SystemInformation.VerticalScrollBarWidth;
					if (Application.RenderWithVisualStyles)
					{
						clientRectangle.Offset(2, 2);
						clientRectangle.Width--;
						clientRectangle.Height -= 4;
					}
					else
					{
						clientRectangle.Inflate(-2, -2);
					}
					Color color = extendedDateTimePicker.Enabled ? extendedDateTimePicker.BackColor : SystemColors.Control;
					using (SolidBrush solidBrush = new SolidBrush(color))
					{
						graphics.FillRectangle(solidBrush, clientRectangle);
					}
					TextRenderer.DrawText(graphics, cueBanner, extendedDateTimePicker.Font, clientRectangle, extendedDateTimePicker.ForeColor, color, flags);
				}
			}
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			base.HostControl.Invalidate();
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			list.Add("Value");
			return list;
		}

		private const string ManagedPropertyName = "Value";
	}
}
