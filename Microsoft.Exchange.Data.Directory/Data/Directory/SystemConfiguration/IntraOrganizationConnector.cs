using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class IntraOrganizationConnector : ADConfigurationObject
	{
		public MultiValuedProperty<SmtpDomain> TargetAddressDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[IntraOrganizationConnectorSchema.TargetAddressDomains];
			}
			set
			{
				this[IntraOrganizationConnectorSchema.TargetAddressDomains] = value;
			}
		}

		public Uri DiscoveryEndpoint
		{
			get
			{
				return (Uri)this[IntraOrganizationConnectorSchema.DiscoveryEndpoint];
			}
			set
			{
				this[IntraOrganizationConnectorSchema.DiscoveryEndpoint] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[IntraOrganizationConnectorSchema.Enabled];
			}
			set
			{
				this[IntraOrganizationConnectorSchema.Enabled] = value;
			}
		}

		internal static ADObjectId GetContainerId(IConfigurationSession configSession)
		{
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			return configSession.GetOrgContainerId().GetChildId("Intra Organization Connectors");
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return IntraOrganizationConnector.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchIntraOrganizationConnector";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return IntraOrganizationConnector.Container;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal const string TaskNoun = "IntraOrganizationConnector";

		internal const string ContainerName = "Intra Organization Connectors";

		internal const string MostDerivedClass = "msExchIntraOrganizationConnector";

		internal static readonly ADObjectId Container = new ADObjectId(string.Format("CN={0}", "Intra Organization Connectors"));

		private static readonly IntraOrganizationConnectorSchema SchemaObject = ObjectSchema.GetInstance<IntraOrganizationConnectorSchema>();
	}
}
