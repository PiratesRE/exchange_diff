using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ControlValueProperty("Values")]
	[ClientScriptResource("EmailAddressCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:EmailAddressCollectionEditor runat=server></{0}:EmailAddressCollectionEditor>")]
	[DefaultProperty("Text")]
	public class EmailAddressCollectionEditor : EcpCollectionEditor
	{
		public EmailAddressCollectionEditor()
		{
			base.DialogWidth = 600;
		}
	}
}
