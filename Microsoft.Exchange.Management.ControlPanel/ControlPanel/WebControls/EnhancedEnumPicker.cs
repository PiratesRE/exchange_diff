using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:EnhancedEnumPicker runat=server></{0}:EnhancedEnumPicker>")]
	[ClientScriptResource("EnhancedEnumPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ControlValueProperty("Value")]
	public class EnhancedEnumPicker : EnumPicker
	{
	}
}
