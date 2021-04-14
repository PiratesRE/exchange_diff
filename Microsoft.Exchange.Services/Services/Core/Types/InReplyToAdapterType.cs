using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "InReplyToAdapterType")]
	[Serializable]
	public class InReplyToAdapterType : RelatedItemInfoTypeBase
	{
		public InReplyToAdapterType(ItemId itemId, SingleRecipientType from, string preview) : base(itemId, from, preview)
		{
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string InternetMessageId { get; set; }

		public static InReplyToAdapterType FromRelatedItemInfo(IRelatedItemInfo itemInfo)
		{
			return new InReplyToAdapterType(itemInfo.ItemId, itemInfo.From, itemInfo.Preview);
		}
	}
}
