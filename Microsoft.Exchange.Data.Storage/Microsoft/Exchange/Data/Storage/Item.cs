using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Item : StoreObject, ICoreItemContext, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public Item(ICoreItem coreItem, bool shallowDispose = false) : base(coreItem, shallowDispose)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.itemCategoryList = new ItemCategoryList(this);
				this.reminder = this.CreateReminderObject();
				this.CoreItem.SetCoreItemContext(this);
				disposeGuard.Success();
			}
		}

		public static void CopyItemContent(Item source, Item target)
		{
			Microsoft.Exchange.Data.Storage.CoreItem.CopyItemContent(source.CoreItem, target.CoreItem);
		}

		public static void CopyItemContent(Item source, Item target, ICollection<NativeStorePropertyDefinition> properties)
		{
			Microsoft.Exchange.Data.Storage.CoreItem.CopyItemContent(source.CoreItem, target.CoreItem, properties);
		}

		public static Item ConvertFrom(Item item, StoreSession session)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			StoreObjectType storeObjectType = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(item.PropertyBag);
			ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
			item.CoreItem.PropertyBag.Load(itemCreateInfo.Schema.AutoloadProperties);
			Item item2 = itemCreateInfo.Creator(item.CoreItem);
			item2.CharsetDetector.DetectionOptions = item.CharsetDetector.DetectionOptions;
			return item2;
		}

		public static void SafeDisposeConvertedItem(Item originalItem, Item convertedItem)
		{
			if (convertedItem.CoreItem != null)
			{
				convertedItem.CoreItem.SetCoreItemContext(originalItem);
				((IDirectPropertyBag)convertedItem.PropertyBag).Context.StoreObject = originalItem;
				convertedItem.CoreObject = null;
			}
			convertedItem.Dispose();
		}

		public static Item Create(StoreSession session, string messageClass, StoreId parentFolderId)
		{
			Item item = null;
			bool flag = false;
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			if (parentFolderId == null)
			{
				throw new ArgumentNullException("parentFolderId");
			}
			Item result;
			try
			{
				item = ItemBuilder.CreateNewItem<Item>(session, parentFolderId, ItemCreateInfo.GenericItemInfo);
				item.ClassName = messageClass;
				flag = true;
				result = item;
			}
			finally
			{
				if (!flag && item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			return result;
		}

		public static Item CloneItem(StoreSession session, StoreId parentFolderId, Item itemToClone, bool bindAsMessage, bool rebindBeforeCloning, ICollection<PropertyDefinition> propertiesToLoad)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(itemToClone, "itemToClone");
			if (bindAsMessage)
			{
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(MessageItemSchema.Instance.AutoloadProperties, propertiesToLoad);
			}
			else
			{
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(ItemSchema.Instance.AutoloadProperties, propertiesToLoad);
			}
			MapiMessage mapiMessage = null;
			PersistablePropertyBag persistablePropertyBag = null;
			AcrPropertyBag acrPropertyBag = null;
			CoreItem coreItem = null;
			Item item = null;
			bool flag = false;
			Item result;
			try
			{
				mapiMessage = Microsoft.Exchange.Data.Storage.Item.CreateClonedMapiMessage(session, parentFolderId, itemToClone, rebindBeforeCloning);
				persistablePropertyBag = new StoreObjectPropertyBag(session, mapiMessage, propertiesToLoad);
				StoreObjectType storeObjectType = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(persistablePropertyBag);
				ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
				acrPropertyBag = new AcrPropertyBag(persistablePropertyBag, itemCreateInfo.AcrProfile, null, new RetryBagFactory(session), null);
				propertiesToLoad = InternalSchema.Combine<PropertyDefinition>(itemCreateInfo.Schema.AutoloadProperties, propertiesToLoad);
				coreItem = new CoreItem(session, acrPropertyBag, null, null, Origin.New, ItemLevel.TopLevel, propertiesToLoad, ItemBindOption.None);
				ItemCreateInfo.ItemCreator itemCreator = bindAsMessage ? ItemCreateInfo.MessageItemInfo.Creator : itemCreateInfo.Creator;
				item = itemCreator(coreItem);
				flag = true;
				result = item;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(item);
					Util.DisposeIfPresent(coreItem);
					Util.DisposeIfPresent(acrPropertyBag);
					Util.DisposeIfPresent(persistablePropertyBag);
					Util.DisposeIfPresent(mapiMessage);
				}
			}
			return result;
		}

		public static Item Bind(StoreSession session, StoreId storeId)
		{
			return Microsoft.Exchange.Data.Storage.Item.Bind(session, storeId, null);
		}

		public static Item Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return Microsoft.Exchange.Data.Storage.Item.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static Item Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return Microsoft.Exchange.Data.Storage.Item.Bind(session, storeId, ItemBindOption.None, propsToReturn);
		}

		public static Item Bind(StoreSession session, StoreId storeId, ItemBindOption itemBindOption)
		{
			return Microsoft.Exchange.Data.Storage.Item.Bind(session, storeId, itemBindOption, null);
		}

		public static Item Bind(StoreSession session, StoreId storeId, ItemBindOption itemBindOption, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			EnumValidator.ThrowIfInvalid<ItemBindOption>(itemBindOption);
			if (propsToReturn == null)
			{
				propsToReturn = Array<PropertyDefinition>.Empty;
			}
			ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(StoreId.GetStoreObjectId(storeId).ObjectType);
			return ItemBuilder.ItemBind<Item>(session, storeId, itemCreateInfo.Schema, null, itemBindOption, propsToReturn);
		}

		public static MessageItem BindAsMessage(StoreSession session, StoreId itemId)
		{
			return Microsoft.Exchange.Data.Storage.Item.BindAsMessage(session, itemId, null);
		}

		public static MessageItem BindAsMessage(StoreSession session, StoreId itemId, params PropertyDefinition[] propsToReturn)
		{
			return Microsoft.Exchange.Data.Storage.Item.BindAsMessage(session, itemId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static MessageItem BindAsMessage(StoreSession session, StoreId itemId, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(itemId, "itemId");
			return ItemBuilder.ItemBindAsMessage(session, itemId, null, ItemBindOption.None, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Item>(this);
		}

		public Body Body
		{
			get
			{
				this.CheckDisposed("Body::get");
				return this.CoreItem.Body;
			}
		}

		public IBody IBody
		{
			get
			{
				return this.Body;
			}
		}

		public string Preview
		{
			get
			{
				this.CheckDisposed("Preview::get");
				return base.GetValueOrDefault<string>(InternalSchema.Preview, string.Empty);
			}
		}

		public Sensitivity Sensitivity
		{
			get
			{
				this.CheckDisposed("Sensitivity::get");
				return base.GetValueOrDefault<Sensitivity>(InternalSchema.Sensitivity);
			}
			set
			{
				this.CheckDisposed("Sensitivity::set");
				EnumValidator.ThrowIfInvalid<Sensitivity>(value, "value");
				this[InternalSchema.Sensitivity] = value;
			}
		}

		public virtual string Subject
		{
			get
			{
				this.CheckDisposed("Subject::get");
				return base.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty);
			}
			set
			{
				this.CheckDisposed("Subject::set");
				this[InternalSchema.Subject] = value;
			}
		}

		public Importance Importance
		{
			get
			{
				this.CheckDisposed("Importance::get");
				return base.GetValueOrDefault<Importance>(InternalSchema.Importance, Importance.Normal);
			}
			set
			{
				this.CheckDisposed("Importance::set");
				EnumValidator.ThrowIfInvalid<Importance>(value, "value");
				this[InternalSchema.Importance] = value;
			}
		}

		public ItemCategoryList Categories
		{
			get
			{
				this.CheckDisposed("Categories::get");
				return this.itemCategoryList;
			}
		}

		public long Size()
		{
			this.CheckDisposed("Size");
			return (long)base.GetValueOrDefault<int>(InternalSchema.Size);
		}

		public IAttachmentCollection IAttachmentCollection
		{
			get
			{
				return this.AttachmentCollection;
			}
		}

		public AttachmentCollection AttachmentCollection
		{
			get
			{
				this.CheckDisposed("AttachmentCollection::get");
				return this.FetchAttachmentCollection();
			}
		}

		protected virtual AttachmentCollection FetchAttachmentCollection()
		{
			if (this.attachmentCollection == null)
			{
				this.CoreItem.OpenAttachmentCollection();
				this.attachmentCollection = new AttachmentCollection(this);
			}
			return this.attachmentCollection;
		}

		internal static NativeStorePropertyDefinition[] CheckNativeProperties(params PropertyDefinition[] properties)
		{
			NativeStorePropertyDefinition[] array = new NativeStorePropertyDefinition[properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
			}
			Array.Copy(properties, array, properties.Length);
			return array;
		}

		public void OpenAsReadWrite()
		{
			this.CoreItem.OpenAsReadWrite();
		}

		public ConflictResolutionResult Save(SaveMode saveMode)
		{
			this.CheckDisposed("Save");
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode, "saveMode");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Item::Save. HashCode = {0}", this.GetHashCode());
			return this.SaveInternal(saveMode, true, null, CoreItemOperation.Save);
		}

		public override string ClassName
		{
			get
			{
				this.CheckDisposed("ClassName::get");
				return base.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			}
			set
			{
				this.CheckDisposed("ClassName::set");
				this[InternalSchema.ItemClass] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ItemSchema.Instance;
			}
		}

		public virtual void SetFlag(string flagRequest, ExDateTime? startDate, ExDateTime? dueDate)
		{
			this.CheckDisposed("SetFlag");
			this.CheckFlagAPIsSupported("SetFlag");
			if (flagRequest == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Item::SetFlag. The in parameter is null. {0}.", "flagRequest");
				throw new ArgumentNullException("flagRequest");
			}
			this[InternalSchema.FlagRequest] = flagRequest;
			this[InternalSchema.MessageTagged] = true;
			this.SetFlagInternal(startDate, dueDate);
		}

		public virtual void CompleteFlag(ExDateTime? completeTime)
		{
			this.CheckDisposed("CompleteFlag");
			this.CheckFlagAPIsSupported("CompleteFlag");
			this.CompleteFlagInternal((completeTime != null) ? new ExDateTime?(completeTime.Value.Date) : completeTime, completeTime);
		}

		public virtual void ClearFlag()
		{
			this.CheckDisposed("ClearFlag");
			this.CheckFlagAPIsSupported("ClearFlag");
			base.DeleteProperties(new PropertyDefinition[]
			{
				InternalSchema.FlagCompleteTime,
				InternalSchema.CompleteDate,
				InternalSchema.ItemColor,
				InternalSchema.FlagRequest,
				InternalSchema.FlagStatus,
				InternalSchema.FlagSubject,
				InternalSchema.TaskStatus,
				InternalSchema.StartDate,
				InternalSchema.DueDate,
				InternalSchema.IsComplete,
				InternalSchema.PercentComplete
			});
			this[InternalSchema.IsToDoItem] = false;
			this[InternalSchema.IsFlagSetForRecipient] = false;
			this[InternalSchema.MessageTagged] = false;
		}

		public void SetFlagForUtcSession(string flagRequest, ExDateTime? localStartDate, ExDateTime? utcStartDate, ExDateTime? localDueDate, ExDateTime? utcDueDate)
		{
			this.CheckDisposed("SetFlagForUtcSession");
			this.CheckFlagAPIsSupported("SetFlagForUtcSession");
			if (base.PropertyBag.ExTimeZone != null && base.PropertyBag.ExTimeZone != ExTimeZone.UtcTimeZone)
			{
				throw new NotSupportedException(ServerStrings.CanUseApiOnlyWhenTimeZoneIsNull("SetFlagForUtcSession"));
			}
			this.SetFlag(flagRequest, utcStartDate, utcDueDate);
			base.PropertyBag.SetOrDeleteProperty(InternalSchema.LocalStartDate, TaskDate.PersistentLocalTime(localStartDate));
			base.PropertyBag.SetOrDeleteProperty(InternalSchema.LocalDueDate, TaskDate.PersistentLocalTime(localDueDate));
		}

		public void CompleteFlagForUtcSession(ExDateTime? completeDate, ExDateTime flagCompleteTime)
		{
			this.CheckDisposed("CompleteFlagForUtcSession");
			this.CheckFlagAPIsSupported("CompleteFlagForUtcSession");
			if (base.PropertyBag.ExTimeZone != null && base.PropertyBag.ExTimeZone != ExTimeZone.UtcTimeZone)
			{
				throw new NotSupportedException(ServerStrings.CanUseApiOnlyWhenTimeZoneIsNull("CompleteFlagForUtcSession"));
			}
			this.CompleteFlagInternal(completeDate, TaskDate.PersistentLocalTime(new ExDateTime?(flagCompleteTime)));
		}

		public Reminder Reminder
		{
			get
			{
				this.CheckDisposed("Reminder::get");
				return this.reminder;
			}
		}

		public string VoiceReminderPhoneNumber
		{
			get
			{
				this.CheckDisposed("VoiceReminderPhoneNumber::get");
				return base.GetValueOrDefault<string>(ItemSchema.VoiceReminderPhoneNumber, string.Empty);
			}
			set
			{
				this.CheckDisposed("VoiceReminderPhoneNumber::set");
				this[ItemSchema.VoiceReminderPhoneNumber] = value;
			}
		}

		public bool IsVoiceReminderEnabled
		{
			get
			{
				this.CheckDisposed("IsVoiceReminderEnabled::get");
				return base.GetValueOrDefault<bool>(ItemSchema.IsVoiceReminderEnabled, false);
			}
			set
			{
				this.CheckDisposed("IsVoiceReminderEnabled::set");
				this[ItemSchema.IsVoiceReminderEnabled] = value;
			}
		}

		public string WorkingSetId
		{
			get
			{
				this.CheckDisposed("WorkingSetId::get");
				return base.GetValueOrDefault<string>(ItemSchema.WorkingSetId, string.Empty);
			}
			set
			{
				this.CheckDisposed("WorkingSetId::set");
				this[ItemSchema.WorkingSetId] = value;
			}
		}

		public WorkingSetSource WorkingSetSource
		{
			get
			{
				this.CheckDisposed("WorkingSetSource::get");
				return base.GetValueOrDefault<WorkingSetSource>(ItemSchema.WorkingSetSource);
			}
			set
			{
				this.CheckDisposed("WorkingSetSource::set");
				EnumValidator.ThrowIfInvalid<WorkingSetSource>(value, "value");
				this[ItemSchema.WorkingSetSource] = value;
			}
		}

		public string WorkingSetSourcePartition
		{
			get
			{
				this.CheckDisposed("WorkingSetSourcePartition::get");
				return base.GetValueOrDefault<string>(ItemSchema.WorkingSetSourcePartition);
			}
			set
			{
				this.CheckDisposed("WorkingSetSourcePartition::set");
				this[ItemSchema.WorkingSetSourcePartition] = value;
			}
		}

		public WorkingSetFlags WorkingSetFlags
		{
			get
			{
				this.CheckDisposed("WorkingSetFlags::get");
				return base.GetValueOrDefault<WorkingSetFlags>(ItemSchema.WorkingSetFlags, WorkingSetFlags.Exchange);
			}
			set
			{
				this.CheckDisposed("WorkingSetFlags::set");
				EnumValidator.ThrowIfInvalid<WorkingSetFlags>(value, "value");
				this[ItemSchema.WorkingSetFlags] = value;
			}
		}

		public AttachmentId CreateAttachmentId(byte[] idBytes)
		{
			this.CheckDisposed("CreateAttachmentId");
			ExTraceGlobals.StorageTracer.Information<byte[]>((long)this.GetHashCode(), "Item::CreateAttachmentId. idBytes = {0}.", idBytes);
			AttachmentId result;
			if (AttachmentId.TryParse(idBytes, out result))
			{
				return result;
			}
			throw new CorruptDataException(ServerStrings.ExInvalidAttachmentId((idBytes == null) ? "<Null>" : Convert.ToBase64String(idBytes)));
		}

		public AttachmentId CreateAttachmentId(string idBase64)
		{
			this.CheckDisposed("CreateAttachmentId");
			if (idBase64 == null)
			{
				throw new ArgumentNullException("idBase64");
			}
			byte[] idBytes = null;
			try
			{
				idBytes = Convert.FromBase64String(idBase64);
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.StorageTracer.TraceError<FormatException>((long)this.GetHashCode(), "Item::CreateAttachmentId. IdBase64 is not encrypted in base64 format. Throw out FormatException:{0}.", arg);
				throw new CorruptDataException(ServerStrings.ExInvalidBase64StringFormat(idBase64));
			}
			return this.CreateAttachmentId(idBytes);
		}

		public Conversation GetConversation(params PropertyDefinition[] propsToLoad)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException("Can't get conversation on public folders");
			}
			ConversationId valueOrDefault = base.PropertyBag.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId);
			if (valueOrDefault == null)
			{
				throw new InvalidOperationException("Can't get conversation on item that doesn't have conversationId assigned to it");
			}
			return Conversation.Load(mailboxSession, valueOrDefault, propsToLoad);
		}

		internal static void CopyCustomPublicStrings(Item from, Item to)
		{
			from.Load(StoreObjectSchema.ContentConversionProperties);
			List<StorePropertyDefinition> allPropertiesForPropSet = Microsoft.Exchange.Data.Storage.Item.GetAllPropertiesForPropSet(from, WellKnownPropertySet.PublicStrings);
			CalendarItemBase.CopyPropertiesTo(from, to, allPropertiesForPropSet.ToArray());
		}

		internal static List<StorePropertyDefinition> GetAllPropertiesForPropSet(Item item, Guid propSetGuid)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			List<StorePropertyDefinition> list = new List<StorePropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in item.AllNativeProperties)
			{
				GuidIdPropertyDefinition guidIdPropertyDefinition = propertyDefinition as GuidIdPropertyDefinition;
				if (guidIdPropertyDefinition != null && guidIdPropertyDefinition.Guid == propSetGuid)
				{
					list.Add(guidIdPropertyDefinition);
				}
				GuidNamePropertyDefinition guidNamePropertyDefinition = propertyDefinition as GuidNamePropertyDefinition;
				if (guidNamePropertyDefinition != null && guidNamePropertyDefinition.Guid == propSetGuid)
				{
					list.Add(guidNamePropertyDefinition);
				}
			}
			return list;
		}

		protected MessageItem InternalCreateReply(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::InternalCreateReply");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(session, parentFolderId);
				ReplyCreation replyCreation = new ReplyCreation(this, messageItem, configuration, false, false, true);
				replyCreation.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		protected MessageItem InternalCreateReplyAll(StoreSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::InternalCreateReplyAll");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(session, parentFolderId);
				ReplyCreation replyCreation = new ReplyCreation(this, messageItem, configuration, true, false, true);
				replyCreation.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		protected MessageItem InternalCreateForward(StoreSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Item::InternalCreateForward.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(session, parentFolderId);
				ForwardCreation forwardCreation = new ForwardCreation(this, messageItem, configuration);
				forwardCreation.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		internal void DisableConstraintValidation()
		{
			Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(base.CoreObject).Context.IsValidationDisabled = true;
		}

		internal void SetFlagInternal(ExDateTime? startDate, ExDateTime? dueDate)
		{
			this[InternalSchema.FlagStatus] = 2;
			this[InternalSchema.FlagSubject] = this.Subject;
			this[InternalSchema.TaskStatus] = TaskStatus.NotStarted;
			base.SetOrDeleteProperty(InternalSchema.StartDate, startDate);
			base.SetOrDeleteProperty(InternalSchema.DueDate, dueDate);
			this[InternalSchema.IsComplete] = false;
			this[InternalSchema.PercentComplete] = 0.0;
			base.Delete(InternalSchema.FlagCompleteTime);
			base.Delete(InternalSchema.CompleteDate);
			if (this is MessageItem && base.GetValueOrDefault<bool>(InternalSchema.IsDraft))
			{
				this[InternalSchema.IsFlagSetForRecipient] = true;
			}
			else
			{
				this[InternalSchema.ItemColor] = 6;
				this[InternalSchema.IsToDoItem] = true;
			}
			ExDateTime? valueAsNullable = base.GetValueAsNullable<ExDateTime>(InternalSchema.ReceivedTime);
			if (valueAsNullable != null)
			{
				this[InternalSchema.ValidFlagStringProof] = valueAsNullable.Value;
			}
		}

		internal virtual VersionedId AssociatedItemId
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal ConflictResolutionResult SaveInternal(SaveMode saveMode, bool commit, CallbackContext callbackContext = null, CoreItemOperation operation = CoreItemOperation.Save)
		{
			base.Load(null);
			this.OnBeforeSave();
			bool flag = false;
			if (callbackContext == null)
			{
				callbackContext = new CallbackContext(base.Session);
				flag = true;
			}
			ConflictResolutionResult result;
			try
			{
				ConflictResolutionResult conflictResolutionResult = this.CoreItem.InternalFlush(saveMode, operation, callbackContext);
				if ((conflictResolutionResult.SaveStatus == SaveResult.Success || conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution) && commit)
				{
					ConflictResolutionResult conflictResolutionResult2 = this.CoreItem.InternalSave(saveMode, callbackContext);
					if (conflictResolutionResult2.SaveStatus != SaveResult.Success)
					{
						conflictResolutionResult = conflictResolutionResult2;
					}
				}
				this.OnAfterSave(conflictResolutionResult);
				result = conflictResolutionResult;
			}
			finally
			{
				if (flag)
				{
					callbackContext.Dispose();
				}
			}
			return result;
		}

		protected virtual void OnBeforeSave()
		{
			if (!base.IsNew)
			{
				this.flagStatus.ExistingItemObjectId = base.StoreObjectId;
				this.flagStatus.ParentId = base.GetValueOrDefault<StoreObjectId>(InternalSchema.ParentItemId);
			}
			if (!base.IsInMemoryObject)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				if (mailboxSession != null)
				{
					MasterCategoryList masterCategoryList = mailboxSession.InternalGetMasterCategoryList();
					foreach (string categoryName in this.itemCategoryList.GetNewCategories())
					{
						masterCategoryList.CategoryWasUsed(base.Id, this.ClassName, categoryName);
					}
				}
			}
		}

		protected virtual void OnAfterSave(ConflictResolutionResult acrResults)
		{
			if (acrResults.SaveStatus != SaveResult.IrresolvableConflict)
			{
				this.attachmentCollection = null;
			}
			try
			{
				if (acrResults.SaveStatus != SaveResult.IrresolvableConflict)
				{
					this.SetItemFlagsAndMessageStatus();
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (AccessDeniedException)
			{
				MailboxSession mailboxSession = base.Session as MailboxSession;
				if (mailboxSession != null && mailboxSession.LogonType != LogonType.Delegated && mailboxSession.LogonType != LogonType.BestAccess)
				{
					throw;
				}
			}
			if (this.Reminder != null && this.Reminder.HasAcrAffectedReminders(acrResults))
			{
				base.Load(null);
				this.Reminder.SaveStateAsInitial(true);
			}
		}

		internal void ReplaceAttachments(Item item)
		{
			this.ReplaceAttachments(item, null);
		}

		public void ReplaceAttachments(Item item, BodyFormat? format)
		{
			this.CheckDisposed("ReplaceAttachments");
			Util.ThrowOnNullArgument(item, "item");
			item.AttachmentCollection.RemoveAll();
			AttachmentCollection attachmentCollection = this.AttachmentCollection;
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				using (Attachment attachment = attachmentCollection.Open(handle, null))
				{
					if (!attachment.IsCalendarException && !attachment.GetValueOrDefault<bool>(InternalSchema.AttachInConflict))
					{
						using (Attachment attachment2 = attachment.CreateCopy(item.AttachmentCollection, format))
						{
							attachment2.Save();
						}
					}
				}
			}
		}

		protected void CheckSetNull(string method, string argument, object setValue)
		{
			if (setValue == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}::set. {1} cannot be set to null.", method, argument);
				throw new ArgumentNullException(argument);
			}
		}

		internal static OccurrencePropertyBag CreateOccurrencePropertyBag(StoreSession session, OccurrenceStoreObjectId occurrenceId, ICollection<PropertyDefinition> additionalProperties)
		{
			ICollection<PropertyDefinition> autoloadProperties = InternalSchema.Combine<PropertyDefinition>(CalendarItemBaseSchema.Instance.AutoloadProperties, additionalProperties);
			bool flag = false;
			StoreObjectPropertyBag storeObjectPropertyBag = new StoreObjectPropertyBag(session, session.GetMapiProp(occurrenceId), autoloadProperties);
			OccurrencePropertyBag result;
			try
			{
				byte[] largeBinaryProperty = storeObjectPropertyBag.GetLargeBinaryProperty(InternalSchema.AppointmentRecurrenceBlob);
				if (largeBinaryProperty == null)
				{
					throw new OccurrenceNotFoundException(ServerStrings.ExOccurrenceNotPresent(occurrenceId));
				}
				int valueOrDefault = storeObjectPropertyBag.GetValueOrDefault<int>(InternalSchema.Codepage, CalendarItem.DefaultCodePage);
				ExTimeZone recurringTimeZoneFromPropertyBag = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(storeObjectPropertyBag);
				InternalRecurrence internalRecurrence = InternalRecurrence.InternalParse(largeBinaryProperty, new VersionedId(StoreObjectId.FromProviderSpecificId(occurrenceId.ProviderLevelItemId, StoreObjectType.CalendarItem), Array<byte>.Empty), recurringTimeZoneFromPropertyBag, session.ExTimeZone, valueOrDefault);
				ExDateTime exDateTime = occurrenceId.OccurrenceId;
				if (exDateTime.TimeZone == ExTimeZone.UnspecifiedTimeZone)
				{
					exDateTime = internalRecurrence.CreatedExTimeZone.ConvertDateTime(exDateTime);
				}
				if (!internalRecurrence.IsValidOccurrenceId(exDateTime) || internalRecurrence.IsOccurrenceDeleted(exDateTime))
				{
					ExTraceGlobals.StorageTracer.Information<OccurrenceStoreObjectId>(0L, "Item::CreateOccurrencePropertyBag. Open requested occurence on deleted occurrnece={0}", occurrenceId);
					throw new OccurrenceNotFoundException(ServerStrings.ExItemNotFound);
				}
				OccurrenceInfo occurrenceInfoByDateId = internalRecurrence.GetOccurrenceInfoByDateId(exDateTime);
				OccurrencePropertyBag occurrencePropertyBag = new OccurrencePropertyBag(session, storeObjectPropertyBag, occurrenceInfoByDateId, additionalProperties);
				flag = true;
				result = occurrencePropertyBag;
			}
			finally
			{
				if (!flag && storeObjectPropertyBag != null)
				{
					storeObjectPropertyBag.Dispose();
				}
			}
			return result;
		}

		internal static Item InternalBindCoreItem(ICoreItem coreItem)
		{
			StoreObjectType storeObjectType = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(coreItem.PropertyBag);
			ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
			return itemCreateInfo.Creator(coreItem);
		}

		internal static Item TransferOwnershipOfCoreItem(Item item)
		{
			StoreObjectType storeObjectType = ItemBuilder.ReadStoreObjectTypeFromPropertyBag(item.PropertyBag);
			ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
			Item result = itemCreateInfo.Creator(item.CoreItem);
			item.AssignNullToCoreItem();
			return result;
		}

		public PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed("Item.SaveFlags.get");
				return base.PropertyBag.SaveFlags;
			}
			set
			{
				this.CheckDisposed("Item.SaveFlags.set");
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				base.PropertyBag.SaveFlags = value;
			}
		}

		protected virtual Reminder CreateReminderObject()
		{
			return new Reminder(this);
		}

		public MapiMessage MapiMessage
		{
			get
			{
				return (MapiMessage)base.MapiProp;
			}
		}

		internal bool HasAllPropertiesLoaded
		{
			get
			{
				return base.PropertyBag.HasAllPropertiesLoaded;
			}
		}

		protected void ClearFlagsPropertyForSet(PropertyDefinition propertyDefinition)
		{
			this.flagStatus.ClearFlagsForSet(propertyDefinition);
		}

		internal void SetFlagsApiProperties(PropertyDefinition propertyDefinition, int flag, bool value)
		{
			this.flagStatus.SetFlagsPropertyOnItem(propertyDefinition, flag, value);
		}

		internal bool? GetFlagsApiProperties(PropertyDefinition propertyDefinition, int flag)
		{
			return this.flagStatus.TryGetValue(propertyDefinition, flag);
		}

		private static MapiMessage CreateClonedMapiMessage(StoreSession destinationSession, StoreId parentFolderId, Item itemToClone, bool rebindBeforeCloning)
		{
			MapiMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiMessage mapiMessage = Folder.InternalCreateMapiMessage(destinationSession, parentFolderId, CreateMessageType.Normal);
				disposeGuard.Add<MapiMessage>(mapiMessage);
				StoreSession session = itemToClone.Session;
				if (rebindBeforeCloning)
				{
					using (MapiMessage mapiMessage2 = (MapiMessage)session.GetMapiProp(itemToClone.Id.ObjectId))
					{
						Microsoft.Exchange.Data.Storage.CoreObject.MapiCopyTo(mapiMessage2, mapiMessage, session, destinationSession, CopyPropertiesFlags.None, CopySubObjects.Copy, new NativeStorePropertyDefinition[0]);
						goto IL_70;
					}
				}
				Microsoft.Exchange.Data.Storage.CoreObject.MapiCopyTo(itemToClone.MapiMessage, mapiMessage, session, destinationSession, CopyPropertiesFlags.None, CopySubObjects.Copy, new NativeStorePropertyDefinition[0]);
				IL_70:
				disposeGuard.Success();
				result = mapiMessage;
			}
			return result;
		}

		private void CheckFlagAPIsSupported(string method)
		{
			if (this is CalendarItemBase || this is Task)
			{
				throw new StoragePermanentException(ServerStrings.InvokingMethodNotSupported(base.GetType().Name, method));
			}
		}

		private void CompleteFlagInternal(ExDateTime? completeDate, ExDateTime? flagCompleteTime)
		{
			base.DeleteProperties(new PropertyDefinition[]
			{
				InternalSchema.ItemColor
			});
			this[InternalSchema.IsComplete] = true;
			this[InternalSchema.PercentComplete] = 1.0;
			base.SetOrDeleteProperty(InternalSchema.CompleteDate, completeDate);
			base.SetOrDeleteProperty(InternalSchema.FlagCompleteTime, flagCompleteTime);
			this[InternalSchema.FlagStatus] = 1;
			string value = base.TryGetProperty(InternalSchema.FlagRequest) as string;
			if (string.IsNullOrEmpty(value))
			{
				this[InternalSchema.FlagRequest] = ClientStrings.Followup.ToString(base.Session.InternalPreferedCulture);
			}
		}

		internal bool IsAttachmentCollectionLoaded
		{
			get
			{
				this.CheckDisposed("IsAttachmentCollectiongLoaded::get");
				return this.CoreItem.IsAttachmentCollectionLoaded;
			}
		}

		public ItemCharsetDetector CharsetDetector
		{
			get
			{
				this.CheckDisposed("Item.CharsetDetector::get");
				return this.CoreItem.CharsetDetector;
			}
		}

		public int PreferredInternetCodePageForShiftJis
		{
			set
			{
				this.CharsetDetector.DetectionOptions.PreferredInternetCodePageForShiftJis = value;
			}
		}

		public int RequiredCoverage
		{
			set
			{
				this.CharsetDetector.DetectionOptions.RequiredCoverage = value;
			}
		}

		public ICoreItem CoreItem
		{
			get
			{
				return (ICoreItem)base.CoreObject;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				this.CheckDisposed("IsReadyOnly::get");
				return this.CoreItem.IsReadOnly;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetType().FullName);
			stringBuilder.AppendLine();
			if (base.IsDisposed)
			{
				stringBuilder.AppendLine("disposed");
			}
			else
			{
				if (base.Session != null)
				{
					stringBuilder.AppendFormat("Session: {0}", base.Session.ToString());
					stringBuilder.AppendLine();
				}
				if (base.StoreObjectId != null)
				{
					stringBuilder.AppendFormat("Item StoreObjectId: {0}", base.StoreObjectId.ToBase64String());
					stringBuilder.AppendLine();
				}
				if (base.Id != null)
				{
					stringBuilder.AppendFormat("Item Id: {0}", base.Id.ToBase64String());
					stringBuilder.AppendLine();
				}
				string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.InternetMessageId);
				if (valueOrDefault != null)
				{
					stringBuilder.AppendFormat("Message id: {0}", valueOrDefault);
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		private void SetItemFlagsAndMessageStatus()
		{
			if (base.Session == null)
			{
				return;
			}
			if (!this.flagStatus.IsDirty())
			{
				return;
			}
			if (this.flagStatus.ExistingItemObjectId == null)
			{
				base.Load(new PropertyDefinition[]
				{
					InternalSchema.ItemId
				});
				if (base.Id == null)
				{
					return;
				}
				this.flagStatus.ExistingItemObjectId = base.Id.ObjectId;
				this.flagStatus.ParentId = base.ParentId;
			}
			int setReadFlag = this.flagStatus.GetSetReadFlag();
			if (setReadFlag >= 0)
			{
				this.CoreItem.SetReadFlag(setReadFlag, false);
			}
			int num = 0;
			int num2 = 0;
			if (this.flagStatus.GetNonReadFlagsBits(out num, out num2))
			{
				using (MapiFolder mapiFolder = Folder.DeferBind(base.Session, this.flagStatus.ParentId))
				{
					StoreSession session = base.Session;
					bool flag = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						mapiFolder.SetMessageFlags(this.flagStatus.ExistingItemObjectId.ProviderLevelItemId, (MessageFlags)num, (MessageFlags)num2);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlagStatus, ex, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("Item::SetItemFlagsAndMessageStatus.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlagStatus, ex2, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("Item::SetItemFlagsAndMessageStatus.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (session != null)
							{
								session.EndMapiCall();
								if (flag)
								{
									session.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
			}
			if (this.flagStatus.GetDirtyStatusBits(out num, out num2))
			{
				using (MapiFolder mapiFolder2 = Folder.DeferBind(base.Session, this.flagStatus.ParentId))
				{
					CoreFolder.InternalSetItemStatus(mapiFolder2, base.Session, this, this.flagStatus.ExistingItemObjectId, (MessageStatus)num, (MessageStatus)num2);
				}
			}
		}

		private void AssignNullToCoreItem()
		{
			base.CoreObject = null;
		}

		internal virtual bool AreAttachmentsDirty
		{
			get
			{
				return this.IsAttachmentCollectionLoaded && this.CoreItem.AttachmentCollection.IsDirty;
			}
		}

		void ICoreItemContext.GetContextCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
			this.InternalGetContextCharsetDetectionData(stringBuilder, flags);
		}

		protected virtual void InternalGetContextCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
		}

		internal static void CoreObjectUpdateInternetMessageId(CoreItem coreItem)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.InternetMessageId, null);
			if (valueOrDefault != null && valueOrDefault == string.Empty)
			{
				coreItem.PropertyBag.Delete(InternalSchema.InternetMessageId);
			}
		}

		internal static void CoreObjectUpdatePreview(CoreItem coreItem)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.Preview, null);
			if (coreItem.Body != null && ((ICoreItem)coreItem).AreOptionalAutoloadPropertiesLoaded && (coreItem.Body.IsBodyChanged || coreItem.Body.IsPreviewInvalid || string.IsNullOrEmpty(valueOrDefault)))
			{
				coreItem.PropertyBag.SetProperty(InternalSchema.Preview, coreItem.Body.PreviewText.Trim(Environment.NewLine.ToCharArray()));
			}
		}

		internal static void CoreObjectUpdateSentRepresentingType(CoreItem coreItem)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.SentRepresentingType, null);
			string valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.SentRepresentingEmailAddress, null);
			byte[] valueOrDefault3 = coreItem.PropertyBag.GetValueOrDefault<byte[]>(InternalSchema.SentRepresentingEntryId, null);
			if (!string.IsNullOrWhiteSpace(valueOrDefault) && string.Compare(valueOrDefault, "EX", StringComparison.CurrentCultureIgnoreCase) != 0 && valueOrDefault2 == null && valueOrDefault3 != null && valueOrDefault3.Length > 0)
			{
				coreItem.PropertyBag.Delete(InternalSchema.SentRepresentingType);
			}
		}

		internal static void CoreObjectUpdateAnnotationToken(CoreItem coreItem)
		{
			if (coreItem.IsMoveUser)
			{
				return;
			}
			bool flag = false;
			if (((ICoreItem)coreItem).IsAttachmentCollectionLoaded)
			{
				flag = coreItem.AttachmentCollection.IsDirty;
				if (flag && coreItem.Id == null)
				{
					flag = !coreItem.AttachmentCollection.IsClonedFromAnExistingAttachmentCollection;
				}
			}
			if ((coreItem.Body != null && coreItem.Body.IsBodyChanged) || flag)
			{
				coreItem.PropertyBag.Delete(InternalSchema.AnnotationToken);
			}
		}

		internal static void CoreObjectUpdateAllAttachmentsHidden(CoreItem coreItem)
		{
			if (((ICoreItem)coreItem).IsAttachmentCollectionLoaded && (coreItem.Origin == Origin.New || coreItem.AttachmentCollection.IsDirty))
			{
				string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
				if (ObjectClass.IsDsn(valueOrDefault) || ObjectClass.IsMdn(valueOrDefault))
				{
					Microsoft.Exchange.Data.Storage.Item.EnsureAllAttachmentsHiddenValue(coreItem, true);
					return;
				}
				if (ObjectClass.IsSmime(valueOrDefault))
				{
					return;
				}
				string valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ContentClass, string.Empty);
				if (ObjectClass.IsRightsManagedContentClass(valueOrDefault2) && coreItem.AttachmentCollection.Count == 1)
				{
					return;
				}
				bool flag = false;
				foreach (AttachmentHandle handle in coreItem.AttachmentCollection)
				{
					if (!CoreAttachmentCollection.IsInlineAttachment(handle))
					{
						flag = true;
						break;
					}
				}
				Microsoft.Exchange.Data.Storage.Item.EnsureAllAttachmentsHiddenValue(coreItem, !flag);
			}
		}

		internal static void EnsureAllAttachmentsHiddenValue(CoreItem coreItem, bool value)
		{
			object objA = coreItem.PropertyBag.TryGetProperty(InternalSchema.AllAttachmentsHidden);
			if (!object.Equals(objA, value))
			{
				coreItem.PropertyBag.SetProperty(InternalSchema.AllAttachmentsHidden, value);
			}
		}

		public void ForceUpdateImapiId()
		{
			base.SetProperties(Microsoft.Exchange.Data.Storage.Item.ImapIdProperty, new object[]
			{
				0
			});
			base.PropertyBag.SetUpdateImapIdFlag();
		}

		public RuleHistory GetRuleHistory()
		{
			this.CheckDisposed("GetRuleHistory");
			return this.GetRuleHistory(base.Session);
		}

		public RuleHistory GetRuleHistory(StoreSession session)
		{
			this.CheckDisposed("GetRuleHistory");
			byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(ItemSchema.RuleTriggerHistory, Array<byte>.Empty);
			return new RuleHistory(this, valueOrDefault, session);
		}

		private FlagStatusInternal flagStatus = new FlagStatusInternal();

		private readonly ItemCategoryList itemCategoryList;

		private readonly Reminder reminder;

		protected AttachmentCollection attachmentCollection;

		private static PropertyDefinition[] ImapIdProperty = new PropertyDefinition[]
		{
			ItemSchema.ImapId
		};
	}
}
