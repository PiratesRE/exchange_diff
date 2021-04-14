using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal class StandardNavigationBarItem : NavigationBarItemBase
	{
		public StandardNavigationBarItem(NavigationModule module, UserContext userContext, string text, string idSuffix, string onClickHandler, ThemeFileId largeIcon, ThemeFileId smallIcon) : base(userContext, text, idSuffix)
		{
			this.largeIcon = largeIcon;
			this.smallIcon = smallIcon;
			this.navigationModule = module;
			this.onClickHandler = onClickHandler;
		}

		protected override void RenderImageTag(TextWriter writer, bool useSmallIcons, bool isWunderBar)
		{
			base.UserContext.RenderThemeImage(writer, useSmallIcons ? this.smallIcon : this.largeIcon, isWunderBar ? (useSmallIcons ? "nbMnuImgWS" : "nbMnuImgWB") : "nbMnuImgN", new object[0]);
		}

		protected override void RenderOnClickHandler(TextWriter writer, NavigationModule currentModule)
		{
			Utilities.RenderScriptHandler(writer, "onclick", (currentModule == NavigationModule.Options) ? ("mnNav(" + (int)this.navigationModule + ");") : this.onClickHandler, false);
		}

		protected override bool IsCurrentModule(NavigationModule module)
		{
			return module == this.navigationModule;
		}

		private const int Images = 6;

		private readonly ThemeFileId largeIcon;

		private readonly ThemeFileId smallIcon;

		private readonly string onClickHandler;

		private readonly NavigationModule navigationModule;
	}
}
