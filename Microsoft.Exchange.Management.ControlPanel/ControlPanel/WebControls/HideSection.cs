using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class HideSection : WebControl
	{
		public HideSection() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			for (Control parent = this.Parent; parent != null; parent = parent.Parent)
			{
				Section section = parent as Section;
				if (section != null)
				{
					section.Visible = false;
					return;
				}
			}
		}
	}
}
