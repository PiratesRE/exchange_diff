using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class ComboBoxBulkEditorAdapter : BulkEditorAdapter
	{
		public ComboBoxBulkEditorAdapter(ExchangeComboBox comboBox) : base(comboBox)
		{
			comboBox.Painted += this.OnAppearancePainted;
			comboBox.FocusSetted += this.OnAppearancePainted;
			comboBox.FocusKilled += this.OnAppearancePainted;
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			base.HostControl.Invalidate();
		}

		private void OnAppearancePainted(object sender, EventArgs e)
		{
			if (base["SelectedValue"] == 3)
			{
				this.DrawBulkEditorLockedState(Icons.LockIcon);
				return;
			}
			if (base["SelectedValue"] != null)
			{
				this.DrawBulkEditorInitialState(base.BulkEditingIndicatorText);
			}
		}

		private void DrawBulkEditorLockedState(Icon icon)
		{
			ExchangeComboBox exchangeComboBox = base.HostControl as ExchangeComboBox;
			using (Graphics graphics = exchangeComboBox.CreateGraphics())
			{
				Rectangle rectangle = new Rectangle(exchangeComboBox.ClientRectangle.Right - 8, exchangeComboBox.ClientRectangle.Top, 8, 8);
				Color color = exchangeComboBox.Enabled ? exchangeComboBox.BackColor : SystemColors.Control;
				using (new SolidBrush(color))
				{
					graphics.DrawIcon(icon, LayoutHelper.MirrorRectangle(rectangle, base.HostControl));
				}
			}
			exchangeComboBox.Enabled = false;
		}

		private void DrawBulkEditorInitialState(string cueBanner)
		{
			if (!string.IsNullOrEmpty(cueBanner))
			{
				ComboBoxBulkEditorAdapter.DrawComboBoxText(base.HostControl as ComboBox, cueBanner);
			}
		}

		public static void DrawComboBoxText(ComboBox comboBox, string text)
		{
			using (Graphics graphics = comboBox.CreateGraphics())
			{
				TextFormatFlags textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
				Rectangle clientRectangle = comboBox.ClientRectangle;
				clientRectangle.Width -= SystemInformation.VerticalScrollBarWidth;
				if (Application.RenderWithVisualStyles)
				{
					clientRectangle.Inflate(-2, -2);
				}
				else
				{
					clientRectangle.Inflate(-3, -3);
				}
				if (LayoutHelper.IsRightToLeft(comboBox))
				{
					textFormatFlags |= TextFormatFlags.Right;
					clientRectangle.Offset(SystemInformation.VerticalScrollBarWidth + 1, 0);
				}
				using (SolidBrush solidBrush = new SolidBrush(comboBox.Enabled ? SystemColors.Window : SystemColors.Control))
				{
					graphics.FillRectangle(solidBrush, clientRectangle);
				}
				TextRenderer.DrawText(graphics, text, comboBox.Font, clientRectangle, comboBox.ForeColor, comboBox.Enabled ? SystemColors.Window : SystemColors.Control, textFormatFlags);
				if (comboBox.ContainsFocus)
				{
					ControlPaint.DrawFocusRectangle(graphics, clientRectangle);
				}
			}
		}

		protected override BulkEditorState InnerGetState(string propertyName)
		{
			return base.InnerGetState("SelectedValue");
		}

		protected override void InnerSetState(string propertyName, BulkEditorState state)
		{
			base.InnerSetState("SelectedValue", state);
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			list.Add("SelectedValue");
			list.Add("SelectedIndex");
			list.Add("SelectedItem");
			return list;
		}

		private const string ManagedPropertyName = "SelectedValue";
	}
}
