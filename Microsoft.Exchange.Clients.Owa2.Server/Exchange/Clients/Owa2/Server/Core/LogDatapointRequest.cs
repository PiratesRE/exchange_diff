using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class LogDatapointRequest
	{
		[DataMember(Name = "datapoints", IsRequired = true)]
		public Datapoint[] Datapoints { get; set; }
	}
}
