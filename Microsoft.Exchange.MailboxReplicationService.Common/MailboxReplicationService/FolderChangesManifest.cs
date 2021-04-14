using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class FolderChangesManifest
	{
		public FolderChangesManifest()
		{
		}

		[DataMember(Name = "folderId", IsRequired = true)]
		public byte[] FolderId { get; set; }

		[DataMember(Name = "changedMessages", EmitDefaultValue = false)]
		public List<MessageRec> ChangedMessages { get; set; }

		[DataMember(Name = "readMessages", EmitDefaultValue = false)]
		public List<byte[]> ReadMessages { get; set; }

		[DataMember(Name = "unreadMessages", EmitDefaultValue = false)]
		public List<byte[]> UnreadMessages { get; set; }

		[DataMember(Name = "folderRecoverySync", EmitDefaultValue = false)]
		public bool FolderRecoverySync { get; set; }

		[DataMember(Name = "hasMoreChanges", EmitDefaultValue = false)]
		public bool HasMoreChanges { get; set; }

		public FolderChangesManifest(byte[] folderId)
		{
			this.FolderId = folderId;
			this.ChangedMessages = null;
			this.ReadMessages = null;
			this.UnreadMessages = null;
			this.FolderRecoverySync = false;
			this.HasMoreChanges = false;
		}

		public int EntryCount
		{
			get
			{
				int num = 0;
				if (this.ChangedMessages != null)
				{
					num += this.ChangedMessages.Count;
				}
				if (this.ReadMessages != null)
				{
					num += this.ReadMessages.Count;
				}
				if (this.UnreadMessages != null)
				{
					num += this.UnreadMessages.Count;
				}
				return num;
			}
		}

		public bool HasChanges
		{
			get
			{
				return this.EntryCount > 0 || this.FolderRecoverySync;
			}
		}

		public void GetMessageCounts(out int newMessages, out int updated, out int deleted)
		{
			newMessages = 0;
			updated = 0;
			deleted = 0;
			if (this.ChangedMessages != null)
			{
				foreach (MessageRec messageRec in this.ChangedMessages)
				{
					if (messageRec.IsDeleted)
					{
						deleted++;
					}
					else if (messageRec.IsNew)
					{
						newMessages++;
					}
					else
					{
						updated++;
					}
				}
			}
		}

		public override string ToString()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			if (this.ChangedMessages != null)
			{
				foreach (MessageRec messageRec in this.ChangedMessages)
				{
					if (messageRec.IsDeleted)
					{
						if (messageRec.IsFAI)
						{
							num6++;
						}
						else
						{
							num5++;
						}
					}
					else if (messageRec.IsNew)
					{
						if (messageRec.IsFAI)
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
					else if (messageRec.IsFAI)
					{
						num4++;
					}
					else
					{
						num3++;
					}
				}
			}
			return string.Format("{0} new; {1} new FAI; {2} changed; {3} changed FAI; {4} deleted; {5} deleted FAI; {6} read; {7} unread; more changes to be enumerated: {8}", new object[]
			{
				num,
				num2,
				num3,
				num4,
				num5,
				num6,
				(this.ReadMessages != null) ? this.ReadMessages.Count : 0,
				(this.UnreadMessages != null) ? this.UnreadMessages.Count : 0,
				this.HasMoreChanges
			});
		}
	}
}
