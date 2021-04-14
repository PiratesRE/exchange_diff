using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SyncFolderItemsReadFlagType : SyncFolderItemsChangeTypeBase
	{
		public SyncFolderItemsReadFlagType()
		{
		}

		public SyncFolderItemsReadFlagType(ItemId itemId, bool isRead)
		{
			this.ItemId = itemId;
			this.IsRead = isRead;
		}

		[DataMember(IsRequired = true)]
		public ItemId ItemId { get; set; }

		[DataMember(EmitDefaultValue = true, IsRequired = true)]
		public bool IsRead { get; set; }

		public override SyncFolderItemsChangesEnum ChangeType
		{
			get
			{
				return SyncFolderItemsChangesEnum.ReadFlagChange;
			}
		}
	}
}
