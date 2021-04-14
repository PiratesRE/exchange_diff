using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:EcpMultiSelect runat=server></{0}:EcpMultiSelect>")]
	[ClientScriptResource("EcpMultiSelect", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class EcpMultiSelect : EcpTextBoxSelectBase
	{
	}
}
