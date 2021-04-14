using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class SecondaryNavigationList
	{
		protected SecondaryNavigationList(string elementId)
		{
			this.elementId = elementId;
		}

		protected abstract int Count { get; }

		protected virtual void RenderListAttributes(TextWriter output)
		{
		}

		protected abstract void RenderEntryOnClickHandler(TextWriter output, int entryIndex);

		protected virtual void RenderEntryOnContextMenuHandler(TextWriter output)
		{
		}

		protected virtual void RenderEntryAttributes(TextWriter output, int entryIndex)
		{
		}

		protected virtual void RenderEntryIcon(TextWriter output, int entryIndex)
		{
		}

		protected abstract string GetEntryText(int entryIndex);

		protected virtual void RenderFooter(TextWriter output)
		{
		}

		public void Render(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"");
			output.Write(this.elementId);
			output.Write("\" class=\"secNvLst\" fSNL=\"1\" ");
			this.RenderListAttributes(output);
			output.Write(">");
			output.Write("<div id=\"divDefEnts\">");
			this.RenderEntries(output);
			output.Write("</div>");
			this.RenderFooter(output);
			output.Write("</div>");
		}

		public void RenderEntries(TextWriter output)
		{
			for (int i = 0; i < this.Count; i++)
			{
				output.Write("<div class=\"snlEntW\"><div id=\"divEnt\" class=\"snlEnt snlDef\" _onclick=\"");
				this.RenderEntryOnClickHandler(output, i);
				output.Write("\" ");
				this.RenderEntryOnContextMenuHandler(output);
				this.RenderEntryAttributes(output, i);
				output.Write(">");
				this.RenderEntryIcon(output, i);
				output.Write("<span class=\"snlEntTxt\">");
				Utilities.HtmlEncode(this.GetEntryText(i), output);
				output.Write("</span></div></div>");
			}
		}

		private string elementId;
	}
}
