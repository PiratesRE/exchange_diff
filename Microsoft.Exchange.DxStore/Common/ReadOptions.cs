using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class ReadOptions
	{
		[DataMember]
		public bool IsAllowStale { get; set; }

		[DataMember]
		public bool ReadMajority { get; set; }

		[DataMember]
		public bool ReadAfterTestWrite { get; set; }
	}
}
