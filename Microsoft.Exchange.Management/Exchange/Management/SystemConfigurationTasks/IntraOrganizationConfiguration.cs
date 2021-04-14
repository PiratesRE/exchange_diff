using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class IntraOrganizationConfiguration : ConfigurableObject
	{
		public IntraOrganizationConfiguration() : base(new SimpleProviderPropertyBag())
		{
		}

		public Uri OnPremiseDiscoveryEndpoint
		{
			get
			{
				return (Uri)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseDiscoveryEndpoint];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseDiscoveryEndpoint] = value;
			}
		}

		public Uri OnPremiseWebServiceEndpoint
		{
			get
			{
				return (Uri)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseWebServiceEndpoint];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseWebServiceEndpoint] = value;
			}
		}

		public bool? DeploymentIsCompleteIOCReady
		{
			get
			{
				return (bool?)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.DeploymentIsCompleteIOCReady];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.DeploymentIsCompleteIOCReady] = value;
			}
		}

		public bool? HasNonIOCReadyExchangeCASServerVersions
		{
			get
			{
				return (bool?)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.HasNonIOCReadyExchangeCASServerVersions];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.HasNonIOCReadyExchangeCASServerVersions] = value;
			}
		}

		public bool? HasNonIOCReadyExchangeMailboxServerVersions
		{
			get
			{
				return (bool?)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.HasNonIOCReadyExchangeMailboxServerVersions];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.HasNonIOCReadyExchangeMailboxServerVersions] = value;
			}
		}

		public Uri OnlineDiscoveryEndpoint
		{
			get
			{
				return (Uri)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnlineDiscoveryEndpoint];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnlineDiscoveryEndpoint] = value;
			}
		}

		public string OnlineTargetAddress
		{
			get
			{
				return (string)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnlineTargetAddress];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnlineTargetAddress] = value;
			}
		}

		public MultiValuedProperty<SmtpDomain> OnPremiseTargetAddresses
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseTargetAddresses];
			}
			internal set
			{
				this.propertyBag[IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema.OnPremiseTargetAddresses] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return IntraOrganizationConfiguration.SchemaInstance;
			}
		}

		private static IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema SchemaInstance = ObjectSchema.GetInstance<IntraOrganizationConfiguration.IntraOrganizationConfigurationSchema>();

		internal class IntraOrganizationConfigurationSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition OnPremiseDiscoveryEndpoint = new SimpleProviderPropertyDefinition("OnPremiseDiscoveryEndpoint", ExchangeObjectVersion.Exchange2003, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition OnPremiseWebServiceEndpoint = new SimpleProviderPropertyDefinition("OnPremiseWebServiceEndpoint", ExchangeObjectVersion.Exchange2003, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition DeploymentIsCompleteIOCReady = new SimpleProviderPropertyDefinition("DeploymentIsCompleteIOCReady", ExchangeObjectVersion.Exchange2003, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition HasNonIOCReadyExchangeCASServerVersions = new SimpleProviderPropertyDefinition("HasNonIOCReadyExchangeCASServerVersions", ExchangeObjectVersion.Exchange2003, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition HasNonIOCReadyExchangeMailboxServerVersions = new SimpleProviderPropertyDefinition("HasNonIOCReadyExchangeMailboxServerVersions", ExchangeObjectVersion.Exchange2003, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition OnlineDiscoveryEndpoint = new SimpleProviderPropertyDefinition("OnlineDiscoveryEndpoint", ExchangeObjectVersion.Exchange2003, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition OnlineTargetAddress = new SimpleProviderPropertyDefinition("OnlineTargetAddress", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition OnPremiseTargetAddresses = new SimpleProviderPropertyDefinition("OnPremiseTargetAddresses", ExchangeObjectVersion.Exchange2003, typeof(SmtpDomain), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
