using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(JsonFaultResponse))]
	public class ResendInformation
	{
		public ResendInformation()
		{
			this.ResendDraftId = string.Empty;
		}

		[DataMember]
		public string ResendDraftId { get; set; }

		[DataMember]
		public string[] HiddenRecipientsInTo { get; set; }
	}
}
