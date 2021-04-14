using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetHoldOnMailboxesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetHoldOnMailboxesResponse : ResponseMessage
	{
		public GetHoldOnMailboxesResponse()
		{
		}

		internal GetHoldOnMailboxesResponse(ServiceResultCode code, ServiceError error, MailboxHoldResult mailboxHoldResult) : base(code, error)
		{
			this.mailboxHoldResult = mailboxHoldResult;
		}

		[XmlElement("MailboxHoldResult")]
		[DataMember(Name = "MailboxHoldResult", IsRequired = false)]
		public MailboxHoldResult MailboxHoldResult
		{
			get
			{
				return this.mailboxHoldResult;
			}
			set
			{
				this.mailboxHoldResult = value;
			}
		}

		private MailboxHoldResult mailboxHoldResult;
	}
}
