using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetHoldOnMailboxesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SetHoldOnMailboxesResponse : ResponseMessage
	{
		public SetHoldOnMailboxesResponse()
		{
		}

		internal SetHoldOnMailboxesResponse(ServiceResultCode code, ServiceError error, MailboxHoldResult mailboxHoldResult) : base(code, error)
		{
			this.mailboxHoldResult = mailboxHoldResult;
		}

		[DataMember(Name = "MailboxHoldResult", IsRequired = false)]
		[XmlElement("MailboxHoldResult")]
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
