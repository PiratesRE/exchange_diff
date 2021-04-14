using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ControlsPropertySettingsRule : IAlignRule
	{
		protected virtual void SetControlStyle(AlignUnit unit)
		{
			Control control = unit.Control;
			control.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			if (control is Label && !(control is AutoHeightLabel))
			{
				(control as Label).UseCompatibleTextRendering = (control is LinkLabel);
				((Label)control).AutoSize = true;
				return;
			}
			if (control is AutoHeightLabel)
			{
				AutoHeightLabel autoHeightLabel = (AutoHeightLabel)control;
				autoHeightLabel.UseCompatibleTextRendering = false;
				autoHeightLabel.AutoSize = false;
				autoHeightLabel.ShowDivider = true;
				return;
			}
			if (control is ButtonBase)
			{
				ButtonBase buttonBase = control as ButtonBase;
				buttonBase.UseCompatibleTextRendering = false;
				buttonBase.AutoSize = true;
				return;
			}
			if (control is AutoSizePanel && control.Controls.Count == 1 && control.Controls[0] is TextBox && ((TextBox)control.Controls[0]).Multiline && (((TextBox)control.Controls[0]).ScrollBars == ScrollBars.Vertical || ((TextBox)control.Controls[0]).ScrollBars == ScrollBars.Both))
			{
				control.BackColor = SystemColors.Window;
				((AutoSizePanel)control).BorderStyle = BorderStyle.None;
				control.Controls[0].Dock = DockStyle.Fill;
				control.Controls[0].Location = new Point(0, 0);
				control.Controls[0].Margin = new Padding(0);
				unit.ResultMargin = new Padding(3, 0, 0, 0);
				return;
			}
			if (control is ExchangeUserControl && this.NeedAutoSizeUserControl(control as UserControl))
			{
				(control as UserControl).AutoSize = true;
				(control as UserControl).AutoSizeMode = AutoSizeMode.GrowAndShrink;
			}
		}

		public void Apply(AlignUnitsCollection collection)
		{
			foreach (AlignUnit controlStyle in collection.Units)
			{
				this.SetControlStyle(controlStyle);
			}
		}

		private bool NeedAutoSizeUserControl(UserControl ctrl)
		{
			foreach (Type type in ControlsPropertySettingsRule.nonAutoSizeUserControls)
			{
				if (type.IsAssignableFrom(ctrl.GetType()))
				{
					return false;
				}
			}
			return true;
		}

		private static List<Type> nonAutoSizeUserControls = new List<Type>
		{
			typeof(DataListControl)
		};
	}
}
