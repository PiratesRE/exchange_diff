using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateOnlineMeetingRequestWrapper
	{
		[DataMember(Name = "sipUri")]
		public string SipUri { get; set; }

		[DataMember(Name = "itemId")]
		public ItemId ItemId { get; set; }
	}
}
