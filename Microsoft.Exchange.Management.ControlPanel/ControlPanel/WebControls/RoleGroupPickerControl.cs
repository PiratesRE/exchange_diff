using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:RoleGroupPickerControl runat=server></{0}:RoleGroupPickerControl>")]
	[ClientScriptResource("RoleGroupPickerControl", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class RoleGroupPickerControl : PickerControl
	{
		public RoleGroupPickerControl()
		{
			base.ValueProperty = "RoleGroupObjectIdentity";
			base.DisplayProperty = "DisplayName";
			base.PickerFormUrl = "~/Pickers/RoleGroupPicker.aspx";
		}
	}
}
