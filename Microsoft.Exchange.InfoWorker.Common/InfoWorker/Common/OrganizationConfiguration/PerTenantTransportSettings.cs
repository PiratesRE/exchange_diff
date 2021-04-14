using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class PerTenantTransportSettings : PerTenantConfigurationLoader<TransportConfigContainer>
	{
		public TransportConfigContainer Configuration
		{
			get
			{
				return base.Data;
			}
		}

		public PerTenantTransportSettings(OrganizationId organizationId) : base(organizationId)
		{
		}

		public PerTenantTransportSettings(OrganizationId organizationId, TimeSpan timeoutInterval) : base(organizationId, timeoutInterval)
		{
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantTransportSettings.notificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<TransportConfigContainer>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override TransportConfigContainer Read(IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(this.organizationId.ConfigurationUnit, QueryScope.SubTree, null, null, 0);
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public TransportConfigContainer ReadTransportConfig(IConfigurationSession session)
		{
			return this.Read(session);
		}

		private static object notificationLock = new object();
	}
}
