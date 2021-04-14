using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderItemsCreateOrUpdateType : SyncFolderItemsChangeTypeBase
	{
		public SyncFolderItemsCreateOrUpdateType()
		{
		}

		public SyncFolderItemsCreateOrUpdateType(ItemType item, bool isUpdate)
		{
			this.Item = item;
			this.isUpdate = isUpdate;
		}

		[XmlElement("Contact", typeof(ContactItemType))]
		[XmlElement("PostItem", typeof(PostItemType))]
		[XmlElement("Task", typeof(TaskType))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("Item", typeof(ItemType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("CalendarItem", typeof(EwsCalendarItemType))]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		[XmlElement("DistributionList", typeof(DistributionListType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		public ItemType Item { get; set; }

		public override SyncFolderItemsChangesEnum ChangeType
		{
			get
			{
				if (!this.isUpdate)
				{
					return SyncFolderItemsChangesEnum.Create;
				}
				return SyncFolderItemsChangesEnum.Update;
			}
		}

		private bool isUpdate;
	}
}
