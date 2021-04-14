using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class WriteResult
	{
		[DataMember]
		public bool IsTestUpdate { get; set; }

		[DataMember]
		public bool IsConstraintPassed { get; set; }

		[DataMember]
		public WriteResult.ResponseInfo[] Responses { get; set; }

		[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
		[Serializable]
		public class ResponseInfo
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public int LatencyInMs { get; set; }
		}
	}
}
