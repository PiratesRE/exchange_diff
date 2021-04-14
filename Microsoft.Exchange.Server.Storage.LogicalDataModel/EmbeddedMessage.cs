using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class EmbeddedMessage : Message, ISubobject
	{
		private EmbeddedMessage(Context context, AttachmentTable attachmentTable, Mailbox mailbox, object inid) : base(context, attachmentTable.Table, attachmentTable.Size, mailbox, false, inid == null, inid != null, !ConfigurationSchema.AttachmentMessageSaveChunking.Value, (inid == null) ? new ColumnValue[]
		{
			new ColumnValue(attachmentTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber)
		} : new ColumnValue[]
		{
			new ColumnValue(attachmentTable.MailboxPartitionNumber, mailbox.MailboxPartitionNumber),
			new ColumnValue(attachmentTable.Inid, inid)
		})
		{
			this.attachmentTable = attachmentTable;
		}

		private EmbeddedMessage(Context context, Attachment parent, long? inid, long? copyOriginalInid) : this(context, DatabaseSchema.AttachmentTable(parent.Mailbox.Database), parent.Mailbox, inid)
		{
			this.parentAttachment = parent;
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
				base.SetColumn(context, this.attachmentTable.IsEmbeddedMessage, true);
				PropertySchemaPopulation.InitializeEmbeddedMessage(context, this);
				if (parent.Mailbox.SharedState.UnifiedState != null)
				{
					int num = (int)parent.GetPropertyValue(context, PropTag.Attachment.MailboxNum);
					if (num != parent.Mailbox.MailboxNumber)
					{
						throw new StoreException((LID)64220U, ErrorCodeValue.NotSupported);
					}
					if (num != parent.Mailbox.MailboxPartitionNumber)
					{
						base.SetColumn(context, this.attachmentTable.MailboxNumber, num);
					}
				}
			}
			if (context.PerfInstance != null)
			{
				context.PerfInstance.SubobjectsOpenedRate.Increment();
			}
		}

		public AttachmentTable AttachmentTable
		{
			get
			{
				return this.attachmentTable;
			}
		}

		public Attachment ParentAttachment
		{
			get
			{
				return this.parentAttachment;
			}
		}

		public override bool IsEmbedded
		{
			get
			{
				return true;
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

		int ISubobject.ChildNumber
		{
			get
			{
				return 0;
			}
		}

		internal static EmbeddedMessage CopyEmbeddedMessage(Context context, Attachment parent, long sourceInid)
		{
			long value = ObjectCopyOnWrite.CopyAttachment(context, parent.Mailbox, sourceInid);
			EmbeddedMessage embeddedMessage = new EmbeddedMessage(context, parent, new long?(value), new long?(sourceInid));
			embeddedMessage.DeepCopySubobjects(context);
			return embeddedMessage;
		}

		internal static EmbeddedMessage CreateEmbeddedMessage(Context context, Attachment parent)
		{
			return new EmbeddedMessage(context, parent, null, null);
		}

		internal static EmbeddedMessage OpenEmbeddedMessage(Context context, Attachment parent, long inid)
		{
			return new EmbeddedMessage(context, parent, new long?(inid), null);
		}

		public long? GetInid(Context context)
		{
			return (long?)base.GetColumnValue(context, this.attachmentTable.Inid);
		}

		public override bool SaveChanges(Context context)
		{
			if (this.IsDirty || this.originalInid == null || this.originalInid != this.currentInid)
			{
				if (!this.DataRow.ColumnDirty(this.attachmentTable.LastModificationTime) || base.GetColumnValue(context, this.attachmentTable.LastModificationTime) == null)
				{
					base.SetColumn(context, this.attachmentTable.LastModificationTime, base.Mailbox.UtcNow);
				}
				base.SaveChanges(context);
				this.parentAttachment.SaveChild(context, this);
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
				this.parentAttachment.DeleteChild(context, this);
			}
			base.ReleaseDescendants(context, false);
			if (this.currentInid != null)
			{
				base.SubobjectReferenceState.Release(context, this.currentInid.Value, base.Mailbox);
				this.currentInid = null;
			}
			base.MarkAsDeleted(context);
		}

		public override void Scrub(Context context)
		{
			base.Scrub(context);
			base.SetColumn(context, this.attachmentTable.Content, null);
		}

		protected override IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(context.Database);
			Dictionary<Column, Column> dictionary = new Dictionary<Column, Column>(1);
			dictionary[messageTable.VirtualIsRead] = messageTable.IsRead;
			return dictionary;
		}

		protected override void CopyOnWrite(Context context)
		{
			if (this.originalInid != null && this.originalInid == this.currentInid)
			{
				this.currentInid = new long?(ObjectCopyOnWrite.CopyAttachment(context, base.Mailbox, this.originalInid.Value));
				base.SubobjectReferenceState.Addref(this.currentInid.Value);
				base.SubobjectReferenceState.Release(context, this.originalInid.Value, base.Mailbox);
				SubobjectCleanup.AddTombstone(context, this, this.currentInid.Value, 0L);
				base.SetPrimaryKey(new ColumnValue[]
				{
					new ColumnValue(this.attachmentTable.MailboxPartitionNumber, base.Mailbox.MailboxPartitionNumber),
					new ColumnValue(this.attachmentTable.Inid, this.currentInid)
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

		protected override ObjectType GetObjectType()
		{
			return ObjectType.EmbeddedMessage;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EmbeddedMessage>(this);
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

		private Attachment parentAttachment;

		private long? currentInid;

		private long? originalInid;

		private AttachmentTable attachmentTable;
	}
}
