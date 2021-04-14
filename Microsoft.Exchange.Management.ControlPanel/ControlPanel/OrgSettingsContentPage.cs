using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("OrgSettingsContentPage", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class OrgSettingsContentPage : EcpSdoBaseContentPage
	{
	}
}
