using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal class CustomNavigationBarItem : NavigationBarItemBase
	{
		public CustomNavigationBarItem(UserContext userContext, string text, string targetUrl, string largeIcon, string smallIcon) : base(userContext, text, null)
		{
			this.largeIcon = largeIcon;
			this.smallIcon = smallIcon;
			this.targetUrl = targetUrl;
		}

		protected override void RenderImageTag(TextWriter writer, bool useSmallIcons, bool isWunderBar)
		{
			writer.Write("<img class=\"");
			writer.Write(isWunderBar ? (useSmallIcons ? "nbMnuImgWS" : "nbMnuImgWB") : "nbMnuImgN");
			writer.Write("\" src=\"");
			writer.Write(useSmallIcons ? this.smallIcon : this.largeIcon);
			writer.Write("\">");
		}

		protected override void RenderOnClickHandler(TextWriter writer, NavigationModule currentModule)
		{
			Utilities.RenderScriptHandler(writer, "onclick", "sfWinOpn(\"" + this.targetUrl + "\");", false);
		}

		private readonly string largeIcon;

		private readonly string smallIcon;

		private readonly string targetUrl;
	}
}
