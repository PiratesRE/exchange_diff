using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics.Components.GlobalLocatorCache;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	internal class GlsCacheService : GlobalLocatorCache, IGlsCacheService
	{
		public GlsCacheService()
		{
			if (GlsCacheService.nonexistentNamespaces == null)
			{
				GlsCacheService.nonexistentNamespaces = new List<string>(new string[]
				{
					GlsProperty.ExoPrefix,
					GlsProperty.FfoPrefix
				});
			}
			if (GlsCacheService.smtpNextHopDomainFormat == null)
			{
				GlsCacheService.smtpNextHopDomainFormat = RegistrySettings.ExchangeServerCurrentVersion.SmtpNextHopDomainFormat;
			}
		}

		public FindDomainsResponse FindDomains(RequestIdentity identity, FindDomainsRequest findDomainsRequest)
		{
			throw new NotImplementedException();
		}

		public FindDomainResponse FindDomain(RequestIdentity identity, FindDomainRequest findDomainRequest)
		{
			string domainId = findDomainRequest.Domain.DomainName;
			return this.LoggingWrapper<FindDomainResponse>(identity, domainId, "FindDomain", delegate(bool found, TenantInfo tenantInfo)
			{
				FindDomainResponse findDomainResponse = new FindDomainResponse();
				string empty = string.Empty;
				findDomainResponse.TransactionID = identity.RequestTrackingGuid.ToString();
				findDomainResponse.DomainInfo = new DomainInfo();
				findDomainResponse.DomainInfo.NoneExistNamespaces = tenantInfo.NoneExistNamespaces;
				findDomainResponse.TenantInfo = tenantInfo;
				if (found)
				{
					findDomainResponse.TenantInfo.TenantId = tenantInfo.TenantId;
					findDomainResponse.DomainInfo.DomainName = domainId;
				}
				return findDomainResponse;
			});
		}

		public FindTenantResponse FindTenant(RequestIdentity identity, FindTenantRequest findTenantRequest)
		{
			string tenantId = findTenantRequest.Tenant.TenantId.ToString();
			return this.LoggingWrapper<FindTenantResponse>(identity, tenantId, "FindTenant", delegate(bool found, TenantInfo tenantInfo)
			{
				FindTenantResponse findTenantResponse = new FindTenantResponse();
				string empty = string.Empty;
				findTenantResponse.TransactionID = identity.RequestTrackingGuid.ToString();
				findTenantResponse.TenantInfo = tenantInfo;
				if (found)
				{
					findTenantResponse.TenantInfo.TenantId = new Guid(tenantId);
				}
				return findTenantResponse;
			});
		}

		public T LoggingWrapper<T>(RequestIdentity identity, string parameterValue, string methodName, Func<bool, TenantInfo, T> method) where T : ResponseBase
		{
			ExTraceGlobals.ServiceTracer.TraceDebug<string, string>(0L, "Processing {0}() for {1}", methodName, parameterValue);
			string text = string.Empty;
			int tickCount = Environment.TickCount;
			string text2 = "Exception";
			T result;
			try
			{
				TenantInfo tenantInfo = new TenantInfo();
				bool arg = true;
				OfflineTenantInfo offlineTenantInfo;
				if (!base.TryFindTenantInfoInCache(parameterValue, out offlineTenantInfo, out text))
				{
					text2 = "NotFound";
					arg = false;
					tenantInfo.NoneExistNamespaces = GlsCacheService.nonexistentNamespaces;
				}
				else
				{
					tenantInfo.TenantId = offlineTenantInfo.TenantId;
					tenantInfo.NoneExistNamespaces = new List<string>();
					tenantInfo.Properties = new List<KeyValuePair<string, string>>();
					tenantInfo.Properties.Add(new KeyValuePair<string, string>(TenantProperty.EXOResourceForest.Name, GlobalLocatorCache.GetForest(offlineTenantInfo.ResourceForestId)));
					tenantInfo.Properties.Add(new KeyValuePair<string, string>(TenantProperty.EXOAccountForest.Name, GlobalLocatorCache.GetForest(offlineTenantInfo.AccountForestId)));
					int partnerId;
					if (offlineTenantInfo.PartnerId == -1)
					{
						new MServDirectorySession(null).TryGetPartnerIdFromForestFqdn(GlobalLocatorCache.GetForest(offlineTenantInfo.ResourceForestId), out partnerId);
					}
					else
					{
						partnerId = offlineTenantInfo.PartnerId;
					}
					tenantInfo.Properties.Add(new KeyValuePair<string, string>(TenantProperty.EXOSmtpNextHopDomain.Name, string.Format(GlsCacheService.smtpNextHopDomainFormat, partnerId)));
					text2 = "Found";
				}
				result = method(arg, tenantInfo);
			}
			catch (Exception ex)
			{
				text = ex.Message;
				throw;
			}
			finally
			{
				int num = Environment.TickCount - tickCount;
				GLSLogger.BeginAppend(methodName, parameterValue, "localhost", text2, null, (long)num, text, identity.RequestTrackingGuid.ToString(), string.Empty, string.Empty);
			}
			ExTraceGlobals.ServiceTracer.TraceDebug<string, RequestIdentity, string>(0L, "{0} from GlsCache, {1}, failure:{2}", text2, identity, text);
			return result;
		}

		private static List<string> nonexistentNamespaces;

		private static string smtpNextHopDomainFormat;
	}
}
