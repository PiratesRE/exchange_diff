using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:NumberEditControl runat=server></{0}:NumberEditControl >")]
	[ClientScriptResource("NumberEditControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class NumberEditControl : StringEditControl
	{
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			NumericInputExtender numericInputExtender = new NumericInputExtender();
			numericInputExtender.TargetControlID = this.textBox.UniqueID;
			this.Controls.Add(numericInputExtender);
		}
	}
}
