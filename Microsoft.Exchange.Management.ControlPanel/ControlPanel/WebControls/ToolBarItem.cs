using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public abstract class ToolBarItem : WebControl
	{
		protected ToolBarItem()
		{
			this.CssClass = "ToolBarItem ";
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		public virtual string ToJavaScript()
		{
			return "new ToolBarItem()";
		}
	}
}
