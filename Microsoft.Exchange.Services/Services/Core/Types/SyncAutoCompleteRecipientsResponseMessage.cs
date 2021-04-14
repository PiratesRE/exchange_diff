using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncAutoCompleteRecipientsResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncAutoCompleteRecipientsResponseMessage : SyncPersonaContactsResponseBase
	{
		public SyncAutoCompleteRecipientsResponseMessage()
		{
		}

		internal SyncAutoCompleteRecipientsResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SyncAutoCompleteRecipientsResponseMessage;
		}

		[XmlArray(ElementName = "AutoCompleteRecipient")]
		[XmlArrayItem(ElementName = "AutoCompleteRecipient", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(EmitDefaultValue = false)]
		public AutoCompleteRecipient[] Recipients { get; set; }
	}
}
