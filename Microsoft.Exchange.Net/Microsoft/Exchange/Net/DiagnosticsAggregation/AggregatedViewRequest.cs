using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class AggregatedViewRequest
	{
		public AggregatedViewRequest(RequestType requestType, List<string> serversToInclude, uint resultSize)
		{
			this.RequestType = requestType.ToString();
			this.ServersToInclude = serversToInclude;
			this.ResultSize = resultSize;
			this.ClientInformation = new ClientInformation();
			this.ClientInformation.SetClientInformation();
		}

		[DataMember(IsRequired = true)]
		public string RequestType { get; private set; }

		[DataMember(IsRequired = true)]
		public List<string> ServersToInclude { get; private set; }

		[DataMember(IsRequired = true)]
		public uint ResultSize { get; private set; }

		[DataMember]
		public QueueAggregatedViewRequest QueueAggregatedViewRequest { get; set; }

		[DataMember(IsRequired = true)]
		public ClientInformation ClientInformation { get; private set; }
	}
}
