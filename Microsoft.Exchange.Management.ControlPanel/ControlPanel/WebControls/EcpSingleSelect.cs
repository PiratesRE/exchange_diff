using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:EcpSingleSelect runat=server></{0}:EcpSingleSelect>")]
	[ClientScriptResource("EcpSingleSelect", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class EcpSingleSelect : EcpTextBoxSelectBase
	{
	}
}
