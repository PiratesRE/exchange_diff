using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "BreadcrumbAdapterType")]
	[Serializable]
	public class BreadcrumbAdapterType : RelatedItemInfoTypeBase
	{
		public BreadcrumbAdapterType(ItemId itemId, SingleRecipientType from, string preview, ItemId conversationId, string itemClass, bool isNewTimeProposal) : base(itemId, from, preview)
		{
			this.LinkedConversationId = conversationId;
			this.ItemClass = itemClass;
			this.IsNewTimeProposal = isNewTimeProposal;
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public ItemId LinkedConversationId { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string ItemClass { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public bool IsNewTimeProposal { get; set; }

		public static BreadcrumbAdapterType FromRelatedItemInfo(IRelatedItemInfo itemInfo)
		{
			return new BreadcrumbAdapterType(itemInfo.ItemId, itemInfo.From, itemInfo.Preview, itemInfo.ConversationId, itemInfo.ItemClass, BreadcrumbAdapterType.IsItemANewTimeProposal(itemInfo));
		}

		private static bool IsItemANewTimeProposal(IRelatedItemInfo itemInfo)
		{
			MeetingResponseMessageType meetingResponseMessageType = itemInfo as MeetingResponseMessageType;
			return meetingResponseMessageType != null && meetingResponseMessageType.IsNewTimeProposal;
		}
	}
}
