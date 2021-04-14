using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Common
{
	internal abstract class CobrandingAssetReader
	{
		public static string GetBrandId()
		{
			if (HttpContext.Current.Request.Cookies["MH"] != null)
			{
				return HttpContext.Current.Request.Cookies["MH"].Value;
			}
			return "Null";
		}

		public string GetOrganizationName(string defaultName)
		{
			string text = this.GetString(CobrandingAssetKey.OrganizationName);
			if (string.IsNullOrEmpty(text))
			{
				text = defaultName;
			}
			return text;
		}

		public bool HasAssetValue(CobrandingAssetKey assetKey)
		{
			return !string.IsNullOrEmpty(this.GetString(assetKey));
		}

		public string GetBrandImageFileUrl(CobrandingAssetKey assetKey)
		{
			string @string = this.GetString(assetKey);
			if (string.IsNullOrEmpty(@string))
			{
				return null;
			}
			return this.GetBrandResourceUrlString() + @string;
		}

		public abstract bool IsPreviewBrand();

		public abstract string GetString(CobrandingAssetKey assetKey);

		public abstract string GetBrandVersion(CultureInfo cultureInfo);

		public abstract string GetBrandResourceUrlString();

		public abstract string GetLocale(CultureInfo culture);

		public abstract string GetThemeThumbnailUrl();

		public abstract string GetThemeTitle();

		public abstract bool ShouldEnableCustomTheme { get; }

		protected void LogInitializeException(Exception e, ExEventLog.EventTuple tuple)
		{
			if (!CobrandingAssetReader.initializeErrorLogged)
			{
				CobrandingAssetReader.initializeErrorLogged = true;
				LoggingUtilities.LogEvent(tuple, new object[]
				{
					e.ToString()
				});
				if (!(e is ThreadAbortException))
				{
					LoggingUtilities.SendWatson(e);
				}
			}
		}

		protected static bool initializeErrorLogged;
	}
}
