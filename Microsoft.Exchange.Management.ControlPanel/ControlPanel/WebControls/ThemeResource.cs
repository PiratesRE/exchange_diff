using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ThemeResource
	{
		private static string ContentDeliveryNetworkEndpoint
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ContentDeliveryNetworkEndpoint"];
				if (string.IsNullOrWhiteSpace(text))
				{
					return string.Empty;
				}
				text = text.Trim();
				if (text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - 1);
				}
				return text;
			}
		}

		public static string GetThemeResource(Control c, string resourceName)
		{
			return ThemeResource.Private_GetThemeResource(c, resourceName);
		}

		public static string Private_GetThemeResource(Control c, string resourceName)
		{
			string text = string.Empty;
			IThemable themable = c.Page as IThemable;
			text = ThemeManager.GetDefaultThemeName((themable != null) ? themable.FeatureSet : FeatureSet.Admin);
			string contentDeliveryNetworkEndpoint = ThemeResource.ContentDeliveryNetworkEndpoint;
			int capacity = contentDeliveryNetworkEndpoint.Length + ThemeResource.LocalThemesPath.Length + text.Length + 1 + resourceName.Length;
			StringBuilder stringBuilder = new StringBuilder(contentDeliveryNetworkEndpoint, capacity);
			stringBuilder.Append(ThemeResource.LocalThemesPath);
			stringBuilder.Append(text);
			stringBuilder.Append("/");
			stringBuilder.Append(resourceName.ToLower());
			return c.ResolveUrl(stringBuilder.ToString());
		}

		public static string GetThemeResource(string theme, string resourceName)
		{
			string contentDeliveryNetworkEndpoint = ThemeResource.ContentDeliveryNetworkEndpoint;
			int capacity = contentDeliveryNetworkEndpoint.Length + ThemeResource.LocalThemesPath.Length + theme.Length + 1 + resourceName.Length;
			StringBuilder stringBuilder = new StringBuilder(contentDeliveryNetworkEndpoint, capacity);
			stringBuilder.Append(ThemeResource.LocalThemesPath);
			stringBuilder.Append(theme);
			stringBuilder.Append("/");
			stringBuilder.Append(resourceName.ToLower());
			Uri uri = new Uri(stringBuilder.ToString(), UriKind.RelativeOrAbsolute);
			if (!uri.IsAbsoluteUri)
			{
				uri = new Uri(HttpContext.Current.GetRequestUrl(), uri);
			}
			return uri.ToEscapedString();
		}

		private static string GetApplicationVersion()
		{
			string text = typeof(ThemeResource).GetApplicationVersion();
			if (string.IsNullOrEmpty(text))
			{
				text = "Current";
			}
			return text;
		}

		private const string CurrentDirectory = "Current";

		public const string DefaultTheme = "default";

		public static readonly string ApplicationVersion = ThemeResource.GetApplicationVersion();

		private static readonly string LocalThemesPath = HttpRuntime.AppDomainAppVirtualPath + "/" + ThemeResource.ApplicationVersion + "/themes/";

		private static readonly string LocalScriptPath = HttpRuntime.AppDomainAppVirtualPath + "/" + ThemeResource.ApplicationVersion + "/scripts/";

		private static readonly string LocalExportToolPath = HttpRuntime.AppDomainAppVirtualPath + "/" + ThemeResource.ApplicationVersion + "/exporttool/";

		public static readonly string ScriptPath = ThemeResource.ContentDeliveryNetworkEndpoint + ThemeResource.LocalScriptPath;

		public static readonly string ExportToolPath = ThemeResource.ContentDeliveryNetworkEndpoint + ThemeResource.LocalExportToolPath + (string.IsNullOrEmpty(ThemeResource.ContentDeliveryNetworkEndpoint) ? "{0}/" : string.Empty);

		public static readonly string BlankHtmlPath = ThemeResource.LocalScriptPath + "blank.htm";
	}
}
