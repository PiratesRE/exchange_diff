using System;
using System.IO;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class SessionContextUtilities
	{
		public static void RenderThemeFileUrl(TextWriter writer, ThemeFileId themeFileId, ISessionContext sessionContext)
		{
			ThemeManager.RenderThemeFileUrl(writer, sessionContext.Theme.Id, themeFileId, sessionContext.IsBasicExperience);
		}

		public static void RenderThemeFileUrl(TextWriter writer, int themeFileIndex, ISessionContext sessionContext)
		{
			ThemeManager.RenderThemeFileUrl(writer, sessionContext.Theme.Id, themeFileIndex, sessionContext.IsBasicExperience);
		}

		public static void RenderThemeImage(StringBuilder builder, ThemeFileId themeFileId, string styleClass, ISessionContext sessionContext, params object[] extraAttributes)
		{
			using (StringWriter stringWriter = new StringWriter(builder))
			{
				SessionContextUtilities.RenderThemeImage(stringWriter, themeFileId, styleClass, sessionContext, extraAttributes);
			}
		}

		public static void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId, ISessionContext sessionContext)
		{
			SessionContextUtilities.RenderThemeImage(writer, themeFileId, null, sessionContext, new object[0]);
		}

		public static void RenderThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, ISessionContext sessionContext, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, sessionContext);
			foreach (object obj in extraAttributes)
			{
				if (obj != null)
				{
					writer.Write(obj);
					writer.Write(" ");
				}
			}
			SessionContextUtilities.RenderThemeImageEnd(writer, themeFileId);
		}

		public static void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId, ISessionContext sessionContext)
		{
			SessionContextUtilities.RenderBaseThemeImage(writer, themeFileId, null, sessionContext, new object[0]);
		}

		public static void RenderBaseThemeImage(TextWriter writer, ThemeFileId themeFileId, string styleClass, ISessionContext sessionContext, params object[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, true, sessionContext);
			foreach (object obj in extraAttributes)
			{
				if (obj != null)
				{
					writer.Write(obj);
					writer.Write(" ");
				}
			}
			SessionContextUtilities.RenderThemeImageEnd(writer, themeFileId);
		}

		public static void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, ISessionContext sessionContext, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageWithToolTip(writer, themeFileId, styleClass, -1018465893, sessionContext, extraAttributes);
		}

		public static void RenderThemeImageWithToolTip(TextWriter writer, ThemeFileId themeFileId, string styleClass, Strings.IDs tooltipStringId, ISessionContext sessionContext, params string[] extraAttributes)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, sessionContext);
			foreach (string value in extraAttributes)
			{
				if (!string.IsNullOrEmpty(value))
				{
					writer.Write(value);
					writer.Write(" ");
				}
			}
			Utilities.RenderImageAltAttribute(writer, sessionContext, themeFileId, tooltipStringId);
			SessionContextUtilities.RenderThemeImageEnd(writer, themeFileId);
		}

		public static void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass, ISessionContext sessionContext)
		{
			SessionContextUtilities.RenderThemeImageStart(writer, themeFileId, styleClass, false, sessionContext);
		}

		public static void RenderThemeImageStart(TextWriter writer, ThemeFileId themeFileId, string styleClass, bool renderBaseTheme, ISessionContext sessionContext)
		{
			Theme theme = renderBaseTheme ? ThemeManager.BaseTheme : sessionContext.Theme;
			if (!sessionContext.IsBasicExperience && theme.ShouldUseCssSprites(themeFileId))
			{
				writer.Write("<img src=\"");
				ThemeManager.RenderThemeFileUrl(writer, theme.Id, ThemeFileId.Clear1x1);
				writer.Write("\" class=\"csimg ");
				writer.Write(theme.GetThemeFileClass(themeFileId));
				if (!string.IsNullOrEmpty(styleClass))
				{
					writer.Write(" ");
					writer.Write(styleClass);
				}
			}
			else
			{
				writer.Write("<img src=\"");
				ThemeManager.RenderThemeFileUrl(writer, theme.Id, themeFileId, sessionContext.IsBasicExperience);
				if (!string.IsNullOrEmpty(styleClass))
				{
					writer.Write("\" class=\"");
					writer.Write(styleClass);
				}
			}
			writer.Write("\" ");
		}

		public static void RenderThemeImageEnd(TextWriter writer, ThemeFileId themeFileId)
		{
			writer.Write(">");
		}

		public static string GetThemeFileUrl(ThemeFileId themeFileId, ISessionContext sessionContext)
		{
			return ThemeManager.GetThemeFileUrl(sessionContext.Theme.Id, themeFileId, sessionContext.IsBasicExperience);
		}

		public static void RenderCssFontThemeFileUrl(TextWriter writer, ISessionContext sessionContext)
		{
			ThemeManager.RenderCssFontThemeFileUrl(writer, sessionContext.IsBasicExperience);
		}

		public static void RenderCssLink(TextWriter writer, HttpRequest request, ISessionContext sessionContext, bool phase1Only)
		{
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			SessionContextUtilities.RenderThemeFileUrl(writer, ThemeFileId.PremiumCss, sessionContext);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			SessionContextUtilities.RenderCssFontThemeFileUrl(writer, sessionContext);
			writer.Write("\">");
			writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
			ThemeManager.RenderThemeFileUrl(writer, sessionContext.Theme.Id, ThemeFileId.CssSpritesCss);
			writer.Write("\">");
			if (!phase1Only)
			{
				writer.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"");
				ThemeManager.RenderThemeFileUrl(writer, sessionContext.Theme.Id, ThemeFileId.CssSpritesCss2);
				writer.Write("\">");
			}
		}

		public static string GetDirectionMark(this ISessionContext sessionContext)
		{
			if (!sessionContext.IsRtl)
			{
				return "&#x200E;";
			}
			return "&#x200F;";
		}

		public static string GetBlankPage(this ISessionContext sessionContext)
		{
			return "about:blank";
		}

		public static string GetBlankPage(this ISessionContext sessionContext, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Utilities.HtmlEncode(path) + "blank.htm";
		}

		public static void RenderCssLink(TextWriter writer, HttpRequest request, ISessionContext sessionContext)
		{
			SessionContextUtilities.RenderCssLink(writer, request, sessionContext, false);
		}

		internal const string CssLinkStartMarkup = "<link type=\"text/css\" rel=\"stylesheet\" href=\"";

		internal const string CssLinkEndMarkup = "\">";
	}
}
