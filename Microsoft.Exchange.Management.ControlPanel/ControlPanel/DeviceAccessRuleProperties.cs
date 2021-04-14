using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("DeviceAccessRuleProperties", "Microsoft.Exchange.Management.ControlPanel.Client.DeviceAccessRule.js")]
	public sealed class DeviceAccessRuleProperties : Properties
	{
		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "DeviceAccessRuleProperties";
			scriptDescriptor.AddComponentProperty("DeviceTypeSelector", base.ContentContainer.FindControl("pkrDeviceFamily").ClientID);
			scriptDescriptor.AddComponentProperty("DeviceModelSelector", base.ContentContainer.FindControl("pkrDeviceModel").ClientID);
			return scriptDescriptor;
		}
	}
}
