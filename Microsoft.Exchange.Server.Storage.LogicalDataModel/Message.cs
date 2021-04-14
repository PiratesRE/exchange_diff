using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class Message : Item
	{
		protected Message(Context context, Table table, PhysicalColumn sizeColumn, Mailbox mailbox, bool changeTrackingEnabled, bool newMessage, bool existsInDatabase, bool writeThrough, params ColumnValue[] initialValues) : base(context, table, sizeColumn, mailbox, changeTrackingEnabled, newMessage, existsInDatabase, writeThrough, initialValues)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (newMessage && !existsInDatabase)
				{
					this.SetProperty(context, PropTag.Message.HasAttach, false);
					this.SetProperty(context, PropTag.Message.MailFlags, 0);
					this.recipientList = new RecipientCollection(this);
				}
				disposeGuard.Success();
			}
		}

		protected Message(Context context, Table table, PhysicalColumn sizeColumn, Mailbox mailbox, bool changeTrackingEnabled, bool writeThrough, Reader reader) : base(context, table, sizeColumn, mailbox, changeTrackingEnabled, writeThrough, reader)
		{
		}

		public int AttachCount
		{
			get
			{
				if (base.Subobjects != null)
				{
					return base.Subobjects.ChildrenCount;
				}
				return 0;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return base.IsDirty || (this.recipientList != null && this.recipientList.Changed);
			}
		}

		protected override bool IsDirtyExceptDataRow
		{
			get
			{
				return base.IsDirtyExceptDataRow || (this.recipientList != null && this.recipientList.Changed);
			}
		}

		public abstract bool IsEmbedded { get; }

		protected override StorePropTag ItemSubobjectsBinPropTag
		{
			get
			{
				return PropTag.Message.ItemSubobjectsBin;
			}
		}

		public RecipientCollection GetRecipients(Context context)
		{
			if (this.recipientList == null)
			{
				byte[][] blob = (byte[][])this.GetPropertyValue(context, PropTag.Message.MessageRecipientsMVBin);
				this.recipientList = new RecipientCollection(this, blob);
			}
			return this.recipientList;
		}

		public Attachment OpenAttachment(Context context, int attachmentNumber)
		{
			if (base.Subobjects == null || !base.Subobjects.ContainsChild(attachmentNumber))
			{
				return null;
			}
			return (Attachment)this.OpenChild(context, attachmentNumber, base.Subobjects.GetChildInid(attachmentNumber).Value);
		}

		public Attachment CreateAttachment(Context context)
		{
			int attachNum = base.ReserveChildNumber();
			return Attachment.CreateAttachment(context, this, attachNum);
		}

		protected override Item OpenChild(Context context, int childNumber, long inid)
		{
			Item result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Attachment attachment = disposeGuard.Add<Attachment>(Attachment.OpenAttachment(context, this, childNumber, inid));
				if (base.Subobjects.GetChildSize(childNumber) == -1L)
				{
					base.Subobjects.SetChildSize(childNumber, attachment.OriginalSize);
				}
				disposeGuard.Success();
				result = attachment;
			}
			return result;
		}

		protected override Item CopyChild(Context context, int childNumber, long inid)
		{
			Item item = Attachment.CopyAttachment(context, this, childNumber, inid);
			if (base.Subobjects.GetChildSize(childNumber) == -1L)
			{
				base.Subobjects.SetChildSize(childNumber, item.OriginalSize);
			}
			return item;
		}

		public IEnumerable<int> GetAttachmentNumbers()
		{
			if (base.Subobjects != null)
			{
				return base.Subobjects.GetChildNumbers();
			}
			return Enumerable.Empty<int>();
		}

		public byte[] GetAttachmentsBlob()
		{
			if (base.Subobjects != null)
			{
				return base.Subobjects.SerializeChildren();
			}
			return null;
		}

		public bool GetIsRead(Context context)
		{
			return (bool)this.GetPropertyValue(context, PropTag.Message.Read);
		}

		public bool SetIsRead(Context context, bool value)
		{
			if (value != this.GetIsRead(context))
			{
				this.SetProperty(context, PropTag.Message.Read, value);
				return true;
			}
			return false;
		}

		public bool GetIsDeliveryCompleted(Context context)
		{
			return PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 1, 1);
		}

		public bool SetIsDeliveryCompleted(Context context, bool value)
		{
			return PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 1);
		}

		public bool GetNeedsReadNotification(Context context)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) && PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 34, 2);
		}

		public bool SetNeedsReadNotification(Context context, bool value)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) && PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 2);
		}

		public bool GetNeedsNotReadNotification(Context context)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) && PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 36, 4);
		}

		public bool SetNeedsNotReadNotification(Context context, bool value)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) && PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 4);
		}

		public bool GetOOFCanBeSent(Context context)
		{
			return PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 8, 8);
		}

		public bool SetOOFCanBeSent(Context context, bool value)
		{
			return PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 8);
		}

		public bool GetSentRepresentingAddedByTransport(Context context)
		{
			return PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 16, 16);
		}

		public bool SetSentRepresentingAddedByTransport(Context context, bool value)
		{
			return PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 16);
		}

		public bool GetReadReceiptSent(Context context)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage != null && topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) || PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Message.MailFlags, 32, 32);
		}

		public bool SetReadReceiptSent(Context context, bool value)
		{
			TopMessage topMessage = this as TopMessage;
			return (topMessage == null || !topMessage.ParentFolder.IsPerUserReadUnreadTrackingEnabled) && PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Message.MailFlags, value, 32);
		}

		public string GetMessageClass(Context context)
		{
			return (string)this.GetPropertyValue(context, PropTag.Message.MessageClass);
		}

		public void SetMessageClass(Context context, string value)
		{
			this.SetProperty(context, PropTag.Message.MessageClass, value);
		}

		public short? GetBodyType(Context context)
		{
			return (short?)this.GetPropertyValue(context, PropTag.Message.NativeBodyType);
		}

		public void SetBodyType(Context context, short? value)
		{
			this.SetProperty(context, PropTag.Message.NativeBodyType, value);
		}

		public Stream GetReadNativeBody(Context context)
		{
			Stream result;
			ErrorCode first = this.OpenPropertyReadStream(context, PropTag.Message.NativeBody, out result);
			if (!(first != ErrorCode.NoError))
			{
				return result;
			}
			return null;
		}

		public Stream GetWriteNativeBody(Context context)
		{
			Stream result;
			ErrorCode first = this.OpenPropertyWriteStream(context, PropTag.Message.NativeBody, out result);
			if (!(first != ErrorCode.NoError))
			{
				return result;
			}
			return null;
		}

		public bool AdjustMessageFlags(Context context, MessageFlags flagsToSet, MessageFlags flagsToClear)
		{
			return PropertyBagHelpers.AdjustPropertyFlags(context, this, PropTag.Message.MessageFlags, (int)flagsToSet, (int)flagsToClear);
		}

		public bool AdjustUncomputedMessageFlags(Context context, MessageFlags flagsToSet, MessageFlags flagsToClear)
		{
			return PropertyBagHelpers.AdjustPropertyFlags(context, this, PropTag.Message.MessageFlagsActual, (int)flagsToSet, (int)flagsToClear);
		}

		public string GetSubject(Context context)
		{
			return (string)this.GetPropertyValue(context, PropTag.Message.NormalizedSubject);
		}

		public void SetSubject(Context context, string value)
		{
			this.SetProperty(context, PropTag.Message.NormalizedSubject, value);
		}

		public void SetSubjectPrefix(Context context, string value)
		{
			this.SetProperty(context, PropTag.Message.SubjectPrefix, value);
		}

		public string GetDisplayNameTo(Context context)
		{
			this.RefreshDisplayNameTo(context);
			return (string)this.GetPropertyValue(context, PropTag.Message.DisplayTo);
		}

		public string GetDisplayNameCc(Context context)
		{
			this.RefreshDisplayNameCc(context);
			return (string)this.GetPropertyValue(context, PropTag.Message.DisplayCc);
		}

		public string GetDisplayNameBcc(Context context)
		{
			this.RefreshDisplayNameBcc(context);
			return (string)this.GetPropertyValue(context, PropTag.Message.DisplayBcc);
		}

		public override void Flush(Context context, bool flushLargeDirtyStreams)
		{
			if (this.IsDirty)
			{
				this.UpdateRecipients(context);
				base.Flush(context, flushLargeDirtyStreams);
			}
		}

		public override bool SaveChanges(Context context)
		{
			if (this.IsEmbedded)
			{
				this.SetProperty(context, PropTag.Message.IMAPId, null);
				this.SetProperty(context, PropTag.Message.InternetArticleNumber, null);
			}
			return base.SaveChanges(context);
		}

		internal override void SaveChild(Context context, ISubobject child)
		{
			base.SaveChild(context, child);
			this.SetProperty(context, PropTag.Message.HasAttach, 0 != this.AttachCount);
		}

		internal override void DeleteChild(Context context, ISubobject child)
		{
			base.DeleteChild(context, child);
			this.SetProperty(context, PropTag.Message.HasAttach, 0 != this.AttachCount);
		}

		public override void Scrub(Context context)
		{
			base.Scrub(context);
			this.SetProperty(context, PropTag.Message.HasAttach, false);
			this.recipientList = new RecipientCollection(this);
		}

		protected void UpdateRecipients(Context context)
		{
			if (this.recipientList != null && this.recipientList.Changed)
			{
				this.RefreshDisplayNameTo(context);
				this.RefreshDisplayNameCc(context);
				this.RefreshDisplayNameBcc(context);
				byte[][] value = this.recipientList.ToBinary(context);
				this.SetProperty(context, PropTag.Message.MessageRecipientsMVBin, value);
				this.recipientList.Changed = false;
			}
		}

		private void RefreshDisplayNameTo(Context context)
		{
			if (!this.IsComputedPropertyValid(PropTag.Message.DisplayTo))
			{
				this.ResetDisplayName(context, PropTag.Message.DisplayTo, RecipientType.To);
			}
		}

		private void RefreshDisplayNameCc(Context context)
		{
			if (!this.IsComputedPropertyValid(PropTag.Message.DisplayCc))
			{
				this.ResetDisplayName(context, PropTag.Message.DisplayCc, RecipientType.Cc);
			}
		}

		private void RefreshDisplayNameBcc(Context context)
		{
			if (!this.IsComputedPropertyValid(PropTag.Message.DisplayBcc))
			{
				this.ResetDisplayName(context, PropTag.Message.DisplayBcc, RecipientType.Bcc);
			}
		}

		private void ResetDisplayName(Context context, StorePropTag propTag, RecipientType rt)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this.GetRecipients(context))
			{
				Recipient recipient = (Recipient)obj;
				if (recipient.RecipientType == rt)
				{
					if (flag)
					{
						stringBuilder.Append("; ");
					}
					else
					{
						flag = true;
					}
					stringBuilder.Append(recipient.Name);
				}
			}
			this.SetProperty(context, propTag, stringBuilder.ToString());
			this.MarkComputedPropertyAsValid(propTag);
		}

		protected bool IsComputedPropertyValid(StorePropTag propTag)
		{
			return this.computedPropInvalid == null || !this.computedPropInvalid.Contains(propTag);
		}

		public void MarkComputedPropertyAsInvalid(StorePropTag propTag)
		{
			if (this.computedPropInvalid == null)
			{
				this.computedPropInvalid = new HashSet<StorePropTag>();
			}
			this.computedPropInvalid.Add(propTag);
		}

		protected void MarkComputedPropertyAsValid(StorePropTag propTag)
		{
			if (this.computedPropInvalid != null)
			{
				this.computedPropInvalid.Remove(propTag);
			}
		}

		public const MessageFlags ComputedMessageFlags = MessageFlags.Read | MessageFlags.HasAttachment | MessageFlags.Associated | MessageFlags.ReadNotificationPending | MessageFlags.NonReadNotificationPending | MessageFlags.EverRead;

		private RecipientCollection recipientList;

		private HashSet<StorePropTag> computedPropInvalid;
	}
}
