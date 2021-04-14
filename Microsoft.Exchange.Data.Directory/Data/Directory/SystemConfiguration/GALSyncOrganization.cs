using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class GALSyncOrganization : ADLegacyVersionableObject
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeConfigurationUnit.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return GALSyncOrganization.schema;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		public string GALSyncClientData
		{
			get
			{
				return (string)this[GALSyncOrganizationSchema.GALSyncClientData];
			}
			internal set
			{
				this[GALSyncOrganizationSchema.GALSyncClientData] = value;
			}
		}

		private static GALSyncOrganizationSchema schema = ObjectSchema.GetInstance<GALSyncOrganizationSchema>();
	}
}
