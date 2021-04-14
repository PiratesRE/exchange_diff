using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class Toolbar
	{
		public Toolbar(TextWriter writer, bool isHeader)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.isHeader = isHeader;
			this.userContext = UserContextManager.GetUserContext();
		}

		public Toolbar()
		{
			this.userContext = UserContextManager.GetUserContext();
		}

		public void RenderStart()
		{
			if (this.isHeader)
			{
				this.writer.Write("<table class=\"tbhd\" cellpadding=0 cellspacing=0><caption>");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-1587641320));
				this.writer.Write("</caption><tr>");
				return;
			}
			this.writer.Write("<table class=\"tbft\" cellpadding=0 cellspacing=0><caption>");
			this.writer.Write(LocalizedStrings.GetHtmlEncoded(-997034062));
			this.writer.Write("</caption><tr>");
		}

		public void RenderStartForSubToolbar()
		{
			this.writer.Write("<table class=\"stb\" cellpadding=0 cellspacing=0><tr>");
		}

		public void RenderEndForSubToolbar()
		{
			this.writer.Write("</tr></table>");
		}

		public void RenderEnd()
		{
			if (this.isHeader)
			{
				this.writer.Write("<td align=\"right\" class=\"crvTp\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.CornerTopRight);
				this.writer.Write("\" alt=\"\"></td>");
			}
			else
			{
				this.writer.Write("<td align=\"right\" class=\"crvBtm\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.CornerBottomRight);
				this.writer.Write("\" alt=\"\"></td>");
			}
			this.writer.Write("</tr></table>");
		}

		public void RenderButton(ToolbarButton button)
		{
			this.RenderButton(button, ToolbarButtonFlags.None);
		}

		public void RenderButton(ToolbarButton button, ToolbarButtonFlags flags)
		{
			flags |= button.Flags;
			bool flag = ToolbarButtonFlags.Tab == (flags & ToolbarButtonFlags.Tab);
			bool flag2 = ToolbarButtonFlags.NoAction == (flags & ToolbarButtonFlags.NoAction);
			if (flag2)
			{
				this.writer.Write("<td nowrap><div class=\"divNoRR\">");
				if ((flags & ToolbarButtonFlags.Image) != (ToolbarButtonFlags)0U)
				{
					this.writer.Write("<img src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, button.Image);
					this.writer.Write("\"");
					if ((flags & ToolbarButtonFlags.Text) != (ToolbarButtonFlags)0U)
					{
						this.writer.Write(" alt=\"\">");
						this.writer.Write(' ');
					}
					else
					{
						if (button.TextId != -1018465893)
						{
							this.writer.Write(" alt=\"");
							if (button.ToolTip == null)
							{
								this.writer.Write(LocalizedStrings.GetHtmlEncoded(button.TextId));
							}
							else
							{
								this.writer.Write(button.ToolTip);
							}
							this.writer.Write("\"");
						}
						this.writer.Write(">");
					}
				}
				else
				{
					this.writer.Write("<img class=\"noSrc\" src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
					this.writer.Write("\" alt=\"\">");
				}
				if ((flags & ToolbarButtonFlags.Text) != (ToolbarButtonFlags)0U)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(button.TextId));
				}
				this.writer.Write("</div></td>");
				return;
			}
			if (flag)
			{
				this.writer.Write("<td class=\"tabhk\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear1x1);
				this.writer.Write("\"></td>");
			}
			this.writer.Write("<td");
			if ((flags & ToolbarButtonFlags.Sticky) != (ToolbarButtonFlags)0U)
			{
				this.writer.Write(" id=\"{0}\"", button.Command);
				if ((flags & ToolbarButtonFlags.Selected) != (ToolbarButtonFlags)0U)
				{
					this.writer.Write(" class=\"sl\"");
				}
			}
			if (flag)
			{
				this.writer.Write(" class=\"tab\"");
			}
			this.writer.Write(" nowrap>");
			if (flag)
			{
				this.writer.Write("<div class=\"tabbrd\">");
			}
			string arg = string.Empty;
			if (flag)
			{
				this.writer.Write("<a class=");
				arg = "tab";
			}
			else
			{
				this.writer.Write("<a href=\"#\" onClick=\"return onClkTb('");
				this.writer.Write(button.Command);
				this.writer.Write("');\" class=");
			}
			if ((flags & ToolbarButtonFlags.NoHover) == (ToolbarButtonFlags)0U)
			{
				this.writer.Write("\"btn{0}\"", arg);
			}
			else
			{
				this.writer.Write("\"noHv\"");
			}
			if (button.TextId != -1018465893)
			{
				this.writer.Write(" title=\"");
				if (button.ToolTip == null)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(button.TextId));
				}
				else
				{
					this.writer.Write(button.ToolTip);
				}
				this.writer.Write("\"");
			}
			this.writer.Write(" id=\"");
			if (this.isHeader)
			{
				this.writer.Write("lnkHdr");
			}
			else
			{
				this.writer.Write("lnkFtr");
			}
			this.writer.Write(button.Command);
			this.writer.Write("\">");
			if ((flags & ToolbarButtonFlags.Image) != (ToolbarButtonFlags)0U)
			{
				this.writer.Write("<img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, button.Image);
				this.writer.Write("\"");
				if ((flags & ToolbarButtonFlags.Text) != (ToolbarButtonFlags)0U)
				{
					this.writer.Write(" alt=\"\">");
					this.writer.Write(' ');
				}
				else
				{
					if (button.TextId != -1018465893)
					{
						this.writer.Write(" alt=\"");
						if (button.ToolTip == null)
						{
							this.writer.Write(LocalizedStrings.GetHtmlEncoded(button.TextId));
						}
						else
						{
							this.writer.Write(button.ToolTip);
						}
						this.writer.Write("\"");
					}
					this.writer.Write(">");
				}
			}
			else
			{
				this.writer.Write("<img class=\"noSrc\" src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\">");
			}
			if ((flags & ToolbarButtonFlags.Text) != (ToolbarButtonFlags)0U)
			{
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(button.TextId));
			}
			if (flag2)
			{
				this.writer.Write("</div>");
			}
			else
			{
				this.writer.Write("</a>");
			}
			if (flag)
			{
				this.writer.Write("</div>");
			}
			this.writer.Write("</td>");
		}

		public void RenderDivider(string id, bool displayed)
		{
			this.writer.Write("<td class=\"dv\"");
			if (id != null)
			{
				this.writer.Write(" id=\"");
				this.writer.Write(id);
				this.writer.Write("\"");
			}
			if (!displayed)
			{
				this.writer.Write(" style=\"display:none\"");
			}
			this.writer.Write("><img src=\"");
			this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.ToolbarDivider);
			this.writer.Write("\" alt=\"\"></td>");
		}

		public void RenderDivider()
		{
			this.RenderDivider(null, true);
		}

		public void RenderFill()
		{
			this.writer.Write("<td class=\"w100\">&nbsp;</td>");
		}

		public void RenderSpace()
		{
			this.writer.Write("<td>&nbsp;</td>");
		}

		private TextWriter writer;

		private bool isHeader = true;

		private UserContext userContext;
	}
}
