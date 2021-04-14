using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ArrangeByMenu
	{
		protected static void RenderMenuItem(TextWriter output, Strings.IDs displayString, string id, ColumnId columnId)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			Column column = ListViewColumns.GetColumn(columnId);
			output.Write("<div class=cmLnk");
			if (id != null)
			{
				output.Write(" id=");
				output.Write(id);
			}
			output.Write(" _cid=");
			output.Write((int)columnId);
			output.Write(" _so=");
			output.Write(((int)column.DefaultSortOrder).ToString(CultureInfo.InvariantCulture));
			output.Write(" _lnk=1");
			output.Write(" _tD=");
			output.Write(column.IsTypeDownCapable ? "1" : "0");
			output.Write(">");
			output.Write("<span id=spnT>");
			output.Write(LocalizedStrings.GetHtmlEncoded(displayString));
			output.Write("</span></div>");
		}

		public void Render(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<div id=divAbm class=ctxMnu style=display:none>");
			this.RenderMenuItems(output, userContext);
			RenderingUtilities.RenderDropShadows(output, userContext);
			output.Write("</div>");
		}

		protected abstract void RenderMenuItems(TextWriter output, UserContext userContext);
	}
}
