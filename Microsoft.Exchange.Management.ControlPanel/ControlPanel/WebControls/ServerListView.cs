using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(ListView))]
	[ClientScriptResource("ServerListView", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class ServerListView : ListView
	{
	}
}
