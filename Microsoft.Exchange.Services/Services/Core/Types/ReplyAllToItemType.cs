using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "ReplyAllToItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ReplyAllToItemType : SmartResponseType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlIgnore]
		public bool? IsSpecificMessageReply
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsSpecificMessageReply);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsSpecificMessageReply, value);
				this.IsSpecificMessageReplyStamped = new bool?(true);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool? IsSpecificMessageReplyStamped
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(MessageSchema.IsSpecificMessageReplyStamped);
			}
			private set
			{
				base.PropertyBag.SetNullableValue<bool>(MessageSchema.IsSpecificMessageReplyStamped, value);
			}
		}
	}
}
