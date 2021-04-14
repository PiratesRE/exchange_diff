using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class DomainPropertyCache : LazyLookupExactTimeoutCache<SmtpDomainWithSubdomains, DomainProperties>
	{
		public static DomainPropertyCache Singleton
		{
			get
			{
				if (DomainPropertyCache.instance == null)
				{
					lock (DomainPropertyCache.singletonLocker)
					{
						if (DomainPropertyCache.instance == null)
						{
							DomainPropertyCache.instance = new DomainPropertyCache();
						}
					}
				}
				return DomainPropertyCache.instance;
			}
		}

		private DomainPropertyCache() : base(DomainPropertyCache.MaxCacheCount.Value, false, DomainPropertyCache.SlidingLiveTime.Value, DomainPropertyCache.AbsoluteLiveTime.Value, CacheFullBehavior.ExpireExisting)
		{
		}

		protected override DomainProperties CreateOnCacheMiss(SmtpDomainWithSubdomains key, ref bool shouldAdd)
		{
			DomainProperties result = this.QueryFromAD(key);
			shouldAdd = true;
			return result;
		}

		private DomainProperties QueryFromAD(SmtpDomainWithSubdomains smtpDomainWithSubdomains)
		{
			ADSessionSettings adsessionSettings = null;
			if (Datacenter.IsMultiTenancyEnabled())
			{
				try
				{
					adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(smtpDomainWithSubdomains.ToString());
					goto IL_5C;
				}
				catch (CannotResolveTenantNameException arg)
				{
					DomainPropertyCache.Tracer.TraceError<SmtpDomainWithSubdomains, CannotResolveTenantNameException>((long)this.GetHashCode(), "Failed to resolve tenant with domain {0}. Error: {1}", smtpDomainWithSubdomains, arg);
					adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					goto IL_5C;
				}
			}
			adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IL_5C:
			DomainPropertyCache.Tracer.TraceDebug<SmtpDomainWithSubdomains, OrganizationId>((long)this.GetHashCode(), "The OrganizationId for domain {0} is {1}.", smtpDomainWithSubdomains, adsessionSettings.CurrentOrganizationId);
			IConfigurationSession configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 219, "QueryFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\DomainPropertyCache.cs");
			DomainProperties domainProperty = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				AcceptedDomain acceptedDomainByDomainName = configSession.GetAcceptedDomainByDomainName(smtpDomainWithSubdomains.ToString());
				if (acceptedDomainByDomainName != null)
				{
					domainProperty = new DomainProperties(smtpDomainWithSubdomains.SmtpDomain);
					domainProperty.OrganizationId = acceptedDomainByDomainName.OrganizationId;
					domainProperty.LiveIdInstanceType = acceptedDomainByDomainName.LiveIdInstanceType;
					DomainPropertyCache.Tracer.TraceDebug<AcceptedDomain, OrganizationId>((long)this.GetHashCode(), "Accepted domain {0} in organization {1}", acceptedDomainByDomainName, acceptedDomainByDomainName.OrganizationId);
				}
			});
			return domainProperty;
		}

		private static object singletonLocker = new object();

		private static DomainPropertyCache instance;

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly IntAppSettingsEntry MaxCacheCount = new IntAppSettingsEntry("DomainPropertyCache.MaxCacheCount", 10000, DomainPropertyCache.Tracer);

		private static readonly TimeSpanAppSettingsEntry SlidingLiveTime = new TimeSpanAppSettingsEntry("DomainPropertyCache.SlidingLiveTime", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(30.0), DomainPropertyCache.Tracer);

		private static readonly TimeSpanAppSettingsEntry AbsoluteLiveTime = new TimeSpanAppSettingsEntry("DomainPropertyCache.AbsoluteLiveTime", TimeSpanUnit.Minutes, TimeSpan.FromHours(6.0), DomainPropertyCache.Tracer);
	}
}
