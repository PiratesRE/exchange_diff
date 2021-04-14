using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(UpdateCalendarEventAction))]
	[KnownType(typeof(CreateCalendarEventAction))]
	[KnownType(typeof(MoveAction))]
	[DataContract]
	[KnownType(typeof(MarkAsReadAction))]
	[KnownType(typeof(MarkAsUnReadAction))]
	[KnownType(typeof(DeleteAction))]
	[KnownType(typeof(SendAction))]
	[KnownType(typeof(FlagAction))]
	[KnownType(typeof(FlagClearAction))]
	[KnownType(typeof(FlagCompleteAction))]
	internal abstract class ReplayAction
	{
		public ReplayAction()
		{
		}

		[DataMember]
		public string Watermark { get; set; }

		[DataMember]
		public byte[] ItemId { get; set; }

		[DataMember]
		public byte[] FolderId { get; set; }

		internal abstract ActionId Id { get; }

		internal bool Ignored { get; set; }

		internal byte[] OriginalItemId { get; set; }

		internal byte[] OriginalFolderId { get; set; }

		protected ReplayAction(string watermark)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("watermark", watermark);
			this.Watermark = watermark;
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}

		internal virtual EntryIdMap<byte[]> GetMessageEntryIdsToTranslate()
		{
			return new EntryIdMap<byte[]>(1)
			{
				{
					this.FolderId,
					this.ItemId
				}
			};
		}

		internal virtual void TranslateEntryIds(IEntryIdTranslator translator)
		{
			this.TranslateFolderId(translator);
			this.TranslateEntryId(translator);
		}

		internal void TranslateEntryId(IEntryIdTranslator translator)
		{
			byte[] itemId = this.ItemId;
			byte[] sourceMessageIdFromTargetMessageId = translator.GetSourceMessageIdFromTargetMessageId(itemId);
			if (sourceMessageIdFromTargetMessageId == null)
			{
				MrsTracer.Common.Warning("Destination message {0} doesn't have mapped source message for action {1}", new object[]
				{
					TraceUtils.DumpEntryId(itemId),
					this
				});
				this.Ignored = true;
			}
			this.OriginalItemId = itemId;
			this.ItemId = sourceMessageIdFromTargetMessageId;
		}

		internal void TranslateFolderId(IEntryIdTranslator translator)
		{
			byte[] folderId = this.FolderId;
			byte[] sourceFolderIdFromTargetFolderId = translator.GetSourceFolderIdFromTargetFolderId(folderId);
			if (sourceFolderIdFromTargetFolderId == null)
			{
				MrsTracer.Common.Warning("Destination folder {0} doesn't have mapped source folder for action {1}", new object[]
				{
					TraceUtils.DumpEntryId(folderId),
					this
				});
				this.Ignored = true;
			}
			this.OriginalFolderId = folderId;
			this.FolderId = sourceFolderIdFromTargetFolderId;
		}
	}
}
