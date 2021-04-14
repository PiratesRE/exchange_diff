using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class Attachment : Item, ISubobject
	{
		private Attachment(Context context, AttachmentTable attachmentTable, Message parent, object inid) : base(context, attachmentTable.Table, attachmentTable.Size, parent.Mailbox, false, inid == null, inid != null, !ConfigurationSchema.AttachmentMessageSaveChunking.Value, (inid == null) ? new ColumnValue[]
		{
			new ColumnValue(attachmentTable.MailboxPartitionNumber, parent.Mailbox.MailboxPartitionNumber)
		} : new ColumnValue[]
		{
			new ColumnValue(attachmentTable.MailboxPartitionNumber, parent.Mailbox.MailboxPartitionNumber),
			new ColumnValue(attachmentTable.Inid, inid)
		})
		{
			this.attachmentTable = attachmentTable;
		}

		private Attachment(Context context, Message parent, int attachNum, long? inid, long? copyOriginalInid) : this(context, DatabaseSchema.AttachmentTable(parent.Mailbox.Database), parent, inid)
		{
			this.parentMessage = parent;
			this.attachmentNumber = attachNum;
			if (inid != null)
			{
				this.currentInid = inid;
				this.originalInid = ((copyOriginalInid != null) ? copyOriginalInid : inid);
				base.SubobjectReferenceState.Addref(this.currentInid.Value);
				if (copyOriginalInid != null)
				{
					SubobjectCleanup.AddTombstone(context, parent, this.currentInid.Value, 0L);
				}
			}
			else
			{
				PropertySchemaPopulation.InitializeAttachment(context, this);
				base.SetColumn(context, this.attachmentTable.MailboxPartitionNumber, parent.Mailbox.MailboxPartitionNumber);
				if (parent.Mailbox.SharedState.UnifiedState != null)
				{
					int num = (int)parent.GetPropertyValue(context, PropTag.Message.MailboxNum);
					if (num != parent.Mailbox.MailboxNumber)
					{
						throw new StoreException((LID)59932U, ErrorCodeValue.NotSupported);
					}
					if (num != parent.Mailbox.MailboxPartitionNumber)
					{
						base.SetColumn(context, this.attachmentTable.MailboxNumber, num);
					}
				}
				DateTime utcNow = parent.Mailbox.UtcNow;
				base.SetColumn(context, this.attachmentTable.CreationTime, utcNow);
				base.SetColumn(context, this.attachmentTable.LastModificationTime, utcNow);
				base.SetColumn(context, this.attachmentTable.RenderingPosition, -1);
				base.SetColumn(context, this.attachmentTable.AttachmentMethod, 0);
				base.SetColumn(context, this.attachmentTable.MailFlags, 0);
			}
			if (context.PerfInstance != null)
			{
				context.PerfInstance.SubobjectsOpenedRate.Increment();
			}
		}

		public Message ParentMessage
		{
			get
			{
				return this.parentMessage;
			}
		}

		public long? CurrentInid
		{
			get
			{
				return this.currentInid;
			}
		}

		public long? OriginalInid
		{
			get
			{
				return this.originalInid;
			}
		}

		public int AttachmentNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		int ISubobject.ChildNumber
		{
			get
			{
				return this.attachmentNumber;
			}
		}

		protected override StorePropTag ItemSubobjectsBinPropTag
		{
			get
			{
				return PropTag.Attachment.ItemSubobjectsBin;
			}
		}

		internal static Attachment CopyAttachment(Context context, Message parent, int attachNum, long sourceInid)
		{
			long value = ObjectCopyOnWrite.CopyAttachment(context, parent.Mailbox, sourceInid);
			Attachment attachment = new Attachment(context, parent, attachNum, new long?(value), new long?(sourceInid));
			attachment.DeepCopySubobjects(context);
			return attachment;
		}

		internal static Attachment CreateAttachment(Context context, Message parent, int attachNum)
		{
			return new Attachment(context, parent, attachNum, null, null);
		}

		internal static Attachment OpenAttachment(Context context, Message parent, int attachNum, long inid)
		{
			return new Attachment(context, parent, attachNum, new long?(inid), null);
		}

		private long? GetInid(Context context)
		{
			return (long?)base.GetColumnValue(context, this.attachmentTable.Inid);
		}

		public EmbeddedMessage OpenEmbeddedMessage(Context context)
		{
			if (base.Subobjects == null || !base.Subobjects.ContainsChild(0))
			{
				return null;
			}
			return (EmbeddedMessage)this.OpenChild(context, 0, base.Subobjects.GetChildInid(0).Value);
		}

		public EmbeddedMessage CreateEmbeddedMessage(Context context)
		{
			if (base.Subobjects == null)
			{
				base.ReserveChildNumber();
			}
			return EmbeddedMessage.CreateEmbeddedMessage(context, this);
		}

		protected override Item OpenChild(Context context, int childNumber, long inid)
		{
			Item result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				EmbeddedMessage embeddedMessage = disposeGuard.Add<EmbeddedMessage>(EmbeddedMessage.OpenEmbeddedMessage(context, this, inid));
				if (base.Subobjects.GetChildSize(0) == -1L)
				{
					base.Subobjects.SetChildSize(0, embeddedMessage.OriginalSize);
				}
				disposeGuard.Success();
				result = embeddedMessage;
			}
			return result;
		}

		protected override Item CopyChild(Context context, int childNumber, long inid)
		{
			EmbeddedMessage embeddedMessage = EmbeddedMessage.CopyEmbeddedMessage(context, this, inid);
			if (base.Subobjects.GetChildSize(childNumber) == -1L)
			{
				base.Subobjects.SetChildSize(childNumber, embeddedMessage.OriginalSize);
			}
			return embeddedMessage;
		}

		public override bool SaveChanges(Context context)
		{
			if (this.IsDirty || this.originalInid == null || this.originalInid != this.currentInid)
			{
				if (!this.DataRow.ColumnDirty(this.attachmentTable.LastModificationTime) || base.GetColumnValue(context, this.attachmentTable.LastModificationTime) == null)
				{
					base.SetColumn(context, this.attachmentTable.LastModificationTime, base.Mailbox.UtcNow);
				}
				if (this.originalInid == null)
				{
					ExchangeId nextObjectId = base.Mailbox.GetNextObjectId(context);
					base.SetColumn(context, this.attachmentTable.AttachmentId, nextObjectId.To26ByteArray());
				}
				base.SaveChanges(context);
				this.parentMessage.SaveChild(context, this);
				this.originalInid = this.currentInid;
				if (base.Subobjects != null)
				{
					base.Subobjects.ClearDeleted(context);
				}
			}
			return true;
		}

		public override void Delete(Context context)
		{
			if (base.IsDead)
			{
				return;
			}
			if (this.originalInid != null)
			{
				this.parentMessage.DeleteChild(context, this);
			}
			base.ReleaseDescendants(context, false);
			if (this.currentInid != null)
			{
				base.SubobjectReferenceState.Release(context, this.currentInid.Value, base.Mailbox);
				this.currentInid = null;
			}
			base.MarkAsDeleted(context);
		}

		public bool IsEmbeddedMessage(Context context)
		{
			return base.Subobjects != null && base.Subobjects.ContainsChild(0);
		}

		protected override void CopyOnWrite(Context context)
		{
			if (this.originalInid != null && this.originalInid == this.currentInid)
			{
				this.currentInid = new long?(ObjectCopyOnWrite.CopyAttachment(context, base.Mailbox, this.originalInid.Value));
				base.SubobjectReferenceState.Addref(this.currentInid.Value);
				base.SubobjectReferenceState.Release(context, this.originalInid.Value, base.Mailbox);
				SubobjectCleanup.AddTombstone(context, this, this.currentInid.Value, base.CurrentSizeEstimateWithoutChildren);
				base.SetPrimaryKey(new ColumnValue[]
				{
					new ColumnValue(this.attachmentTable.MailboxPartitionNumber, base.Mailbox.MailboxPartitionNumber),
					new ColumnValue(this.attachmentTable.Inid, this.currentInid.Value)
				});
				if (context.PerfInstance != null)
				{
					context.PerfInstance.SubobjectsCreatedRate.Increment();
				}
			}
		}

		public override void Flush(Context context, bool flushLargeDirtyStreams)
		{
			base.Flush(context, flushLargeDirtyStreams);
			if (this.currentInid == null)
			{
				this.currentInid = this.GetInid(context);
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail((this.currentInid.Value & 140737488355328L) == 0L, "Negative Inid - unexpected. This assumtion comes from SubobjectCleanup logic");
				base.SubobjectReferenceState.Addref(this.currentInid.Value);
				SubobjectCleanup.AddTombstone(context, this, this.currentInid.Value, base.CurrentSizeEstimateWithoutChildren);
				if (context.PerfInstance != null)
				{
					context.PerfInstance.SubobjectsCreatedRate.Increment();
					return;
				}
			}
			else if (base.SubobjectReferenceState.IsInTombstone(this.currentInid.Value))
			{
				SubobjectCleanup.UpdateTombstonedSize(context, this, this.currentInid.Value, base.CurrentSizeEstimateWithoutChildren);
			}
		}

		public ExchangeId GetId(Context context)
		{
			return ExchangeId.CreateFrom26ByteArray(context, base.Mailbox.ReplidGuidMap, (byte[])base.GetColumnValue(context, this.attachmentTable.AttachmentId));
		}

		public DateTime GetCreationTime(Context context)
		{
			return (DateTime)this.GetPropertyValue(context, PropTag.Attachment.CreationTime);
		}

		public DateTime GetLastModificationTime(Context context)
		{
			return (DateTime)this.GetPropertyValue(context, PropTag.Attachment.LastModificationTime);
		}

		internal void SetLastModificationTime(Context context, DateTime value)
		{
			this.SetProperty(context, PropTag.Attachment.LastModificationTime, value);
		}

		public object GetContent(Context context)
		{
			return this.GetPropertyValue(context, PropTag.Attachment.Content);
		}

		public void SetContent(Context context, byte[] value)
		{
			this.SetProperty(context, PropTag.Attachment.Content, value);
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.Attachment;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Attachment>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (this.currentInid != null)
			{
				base.SubobjectReferenceState.Release(calledFromDispose ? base.Mailbox.CurrentOperationContext : null, this.currentInid.Value, calledFromDispose ? base.Mailbox : null);
				this.currentInid = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private Message parentMessage;

		private int attachmentNumber;

		private long? currentInid;

		private long? originalInid;

		private AttachmentTable attachmentTable;
	}
}
