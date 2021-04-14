using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateItemResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateItemResponseMessage : ItemInfoResponseMessage
	{
		public UpdateItemResponseMessage()
		{
		}

		internal UpdateItemResponseMessage(ServiceResultCode code, ServiceError error, ItemType item, ConflictResults conflictResults) : base(code, error, item)
		{
			this.ConflictResults = conflictResults;
		}

		[DataMember(EmitDefaultValue = false)]
		[XmlElement("ConflictResults", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ConflictResults ConflictResults { get; set; }
	}
}
