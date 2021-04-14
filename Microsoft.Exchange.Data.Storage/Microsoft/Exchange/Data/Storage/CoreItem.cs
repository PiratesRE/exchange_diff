using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreItem : CoreObject, ICoreItem, ICoreObject, ICoreState, IValidatable, IDisposeTrackable, IDisposable, ILocationIdentifierController
	{
		internal CoreItem(StoreSession session, PersistablePropertyBag propertyBag, StoreObjectId storeObjectId, byte[] changeKey, Origin origin, ItemLevel itemLevel, ICollection<PropertyDefinition> prefetchProperties, ItemBindOption itemBindOption) : base(session, propertyBag, storeObjectId, changeKey, origin, itemLevel, prefetchProperties)
		{
			this.itemBindOption = itemBindOption;
			this.locationIdentifierHelperInstance = new LocationIdentifierHelper();
			this.locationIdentifierHelperInstance.ResetChangeList();
			propertyBag.OnLocationIdentifierReached = (Action<uint>)Delegate.Combine(propertyBag.OnLocationIdentifierReached, new Action<uint>(this.locationIdentifierHelperInstance.SetLocationIdentifier));
			propertyBag.OnNamedLocationIdentifierReached = (Action<uint, LastChangeAction>)Delegate.Combine(propertyBag.OnNamedLocationIdentifierReached, new Action<uint, LastChangeAction>(this.locationIdentifierHelperInstance.SetLocationIdentifier));
			this.charsetDetector = new ItemCharsetDetector(this);
			if (session != null && session.PreferredInternetCodePageForShiftJis != 0)
			{
				this.charsetDetector.DetectionOptions.PreferredInternetCodePageForShiftJis = session.PreferredInternetCodePageForShiftJis;
				this.charsetDetector.DetectionOptions.RequiredCoverage = session.RequiredCoverage;
			}
		}

		public event Action BeforeSend
		{
			add
			{
				this.CheckDisposed(null);
				this.beforeSendEventHandler = (Action)Delegate.Combine(this.beforeSendEventHandler, value);
			}
			remove
			{
				this.beforeSendEventHandler = (Action)Delegate.Remove(this.beforeSendEventHandler, value);
			}
		}

		public event Action BeforeFlush
		{
			add
			{
				this.CheckDisposed(null);
				this.beforeFlushEventHandler = (Action)Delegate.Combine(this.beforeFlushEventHandler, value);
			}
			remove
			{
				this.beforeFlushEventHandler = (Action)Delegate.Remove(this.beforeFlushEventHandler, value);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				AcrPropertyBag acrPropertyBag = base.PropertyBag as AcrPropertyBag;
				return acrPropertyBag != null && acrPropertyBag.IsReadOnly;
			}
		}

		public bool IsMoveUser
		{
			get
			{
				return base.Session != null && base.Session.IsMoveUser;
			}
		}

		public ICoreItem TopLevelItem
		{
			get
			{
				this.CheckDisposed(null);
				return this.topLevelItem;
			}
			set
			{
				this.CheckDisposed(null);
				this.topLevelItem = value;
			}
		}

		public bool IsFlushNeeded
		{
			get
			{
				return base.IsDirty || this.IsRecipientCollectionDirty;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return base.IsDirty || this.IsRecipientCollectionDirty || this.IsAttachmentCollectionDirty;
			}
		}

		public bool IsLegallyDirty
		{
			get
			{
				return this.legallyDirtyProperties != null && this.legallyDirtyProperties.Count<string>() > 0;
			}
		}

		public PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed(null);
				return CoreObject.GetPersistablePropertyBag(this).SaveFlags;
			}
			set
			{
				this.CheckDisposed(null);
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				CoreObject.GetPersistablePropertyBag(this).SaveFlags = value;
			}
		}

		public CoreAttachmentCollection AttachmentCollection
		{
			get
			{
				this.CheckDisposed(null);
				if (this.attachmentCollection == null)
				{
					((ICoreItem)this).OpenAttachmentCollection();
				}
				return this.attachmentCollection;
			}
		}

		public CoreRecipientCollection Recipients
		{
			get
			{
				return ((ICoreItem)this).GetRecipientCollection(true);
			}
		}

		public Body Body
		{
			get
			{
				this.CheckDisposed(null);
				if (this.body == null)
				{
					this.body = new Body(this);
				}
				return this.body;
			}
		}

		public ItemCharsetDetector CharsetDetector
		{
			get
			{
				this.CheckDisposed(null);
				return this.charsetDetector;
			}
		}

		public int PreferredInternetCodePageForShiftJis
		{
			set
			{
				this.CheckDisposed(null);
				this.charsetDetector.DetectionOptions.PreferredInternetCodePageForShiftJis = value;
			}
		}

		public int RequiredCoverage
		{
			set
			{
				this.CheckDisposed(null);
				this.charsetDetector.DetectionOptions.RequiredCoverage = value;
			}
		}

		public LocationIdentifierHelper LocationIdentifierHelperInstance
		{
			get
			{
				this.CheckDisposed(null);
				return this.locationIdentifierHelperInstance;
			}
		}

		public Dictionary<ItemSchema, object> CoreObjectUpdateContext
		{
			get
			{
				if (this.coreObjectUpdateContext == null)
				{
					this.coreObjectUpdateContext = new Dictionary<ItemSchema, object>();
				}
				return this.coreObjectUpdateContext;
			}
		}

		MapiMessage ICoreItem.MapiMessage
		{
			get
			{
				return (MapiMessage)CoreObject.GetPersistablePropertyBag(this).MapiProp;
			}
		}

		bool ICoreItem.IsAttachmentCollectionLoaded
		{
			get
			{
				this.CheckDisposed(null);
				return this.attachmentCollection != null && this.attachmentCollection.IsInitialized;
			}
		}

		bool ICoreItem.AreOptionalAutoloadPropertiesLoaded
		{
			get
			{
				return (this.itemBindOption & ItemBindOption.LoadRequiredPropertiesOnly) != ItemBindOption.LoadRequiredPropertiesOnly;
			}
		}

		internal bool IsRecipientCollectionLoaded
		{
			get
			{
				return this.recipients != null;
			}
		}

		protected override StorePropertyDefinition IdProperty
		{
			get
			{
				return CoreItemSchema.Id;
			}
		}

		private IAttachmentProvider AttachmentProvider
		{
			get
			{
				this.CheckDisposed(null);
				if (this.attachmentProvider == null)
				{
					if (((ICoreObject)this).Session != null)
					{
						this.attachmentProvider = new MapiAttachmentProvider();
					}
					else
					{
						this.attachmentProvider = new InMemoryAttachmentProvider();
					}
				}
				return this.attachmentProvider;
			}
		}

		private bool IsRecipientCollectionDirty
		{
			get
			{
				return this.IsRecipientCollectionLoaded && this.recipients.IsDirty;
			}
		}

		private bool IsAttachmentCollectionDirty
		{
			get
			{
				return ((ICoreItem)this).IsAttachmentCollectionLoaded && this.attachmentCollection.IsDirty;
			}
		}

		public static CoreItem Bind(StoreSession session, StoreId storeId)
		{
			return CoreItem.Bind(session, storeId, null);
		}

		public static CoreItem Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(storeId, "storeId");
			if (!IdConverter.IsMessageId(StoreId.GetStoreObjectId(storeId)))
			{
				throw new ArgumentException(ServerStrings.ExInvalidItemId);
			}
			StoreObjectType storeObjectType = StoreId.GetStoreObjectId(storeId).ObjectType;
			if (storeObjectType == StoreObjectType.Unknown)
			{
				storeObjectType = StoreObjectType.Message;
			}
			ItemCreateInfo itemCreateInfo = ItemCreateInfo.GetItemCreateInfo(storeObjectType);
			propsToReturn = InternalSchema.Combine<PropertyDefinition>(itemCreateInfo.Schema.AutoloadProperties, propsToReturn);
			CoreItem result = ItemBuilder.CoreItemBind(session, storeId, null, ItemBindOption.None, propsToReturn, ref storeObjectType);
			session.MessagesWereDownloaded = true;
			return result;
		}

		public static CoreItem Create(StoreSession session, StoreId parentFolderId, CreateMessageType createMessageType)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.ThrowIfInvalid<CreateMessageType>(createMessageType, "createMessageType");
			session.CheckSystemFolderAccess(StoreId.GetStoreObjectId(parentFolderId));
			return ItemBuilder.CreateNewCoreItem(session, ItemCreateInfo.MessageItemInfo, true, () => Folder.InternalCreateMapiMessage(session, parentFolderId, createMessageType));
		}

		public static ImportResult Import(ContentsSynchronizationUploadContext context, CreateMessageType createMessageType, bool failOnConflict, VersionedId itemId, ExDateTime lastModificationTime, byte[] predecessorChangeList, out CoreItem coreItem)
		{
			Util.ThrowOnNullArgument(context, "context");
			EnumValidator.ThrowIfInvalid<CreateMessageType>(createMessageType, "createMessageType");
			Util.ThrowOnNullArgument(itemId, "itemId");
			Util.ThrowOnNullArgument(predecessorChangeList, "predecessorChangeList");
			object[] propertyValues = new object[]
			{
				context.Session.IdConverter.GetLongTermIdFromId(itemId.ObjectId),
				itemId.ChangeKeyAsByteArray(),
				lastModificationTime,
				predecessorChangeList
			};
			coreItem = null;
			MapiMessage mapiMessage = null;
			bool flag = false;
			ImportResult result;
			try
			{
				ImportResult importResult = context.ImportChange(createMessageType, failOnConflict, CoreItem.importChangeProperties, propertyValues, out mapiMessage);
				if (mapiMessage != null)
				{
					coreItem = ItemBuilder.CreateNewCoreItem(context.Session, ItemCreateInfo.GenericItemInfo, itemId, false, () => mapiMessage);
					mapiMessage = null;
				}
				flag = true;
				result = importResult;
			}
			finally
			{
				if (!flag)
				{
					if (coreItem != null)
					{
						coreItem.Dispose();
						coreItem = null;
					}
					if (mapiMessage != null)
					{
						mapiMessage.Dispose();
						mapiMessage = null;
					}
				}
			}
			return result;
		}

		public static ImportResult Move(ContentsSynchronizationUploadContext destinationContext, StoreObjectId sourceItemId, byte[] sourcePredecessorChangeList, VersionedId destinationItemId)
		{
			Util.ThrowOnNullArgument(destinationContext, "destinationContext");
			Util.ThrowOnNullArgument(sourceItemId, "sourceItemId");
			Util.ThrowOnNullArgument(sourcePredecessorChangeList, "sourcePredecessorChangeList");
			Util.ThrowOnNullArgument(destinationItemId, "destinationItemId");
			FolderChangeOperation operation = CoreFolder.IsMovingToDeletedItems(destinationContext.Session, destinationContext.SyncRootFolderId) ? FolderChangeOperation.MoveToDeletedItems : FolderChangeOperation.Move;
			TeamMailboxClientOperations.ThrowIfInvalidItemOperation(destinationContext.Session, new StoreObjectId[]
			{
				destinationItemId.ObjectId
			});
			StoreObjectId[] array = new StoreObjectId[]
			{
				sourceItemId
			};
			StoreObjectId parentIdFromMessageId = IdConverter.GetParentIdFromMessageId(sourceItemId);
			byte[] array2 = destinationItemId.ChangeKeyAsByteArray();
			GroupOperationResult result = null;
			ImportResult result2;
			using (CallbackContext callbackContext = new CallbackContext(destinationContext.Session))
			{
				try
				{
					destinationContext.Session.OnBeforeFolderChange(operation, FolderChangeOperationFlags.IncludeAll, destinationContext.Session, destinationContext.Session, parentIdFromMessageId, destinationContext.SyncRootFolderId, array, callbackContext);
					try
					{
						result2 = destinationContext.ImportMove(destinationContext.Session.IdConverter.GetLongTermIdFromId(destinationContext.Session.IdConverter.GetFidFromId(sourceItemId)), destinationContext.Session.IdConverter.GetLongTermIdFromId(sourceItemId), sourcePredecessorChangeList, destinationContext.Session.IdConverter.GetLongTermIdFromId(destinationItemId.ObjectId), array2);
					}
					catch (StoragePermanentException storageException)
					{
						result = new GroupOperationResult(OperationResult.Failed, array, storageException);
						throw;
					}
					catch (StorageTransientException storageException2)
					{
						result = new GroupOperationResult(OperationResult.Failed, array, storageException2);
						throw;
					}
					LocalizedException storageException3;
					if (CoreItem.ImportOperationSucceeded(result2, out storageException3))
					{
						result = new GroupOperationResult(OperationResult.Succeeded, array, null, new StoreObjectId[]
						{
							destinationItemId.ObjectId
						}, new byte[][]
						{
							array2
						});
					}
					else
					{
						result = new GroupOperationResult(OperationResult.Failed, array, storageException3);
					}
				}
				finally
				{
					destinationContext.Session.OnAfterFolderChange(operation, FolderChangeOperationFlags.IncludeAll, destinationContext.Session, destinationContext.Session, parentIdFromMessageId, destinationContext.SyncRootFolderId, array, result, callbackContext);
				}
			}
			return result2;
		}

		public static OperationResult Delete(ContentsSynchronizationUploadContext context, DeleteItemFlags deleteItemFlags, IList<StoreObjectId> itemIds)
		{
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(itemIds, "itemIds");
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags, "deleteItemFlags");
			TeamMailboxClientOperations.ThrowIfInvalidItemOperation(context.Session, itemIds);
			FolderChangeOperation operation = ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete) ? FolderChangeOperation.HardDelete : FolderChangeOperation.SoftDelete;
			FolderChangeOperationFlags flags = FolderChangeOperationFlags.IncludeAssociated | FolderChangeOperationFlags.IncludeItems;
			GroupOperationResult groupOperationResult = null;
			using (CallbackContext callbackContext = new CallbackContext(context.Session))
			{
				try
				{
					context.CoreFolder.ClearNotReadNotificationPending(itemIds.ToArray<StoreObjectId>());
					bool flag = context.Session.OnBeforeFolderChange(operation, flags, context.Session, null, context.SyncRootFolderId, null, itemIds, callbackContext);
					if (flag)
					{
						groupOperationResult = context.Session.GetCallbackResults();
					}
					else
					{
						context.ImportDeletes(deleteItemFlags, context.Session.IdConverter.GetSourceKeys(itemIds, new Predicate<StoreObjectId>(IdConverter.IsMessageId)));
						groupOperationResult = new GroupOperationResult(OperationResult.Succeeded, itemIds, null);
					}
				}
				catch (StoragePermanentException storageException)
				{
					groupOperationResult = new GroupOperationResult(OperationResult.Failed, itemIds, storageException);
					throw;
				}
				catch (StorageTransientException storageException2)
				{
					groupOperationResult = new GroupOperationResult(OperationResult.Failed, itemIds, storageException2);
					throw;
				}
				finally
				{
					context.Session.OnAfterFolderChange(operation, flags, context.Session, null, context.SyncRootFolderId, null, itemIds, groupOperationResult, callbackContext);
				}
			}
			return groupOperationResult.OperationResult;
		}

		public static void SetReadFlag(ContentsSynchronizationUploadContext context, bool isRead, ICollection<StoreObjectId> itemIds)
		{
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(itemIds, "itemIds");
			TeamMailboxClientOperations.ThrowIfInvalidItemOperation(context.Session, itemIds);
			context.ImportReadStateChange(isRead, context.Session.IdConverter.GetSourceKeys(itemIds, new Predicate<StoreObjectId>(IdConverter.IsMessageId)));
			if (context.Session.ActivitySession != null)
			{
				if (isRead)
				{
					context.Session.ActivitySession.CaptureMarkAsRead(itemIds);
					return;
				}
				context.Session.ActivitySession.CaptureMarkAsUnread(itemIds);
			}
		}

		public void OpenAsReadWrite()
		{
			AcrPropertyBag acrPropertyBag = base.PropertyBag as AcrPropertyBag;
			if (acrPropertyBag != null)
			{
				acrPropertyBag.OpenAsReadWrite();
			}
			this.ResetAttachmentCollection();
			if (this.attachmentCollection != null)
			{
				this.attachmentCollection.OpenAsReadWrite();
			}
		}

		public ConflictResolutionResult Save(SaveMode saveMode)
		{
			this.CheckDisposed(null);
			ConflictResolutionResult result;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				ConflictResolutionResult conflictResolutionResult = this.InternalFlush(saveMode, CoreItemOperation.Save, callbackContext);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success && conflictResolutionResult.SaveStatus != SaveResult.SuccessWithConflictResolution)
				{
					result = conflictResolutionResult;
				}
				else
				{
					ConflictResolutionResult conflictResolutionResult2 = this.InternalSave(saveMode, callbackContext);
					if (conflictResolutionResult2.SaveStatus != SaveResult.Success)
					{
						result = conflictResolutionResult2;
					}
					else
					{
						result = conflictResolutionResult;
					}
				}
			}
			return result;
		}

		public ConflictResolutionResult Flush(SaveMode saveMode)
		{
			this.CheckDisposed(null);
			ConflictResolutionResult result;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				ConflictResolutionResult conflictResolutionResult = ((ICoreItem)this).InternalFlush(saveMode, callbackContext);
				this.Body.ResetBodyFormat();
				result = conflictResolutionResult;
			}
			return result;
		}

		public void Submit()
		{
			this.CheckDisposed(null);
			this.Submit(SubmitMessageFlags.None);
		}

		public void Submit(SubmitMessageFlags submitFlags)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SubmitMessageFlags>(submitFlags, "submitFlags");
			SubmitMessageExFlags submitMessageExFlags = SubmitMessageExFlags.None;
			if ((submitFlags & SubmitMessageFlags.Preprocess) != SubmitMessageFlags.None)
			{
				submitMessageExFlags |= SubmitMessageExFlags.Preprocess;
			}
			if ((submitFlags & SubmitMessageFlags.NeedsSpooler) != SubmitMessageFlags.None)
			{
				submitMessageExFlags |= SubmitMessageExFlags.NeedsSpooler;
			}
			if ((submitFlags & SubmitMessageFlags.IgnoreSendAsRight) != SubmitMessageFlags.None)
			{
				submitMessageExFlags |= SubmitMessageExFlags.IgnoreSendAsRight;
			}
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				base.Session.OnBeforeItemChange(ItemChangeOperation.Submit, base.Session, null, this, callbackContext);
				this.OnBeforeSend();
				ConflictResolutionResult result = ConflictResolutionResult.Success;
				try
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
						((ICoreItem)this).MapiMessage.SubmitMessageEx(submitMessageExFlags);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreItem::Submit. The submit flags = {0}.", submitMessageExFlags),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSubmitMessage, ex2, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreItem::Submit. The submit flags = {0}.", submitMessageExFlags),
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
				catch (Exception)
				{
					result = new ConflictResolutionResult(SaveResult.IrresolvableConflict, null);
					throw;
				}
				finally
				{
					base.PropertyBag.Clear();
					base.Session.OnAfterItemChange(ItemChangeOperation.Submit, base.Session, null, this, result, callbackContext);
				}
			}
		}

		public void TransportSend(out PropertyDefinition[] propertyDefinitions, out object[] propertyValues)
		{
			this.CheckDisposed(null);
			PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(this);
			if (persistablePropertyBag == null)
			{
				throw new NotSupportedException("The item is not sendable.");
			}
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				base.Session.OnBeforeItemChange(ItemChangeOperation.Submit, base.Session, null, this, callbackContext);
				this.OnBeforeSend();
				ConflictResolutionResult result = ConflictResolutionResult.Success;
				try
				{
					PropValue[] mapiPropValues = null;
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
						((ICoreItem)this).MapiMessage.TransportSendMessage(out mapiPropValues);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotTransportSendMessage, ex, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreItem::TransportSend.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotTransportSendMessage, ex2, session, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("CoreItem::TransportSend.", new object[0]),
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
					NativeStorePropertyDefinition[] array;
					PropTag[] array2;
					PropertyTagCache.ResolveAndFilterPropertyValues(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, base.Session, persistablePropertyBag.MapiProp, persistablePropertyBag.ExTimeZone, mapiPropValues, out array, out array2, out propertyValues);
					propertyDefinitions = new PropertyDefinition[array.Length];
					for (int i = 0; i < propertyDefinitions.Length; i++)
					{
						propertyDefinitions[i] = array[i];
					}
				}
				catch (Exception)
				{
					result = new ConflictResolutionResult(SaveResult.IrresolvableConflict, null);
					throw;
				}
				finally
				{
					base.PropertyBag.Clear();
					base.Session.OnAfterItemChange(ItemChangeOperation.Submit, base.Session, null, this, result, callbackContext);
				}
			}
		}

		public void SetReadFlag(int flags, out bool hasChanged)
		{
			this.CheckDisposed(null);
			int num = (int)base.PropertyBag.TryGetProperty(CoreItemSchema.Flags);
			this.SetReadFlag(flags, true);
			MapiMessage mapiMessage = ((ICoreItem)this).MapiMessage;
			int num2 = 0;
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
				num2 = mapiMessage.GetProp(PropTag.MessageFlags).GetInt();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to set read flag on a message. Flags: {0}", flags),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to set read flag on a message. Flags: {0}", flags),
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
			if (!this.IsReadOnly)
			{
				base.PropertyBag[CoreItemSchema.Flags] = ((num2 & 769) | (num & -770));
			}
			hasChanged = (num2 != num);
		}

		public void SetReadFlag(int flags, bool deferErrors)
		{
			this.CheckDisposed(null);
			MapiMessage mapiMessage = ((ICoreItem)this).MapiMessage;
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
				if (deferErrors)
				{
					mapiMessage.SetReadFlag((SetReadFlags)(flags | 8));
				}
				else
				{
					mapiMessage.SetReadFlag((SetReadFlags)flags);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to set read flag on a message. Flags: {0}", flags),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to set read flag on a message. Flags: {0}", flags),
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
			if (base.Session.ActivitySession != null)
			{
				if ((112 & flags) != 0)
				{
					return;
				}
				if ((4 & flags) == 4)
				{
					base.Session.ActivitySession.CaptureMarkAsUnread(this);
					return;
				}
				base.Session.ActivitySession.CaptureMarkAsRead(this);
			}
		}

		public PropertyError[] CopyItem(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, NativeStorePropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationItem, "destinationItem");
			Util.ThrowOnNullArgument(excludeProperties, "excludeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			EnumValidator.ThrowIfInvalid<CopySubObjects>(copySubObjects, "copySubObjects");
			PropertyError[] result = CoreObject.MapiCopyTo(((ICoreItem)this).MapiMessage, destinationItem.MapiMessage, base.Session, destinationItem.Session, copyPropertiesFlags, copySubObjects, excludeProperties);
			destinationItem.Reload();
			destinationItem.SetIrresolvableChange();
			return result;
		}

		public PropertyError[] CopyProperties(ICoreItem destinationItem, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] includeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationItem, "destinationItem");
			Util.ThrowOnNullArgument(includeProperties, "includeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			PropertyError[] result = CoreObject.MapiCopyProps(((ICoreItem)this).MapiMessage, destinationItem.MapiMessage, base.Session, destinationItem.Session, copyPropertiesFlags, includeProperties);
			destinationItem.Reload();
			destinationItem.SetIrresolvableChange();
			return result;
		}

		public void ResetLegallyDirtyProperties()
		{
			this.CheckDisposed(null);
			this.legallyDirtyProperties = null;
		}

		public List<string> GetLegallyDirtyProperties()
		{
			this.CheckDisposed(null);
			List<string> result;
			lock (this)
			{
				if (this.legallyDirtyProperties == null)
				{
					result = null;
				}
				else
				{
					result = new List<string>(this.legallyDirtyProperties);
				}
			}
			return result;
		}

		public bool IsLegallyDirtyProperty(string property)
		{
			return this.legallyDirtyProperties != null && this.legallyDirtyProperties.Contains(property);
		}

		public void PopulateLegallyDirtyProperties(StoreSession session, bool verifyLegallyDirty)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(session, "session");
			List<string> collection;
			bool flag = COWDumpster.IsItemLegallyDirty(session, this, verifyLegallyDirty, out collection);
			lock (this)
			{
				if (!flag)
				{
					this.legallyDirtyProperties = null;
				}
				else
				{
					this.legallyDirtyProperties = new HashSet<string>(collection, StringComparer.OrdinalIgnoreCase);
				}
			}
		}

		CoreRecipientCollection ICoreItem.GetRecipientCollection(bool forceOpen)
		{
			this.CheckDisposed(null);
			if (!this.IsRecipientCollectionLoaded && forceOpen)
			{
				this.recipients = new CoreRecipientCollection(this);
			}
			return this.recipients;
		}

		void ICoreItem.OpenAttachmentCollection()
		{
			if (this.attachmentCollection == null)
			{
				this.InternalOpenAttachmentCollection(this, false);
			}
		}

		void ICoreItem.OpenAttachmentCollection(ICoreItem collectionOwner)
		{
			if (this.attachmentCollection == null)
			{
				this.InternalOpenAttachmentCollection(collectionOwner, true);
			}
		}

		void ICoreItem.DisposeAttachmentCollection()
		{
			this.CheckDisposed(null);
			if (this.attachmentCollection != null)
			{
				this.attachmentCollection.Dispose();
				this.attachmentCollection = null;
				this.AttachmentProvider.SetCollection(null);
			}
		}

		public ConflictResolutionResult InternalFlush(SaveMode saveMode, CallbackContext callbackContext)
		{
			return this.InternalFlush(saveMode, CoreItemOperation.Save, callbackContext);
		}

		public ConflictResolutionResult InternalFlush(SaveMode saveMode, CoreItemOperation operation, CallbackContext callbackContext)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode, "saveMode");
			TeamMailboxClientOperations.ThrowIfInvalidItemOperation(base.Session, this);
			this.OnBeforeFlush();
			this.CoreObjectUpdate(operation);
			if (base.Session != null)
			{
				if (base.Origin == Origin.Existing)
				{
					bool flag = false;
					bool verifyLegallyDirty = false;
					if (base.Session is MailboxSession)
					{
						flag = (((MailboxSession)base.Session).CowSession != null);
						verifyLegallyDirty = (flag && ((MailboxSession)base.Session).COWSettings.LegalHoldEnabled());
					}
					else if (base.Session is PublicFolderSession)
					{
						flag = (((PublicFolderSession)base.Session).CowSession != null);
					}
					if (flag)
					{
						this.PopulateLegallyDirtyProperties(base.Session, verifyLegallyDirty);
					}
				}
				base.Session.OnBeforeItemChange((base.Origin == Origin.New) ? ItemChangeOperation.Create : ItemChangeOperation.Update, base.Session, ((ICoreObject)this).StoreObjectId, this, callbackContext);
			}
			PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(this);
			if (persistablePropertyBag == null || !persistablePropertyBag.Context.IsValidationDisabled)
			{
				base.ValidateCoreObject();
			}
			if (((IValidatable)this).ValidateAllProperties)
			{
				this.Body.ValidateBody();
				if (ObjectClass.IsSmime(this.ClassName()) && !ObjectClass.IsSmimeClearSigned(this.ClassName()))
				{
					foreach (StorePropertyDefinition propertyDefinition in Body.BodyProps)
					{
						base.PropertyBag.Delete(propertyDefinition);
					}
				}
			}
			((ICoreItem)this).SaveRecipients();
			if (this.attachmentCollection != null && this.AttachmentCollection.IsDirty)
			{
				((ICoreItem)this).SetIrresolvableChange();
			}
			ConflictResolutionResult conflictResolutionResult = ConflictResolutionResult.Success;
			try
			{
				if (this.ShouldApplyACR(saveMode))
				{
					conflictResolutionResult = ((AcrPropertyBag)base.PropertyBag).FlushChangesWithAcr(saveMode);
				}
				else
				{
					CoreObject.GetPersistablePropertyBag(this).FlushChanges();
				}
			}
			finally
			{
				if (conflictResolutionResult.SaveStatus != SaveResult.Success && conflictResolutionResult.SaveStatus != SaveResult.SuccessWithConflictResolution && base.Session != null)
				{
					base.Session.OnAfterItemChange((base.Origin == Origin.New) ? ItemChangeOperation.Create : ItemChangeOperation.Update, base.Session, null, this, conflictResolutionResult, callbackContext);
				}
			}
			return conflictResolutionResult;
		}

		void ICoreItem.SaveRecipients()
		{
			this.CheckDisposed(null);
			if (this.IsRecipientCollectionDirty)
			{
				((ICoreItem)this).SetIrresolvableChange();
				this.recipients.Save();
			}
		}

		void ICoreItem.AbandonRecipientChanges()
		{
			this.CheckDisposed(null);
			Util.DisposeIfPresent(this.recipients);
			this.recipients = null;
		}

		void ICoreItem.Reload()
		{
			this.CheckDisposed(null);
			((ILocationIdentifierController)this).LocationIdentifierHelperInstance.SetLocationIdentifier(56351U, LastChangeAction.Reload);
			((ICoreObject)this).PropertyBag.Reload();
			((ICoreItem)this).AbandonRecipientChanges();
			this.ResetAttachmentCollection();
			((ICoreItem)this).Body.Reset();
		}

		void ICoreItem.SetIrresolvableChange()
		{
			this.CheckDisposed(null);
			AcrPropertyBag acrPropertyBag = base.PropertyBag as AcrPropertyBag;
			if (acrPropertyBag != null)
			{
				acrPropertyBag.SetIrresolvableChange();
			}
		}

		public void GetCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(stringBuilder, "stringBuilder");
			EnumValidator.ThrowIfInvalid<CharsetDetectionDataFlags>(flags, "flags");
			bool isComplete = (flags & CharsetDetectionDataFlags.Complete) == CharsetDetectionDataFlags.Complete;
			this.GetPropertyCharsetDetectionData(stringBuilder, isComplete);
			this.GetAttachmentCharsetDetectionData(stringBuilder, isComplete);
			this.GetRecipientCharsetDetectionData(stringBuilder, isComplete);
			this.context.GetContextCharsetDetectionData(stringBuilder, flags);
		}

		void ICoreItem.SetCoreItemContext(ICoreItemContext context)
		{
			this.CheckDisposed(null);
			this.context = context;
		}

		internal static void CopyItemContentExcept(ICoreItem sourceItem, ICoreItem destinationItem, HashSet<NativeStorePropertyDefinition> excludeProperties)
		{
			CoreItem.CopyItemContent(sourceItem, destinationItem, null, excludeProperties);
		}

		internal static void CopyItemContent(ICoreItem sourceItem, ICoreItem destinationItem)
		{
			CoreItem.CopyItemContent(sourceItem, destinationItem, null, null);
		}

		internal static void CopyItemContent(ICoreItem sourceItem, ICoreItem destinationItem, IEnumerable<NativeStorePropertyDefinition> properties)
		{
			CoreItem.CopyItemContent(sourceItem, destinationItem, properties, null);
		}

		private static void CopyItemContent(ICoreItem sourceItem, ICoreItem destinationItem, IEnumerable<NativeStorePropertyDefinition> properties, HashSet<NativeStorePropertyDefinition> excludeProperties)
		{
			if (properties == null)
			{
				sourceItem.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				properties = CoreObject.GetPersistablePropertyBag(sourceItem).AllNativeProperties;
			}
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in properties)
			{
				if (excludeProperties == null || !excludeProperties.Contains(nativeStorePropertyDefinition))
				{
					PersistablePropertyBag.CopyProperty(CoreObject.GetPersistablePropertyBag(sourceItem), nativeStorePropertyDefinition, CoreObject.GetPersistablePropertyBag(destinationItem));
				}
			}
			destinationItem.Recipients.CopyRecipientsFrom(sourceItem.Recipients);
			CoreAttachmentCollection.CloneAttachmentCollection(sourceItem, destinationItem);
			CoreObject.GetPersistablePropertyBag(destinationItem).SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreUnresolvedHeaders);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreItem>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.recipients != null)
				{
					this.recipients.Dispose();
					this.recipients = null;
				}
				if (this.attachmentCollection != null)
				{
					this.attachmentCollection.Dispose();
					this.attachmentCollection = null;
				}
				if (this.attachmentProvider != null)
				{
					this.attachmentProvider.Dispose();
					this.attachmentProvider = null;
				}
			}
			base.InternalDispose(disposing);
		}

		protected override Schema GetSchema()
		{
			string valueOrDefault = base.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.ItemClass, string.Empty);
			Schema schema = ObjectClass.GetSchema(valueOrDefault);
			if (!(schema is ItemSchema))
			{
				schema = ItemSchema.Instance;
			}
			ICollection<PropertyDefinition> propsToLoad = ((this.itemBindOption & ItemBindOption.LoadRequiredPropertiesOnly) == ItemBindOption.LoadRequiredPropertiesOnly) ? schema.RequiredAutoloadProperties : schema.AutoloadProperties;
			base.PropertyBag.Load(propsToLoad);
			return schema;
		}

		protected override void ValidateContainedObjects(IList<StoreObjectValidationError> validationErrors)
		{
			base.ValidateContainedObjects(validationErrors);
			if (this.IsRecipientCollectionLoaded)
			{
				foreach (CoreRecipient coreRecipient in this.recipients)
				{
					ValidationContext validationContext = new ValidationContext(base.Session);
					((IValidatable)coreRecipient).Validate(validationContext, validationErrors);
				}
			}
		}

		private static bool ImportOperationSucceeded(ImportResult result, out LocalizedException exception)
		{
			exception = null;
			switch (result)
			{
			case ImportResult.Success:
			case ImportResult.SyncClientChangeNewer:
			case ImportResult.SyncIgnore:
				return true;
			}
			exception = new ImportException(ServerStrings.CannotImportMessageMove, result);
			return false;
		}

		private void SetClientAndServerIPs()
		{
			if (base.Session != null && base.PropertyBag != null)
			{
				IPAddress clientIPAddress = base.Session.ClientIPAddress;
				IPAddress serverIPAddress = base.Session.ServerIPAddress;
				if (clientIPAddress.Equals(serverIPAddress) && clientIPAddress.Equals(IPAddress.IPv6Loopback))
				{
					return;
				}
				base.PropertyBag.SetProperty(CoreItemSchema.XMsExchOrganizationOriginalClientIPAddress, clientIPAddress.ToString());
				base.PropertyBag.SetProperty(CoreItemSchema.XMsExchOrganizationOriginalServerIPAddress, serverIPAddress.ToString());
			}
		}

		private void InternalOpenAttachmentCollection(ICoreItem owner, bool forceReadOnly)
		{
			bool valueOrDefault = owner.PropertyBag.GetValueOrDefault<bool>(CoreItemSchema.MapiHasAttachment, true);
			this.attachmentCollection = new CoreAttachmentCollection(this.AttachmentProvider, owner, forceReadOnly, valueOrDefault);
			this.AttachmentProvider.SetCollection(this.attachmentCollection);
		}

		private void ResetAttachmentCollection()
		{
			this.CheckDisposed(null);
			if (this.attachmentCollection != null)
			{
				this.attachmentCollection.Reset();
			}
		}

		private void CoreObjectUpdate(CoreItemOperation operation)
		{
			this.coreObjectUpdateSchema = (ItemSchema)this.GetSchema();
			this.coreObjectUpdateSchema.CoreObjectUpdate(this, operation);
		}

		private void CoreObjectUpdateComplete(SaveResult saveResult)
		{
			if (this.coreObjectUpdateSchema != null)
			{
				this.coreObjectUpdateSchema.CoreObjectUpdateComplete(this, saveResult);
				this.coreObjectUpdateSchema = null;
			}
		}

		private bool ShouldApplyACR(SaveMode saveMode)
		{
			return base.PropertyBag is AcrPropertyBag && saveMode != SaveMode.NoConflictResolution && saveMode != SaveMode.NoConflictResolutionForceSave;
		}

		private void OnBeforeFlush()
		{
			if (this.beforeFlushEventHandler != null)
			{
				this.beforeFlushEventHandler();
			}
		}

		private void OnBeforeSend()
		{
			if (!base.Session.CheckSubmissionQuota(this.Recipients.Count))
			{
				throw new SubmissionQuotaExceededException(ServerStrings.ExSubmissionQuotaExceeded);
			}
			this.EnsureNoUnresolvedRecipients();
			if (!base.PropertyBag.GetValueOrDefault<bool>(CoreItemSchema.IsResend))
			{
				this.Recipients.FilterRecipients((CoreRecipient recipient) => recipient.RecipientItemType != RecipientItemType.P1);
			}
			if (PropertyError.IsPropertyNotFound(base.PropertyBag.TryGetProperty(CoreItemSchema.DavSubmitData)) && this.Recipients.Count == 0)
			{
				throw new InvalidRecipientsException(ServerStrings.ExCantSubmitWithoutRecipients, null);
			}
			this.SetClientAndServerIPs();
			bool isFlushNeeded = this.IsFlushNeeded;
			if (this.beforeSendEventHandler != null)
			{
				this.beforeSendEventHandler();
			}
			if (this.IsFlushNeeded || !isFlushNeeded)
			{
				using (CallbackContext callbackContext = new CallbackContext(base.Session))
				{
					ConflictResolutionResult conflictResolutionResult = this.InternalFlush(SaveMode.ResolveConflicts, CoreItemOperation.Send, callbackContext);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						throw new SaveConflictException(ServerStrings.MapiCannotSubmitMessage, conflictResolutionResult);
					}
				}
			}
		}

		private void EnsureNoUnresolvedRecipients()
		{
			foreach (CoreRecipient coreRecipient in this.Recipients)
			{
				if (string.IsNullOrEmpty(coreRecipient.PropertyBag.GetValueOrDefault<string>(RecipientSchema.EmailAddrType)))
				{
					throw new InvalidRecipientsException(ServerStrings.ExUnresolvedRecipient(coreRecipient.PropertyBag.GetValueOrDefault<string>(RecipientSchema.EmailDisplayName)), null);
				}
			}
		}

		private void GetPropertyCharsetDetectionData(StringBuilder stringBuilder, bool isComplete)
		{
			foreach (StorePropertyDefinition propertyDefinition in this.GetSchema().DetectCodepageProperties)
			{
				if (isComplete || base.PropertyBag.IsPropertyDirty(propertyDefinition))
				{
					object obj = base.PropertyBag.TryGetProperty(propertyDefinition);
					if (obj is string)
					{
						stringBuilder.AppendLine(obj as string);
					}
					else if (obj is string[])
					{
						foreach (string value in obj as string[])
						{
							stringBuilder.AppendLine(value);
						}
					}
				}
			}
		}

		private void GetAttachmentCharsetDetectionData(StringBuilder stringBuilder, bool isComplete)
		{
			if ((isComplete || (((ICoreItem)this).IsAttachmentCollectionLoaded && this.AttachmentCollection.IsDirty)) && (base.Origin != Origin.New || ((ICoreItem)this).IsAttachmentCollectionLoaded))
			{
				this.AttachmentCollection.GetCharsetDetectionData(stringBuilder);
			}
		}

		private void GetRecipientCharsetDetectionData(StringBuilder stringBuilder, bool isComplete)
		{
			((ICoreItem)this).GetRecipientCollection(false);
			if (isComplete || this.recipients != null)
			{
				foreach (CoreRecipient coreRecipient in this.Recipients)
				{
					coreRecipient.GetCharsetDetectionData(stringBuilder);
				}
			}
		}

		public ConflictResolutionResult InternalSave(SaveMode saveMode, CallbackContext callbackContext)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode, "saveMode");
			TeamMailboxClientOperations.ThrowIfInvalidItemOperation(base.Session, this);
			ConflictResolutionResult conflictResolutionResult = ConflictResolutionResult.Failure;
			using (MailboxEvaluationResult mailboxEvaluationResult = (base.Session != null) ? base.Session.EvaluateFolderRules(this, null) : null)
			{
				if (base.Session != null)
				{
					base.Session.OnBeforeItemSave((base.Origin == Origin.New) ? ItemChangeOperation.Create : ItemChangeOperation.Update, base.Session, ((ICoreObject)this).StoreObjectId, this, callbackContext);
					callbackContext.ItemOperationAuditInfo = new ItemAuditInfo((base.Id == null) ? null : base.Id.ObjectId, null, null, base.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string, base.PropertyBag.TryGetProperty(ItemSchema.From) as Participant, base.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsAssociated), this.GetLegallyDirtyProperties());
				}
				try
				{
					bool flag = false;
					if (base.Session != null)
					{
						FolderRuleEvaluationStatus folderRuleEvaluationStatus = base.Session.ExecuteFolderRulesOnBefore(mailboxEvaluationResult);
						if (FolderRuleEvaluationStatus.InterruptWithException == folderRuleEvaluationStatus)
						{
							throw new StoragePermanentException(ServerStrings.FolderRuleCannotSaveItem);
						}
						if (FolderRuleEvaluationStatus.InterruptSilently == folderRuleEvaluationStatus)
						{
							flag = true;
							conflictResolutionResult = ConflictResolutionResult.SuccessWithoutSaving;
						}
					}
					if (!flag)
					{
						if (this.ShouldApplyACR(saveMode))
						{
							conflictResolutionResult = ((AcrPropertyBag)base.PropertyBag).SaveChangesWithAcr(saveMode);
							if (conflictResolutionResult.SaveStatus == SaveResult.SuccessWithConflictResolution)
							{
								((ICoreItem)this).AbandonRecipientChanges();
							}
						}
						else
						{
							bool force = (saveMode & SaveMode.NoConflictResolutionForceSave) == SaveMode.NoConflictResolutionForceSave;
							CoreObject.GetPersistablePropertyBag(this).SaveChanges(force);
							conflictResolutionResult = ConflictResolutionResult.Success;
						}
						this.CoreObjectUpdateComplete(conflictResolutionResult.SaveStatus);
						this.LocationIdentifierHelperInstance.ResetChangeList();
					}
					if (this.attachmentCollection != null)
					{
						this.attachmentCollection.OnAfterCoreItemSave(conflictResolutionResult.SaveStatus);
					}
				}
				finally
				{
					if (base.Session != null && SaveResult.IrresolvableConflict != conflictResolutionResult.SaveStatus)
					{
						base.Session.ExecuteFolderRulesOnAfter(mailboxEvaluationResult);
					}
					if (base.Session != null)
					{
						base.Session.OnAfterItemSave((base.Origin == Origin.New) ? ItemChangeOperation.Create : ItemChangeOperation.Update, base.Session, null, this, conflictResolutionResult, callbackContext);
					}
				}
			}
			base.Origin = Origin.Existing;
			((ICoreObject)this).ResetId();
			this.Body.Reset();
			return conflictResolutionResult;
		}

		private static readonly IList<PropertyDefinition> importChangeProperties = new ReadOnlyCollection<PropertyDefinition>(new PropertyDefinition[]
		{
			CoreObjectSchema.SourceKey,
			CoreObjectSchema.ChangeKey,
			CoreObjectSchema.LastModifiedTime,
			CoreObjectSchema.PredecessorChangeList
		});

		private readonly ItemCharsetDetector charsetDetector;

		private ItemBindOption itemBindOption;

		private CoreAttachmentCollection attachmentCollection;

		private IAttachmentProvider attachmentProvider;

		private CoreRecipientCollection recipients;

		private ICoreItem topLevelItem;

		private Action beforeSendEventHandler;

		private Action beforeFlushEventHandler;

		private Body body;

		private ICoreItemContext context = NullCoreItemContext.Instance;

		private Dictionary<ItemSchema, object> coreObjectUpdateContext;

		private ItemSchema coreObjectUpdateSchema;

		private LocationIdentifierHelper locationIdentifierHelperInstance;

		private HashSet<string> legallyDirtyProperties;
	}
}
