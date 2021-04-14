using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ViewDropDownMenu
	{
		public ViewDropDownMenu(UserContext userContext, ReadingPanePosition readingPanePosition, bool showConversationOptions, bool allowReadingPaneBottom)
		{
			this.userContext = userContext;
			this.readingPanePosition = readingPanePosition;
			this.showConversationOptions = showConversationOptions;
			this.allowReadingPaneBottom = allowReadingPaneBottom;
		}

		public void Render(TextWriter writer)
		{
			writer.Write("<div id=\"divRPddm\">");
			if (this.showConversationOptions)
			{
				this.RenderTitleMenuItem(writer, 2107827829, new Strings.IDs?(-1637335381));
				this.RenderCheckboxMenuItem(writer, ToolbarButtons.UseConversations, true, false);
				if (!this.userContext.IsWebPartRequest)
				{
					this.RenderLinkMenuItem(writer, ToolbarButtons.ConversationOptions);
				}
			}
			this.RenderTitleMenuItem(writer, 549375552, null);
			this.RenderCheckboxMenuItem(writer, ToolbarButtons.ReadingPaneRight, this.readingPanePosition == ReadingPanePosition.Right, true);
			if (this.allowReadingPaneBottom)
			{
				this.RenderCheckboxMenuItem(writer, ToolbarButtons.ReadingPaneBottom, this.readingPanePosition == ReadingPanePosition.Bottom, true);
			}
			this.RenderCheckboxMenuItem(writer, ToolbarButtons.ReadingPaneOff, this.readingPanePosition == ReadingPanePosition.Off, true);
			writer.Write("</div>");
		}

		private void RenderTitleMenuItem(TextWriter writer, Strings.IDs title, Strings.IDs? caption)
		{
			writer.Write("<div class=\"vwMnTtl\">");
			writer.Write(SanitizedHtmlString.FromStringId(title));
			writer.Write("</div>");
			if (caption != null)
			{
				writer.Write("<div class=\"vwMnCap\">");
				writer.Write(SanitizedHtmlString.FromStringId(caption.Value));
				writer.Write("</div>");
			}
		}

		private void RenderCheckboxMenuItem(TextWriter writer, ToolbarButton button, bool selected, bool radioButton)
		{
			writer.Write("<div id=\"divMnuItm\" ");
			writer.Write(radioButton ? "fRdo=\"1\"" : "fChk=\"1\"");
			if (selected)
			{
				writer.Write(" fSel=\"1\"");
			}
			writer.Write(" cmd=\"");
			writer.Write(button.Command);
			writer.Write("\"><a id=\"");
			writer.Write(button.Command);
			writer.Write("\" class=\"vwMnItm\" href=\"#\"><div class=\"vwMnChk\">");
			OwaContext.Current.SessionContext.RenderThemeImage(writer, ThemeFileId.Checkmark, "tbLh", new object[]
			{
				"id=\"imgChk\"",
				selected ? null : "style=\"display:none\""
			});
			writer.Write("</div><span class=\"tbLh tbBtwn\">");
			Utilities.SanitizeHtmlEncode(LocalizedStrings.GetNonEncoded(button.TextId), writer);
			writer.Write("</span></a></div>");
		}

		private void RenderLinkMenuItem(TextWriter writer, ToolbarButton button)
		{
			writer.Write("<div id=\"divMnuItm\" cmd=\"");
			writer.Write(button.Command);
			writer.Write("\"><a id=\"");
			writer.Write(button.Command);
			writer.Write("\" class=\"vwMnItm\" href=\"#\"><div class=\"vwMnChk\"></div><span class=\"tbLh tbBtwn\">");
			Utilities.SanitizeHtmlEncode(LocalizedStrings.GetNonEncoded(button.TextId), writer);
			writer.Write("</span></a></div>");
		}

		private UserContext userContext;

		private ReadingPanePosition readingPanePosition = ReadingPanePosition.Right;

		private bool showConversationOptions;

		private bool allowReadingPaneBottom = true;
	}
}
