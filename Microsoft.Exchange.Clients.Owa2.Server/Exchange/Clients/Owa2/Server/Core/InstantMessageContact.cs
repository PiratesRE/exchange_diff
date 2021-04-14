using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InstantMessageContact
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string SipUri { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string DisplayName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string Title { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string Company { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string Office { get; set; }
	}
}
