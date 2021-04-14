using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:FlexibleHeader runat=server></{0}:FlexibleHeader>")]
	public class FlexibleHeader : WebControl
	{
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			this.AddAttributesToRender(writer);
			int num = 0;
			for (Control parent = this.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is ListView || parent is Section)
				{
					num++;
				}
			}
			switch (num)
			{
			case 0:
				throw new NotSupportedException("FlexibleHeader should be used inside ListView or Section.");
			case 1:
				writer.RenderBeginTag(HtmlTextWriterTag.H1);
				return;
			case 2:
				writer.RenderBeginTag(HtmlTextWriterTag.H2);
				return;
			default:
				throw new NotSupportedException("The hierarchy exceeds the expectation.");
			}
		}
	}
}
