using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class MenuSeparator : MenuItem
	{
		public MenuSeparator()
		{
			this.CssClass = "MenuSeparator";
		}

		public override string ToJavaScript()
		{
			return "new MenuSeparator()";
		}
	}
}
