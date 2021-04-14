using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class TenantIPFilterSetting : ADConfigurationObject
	{
		public TenantIPFilterSetting()
		{
		}

		public TenantIPFilterSetting(SerializationInfo info, StreamingContext context)
		{
			this.AllowedIPRanges = (MultiValuedProperty<IPRange>)info.GetValue("AllowedIPRanges", typeof(MultiValuedProperty<IPRange>));
			this.BlockedIPRanges = (MultiValuedProperty<IPRange>)info.GetValue("BlockedIPRanges", typeof(MultiValuedProperty<IPRange>));
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> AllowedIPRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[TenantIPFilterSettingSchema.AllowedIPRanges];
			}
			set
			{
				this[TenantIPFilterSettingSchema.AllowedIPRanges] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> BlockedIPRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[TenantIPFilterSettingSchema.BlockedIPRanges];
			}
			set
			{
				this[TenantIPFilterSettingSchema.BlockedIPRanges] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return TenantIPFilterSetting.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchTenantIPFilterSetting";
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("AllowedIPRanges", this.AllowedIPRanges);
			info.AddValue("BlockedIPRanges", this.BlockedIPRanges);
		}

		private const string AllowedIPRangesPropertyName = "AllowedIPRanges";

		private const string BlockedIPRangesPropertyName = "BlockedIPRanges";

		private const string MostDerivedClass = "msExchTenantIPFilterSetting";

		private static TenantIPFilterSettingSchema schema = ObjectSchema.GetInstance<TenantIPFilterSettingSchema>();
	}
}
