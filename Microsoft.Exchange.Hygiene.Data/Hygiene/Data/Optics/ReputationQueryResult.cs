using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	internal class ReputationQueryResult : ConfigurablePropertyBag
	{
		public ReputationQueryResult()
		{
			this.identity = new ConfigObjectId(Guid.NewGuid().ToString());
		}

		public override ObjectId Identity
		{
			get
			{
				return this.Identity;
			}
		}

		public long Value
		{
			get
			{
				return (long)this[ReputationQueryResult.ValueProperty];
			}
			set
			{
				this[ReputationQueryResult.ValueProperty] = value;
			}
		}

		public byte EntityType
		{
			get
			{
				return (byte)this[ReputationQueryResult.EntityTypeProperty];
			}
			set
			{
				this[ReputationQueryResult.EntityTypeProperty] = value;
			}
		}

		public string EntityKey
		{
			get
			{
				return (string)this[ReputationQueryResult.EntityKeyProperty];
			}
			set
			{
				this[ReputationQueryResult.EntityKeyProperty] = value;
			}
		}

		public int DataPointType
		{
			get
			{
				return (int)this[ReputationQueryResult.DataPointTypeProperty];
			}
			set
			{
				this[ReputationQueryResult.DataPointTypeProperty] = value;
			}
		}

		public int UdpTimeout
		{
			get
			{
				return (int)this[ReputationQueryResult.UdpTimeoutProperty];
			}
			set
			{
				this[ReputationQueryResult.UdpTimeoutProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition QueryInputCountProperty = new HygienePropertyDefinition("inputcount", typeof(int?));

		public static readonly HygienePropertyDefinition EntityTypeProperty = new HygienePropertyDefinition("entitytype", typeof(byte));

		public static readonly HygienePropertyDefinition EntityKeyProperty = new HygienePropertyDefinition("entitykey", typeof(string));

		public static readonly HygienePropertyDefinition DataPointTypeProperty = new HygienePropertyDefinition("datapointtype", typeof(int?));

		public static readonly HygienePropertyDefinition UdpTimeoutProperty = new HygienePropertyDefinition("timeout", typeof(long?));

		public static readonly HygienePropertyDefinition FlagsProperty = new HygienePropertyDefinition("flags", typeof(int?));

		public static readonly HygienePropertyDefinition ValueProperty = new HygienePropertyDefinition("value", typeof(long?));

		private ObjectId identity;
	}
}
