using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AvailabilityConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AvailabilityConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchAvailabilityConfig";
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public ADObjectId PerUserAccount
		{
			get
			{
				return (ADObjectId)this[AvailabilityConfigSchema.PerUserAccount];
			}
			set
			{
				this[AvailabilityConfigSchema.PerUserAccount] = value;
			}
		}

		public ADObjectId OrgWideAccount
		{
			get
			{
				return (ADObjectId)this[AvailabilityConfigSchema.OrgWideAccount];
			}
			set
			{
				this[AvailabilityConfigSchema.OrgWideAccount] = value;
			}
		}

		internal const string LdapName = "msExchAvailabilityConfig";

		public static string ContainerName = "Availability Configuration";

		private static AvailabilityConfigSchema schema = ObjectSchema.GetInstance<AvailabilityConfigSchema>();

		private static ADObjectId path = new ADObjectId("CN=Availability Configuration");

		internal static readonly ADObjectId Container = new ADObjectId(string.Format("CN={0}", AvailabilityConfig.ContainerName));
	}
}
