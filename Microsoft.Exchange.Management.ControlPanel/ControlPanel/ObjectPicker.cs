using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ControlValueProperty("Value")]
	[ClientScriptResource("ObjectPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:ObjectPicker runat=server></{0}:ObjectPicker>")]
	public class ObjectPicker : PickerControlBase
	{
		public ObjectPicker() : base(HtmlTextWriterTag.Div)
		{
			base.ValueProperty = "Identity";
			base.PickerFormUrl = "~/pickerurlplaceholder.aspx";
		}
	}
}
