using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class TransportSettingsConfiguration
	{
		private TransportSettingsConfiguration(TransportConfigContainer settings, Guid organizationGuid)
		{
			this.settings = settings;
			this.perTenantSettings = new PerTenantTransportSettings(settings);
			this.organizationGuid = organizationGuid;
		}

		public TransportConfigContainer TransportSettings
		{
			get
			{
				return this.settings;
			}
		}

		public PerTenantTransportSettings PerTenantTransportSettings
		{
			get
			{
				return this.perTenantSettings;
			}
		}

		public Guid OrganizationGuid
		{
			get
			{
				return this.organizationGuid;
			}
		}

		private TransportConfigContainer settings;

		private PerTenantTransportSettings perTenantSettings;

		private Guid organizationGuid;

		public class Builder : ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder>.SimpleBuilder<TransportConfigContainer>
		{
			public override void Register()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 90, "Register", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\TransportSettingsConfiguration.cs");
				base.RootId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
				base.Register();
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				base.RootId = session.GetOrgContainerId();
				base.LoadData(session, QueryScope.OneLevel);
			}

			protected override TransportSettingsConfiguration BuildCache(List<TransportConfigContainer> configObjects)
			{
				if (configObjects.Count != 1)
				{
					base.FailureMessage = Strings.ReadTransportConfigConfigFailed;
					return null;
				}
				return new TransportSettingsConfiguration(configObjects[0], base.RootId.ObjectGuid);
			}
		}
	}
}
