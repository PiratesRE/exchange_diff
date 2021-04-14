using System;
using System.IO;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class InlineHtmlTextWriter : HtmlTextWriter
	{
		public InlineHtmlTextWriter(TextWriter writer) : base(writer, string.Empty)
		{
		}

		public override void WriteLine()
		{
		}
	}
}
