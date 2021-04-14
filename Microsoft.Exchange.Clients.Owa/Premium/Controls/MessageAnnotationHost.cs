using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class MessageAnnotationHost : OwaPage
	{
		public static void RenderMessageAnnotationDivStart(TextWriter output, string messageNoteId)
		{
			output.Write("<div id=\"");
			Utilities.HtmlEncode(messageNoteId, output);
			output.Write("\"");
			output.Write(" class=\"divAnnot\"");
			output.Write(">");
		}

		public static void RenderMessageAnnotationDivEnd(TextWriter output)
		{
			output.Write("</div>");
		}

		public static void RenderMessageAnnotationControl(TextWriter output, string messageNoteControlId, string messageNoteText)
		{
			output.Write("<textarea id=\"");
			Utilities.HtmlEncode(messageNoteControlId, output);
			output.Write(string.Format("\" class=\"Annot\">{0}</textarea>", messageNoteText));
		}
	}
}
