using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetPersonaModernGroupMembershipRequestWrapper
	{
		[DataMember(Name = "request")]
		public GetPersonaModernGroupMembershipJsonRequest Request { get; set; }
	}
}
