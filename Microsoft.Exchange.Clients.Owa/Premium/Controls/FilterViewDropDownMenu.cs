using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class FilterViewDropDownMenu : ContextMenu
	{
		public FilterViewDropDownMenu(UserContext userContext) : base("divFltrDDM", userContext)
		{
		}

		protected override bool HasShadedColumn
		{
			get
			{
				return false;
			}
		}

		protected override ushort ImagePadding
		{
			get
			{
				return 3;
			}
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=divFltrMnuTtl _lnk=0 class=mnuTtl><span><img class=\"vaM\" src=\"");
			base.UserContext.RenderThemeFileUrl(output, ThemeFileId.Clear1x1);
			output.Write("\"></span><span class=\"vaM\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(-1508130752));
			output.Write("</span></div>");
			base.RenderMenuItem(output, 226051813, ThemeFileId.Clear, "_divToMe", "tome");
			base.RenderMenuItem(output, 954766149, ThemeFileId.Clear, "_divCCMe", "ccme");
			base.RenderMenuItem(output, -1020805457, ThemeFileId.Clear, "_divUnread", "unread");
			ContextMenu.RenderMenuDivider(output, "divS21");
			base.RenderMenuItem(output, -1642040455, ThemeFileId.Clear, "_divCat", "category", false, null, null, CategoryContextMenu.Create(this.userContext, OutlookModule.Mail, "divFltrCat", false));
			base.RenderMenuItem(output, 1414246128, ThemeFileId.Clear, "_divRp", "rp", false, null, null, new FromOfFilterContextMenu(base.UserContext));
			ContextMenu.RenderMenuDivider(output, "divS22");
			base.RenderMenuItem(output, 1398003256, ThemeFileId.Clear, "_divFlg", "flagged");
			base.RenderMenuItem(output, -1062318782, ThemeFileId.Clear, "_divHI", "highimp");
			base.RenderMenuItem(output, 796893232, ThemeFileId.Clear, "_divAtt", "attachments");
			output.Write("<div class=\"divCtxBtnArea ");
			if (base.UserContext.IsRtl)
			{
				output.Write("taL\">");
			}
			else
			{
				output.Write("taR\">");
			}
			RenderingUtilities.RenderButton(output, "btnFltrApp", string.Empty, string.Empty, LocalizedStrings.GetHtmlEncoded(1548124618));
			RenderingUtilities.RenderButton(output, "btnFltrCcl", string.Empty, string.Empty, LocalizedStrings.GetHtmlEncoded(-1936577052));
			output.Write("</div>");
		}
	}
}
