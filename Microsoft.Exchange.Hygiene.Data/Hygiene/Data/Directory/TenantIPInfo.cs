using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TenantIPInfo : ConfigurablePropertyBag
	{
		public IPRange IPRange
		{
			get
			{
				return (IPRange)this[TenantIPInfo.IPRangeProp];
			}
			set
			{
				this[TenantIPInfo.IPRangeProp] = value;
			}
		}

		public IPListType IPListType
		{
			get
			{
				return (IPListType)this[TenantIPInfo.IPListTypeProp];
			}
			set
			{
				this[TenantIPInfo.IPListTypeProp] = value;
			}
		}

		public bool IsRemoved
		{
			get
			{
				return (bool)this[TenantIPInfo.IsRemovedProp];
			}
			set
			{
				this[TenantIPInfo.IsRemovedProp] = value;
			}
		}

		public DateTime ChangeDatetime
		{
			get
			{
				return (DateTime)this[TenantIPInfo.ChangedDatetimeProp];
			}
			set
			{
				this[TenantIPInfo.ChangedDatetimeProp] = value;
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[TenantIPInfo.TenantIdProp];
			}
			set
			{
				this[TenantIPInfo.TenantIdProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ToString());
			}
		}

		public override string ToString()
		{
			return this.IPRange.ToString() + this.IPListType + this.IsRemoved;
		}

		public static readonly HygienePropertyDefinition IPListTypeProp = new HygienePropertyDefinition("IPListType", typeof(IPListType), IPListType.TenantAllowList, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IPRangeProp = new HygienePropertyDefinition("IPRange", typeof(IPRange));

		public static readonly HygienePropertyDefinition IsRemovedProp = new HygienePropertyDefinition("IsRemoved", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChangedDatetimeProp = new HygienePropertyDefinition("dt_ChangedDatetime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("id_TenantId", typeof(Guid?));
	}
}
