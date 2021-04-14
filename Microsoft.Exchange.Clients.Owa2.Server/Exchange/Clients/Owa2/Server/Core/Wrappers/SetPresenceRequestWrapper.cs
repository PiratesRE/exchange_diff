using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetPresenceRequestWrapper
	{
		[DataMember(Name = "presenceSetting")]
		public InstantMessagePresence PresenceSetting { get; set; }
	}
}
