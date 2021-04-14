using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncPeopleResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncPeopleResponseMessage : SyncPersonaContactsResponseBase
	{
		public SyncPeopleResponseMessage()
		{
		}

		internal SyncPeopleResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SyncPeopleResponseMessage;
		}

		[DataMember(EmitDefaultValue = false)]
		[XmlArray(ElementName = "People")]
		[XmlArrayItem(ElementName = "Persona", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public Persona[] People { get; set; }
	}
}
