using System;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	internal class DivLabel : EncodingLabel
	{
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}
	}
}
