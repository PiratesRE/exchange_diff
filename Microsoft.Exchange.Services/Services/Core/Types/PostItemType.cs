using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PostItem")]
	[Serializable]
	public class PostItemType : ItemType, IRelatedItemInfo
	{
		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] ConversationIndex
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<byte[]>(PostItemSchema.ConversationIndex);
			}
			set
			{
				base.PropertyBag[PostItemSchema.ConversationIndex] = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ConversationIndex", EmitDefaultValue = false, Order = 1)]
		public string ConversationIndexString
		{
			get
			{
				byte[] conversationIndex = this.ConversationIndex;
				if (conversationIndex == null)
				{
					return null;
				}
				return Convert.ToBase64String(conversationIndex);
			}
			set
			{
				this.ConversationIndex = ((value != null) ? Convert.FromBase64String(value) : null);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string ConversationTopic
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PostItemSchema.ConversationTopic);
			}
			set
			{
				base.PropertyBag[PostItemSchema.ConversationTopic] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public SingleRecipientType From
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(PostItemSchema.From);
			}
			set
			{
				base.PropertyBag[PostItemSchema.From] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string InternetMessageId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PostItemSchema.InternetMessageId);
			}
			set
			{
				base.PropertyBag[PostItemSchema.InternetMessageId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool? IsRead
		{
			get
			{
				return new bool?(base.PropertyBag.GetValueOrDefault<bool>(PostItemSchema.IsRead));
			}
			set
			{
				base.PropertyBag[PostItemSchema.IsRead] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsReadSpecified
		{
			get
			{
				return base.IsSet(PostItemSchema.IsRead);
			}
			set
			{
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string PostedTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PostItemSchema.PostedTime);
			}
			set
			{
				base.PropertyBag[PostItemSchema.PostedTime] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool PostedTimeSpecified
		{
			get
			{
				return base.IsSet(PostItemSchema.PostedTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string References
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(PostItemSchema.References);
			}
			set
			{
				base.PropertyBag[PostItemSchema.References] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public SingleRecipientType Sender
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<SingleRecipientType>(PostItemSchema.Sender);
			}
			set
			{
				base.PropertyBag[PostItemSchema.Sender] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Post;
			}
		}
	}
}
