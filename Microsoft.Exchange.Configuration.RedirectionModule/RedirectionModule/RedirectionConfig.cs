using System;
using System.Configuration;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	internal static class RedirectionConfig
	{
		static RedirectionConfig()
		{
			RedirectionConfig.podRedirectTemplate = ConfigurationManager.AppSettings["PodRedirectTemplate"];
			int.TryParse(ConfigurationManager.AppSettings["PodSiteStartRange"], out RedirectionConfig.podSiteStartRange);
			int.TryParse(ConfigurationManager.AppSettings["PodSiteEndRange"], out RedirectionConfig.podSiteEndRange);
			RedirectionConfig.siteRedirectTemplate = ConfigurationManager.AppSettings["SiteRedirectTemplate"];
			if (!int.TryParse(ConfigurationManager.AppSettings["RedirectionTenantSiteCacheExpirationInHours"], out RedirectionConfig.currentSiteTenantsCacheExpirationInHours))
			{
				RedirectionConfig.currentSiteTenantsCacheExpirationInHours = 6;
			}
			if (ConfigurationManager.AppSettings["SessionKeyCreation"] != null)
			{
				try
				{
					RedirectionConfig.sessionKeyCreationStatus = (RedirectionConfig.SessionKeyCreation)Enum.Parse(typeof(RedirectionConfig.SessionKeyCreation), ConfigurationManager.AppSettings["SessionKeyCreation"], true);
				}
				catch (ArgumentException)
				{
					RedirectionConfig.sessionKeyCreationStatus = RedirectionConfig.SessionKeyCreation.Partner;
				}
			}
		}

		internal static string[] RedirectionUriFilterProperties
		{
			get
			{
				return RedirectionConfig.redirectionUriFilterProperties;
			}
		}

		internal static int PodSiteStartRange
		{
			get
			{
				return RedirectionConfig.podSiteStartRange;
			}
		}

		internal static int PodSiteEndRange
		{
			get
			{
				return RedirectionConfig.podSiteEndRange;
			}
		}

		internal static string PodRedirectTemplate
		{
			get
			{
				return RedirectionConfig.podRedirectTemplate;
			}
		}

		internal static string SiteRedirectTemplate
		{
			get
			{
				return RedirectionConfig.siteRedirectTemplate;
			}
		}

		internal static int CurrentSiteTenantsCacheExpirationInHours
		{
			get
			{
				return RedirectionConfig.currentSiteTenantsCacheExpirationInHours;
			}
		}

		internal static RedirectionConfig.SessionKeyCreation SessionKeyCreationStatus
		{
			get
			{
				return RedirectionConfig.sessionKeyCreationStatus;
			}
		}

		internal const int DefaultCurrentSiteTenantCacheExpirationInHours = 6;

		internal const int CurrentSiteTenantsCacheMaximumSize = 10000;

		private static string[] redirectionUriFilterProperties = new string[]
		{
			"sessionId"
		};

		private static int podSiteStartRange = 0;

		private static int podSiteEndRange = 0;

		private static string podRedirectTemplate = null;

		private static string siteRedirectTemplate = null;

		private static int currentSiteTenantsCacheExpirationInHours = 0;

		private static RedirectionConfig.SessionKeyCreation sessionKeyCreationStatus = RedirectionConfig.SessionKeyCreation.Partner;

		public enum SessionKeyCreation
		{
			Disable,
			Partner,
			Enable
		}
	}
}
