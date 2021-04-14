using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SyncAutoCompleteRecipientType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SyncAutoCompleteRecipientsRequest : SyncPersonaContactsRequestBase
	{
		[XmlElement("FullSyncOnly", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "FullSyncOnly", IsRequired = false)]
		public bool FullSyncOnly { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SyncAutoCompleteRecipients(callContext, this);
		}
	}
}
