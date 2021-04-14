using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal class AttachmentDataProvider : StorageDataProvider<IStoreSession, IAttachment, string>
	{
		public AttachmentDataProvider(IStorageEntitySetScope<IStoreSession> scope, StoreId parentItemId) : base(scope, ExTraceGlobals.AttachmentDataProviderTracer)
		{
			this.parentItemId = parentItemId;
		}

		public override IAttachment Create(IAttachment entity)
		{
			return this.Create(entity, null);
		}

		public IAttachment Create(IAttachment entity, CommandContext commandContext)
		{
			IAttachment result;
			using (IItem item = this.BindToParentItem())
			{
				item.OpenAsReadWrite();
				AttachmentType attachmentType;
				if (entity is ItemIdAttachment)
				{
					attachmentType = AttachmentType.EmbeddedMessage;
				}
				else if (entity is ReferenceAttachment)
				{
					attachmentType = AttachmentType.Reference;
				}
				else
				{
					attachmentType = AttachmentType.Stream;
				}
				using (IAttachment attachment = this.CreateAttachment(item, entity, attachmentType))
				{
					attachment.Save();
					attachment.Load();
					this.SaveParentItem(item, commandContext);
					item.Load();
					StorageTranslator<IAttachment, IAttachment> attachmentTranslator = this.GetAttachmentTranslator(attachment.AttachmentType, true);
					IAttachment attachment2 = attachmentTranslator.ConvertToEntity(attachment);
					result = attachment2;
				}
			}
			return result;
		}

		public override void Delete(string attachmentId, DeleteItemFlags flags)
		{
			this.Delete(attachmentId, null);
		}

		public void Delete(string attachmentId, CommandContext commandContext = null)
		{
			IList<AttachmentId> attachmentIds = IdConverter.GetAttachmentIds(attachmentId);
			if (attachmentIds.Count > 1)
			{
				throw new InvalidRequestException(Strings.ErrorNestedAttachmentsCannotBeRemoved);
			}
			using (IItem item = this.BindToParentItem())
			{
				item.OpenAsReadWrite();
				this.GetAttachmentCollection(item).Remove(attachmentIds[0]);
				this.SaveParentItem(item, commandContext);
			}
		}

		public IEnumerable<IAttachment> GetAllAttachments()
		{
			using (IItem parentItem = this.BindToParentItem())
			{
				AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(parentItem);
				foreach (AttachmentHandle attachmentHandle in attachmentCollection.GetHandles())
				{
					using (Attachment attachment = attachmentCollection.Open(attachmentHandle))
					{
						StorageTranslator<IAttachment, IAttachment> translator = this.GetAttachmentTranslator(attachment.AttachmentType, false);
						IAttachment resultAttachment = translator.ConvertToEntity(attachment);
						yield return resultAttachment;
					}
				}
			}
			yield break;
		}

		public override IAttachment Read(string attachmentId)
		{
			IList<AttachmentId> attachmentIds = IdConverter.GetAttachmentIds(attachmentId);
			IAttachment result;
			using (IItem item = this.BindToParentItem())
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					IItem item2 = item;
					Attachment attachment = null;
					for (int i = 0; i < attachmentIds.Count; i++)
					{
						attachment = IrmUtils.GetAttachmentCollection(item2).Open(attachmentIds[i]);
						disposeGuard.Add<Attachment>(attachment);
						if (i < attachmentIds.Count - 1)
						{
							if (!(attachment is ItemAttachment))
							{
								throw new CorruptDataException(Strings.ErrorAllButLastNestedAttachmentMustBeItemAttachment);
							}
							ItemAttachment itemAttachment = attachment as ItemAttachment;
							item2 = itemAttachment.GetItem();
							disposeGuard.Add<IItem>(item2);
						}
					}
					StorageTranslator<IAttachment, IAttachment> attachmentTranslator = this.GetAttachmentTranslator(attachment.AttachmentType, false);
					IAttachment attachment2 = attachmentTranslator.ConvertToEntity(attachment);
					attachment2.Id = attachmentId;
					result = attachment2;
				}
			}
			return result;
		}

		public void SaveParentItem(IItem parentItem, CommandContext commandContext)
		{
			this.BeforeParentItemSave(parentItem);
			SaveMode saveMode = base.GetSaveMode(null, commandContext);
			ConflictResolutionResult result = parentItem.Save(saveMode);
			result.ThrowOnIrresolvableConflict();
		}

		public override IAttachment Update(IAttachment entity, CommandContext commandContext)
		{
			throw new NotSupportedException(Strings.ErrorUnsupportedOperation("Update"));
		}

		protected virtual void BeforeParentItemSave(IItem parentItem)
		{
		}

		protected virtual IItem BindToParentItem()
		{
			return this.BindToItem(this.parentItemId);
		}

		protected virtual AttachmentCollection GetAttachmentCollection(IItem parentItem)
		{
			return IrmUtils.GetAttachmentCollection(parentItem);
		}

		private IAttachment CreateAttachment(IItem parentItem, IAttachment entity, AttachmentType attachmentType)
		{
			ItemIdAttachment itemIdAttachment = entity as ItemIdAttachment;
			AttachmentCollection attachmentCollection = this.GetAttachmentCollection(parentItem);
			IAttachment attachment;
			StorageTranslator<IAttachment, IAttachment> storageTranslator;
			if (itemIdAttachment != null)
			{
				using (IItem item = this.BindToItem(base.IdConverter.ToStoreObjectId(itemIdAttachment.ItemToAttachId)))
				{
					attachment = attachmentCollection.AddExistingItem(item);
					storageTranslator = AttachmentTranslator<ItemIdAttachment, ItemIdAttachmentSchema>.MetadataInstance;
					goto IL_5E;
				}
			}
			attachment = attachmentCollection.CreateIAttachment(attachmentType);
			storageTranslator = this.GetAttachmentTranslator(attachment.AttachmentType, false);
			IL_5E:
			storageTranslator.SetPropertiesFromEntityOnStorageObject(entity, attachment);
			return attachment;
		}

		private StorageTranslator<IAttachment, IAttachment> GetAttachmentTranslator(AttachmentType attachmentType, bool metadataOnly = false)
		{
			switch (attachmentType)
			{
			case AttachmentType.EmbeddedMessage:
				return AttachmentTranslator<ItemAttachment, ItemAttachmentSchema>.GetTranslatorInstance(metadataOnly);
			case AttachmentType.Reference:
				return AttachmentTranslator<ReferenceAttachment, ReferenceAttachmentSchema>.GetTranslatorInstance(metadataOnly);
			}
			return AttachmentTranslator<FileAttachment, FileAttachmentSchema>.GetTranslatorInstance(metadataOnly);
		}

		private readonly StoreId parentItemId;
	}
}
