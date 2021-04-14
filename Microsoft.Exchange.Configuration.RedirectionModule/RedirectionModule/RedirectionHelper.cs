using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.RedirectionModule.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	public sealed class RedirectionHelper
	{
		internal static Uri GetRedirectUrlForTenantForest(string domain, string podRedirectTemplate, Uri originalUrl, int podSiteStartRange, int podSiteEndRange)
		{
			int partnerId = LocalSiteCache.LocalSite.PartnerId;
			Guid orgId;
			string redirectServer;
			if (GuidHelper.TryParseGuid(domain, out orgId))
			{
				redirectServer = EdgeSyncMservConnector.GetRedirectServer(podRedirectTemplate, orgId, partnerId, podSiteStartRange, podSiteEndRange);
			}
			else
			{
				redirectServer = EdgeSyncMservConnector.GetRedirectServer(podRedirectTemplate, string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domain), partnerId, podSiteStartRange, podSiteEndRange);
			}
			if (string.IsNullOrEmpty(redirectServer))
			{
				return null;
			}
			UriBuilder uriBuilder = new UriBuilder(originalUrl);
			uriBuilder.Host = redirectServer;
			if (uriBuilder.Port == 444)
			{
				uriBuilder.Port = 443;
			}
			return uriBuilder.Uri;
		}

		internal static Uri GetRedirectUrlForTenantSite(string domainName, string redirectTemplate, Uri originalUrl)
		{
			return RedirectionHelper.GetRedirectUrlForTenantSite(domainName, redirectTemplate, originalUrl, null);
		}

		internal static Uri GetRedirectUrlForTenantSite(string organization, string redirectTemplate, Uri originalUrl, ExEventLog eventLogger)
		{
			if (organization == null)
			{
				return null;
			}
			ADSessionSettings sessionSettings;
			try
			{
				Guid externalDirectoryOrganizationId;
				sessionSettings = (Guid.TryParse(organization, out externalDirectoryOrganizationId) ? ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId) : ADSessionSettings.FromTenantCUName(organization));
			}
			catch (CannotResolveTenantNameException)
			{
				return null;
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException)
			{
				return null;
			}
			ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 180, "GetRedirectUrlForTenantSite", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\RedirectionModule\\RedirectionHelper.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnit = RedirectionHelper.ResolveConfigurationUnitByName(organization, session);
			if (exchangeConfigurationUnit == null || exchangeConfigurationUnit.ManagementSiteLink == null)
			{
				return null;
			}
			ADSite localSite = LocalSiteCache.LocalSite;
			ADObjectId adobjectId = ADObjectIdResolutionHelper.ResolveDN(exchangeConfigurationUnit.ManagementSiteLink);
			Logger.LogEvent(eventLogger, TaskEventLogConstants.Tuple_LiveIdRedirection_UsingManagementSiteLink, organization, new object[]
			{
				organization,
				exchangeConfigurationUnit.AdminDisplayVersion,
				exchangeConfigurationUnit.ManagementSiteLink
			});
			if (adobjectId.Equals(localSite.Id))
			{
				return null;
			}
			foreach (ADObjectId adobjectId2 in localSite.ResponsibleForSites)
			{
				if (adobjectId2.Equals(adobjectId))
				{
					Logger.LogEvent(eventLogger, TaskEventLogConstants.Tuple_LiveIdRedirection_TargetSitePresentOnResponsibleForSite, organization, new object[]
					{
						organization,
						adobjectId,
						adobjectId2
					});
					return null;
				}
			}
			return RedirectionHelper.GetRedirectUrlForTenantSite(adobjectId, redirectTemplate, originalUrl);
		}

		internal static bool ShouldProcessLiveIdRedirection(HttpContext context)
		{
			return !(context.User is DelegatedPrincipal) && string.IsNullOrEmpty(RedirectionHelper.ResolveOrganizationName(context));
		}

		internal static string ResolveOrganizationName(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			HttpRequest request = httpContext.Request;
			NameValueCollection urlProperties = RedirectionHelper.GetUrlProperties(request.Url);
			string text = null;
			foreach (string name in RedirectionHelper.tenantRedirectionPropertyNames)
			{
				if (!string.IsNullOrEmpty(urlProperties[name]))
				{
					text = urlProperties[name];
				}
			}
			if (text == null && httpContext.Items["Cert-MemberOrg"] != null)
			{
				text = (string)httpContext.Items["Cert-MemberOrg"];
			}
			return text;
		}

		internal static NameValueCollection GetUrlProperties(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			UriBuilder uriBuilder = new UriBuilder(uri);
			return HttpUtility.ParseQueryString(uriBuilder.Query.Replace(';', '&'));
		}

		internal static void InitTenantsOnCurrentSiteCache()
		{
			if (RedirectionConfig.CurrentSiteTenantsCacheExpirationInHours <= 0)
			{
				return;
			}
			lock (RedirectionHelper.syncObject)
			{
				if (RedirectionHelper.currentSiteTenants == null)
				{
					RedirectionHelper.ExpireCurrentSiteTenantsCache();
				}
			}
		}

		internal static bool IsTenantOnCurrentSiteCache(string tenantName)
		{
			if (RedirectionHelper.currentSiteTenants == null)
			{
				return false;
			}
			bool result;
			lock (RedirectionHelper.syncObject)
			{
				if (RedirectionHelper.IsCurrentSiteTenantsCacheExpired())
				{
					RedirectionHelper.ExpireCurrentSiteTenantsCache();
				}
				result = RedirectionHelper.currentSiteTenants.Contains(tenantName);
			}
			return result;
		}

		internal static bool IsUserTenantOnCurrentSiteCache(string userName)
		{
			if (RedirectionHelper.currentSiteTenants == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(userName) && SmtpAddress.IsValidSmtpAddress(userName))
			{
				string domain = SmtpAddress.Parse(userName).Domain;
				return RedirectionHelper.IsTenantOnCurrentSiteCache(domain);
			}
			return false;
		}

		internal static void AddTenantToCurrentSiteCache(string tenantName)
		{
			if (RedirectionHelper.currentSiteTenants == null)
			{
				return;
			}
			lock (RedirectionHelper.syncObject)
			{
				if (RedirectionHelper.IsCurrentSiteTenantsCacheExpired())
				{
					RedirectionHelper.ExpireCurrentSiteTenantsCache();
				}
				if (RedirectionHelper.currentSiteTenants.Count < 10000)
				{
					RedirectionHelper.currentSiteTenants.Add(tenantName);
				}
			}
		}

		internal static Uri RemovePropertiesFromOriginalUri(Uri originalUrl, string[] redirectionUriFilterProperties)
		{
			UriBuilder uriBuilder = new UriBuilder(originalUrl);
			NameValueCollection urlProperties = RedirectionHelper.GetUrlProperties(originalUrl);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in urlProperties.AllKeys)
			{
				bool flag = false;
				foreach (string text2 in redirectionUriFilterProperties)
				{
					if (text2.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(text);
					stringBuilder.Append("=");
					stringBuilder.Append(urlProperties[text]);
				}
			}
			uriBuilder.Query = stringBuilder.ToString();
			return uriBuilder.Uri;
		}

		internal static ExchangeConfigurationUnit ResolveConfigurationUnitByName(string domainName)
		{
			ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(domainName), 454, "ResolveConfigurationUnitByName", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\RedirectionModule\\RedirectionHelper.cs");
			return RedirectionHelper.ResolveConfigurationUnitByName(domainName, session);
		}

		private static ExchangeConfigurationUnit ResolveConfigurationUnitByName(string domainName, ITenantConfigurationSession session)
		{
			return session.GetExchangeConfigurationUnitByName(domainName);
		}

		private static Uri GetRedirectUrlForTenantSite(ADObjectId siteId, string redirectTemplate, Uri originalUrl)
		{
			ADObjectId adobjectId = siteId;
			if (adobjectId == null)
			{
				adobjectId = TenantOrganizationPresentationObject.DefaultManagementSiteId;
			}
			return RedirectionHelper.ConvertSiteNameToUri(adobjectId.Rdn.UnescapedName, originalUrl, redirectTemplate);
		}

		private static void ExpireCurrentSiteTenantsCache()
		{
			RedirectionHelper.currentSiteTenants = new HashSet<string>();
			RedirectionHelper.currentSiteTenantsCacheExpiration = DateTime.UtcNow.AddHours((double)RedirectionConfig.CurrentSiteTenantsCacheExpirationInHours);
		}

		private static bool IsCurrentSiteTenantsCacheExpired()
		{
			return DateTime.UtcNow > RedirectionHelper.currentSiteTenantsCacheExpiration;
		}

		private static Uri ConvertSiteNameToUri(string siteName, Uri originalUrl, string redirectTemplate)
		{
			return new UriBuilder(originalUrl)
			{
				Host = string.Format(redirectTemplate, siteName)
			}.Uri;
		}

		public const string SecurityTokenUriPropertyName = "SecurityToken";

		private static readonly string[] tenantRedirectionPropertyNames = new string[]
		{
			"Organization",
			"OrganizationContext"
		};

		private static readonly object syncObject = new object();

		private static HashSet<string> currentSiteTenants = null;

		private static DateTime currentSiteTenantsCacheExpiration;
	}
}
