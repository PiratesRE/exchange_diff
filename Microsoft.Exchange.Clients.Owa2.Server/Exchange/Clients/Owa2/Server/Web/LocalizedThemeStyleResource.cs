using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class LocalizedThemeStyleResource : ThemeStyleResource
	{
		public LocalizedThemeStyleResource(string resourceName, ResourceTarget.Filter targetFilter, string currentOwaVersion, bool skipThemeFolder) : base(resourceName, targetFilter, currentOwaVersion, skipThemeFolder)
		{
		}

		public override string StyleDirectory
		{
			get
			{
				if (this.styleDirectory == null)
				{
					this.styleDirectory = base.ThemesPath;
				}
				string cultureDirectory = LocalizedThemeStyleResource.GetCultureDirectory(Thread.CurrentThread.CurrentUICulture);
				return this.styleDirectory + cultureDirectory + "/";
			}
		}

		public static string GetCultureDirectory(CultureInfo currentCulture)
		{
			int lcid = currentCulture.LCID;
			if (lcid <= 2052)
			{
				if (lcid <= 18)
				{
					if (lcid == 4)
					{
						goto IL_97;
					}
					switch (lcid)
					{
					case 17:
						break;
					case 18:
						goto IL_8F;
					default:
						goto IL_A7;
					}
				}
				else
				{
					if (lcid == 1028)
					{
						goto IL_9F;
					}
					switch (lcid)
					{
					case 1041:
						break;
					case 1042:
						goto IL_8F;
					default:
						if (lcid != 2052)
						{
							goto IL_A7;
						}
						goto IL_97;
					}
				}
				return "ja";
				IL_8F:
				return "ko";
			}
			if (lcid <= 4100)
			{
				if (lcid == 3076)
				{
					goto IL_9F;
				}
				if (lcid != 4100)
				{
					goto IL_A7;
				}
			}
			else
			{
				if (lcid == 5124)
				{
					goto IL_9F;
				}
				if (lcid != 30724)
				{
					if (lcid != 31748)
					{
						goto IL_A7;
					}
					goto IL_9F;
				}
			}
			IL_97:
			return "zhs";
			IL_9F:
			return "zht";
			IL_A7:
			return currentCulture.TextInfo.IsRightToLeft ? "rtl" : "0";
		}

		protected override string GetStyleDirectory(IPageContext pageContext, string theme, bool isBootStylesDirectory)
		{
			return pageContext.FormatURIForCDN(string.Format(this.StyleDirectory, theme), isBootStylesDirectory);
		}

		public const string BaseCultureDirectory = "0";

		public const string RtlCultureDirectory = "rtl";

		private string styleDirectory;
	}
}
