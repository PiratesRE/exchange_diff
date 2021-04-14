using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class PerTenantSmimeSettings : PerTenantConfigurationLoader<SmimeConfigurationContainer>
	{
		public SmimeConfigurationContainer Configuration
		{
			get
			{
				return base.Data;
			}
		}

		public PerTenantSmimeSettings(OrganizationId organizationId) : base(organizationId)
		{
		}

		public PerTenantSmimeSettings(OrganizationId organizationId, TimeSpan timeoutInterval) : base(organizationId, timeoutInterval)
		{
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantSmimeSettings.notificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<SmimeConfigurationContainer>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override SmimeConfigurationContainer Read(IConfigurationSession session)
		{
			SmimeConfigurationContainer[] array = session.Find<SmimeConfigurationContainer>(SmimeConfigurationContainer.GetWellKnownParentLocation(session.GetOrgContainerId()), QueryScope.SubTree, null, null, 1);
			if (array.Length == 0)
			{
				return new SmimeConfigurationContainer();
			}
			return array[0];
		}

		public SmimeConfigurationContainer ReadSmimeConfig(IConfigurationSession session)
		{
			return this.Read(session);
		}

		private static object notificationLock = new object();
	}
}
