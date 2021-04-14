using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderBatch
	{
		public FolderBatch(byte[] folderId)
		{
			this.folderId = folderId;
			this.batchByteSize = 0UL;
			this.batch = new List<MessageRec>(5);
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public ulong BatchByteSize
		{
			get
			{
				return this.batchByteSize;
			}
			set
			{
				this.batchByteSize = value;
			}
		}

		public List<MessageRec> Batch
		{
			get
			{
				return this.batch;
			}
			set
			{
				this.batch = value;
			}
		}

		public DateTime HeadTimestamp
		{
			get
			{
				return this.batch[0].CreationTimestamp;
			}
		}

		public void AddMsg(MessageRec msgRec)
		{
			this.batch.Add(msgRec);
			this.batchByteSize += (ulong)((long)msgRec.MessageSize);
		}

		private byte[] folderId;

		private ulong batchByteSize;

		private List<MessageRec> batch;
	}
}
