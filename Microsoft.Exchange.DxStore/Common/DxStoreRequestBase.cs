using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class DxStoreRequestBase
	{
		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public DateTimeOffset TimeRequested { get; set; }

		[DataMember]
		public string Requester { get; set; }

		[DataMember]
		public ProcessBasicInfo ProcessInfo { get; set; }

		[DataMember]
		public string DebugString { get; set; }
	}
}
