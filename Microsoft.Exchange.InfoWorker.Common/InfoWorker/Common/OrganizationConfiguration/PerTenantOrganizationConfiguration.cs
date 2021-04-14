using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class PerTenantOrganizationConfiguration : PerTenantConfigurationLoader<Organization>
	{
		public Organization Configuration
		{
			get
			{
				return this.data;
			}
			internal set
			{
				this.data = value;
			}
		}

		public PerTenantOrganizationConfiguration(OrganizationId organizationId) : base(organizationId)
		{
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantOrganizationConfiguration.notificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<Organization>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override Organization Read(IConfigurationSession session)
		{
			return session.GetOrgContainer();
		}

		private static object notificationLock = new object();
	}
}
