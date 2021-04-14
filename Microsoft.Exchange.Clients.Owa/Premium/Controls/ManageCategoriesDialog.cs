using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ManageCategoriesDialog
	{
		public ManageCategoriesDialog(UserContext userContext)
		{
			this.userContext = userContext;
		}

		public void Render(TextWriter writer)
		{
			ManageCategoriesDialog.RenderStrings(writer);
			writer.Write("<div id=divCtgLst tabindex=0>");
			MasterCategoryList masterCategoryList = this.userContext.GetMasterCategoryList();
			foreach (Category category in masterCategoryList)
			{
				ManageCategoriesDialog.RenderCategory(writer, category);
			}
			writer.Write("</div>");
			writer.Write("<div class=mngCtgLnk><a href=# id=\"lnkCrtCat\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1641878163));
			writer.Write("</a></div><div class=mngCtgLnk><a href=# id=\"lnkChgCatClr\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(782364430));
			writer.Write("</a></div><div class=mngCtgLnk><a href=# id=\"lnkDelCat\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1478174110));
			writer.Write("</a></div>");
		}

		internal static void RenderCategory(TextWriter writer, Category category)
		{
			string text = category.Name;
			string text2 = null;
			if (50 < text.Length)
			{
				text = text.Substring(0, 50) + "...";
				text2 = category.Name;
			}
			writer.Write("<div nowrap");
			if (text2 != null)
			{
				writer.Write(" title=\"");
				Utilities.HtmlEncode(text2, writer);
				writer.Write("\"");
			}
			writer.Write(">");
			CategorySwatch.RenderSwatch(writer, category);
			writer.Write("&nbsp;");
			Utilities.HtmlEncode(text, writer, true);
			writer.Write("</div>");
		}

		private static void RenderStrings(TextWriter writer)
		{
			writer.Write("<div id=divStr style=display:none L_MngCtgs=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1754071941));
			writer.Write("\" L_DltCtg=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1566500907));
			writer.Write("\" L_CnfDlt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(367329051));
			writer.Write("\" L_ExpDlt=\"");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1365940928));
			writer.Write("\"></div>");
		}

		private const int MaximumCharactersToDisplayCategoryName = 50;

		private UserContext userContext;
	}
}
