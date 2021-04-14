using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class PropertyNameInfo
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int Kind { get; set; }
	}
}
