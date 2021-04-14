using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	[Serializable]
	public class DxStoreReplyBase
	{
		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public DateTimeOffset TimeReceived { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }

		[DataMember]
		public string Responder { get; set; }

		[DataMember]
		public ProcessBasicInfo ProcessInfo { get; set; }

		[DataMember]
		public int MostRecentUpdateNumber { get; set; }

		[DataMember]
		public string DebugString { get; set; }
	}
}
