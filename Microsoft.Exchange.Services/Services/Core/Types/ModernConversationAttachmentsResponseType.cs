using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ModernConversationAttachments")]
	public class ModernConversationAttachmentsResponseType
	{
		[DataMember(Name = "ConversationId", IsRequired = true)]
		public ItemId ConversationId { get; set; }

		public byte[] SyncState { get; set; }

		[DataMember(Name = "SyncState", EmitDefaultValue = false)]
		public string SyncStateString
		{
			get
			{
				byte[] syncState = this.SyncState;
				if (syncState == null)
				{
					return null;
				}
				return Convert.ToBase64String(syncState);
			}
			set
			{
				this.SyncState = (string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value));
			}
		}

		[DataMember(IsRequired = true)]
		public ItemType[] ItemsWithAttachments { get; set; }
	}
}
