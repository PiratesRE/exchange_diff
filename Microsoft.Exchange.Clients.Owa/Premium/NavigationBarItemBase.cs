using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class NavigationBarItemBase
	{
		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		protected virtual bool IsCurrentModule(NavigationModule module)
		{
			return false;
		}

		protected NavigationBarItemBase(UserContext userContext, string text, string idSuffix)
		{
			this.userContext = userContext;
			this.text = text;
			this.idSuffix = idSuffix;
		}

		public void Render(TextWriter writer, NavigationModule currentModule, int wunderBarWidth, bool useSmallIcon)
		{
			bool flag = wunderBarWidth != 0;
			writer.Write("<a href=\"#\" class=\"nbMnuItm");
			if (flag)
			{
				writer.Write(useSmallIcon ? " nbMnuItmWS" : " nbMnuItmWB");
			}
			else
			{
				writer.Write(" nbMnuItmN");
			}
			writer.Write(this.IsCurrentModule(currentModule) ? " nbHiLt" : " nbNoHiLt");
			writer.Write("\"");
			if (flag)
			{
				writer.Write(" style=\"width:");
				writer.Write(wunderBarWidth);
				writer.Write("%\"");
			}
			if (this.idSuffix != null)
			{
				writer.Write(" id=\"");
				writer.Write((flag ? "lnkQl" : "lnk") + this.idSuffix);
				writer.Write("\" ");
			}
			this.RenderOnClickHandler(writer, currentModule);
			if (flag)
			{
				writer.Write(" title=\"");
				Utilities.HtmlEncode(this.text, writer);
				writer.Write("\"");
			}
			writer.Write(">");
			this.RenderImageTag(writer, useSmallIcon, flag);
			if (!flag)
			{
				writer.Write("<span class=\"nbMainTxt\">");
				Utilities.HtmlEncode(this.text, writer);
				writer.Write("</span>");
			}
			writer.Write("</a>");
		}

		protected abstract void RenderImageTag(TextWriter writer, bool useSmallIcons, bool isWunderBar);

		protected abstract void RenderOnClickHandler(TextWriter writer, NavigationModule currentModule);

		private const string LnkIdPrefix = "lnk";

		private const string LnkIdQlPrefix = "lnkQl";

		private readonly UserContext userContext;

		private readonly string text;

		private readonly string idSuffix;
	}
}
