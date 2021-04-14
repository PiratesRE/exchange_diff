using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ProvisioningReconciliationConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ProvisioningReconciliationConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ProvisioningReconciliationConfig.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ProvisioningReconciliationConfig.parentPath;
			}
		}

		public MultiValuedProperty<ReconciliationCookie> ReconciliationCookies
		{
			get
			{
				return (MultiValuedProperty<ReconciliationCookie>)this[ProvisioningReconciliationConfigSchema.ReconciliationCookies];
			}
			set
			{
				this[ProvisioningReconciliationConfigSchema.ReconciliationCookies] = value;
			}
		}

		public MultiValuedProperty<ReconciliationCookie> ReconciliationCookiesForNextCycle
		{
			get
			{
				return (MultiValuedProperty<ReconciliationCookie>)this[ProvisioningReconciliationConfigSchema.ReconciliationCookiesForNextCycle];
			}
			internal set
			{
				this[ProvisioningReconciliationConfigSchema.ReconciliationCookiesForNextCycle] = value;
			}
		}

		public ReconciliationCookie ReconciliationCookieForCurrentCycle
		{
			get
			{
				return (ReconciliationCookie)this[ProvisioningReconciliationConfigSchema.ReconciliationCookieForCurrentCycle];
			}
			internal set
			{
				this[ProvisioningReconciliationConfigSchema.ReconciliationCookieForCurrentCycle] = value;
			}
		}

		private static ProvisioningReconciliationConfigSchema schema = ObjectSchema.GetInstance<ProvisioningReconciliationConfigSchema>();

		private static string mostDerivedClass = "msExchReconciliationConfig";

		private static ADObjectId parentPath = new ADObjectId("CN=Global Settings");

		public static readonly string CanonicalName = "ProvisioningReconciliationConfig";
	}
}
