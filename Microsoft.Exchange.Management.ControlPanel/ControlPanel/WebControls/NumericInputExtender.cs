using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[TargetControlType(typeof(TextBox))]
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("NumericInputBehavior", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class NumericInputExtender : ExtenderControlBase
	{
	}
}
