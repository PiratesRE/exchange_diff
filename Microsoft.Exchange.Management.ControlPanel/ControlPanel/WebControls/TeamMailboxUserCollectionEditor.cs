using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("TeamMailboxUserCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ControlValueProperty("Values")]
	[ToolboxData("<{0}:TeamMailboxUserCollectionEditor runat=server></{0}:TeamMailboxUserCollectionEditor>")]
	[DefaultProperty("Text")]
	public class TeamMailboxUserCollectionEditor : EcpCollectionEditor
	{
	}
}
