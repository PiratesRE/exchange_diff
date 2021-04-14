using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class LocalViewRequest
	{
		public LocalViewRequest(RequestType requestType)
		{
			this.RequestType = requestType.ToString();
			this.ClientInformation = new ClientInformation();
			this.ClientInformation.SetClientInformation();
		}

		[DataMember(IsRequired = true)]
		public string RequestType { get; private set; }

		[DataMember]
		public QueueLocalViewRequest QueueLocalViewRequest { get; set; }

		[DataMember(IsRequired = true)]
		public ClientInformation ClientInformation { get; private set; }
	}
}
