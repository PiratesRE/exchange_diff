using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal sealed class TrafficUserDetail : ConfigurablePropertyBag
	{
		public TrafficUserDetail()
		{
		}

		internal TrafficUserDetail(Guid tenantId, DataTable tvpTrafficUser)
		{
			this.TenantId = tenantId;
			this.TvpTrafficUser = tvpTrafficUser;
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[TrafficUserDetail.TenantIdProp];
			}
			internal set
			{
				this[TrafficUserDetail.TenantIdProp] = value;
			}
		}

		public DataTable TvpTrafficUser
		{
			get
			{
				return (DataTable)this[TrafficUserDetail.TvpTrafficUserProp];
			}
			internal set
			{
				this[TrafficUserDetail.TvpTrafficUserProp] = value;
			}
		}

		public int TrafficType
		{
			get
			{
				return (int)this[TrafficUserDetail.TrafficTypeProp];
			}
			internal set
			{
				this[TrafficUserDetail.TrafficTypeProp] = value;
			}
		}

		public DateTime LogTime
		{
			get
			{
				return (DateTime)this[TrafficUserDetail.LogTimeProp];
			}
			internal set
			{
				this[TrafficUserDetail.LogTimeProp] = value;
			}
		}

		public Guid DomainId
		{
			get
			{
				return (Guid)this[TrafficUserDetail.DomainIdProp];
			}
			internal set
			{
				this[TrafficUserDetail.DomainIdProp] = value;
			}
		}

		public string AddressPrefix
		{
			get
			{
				return (string)this[TrafficUserDetail.AddressPrefixProp];
			}
			internal set
			{
				this[TrafficUserDetail.AddressPrefixProp] = value;
			}
		}

		public string AddressDomain
		{
			get
			{
				return (string)this[TrafficUserDetail.AddressDomainProp];
			}
			internal set
			{
				this[TrafficUserDetail.AddressDomainProp] = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[TrafficUserDetail.MessageCountProp];
			}
			internal set
			{
				this[TrafficUserDetail.MessageCountProp] = value;
			}
		}

		public long MessageSize
		{
			get
			{
				return (long)this[TrafficUserDetail.MessageSizeProp];
			}
			internal set
			{
				this[TrafficUserDetail.MessageSizeProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Concat(new string[]
				{
					this.DomainId.ToString(),
					this.LogTime.ToString(),
					this.AddressPrefix,
					this.AddressDomain,
					this.TrafficType.ToString()
				}));
			}
		}

		internal static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("tenantId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TvpTrafficUserProp = new HygienePropertyDefinition("Tvp_TrafficUser", typeof(DataTable));

		internal static readonly HygienePropertyDefinition TvpDomainIdsProp = new HygienePropertyDefinition("tvp_DomainIds", typeof(DataTable));

		internal static readonly HygienePropertyDefinition TrafficTypeProp = new HygienePropertyDefinition("trafficType", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LogTimeProp = new HygienePropertyDefinition("LogTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainIdProp = new HygienePropertyDefinition("DomainID", typeof(Guid));

		internal static readonly HygienePropertyDefinition AddressPrefixProp = new HygienePropertyDefinition("AddressPrefix", typeof(string));

		internal static readonly HygienePropertyDefinition AddressDomainProp = new HygienePropertyDefinition("AddressDomain", typeof(string));

		internal static readonly HygienePropertyDefinition MessageCountProp = new HygienePropertyDefinition("MessageCount", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageSizeProp = new HygienePropertyDefinition("MessageSize", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
