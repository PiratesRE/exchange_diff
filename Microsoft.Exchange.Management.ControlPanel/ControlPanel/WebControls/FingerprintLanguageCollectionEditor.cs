using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("FingerprintLanguageCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[DefaultProperty("Text")]
	[ControlValueProperty("Values")]
	[ToolboxData("<{0}:FingerprintLanguageCollectionEditor runat=server></{0}:FingerprintLanguageCollectionEditor>")]
	public class FingerprintLanguageCollectionEditor : EcpCollectionEditor
	{
	}
}
