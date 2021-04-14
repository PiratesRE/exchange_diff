using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:VDirDetailsPane runat=server></{0}:VDirDetailsPane>")]
	[ClientScriptResource("VDirDetailsPane", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class VDirDetailsPane : DetailsPane
	{
	}
}
