using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ItemInfoResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ItemInfoResponseMessage : ResponseMessage
	{
		public ItemInfoResponseMessage()
		{
		}

		internal ItemInfoResponseMessage(ServiceResultCode code, ServiceError error, ItemType item) : base(code, error)
		{
			this.Items = new ArrayOfRealItemsType
			{
				Items = new ItemType[]
				{
					item
				}
			};
		}

		internal ItemInfoResponseMessage(ServiceResultCode code, ServiceError error, ItemType[] items) : base(code, error)
		{
			this.Items = new ArrayOfRealItemsType
			{
				Items = items
			};
		}

		[XmlElement("Items", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[IgnoreDataMember]
		public ArrayOfRealItemsType Items { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Items", IsRequired = true, Order = 1)]
		public ItemType[] ItemsArray
		{
			get
			{
				if (this.Items == null)
				{
					return null;
				}
				return this.Items.Items;
			}
			set
			{
				this.Items = new ArrayOfRealItemsType
				{
					Items = value
				};
			}
		}
	}
}
