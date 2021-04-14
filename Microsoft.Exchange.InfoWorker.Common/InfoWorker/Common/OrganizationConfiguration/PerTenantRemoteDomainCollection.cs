using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class PerTenantRemoteDomainCollection : PerTenantConfigurationLoader<DomainContentConfig[]>
	{
		public DomainContentConfig[] RemoteDomains
		{
			get
			{
				return base.Data;
			}
		}

		public PerTenantRemoteDomainCollection(OrganizationId organizationId) : base(organizationId)
		{
		}

		public PerTenantRemoteDomainCollection(OrganizationId organizationId, TimeSpan timeoutInterval) : base(organizationId, timeoutInterval)
		{
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantRemoteDomainCollection.notificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<DomainContentConfig>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override DomainContentConfig[] Read(IConfigurationSession session)
		{
			ADPagedReader<DomainContentConfig> adpagedReader = session.FindAllPaged<DomainContentConfig>();
			return adpagedReader.ReadAllPages();
		}

		private static object notificationLock = new object();
	}
}
