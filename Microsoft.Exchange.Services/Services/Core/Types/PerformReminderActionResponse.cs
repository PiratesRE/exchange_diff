using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PerformReminderActionResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class PerformReminderActionResponse : ResponseMessage
	{
		public PerformReminderActionResponse()
		{
		}

		internal PerformReminderActionResponse(ServiceResultCode code, ServiceError error, ItemId[] newItemIds) : base(code, error)
		{
			this.ItemIds = newItemIds;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlArrayItem("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ItemId))]
		[XmlArray("UpdatedItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ItemId[] ItemIds { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.PerformReminderActionResponseMessage;
		}
	}
}
