using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class PerUserTableIterator : DisposableBase, IMessageIterator, IMessageIteratorClient, IDisposable
	{
		public PerUserTableIterator(FastTransferDownloadContext downloadContext, ExchangeId folderId)
		{
			if (downloadContext == null)
			{
				throw new ArgumentNullException("downloadContext");
			}
			this.context = downloadContext;
			this.readOnly = true;
			this.folderId = folderId;
		}

		public PerUserTableIterator(FastTransferUploadContext uploadContext, ExchangeId folderId)
		{
			if (uploadContext == null)
			{
				throw new ArgumentNullException("uploadContext");
			}
			this.context = uploadContext;
			this.readOnly = false;
			this.folderId = folderId;
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		public FastTransferContext Context
		{
			get
			{
				return this.context;
			}
		}

		public IEnumerator<IMessage> GetMessages()
		{
			if (this.folderId.IsZero)
			{
				foreach (PerUser perUserObject in PerUser.ResidentEntries(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox))
				{
					IMessage result;
					using (LockManager.Lock(perUserObject, LockManager.LockType.PerUserShared))
					{
						result = new PerUserTableIterator.Record(this.Context, perUserObject);
					}
					yield return result;
				}
			}
			else
			{
				foreach (PerUser perUserObject2 in PerUser.ResidentEntriesForFolder(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox, this.folderId))
				{
					IMessage result2;
					using (LockManager.Lock(perUserObject2, LockManager.LockType.PerUserShared))
					{
						result2 = new PerUserTableIterator.Record(this.Context, perUserObject2);
					}
					yield return result2;
				}
			}
			yield break;
		}

		public IMessage UploadMessage(bool isAssociatedMessage)
		{
			if (!this.oldEntriesDeleted)
			{
				if (this.folderId.IsZero)
				{
					PerUser.DeleteAllResidentEntries(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox);
				}
				else
				{
					Folder folder = Folder.OpenFolder(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox, this.folderId);
					PerUser.DeleteAllResidentEntriesForFolder(this.context.CurrentOperationContext, folder);
				}
				this.oldEntriesDeleted = true;
			}
			return new PerUserTableIterator.Record(this.context, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PerUserTableIterator>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
		}

		private readonly bool readOnly;

		private ExchangeId folderId = ExchangeId.Zero;

		private FastTransferContext context;

		private bool oldEntriesDeleted;

		private sealed class MyMemoryPropertyBag : MemoryPropertyBag
		{
			public MyMemoryPropertyBag(ISession session) : base(session)
			{
			}

			public override AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
			{
				if (propertyTag == PropertyTag.InstanceIdBin)
				{
					return base.GetAnnotatedProperty(PropertyTag.LongTermId);
				}
				return base.GetAnnotatedProperty(propertyTag);
			}
		}

		private class Record : IMessage, IDisposable
		{
			public Record(FastTransferContext context, PerUser perUserObject)
			{
				this.context = context;
				this.propertyBag = new PerUserTableIterator.MyMemoryPropertyBag(context);
				if (perUserObject != null)
				{
					this.propertyBag.SetProperty(new PropertyValue(PerUser.MailboxGuidPropertyTag, perUserObject.Guid));
					this.propertyBag.SetProperty(new PropertyValue(PerUser.FolderIdPropertyTag, perUserObject.FolderId.To22ByteArray()));
					this.propertyBag.SetProperty(new PropertyValue(PerUser.CNSetPropertyTag, perUserObject.CNSetBytes));
					this.propertyBag.SetProperty(new PropertyValue(PerUser.LastModPropertyTag, (ExDateTime)perUserObject.LastModificationTime));
					this.propertyBag.SetProperty(new PropertyValue(PerUser.TypeTag, 0));
					this.propertyBag.SetProperty(new PropertyValue(PropertyTag.LongTermId, perUserObject.FolderId.To24ByteArray()));
					this.propertyBag.SetProperty(new PropertyValue(PropertyTag.InstanceIdBin, perUserObject.FolderId.To24ByteArray()));
				}
			}

			public IPropertyBag PropertyBag
			{
				get
				{
					return this.propertyBag;
				}
			}

			public bool IsAssociated
			{
				get
				{
					return false;
				}
			}

			public IEnumerable<IRecipient> GetRecipients()
			{
				yield break;
			}

			public IRecipient CreateRecipient()
			{
				throw new ExExceptionNoSupport((LID)49856U, "Recipients are not supported on the per user records");
			}

			public void RemoveRecipient(int rowId)
			{
				throw new ExExceptionNoSupport((LID)42460U, "Recipient removal is not supported on the per user records");
			}

			public IEnumerable<IAttachmentHandle> GetAttachments()
			{
				yield break;
			}

			public IAttachment CreateAttachment()
			{
				throw new ExExceptionNoSupport((LID)48384U, "Attachments are not supported on the per user records");
			}

			public void Save()
			{
				PropertyValue propertyValue = this.PropertyBag.GetAnnotatedProperty(PerUser.MailboxGuidPropertyTag).PropertyValue;
				Guid mailboxGuid = Guid.Empty;
				if (propertyValue.IsError || propertyValue.IsNotFound)
				{
					throw new ExExceptionCorruptData((LID)64768U, "Mailbox Guid is missing");
				}
				mailboxGuid = propertyValue.GetValue<Guid>();
				propertyValue = this.PropertyBag.GetAnnotatedProperty(PerUser.FolderIdPropertyTag).PropertyValue;
				ExchangeId folderId = ExchangeId.Zero;
				if (propertyValue.IsError || propertyValue.IsNotFound)
				{
					throw new ExExceptionCorruptData((LID)56576U, "FolderId is missing");
				}
				byte[] value = propertyValue.GetValue<byte[]>();
				if (value == null || value.Length <= 0)
				{
					throw new ExExceptionCorruptData((LID)40192U, "FolderId is corrupted");
				}
				folderId = ExchangeId.CreateFrom22ByteArray(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox.ReplidGuidMap, value);
				propertyValue = this.PropertyBag.GetAnnotatedProperty(PerUser.CNSetPropertyTag).PropertyValue;
				if (propertyValue.IsError || propertyValue.IsNotFound)
				{
					throw new ExExceptionCorruptData((LID)60672U, "CNSet is missing");
				}
				byte[] value2 = propertyValue.GetValue<byte[]>();
				if (value2 == null || value2.Length <= 0)
				{
					throw new ExExceptionCorruptData((LID)44288U, "CNSet is corrupted");
				}
				IdSet cnSet = IdSet.Parse(this.context.CurrentOperationContext, value2);
				propertyValue = this.PropertyBag.GetAnnotatedProperty(PerUser.TypeTag).PropertyValue;
				if (propertyValue.IsError || propertyValue.IsNotFound)
				{
					throw new ExExceptionCorruptData((LID)36096U, "Type is missing");
				}
				int value3 = propertyValue.GetValue<int>();
				if (value3 != 0)
				{
					throw new ExExceptionNoSupport((LID)52480U, "Foreign PerUser entry is not supported");
				}
				propertyValue = this.PropertyBag.GetAnnotatedProperty(PerUser.LastModPropertyTag).PropertyValue;
				ExDateTime? value4 = new ExDateTime?(ExDateTime.MinValue);
				if (!propertyValue.IsError && !propertyValue.IsNotFound)
				{
					value4 = propertyValue.GetValue<ExDateTime?>();
				}
				PerUser.CreateResidentAndSave(this.context.CurrentOperationContext, this.context.Logon.StoreMailbox, mailboxGuid, folderId, cnSet, (DateTime?)value4);
			}

			public void SetLongTermId(StoreLongTermId longTermId)
			{
			}

			public void Dispose()
			{
			}

			private FastTransferContext context;

			private PerUserTableIterator.MyMemoryPropertyBag propertyBag;
		}
	}
}
