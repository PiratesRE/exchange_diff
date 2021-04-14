using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TenantConfigurationCacheEntry : ADObject
	{
		public Guid TenantId
		{
			get
			{
				return Guid.Parse(this[ADObjectSchema.RawName] as string);
			}
			set
			{
				string text = value.ToString();
				this[ADObjectSchema.RawName] = text;
				base.SetId(new ADObjectId(DalHelper.GetTenantDistinguishedName(text), value));
			}
		}

		public TenantConfigurationCacheEntryReason Reason
		{
			get
			{
				return (TenantConfigurationCacheEntryReason)this[TenantConfigurationCacheEntrySchema.Reason];
			}
			set
			{
				this[TenantConfigurationCacheEntrySchema.Reason] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantConfigurationCacheEntry.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return TenantConfigurationCacheEntry.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly string mostDerivedClass = "TenantConfigurationCacheEntry";

		private static readonly TenantConfigurationCacheEntrySchema schema = ObjectSchema.GetInstance<TenantConfigurationCacheEntrySchema>();
	}
}
