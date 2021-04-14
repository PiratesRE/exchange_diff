using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ScopeFlightsSetting
	{
		public ScopeFlightsSetting(string scope, string[] flights)
		{
			this.Scope = scope;
			this.Flights = flights;
		}

		[DataMember]
		public string Scope { get; set; }

		[DataMember]
		public string[] Flights { get; set; }
	}
}
