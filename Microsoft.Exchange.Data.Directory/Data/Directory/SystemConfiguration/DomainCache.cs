using System;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class DomainCache : TimeoutCache<SmtpDomainWithSubdomains, DomainCacheValue>
	{
		public static DomainCache Singleton
		{
			get
			{
				if (DomainCache.singleton == null)
				{
					DomainCache domainCache = new DomainCache();
					Interlocked.CompareExchange<DomainCache>(ref DomainCache.singleton, domainCache, null);
					if (domainCache != DomainCache.singleton)
					{
						domainCache.Dispose();
					}
				}
				return DomainCache.singleton;
			}
		}

		public new DomainCacheValue Get(SmtpDomainWithSubdomains smtpDomainWithSubdomains)
		{
			return this.Get(smtpDomainWithSubdomains, null);
		}

		public DomainCacheValue Get(SmtpDomainWithSubdomains smtpDomainWithSubdomains, OrganizationId orgId)
		{
			if (smtpDomainWithSubdomains == null)
			{
				throw new ArgumentNullException("smtpDomainWithSubdomains");
			}
			DomainCacheValue domainCacheValue;
			if (!base.TryGetValue(smtpDomainWithSubdomains, out domainCacheValue))
			{
				domainCacheValue = this.QueryFromADAndAddToCache(smtpDomainWithSubdomains, orgId);
			}
			if (domainCacheValue == null)
			{
				DomainCache.Tracer.TraceError<SmtpDomainWithSubdomains, OrganizationId>((long)this.GetHashCode(), "Unable to get domain property for domain='{0}', orgId='{1}'.", smtpDomainWithSubdomains, orgId);
			}
			return domainCacheValue;
		}

		private DomainCacheValue QueryFromADAndAddToCache(SmtpDomainWithSubdomains smtpDomainWithSubdomains, OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = null;
			if (Datacenter.IsMultiTenancyEnabled())
			{
				try
				{
					if (orgId != null)
					{
						sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
					}
					else
					{
						sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(smtpDomainWithSubdomains.ToString());
					}
					goto IL_51;
				}
				catch (CannotResolveTenantNameException)
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					goto IL_51;
				}
			}
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_51:
			IConfigurationSession configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 159, "QueryFromADAndAddToCache", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\DomainCache.cs");
			DomainCacheValue domainCacheValue = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				AcceptedDomain acceptedDomainByDomainName = configSession.GetAcceptedDomainByDomainName(smtpDomainWithSubdomains.ToString());
				if (acceptedDomainByDomainName != null)
				{
					domainCacheValue = new DomainCacheValue();
					domainCacheValue.OrganizationId = acceptedDomainByDomainName.OrganizationId;
					domainCacheValue.LiveIdInstanceType = acceptedDomainByDomainName.LiveIdInstanceType;
					domainCacheValue.AuthenticationType = acceptedDomainByDomainName.AuthenticationType;
					this.InsertLimitedSliding(acceptedDomainByDomainName.DomainName, domainCacheValue, DomainCache.slidingTimeOut.Value, DomainCache.absoluteTimeOut.Value, null);
				}
			});
			return domainCacheValue;
		}

		private DomainCache() : base(1, DomainCache.cacheMaxSize.Value, false)
		{
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly IntAppSettingsEntry cacheMaxSize = new IntAppSettingsEntry("DomainCacheMaxSize", 2048, DomainCache.Tracer);

		private static readonly TimeSpanAppSettingsEntry absoluteTimeOut = new TimeSpanAppSettingsEntry("DomainCacheEntryAbsoluteTimeOut", TimeSpanUnit.Seconds, TimeSpan.FromHours(24.0), DomainCache.Tracer);

		private static readonly TimeSpanAppSettingsEntry slidingTimeOut = new TimeSpanAppSettingsEntry("DomainCacheEntrySlidingTimeOut", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(15.0), DomainCache.Tracer);

		private static DomainCache singleton;
	}
}
