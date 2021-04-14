using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(SmartResponseBaseType))]
	[KnownType(typeof(ApproveRequestItemType))]
	[KnownType(typeof(DeclineItemType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(ProposeNewTimeType))]
	[KnownType(typeof(ProposeNewTimeType))]
	[XmlInclude(typeof(PostReplyItemBaseType))]
	[XmlInclude(typeof(PostReplyItemType))]
	[XmlInclude(typeof(RemoveItemType))]
	[XmlInclude(typeof(ReferenceItemResponseType))]
	[XmlInclude(typeof(AcceptSharingInvitationType))]
	[XmlInclude(typeof(SuppressReadReceiptType))]
	[XmlInclude(typeof(SmartResponseType))]
	[XmlInclude(typeof(CancelCalendarItemType))]
	[XmlInclude(typeof(ForwardItemType))]
	[XmlInclude(typeof(ReplyAllToItemType))]
	[XmlInclude(typeof(ReplyToItemType))]
	[XmlInclude(typeof(WellKnownResponseObjectType))]
	[XmlInclude(typeof(DeclineItemType))]
	[XmlInclude(typeof(TentativelyAcceptItemType))]
	[XmlInclude(typeof(AcceptItemType))]
	[XmlInclude(typeof(AddItemToMyCalendarType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(PostReplyItemBaseType))]
	[KnownType(typeof(PostReplyItemType))]
	[KnownType(typeof(RemoveItemType))]
	[KnownType(typeof(ReferenceItemResponseType))]
	[KnownType(typeof(AcceptSharingInvitationType))]
	[KnownType(typeof(SuppressReadReceiptType))]
	[KnownType(typeof(SmartResponseBaseType))]
	[KnownType(typeof(SmartResponseType))]
	[KnownType(typeof(CancelCalendarItemType))]
	[KnownType(typeof(ForwardItemType))]
	[KnownType(typeof(ReplyAllToItemType))]
	[KnownType(typeof(ReplyToItemType))]
	[KnownType(typeof(WellKnownResponseObjectType))]
	[KnownType(typeof(TentativelyAcceptItemType))]
	[KnownType(typeof(AcceptItemType))]
	[KnownType(typeof(RejectRequestItemType))]
	[KnownType(typeof(VotingResponseItemType))]
	[KnownType(typeof(AddItemToMyCalendarType))]
	[Serializable]
	public class ResponseObjectType : ResponseObjectCoreType
	{
		[DataMember(EmitDefaultValue = false)]
		[XmlAttribute]
		public string ObjectName { get; set; }
	}
}
