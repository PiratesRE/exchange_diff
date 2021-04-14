using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:FingerprintUploader runat=server></{0}:FingerprintUploader>")]
	[ClientScriptResource("FingerprintUploader", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class FingerprintUploader : AjaxUploader
	{
	}
}
