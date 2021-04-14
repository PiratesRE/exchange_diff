using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class DataStoreStats
	{
		[DataMember]
		public int LastUpdateNumber { get; set; }

		[DataMember]
		public DateTimeOffset LastUpdateTime { get; set; }

		public DataStoreStats Clone()
		{
			return (DataStoreStats)base.MemberwiseClone();
		}
	}
}
