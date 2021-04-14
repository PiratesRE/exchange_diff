using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class ThemeStyleResource : StyleResource
	{
		public ThemeStyleResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion, bool skipThemeFolder) : base(resourceName, targetFilter, currentOwaVersion, true)
		{
			this.skipThemeFolder = skipThemeFolder;
		}

		public string ThemesPath
		{
			get
			{
				if (!this.skipThemeFolder)
				{
					return ResourcePathBuilderUtilities.GetThemeResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath);
				}
				return ResourcePathBuilderUtilities.GetStyleResourcesRelativeFolderPathWithSlash(base.ResourcesRelativeFolderPath);
			}
		}

		public virtual string StyleDirectory
		{
			get
			{
				if (this.styleDirectory == null)
				{
					this.styleDirectory = (this.skipThemeFolder ? ResourcePathBuilderUtilities.GetImageResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath) : ResourcePathBuilderUtilities.GetThemeImageResourcesRelativeFolderPath(base.ResourcesRelativeFolderPath));
				}
				string spriteDirectory = ThemeStyleResource.GetSpriteDirectory(Thread.CurrentThread.CurrentUICulture);
				return this.styleDirectory + spriteDirectory + "/";
			}
		}

		public static ThemeStyleResource FromSlabStyle(SlabStyleFile style, string owaVersion, bool shouldSkipThemeFolder)
		{
			ResourceTarget.Filter targetFilter = ResourceTarget.Any;
			if (style.IsHighResolutionSprite())
			{
				if (style.IsForLayout(LayoutType.TouchWide))
				{
					targetFilter = ResourceTarget.WideHighResolution;
				}
				if (style.IsForLayout(LayoutType.TouchNarrow))
				{
					targetFilter = ResourceTarget.NarrowHighResolution;
				}
			}
			else
			{
				if (style.IsForLayout(LayoutType.Mouse))
				{
					targetFilter = ResourceTarget.MouseOnly;
				}
				if (style.IsForLayout(LayoutType.TouchWide))
				{
					targetFilter = ResourceTarget.WideOnly;
				}
				if (style.IsForLayout(LayoutType.TouchNarrow))
				{
					targetFilter = ResourceTarget.NarrowOnly;
				}
			}
			if (style.IsSprite())
			{
				return new ThemeStyleResource(style.Name, targetFilter, owaVersion, shouldSkipThemeFolder);
			}
			return new LocalizedThemeStyleResource(style.Name, targetFilter, owaVersion, shouldSkipThemeFolder);
		}

		public static string GetSpriteDirectory(CultureInfo currentCulture)
		{
			if (!currentCulture.TextInfo.IsRightToLeft)
			{
				return "0";
			}
			return "rtl";
		}

		protected override string GetStyleDirectory(IPageContext pageContext, string theme, bool isBootStylesDirectory)
		{
			return pageContext.FormatURIForCDN(string.Format(this.StyleDirectory, theme), isBootStylesDirectory);
		}

		private readonly bool skipThemeFolder;

		private string styleDirectory;
	}
}
