using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ClientWatson
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public string StackTrace { get; set; }

		[DataMember]
		public string RequestId { get; set; }

		[DataMember]
		public string Time { get; set; }

		[DataMember]
		public string ErrorType { get; set; }
	}
}
