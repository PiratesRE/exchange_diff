using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("ExtendedListView", "Microsoft.Exchange.Management.ControlPanel.Client.DLPPolicy.js")]
	public class ExtendedListView : ListView
	{
	}
}
