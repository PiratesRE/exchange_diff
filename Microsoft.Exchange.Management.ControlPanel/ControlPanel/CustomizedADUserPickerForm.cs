using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("CustomizedADUserPickerForm", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class CustomizedADUserPickerForm : PickerForm
	{
	}
}
