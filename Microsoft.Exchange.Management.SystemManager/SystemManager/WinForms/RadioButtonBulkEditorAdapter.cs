using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class RadioButtonBulkEditorAdapter : ButtonBaseBulkEditorAdapter
	{
		public RadioButtonBulkEditorAdapter(AutoHeightRadioButton radioButton) : base(radioButton)
		{
			this.bulkEditSupport.Entering += this.OnEntering;
		}

		private void OnEntering(object sender, HandledEventArgs e)
		{
			e.Handled = (base["Checked"] != 0);
		}

		protected override void OnOwnerDraw(Graphics g)
		{
			RadioButton radioButton = base.HostControl as RadioButton;
			RadioButtonState state = radioButton.Focused ? RadioButtonState.UncheckedHot : RadioButtonState.UncheckedNormal;
			System.Drawing.ContentAlignment checkAlign = radioButton.CheckAlign;
			Size glyphSize = RadioButtonRenderer.GetGlyphSize(g, state);
			Rectangle rectangle = base.CalculateCheckBounds(checkAlign, glyphSize);
			if (base["Checked"] == 3)
			{
				int num = LayoutHelper.IsRightToLeft(base.HostControl) ? 12 : 8;
				Rectangle targetRect = new Rectangle(rectangle.Left + rectangle.Width - num, rectangle.Top, 8, 8);
				Color color = radioButton.Enabled ? radioButton.BackColor : SystemColors.Control;
				using (new SolidBrush(color))
				{
					g.DrawIcon(Icons.LockIcon, targetRect);
				}
				radioButton.Enabled = false;
				return;
			}
			if (Application.RenderWithVisualStyles)
			{
				RadioButtonRenderer.DrawRadioButton(g, rectangle.Location, state);
				return;
			}
			rectangle.X--;
			ControlPaint.DrawRadioButton(g, rectangle, ButtonState.Normal);
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			AutoHeightRadioButton autoHeightRadioButton = base.HostControl as AutoHeightRadioButton;
			bool @checked = autoHeightRadioButton.Checked;
			RadioButtonBulkEditorAdapter.UpdatePeerRadioButtons(autoHeightRadioButton, base["Checked"]);
			if (base["Checked"] != null && base["Checked"] != 3)
			{
				this.forceAllowCheckedChangedEvent = true;
				autoHeightRadioButton.Checked = autoHeightRadioButton.BulkEditDefaultChecked;
				this.forceAllowCheckedChangedEvent = false;
				autoHeightRadioButton.Checked = @checked;
				Binding binding = autoHeightRadioButton.DataBindings["Checked"];
				if (binding != null)
				{
					binding.WriteValue();
				}
				autoHeightRadioButton.TabStop = true;
			}
			else if (base["Checked"] != 3)
			{
				autoHeightRadioButton.Checked = !autoHeightRadioButton.Checked;
			}
			autoHeightRadioButton.Invalidate();
		}

		private static void UpdatePeerRadioButtons(RadioButton radioButton, BulkEditorState state)
		{
			if (radioButton.Parent != null)
			{
				foreach (object obj in radioButton.Parent.Controls)
				{
					Control control = (Control)obj;
					if (control is RadioButton)
					{
						IBulkEditor bulkEditor = control as IBulkEditor;
						RadioButtonBulkEditorAdapter radioButtonBulkEditorAdapter = bulkEditor.BulkEditorAdapter as RadioButtonBulkEditorAdapter;
						if (radioButtonBulkEditorAdapter != null)
						{
							radioButtonBulkEditorAdapter["Checked"] = state;
						}
					}
				}
			}
		}
	}
}
