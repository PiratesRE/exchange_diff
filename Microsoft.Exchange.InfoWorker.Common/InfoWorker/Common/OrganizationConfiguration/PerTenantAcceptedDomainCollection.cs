using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class PerTenantAcceptedDomainCollection : PerTenantConfigurationLoader<AcceptedDomain[]>
	{
		public AcceptedDomain[] AcceptedDomains
		{
			get
			{
				return base.Data;
			}
			internal set
			{
				this.data = value;
			}
		}

		public PerTenantAcceptedDomainCollection(OrganizationId organizationId) : base(organizationId)
		{
		}

		public PerTenantAcceptedDomainCollection(OrganizationId organizationId, TimeSpan timeoutInterval) : base(organizationId, timeoutInterval)
		{
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantAcceptedDomainCollection.notificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<AcceptedDomain>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override AcceptedDomain[] Read(IConfigurationSession session)
		{
			ADPagedReader<AcceptedDomain> adpagedReader = session.FindAllPaged<AcceptedDomain>();
			return adpagedReader.ReadAllPages();
		}

		private static object notificationLock = new object();
	}
}
