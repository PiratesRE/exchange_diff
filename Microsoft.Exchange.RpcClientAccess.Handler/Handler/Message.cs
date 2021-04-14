using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Message : PropertyServerObject
	{
		internal Message(CoreItem coreItem, Logon logon, Encoding string8Encoding) : this(coreItem, logon, string8Encoding, ClientSideProperties.MessageInstance, PropertyConverter.Message)
		{
		}

		internal Message(CoreItem coreItem, Logon logon, Encoding string8Encoding, ClientSideProperties clientSideProperties, PropertyConverter converter) : base(logon, clientSideProperties, converter)
		{
			this.recipientTranslator = new RecipientTranslator(coreItem, Array<PropertyTag>.Empty, string8Encoding);
			this.coreItemReference = new ReferenceCount<CoreItem>(coreItem);
			this.string8Encoding = string8Encoding;
			this.bestBodyCoreObjectProperties = new BestBodyCoreObjectProperties(coreItem, coreItem.PropertyBag, string8Encoding, new Func<BodyReadConfiguration, Stream>(this.GetBodyConversionStreamCallback));
			this.propertyDefinitionFactory = new CoreObjectPropertyDefinitionFactory(coreItem.Session, coreItem.PropertyBag);
			this.CoreItem.BeforeFlush += this.OnBeforeFlush;
		}

		public override Encoding String8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
		}

		protected override IPropertyDefinitionFactory PropertyDefinitionFactory
		{
			get
			{
				return this.propertyDefinitionFactory;
			}
		}

		protected override IStorageObjectProperties StorageObjectProperties
		{
			get
			{
				return this.bestBodyCoreObjectProperties;
			}
		}

		public ICorePropertyBag PropertyBag
		{
			get
			{
				return this.coreItemReference.ReferencedObject.PropertyBag;
			}
		}

		public override StoreSession Session
		{
			get
			{
				return this.coreItemReference.ReferencedObject.Session;
			}
		}

		public override Schema Schema
		{
			get
			{
				return MessageItemSchema.Instance;
			}
		}

		public CoreItem CoreItem
		{
			get
			{
				return this.coreItemReference.ReferencedObject;
			}
		}

		public PropertyTag[] ExtraRecipientPropertyTags
		{
			get
			{
				return this.recipientTranslator.ExtraPropertyTags;
			}
		}

		protected override bool SupportsPropertyProblems
		{
			get
			{
				return false;
			}
		}

		protected ReferenceCount<CoreItem> ReferenceCoreItem
		{
			get
			{
				return this.coreItemReference;
			}
		}

		private ReferenceCount<CoreItem> CoreItemReference
		{
			get
			{
				return this.coreItemReference;
			}
		}

		public void OnBeforeFlush()
		{
			if (!this.bestBodyCoreObjectProperties.BodyHelper.IsOpeningStream)
			{
				this.bestBodyCoreObjectProperties.ResetBody();
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).RpcClientAccess.DetectCharsetAndConvertHtmlBodyOnSave.Enabled && this.CoreItem.PropertyBag.IsPropertyDirty(ItemSchema.HtmlBody) && !this.CoreItem.PropertyBag.IsPropertyDirty(ItemSchema.InternetCpid) && ItemCharsetDetector.IsMultipleLanguageCodePage(this.CoreItem.PropertyBag.GetValueOrDefault<int>(ItemSchema.InternetCpid)))
			{
				this.CoreItem.Body.ResetBodyFormat();
				this.CoreItem.Body.ForceRedetectHtmlBodyCharset = true;
			}
		}

		public Attachment CreateAttachment()
		{
			CoreAttachmentCollection attachmentCollection = ((ICoreItem)this.CoreItem).AttachmentCollection;
			Attachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = attachmentCollection.Create(AttachmentType.NoAttachment);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				int attachmentNumber = coreAttachment.AttachmentNumber;
				Attachment attachment = new Attachment(coreAttachment, this.CoreItemReference, base.LogonObject, this.string8Encoding);
				if (this.ignorePropertySaveErrors)
				{
					attachment.IgnorePropertySaveErrors();
				}
				disposeGuard.Success();
				result = attachment;
			}
			return result;
		}

		public bool DeleteAttachment(uint attachmentNumber)
		{
			CoreAttachmentCollection attachmentCollection = this.CoreItem.AttachmentCollection;
			foreach (AttachmentHandle attachmentHandle in attachmentCollection)
			{
				if (attachmentHandle.AttachNumber == (int)attachmentNumber)
				{
					attachmentCollection.Remove(attachmentHandle);
					this.CoreItem.Flush(SaveMode.FailOnAnyConflict);
					this.CoreItem.PropertyBag.Clear();
					this.CoreItem.PropertyBag.Load(null);
					return true;
				}
			}
			return false;
		}

		public void ModifyRecipients(PropertyTag[] extraPropertyTags, IEnumerable<RecipientRow> recipientRows, Action<RecipientTranslationException> recipientTranslationFailureObserver)
		{
			Util.ThrowOnNullArgument(extraPropertyTags, "extraPropertyTags");
			Util.ThrowOnNullArgument(recipientRows, "recipientRows");
			this.recipientTranslator.ExtraPropertyTags = extraPropertyTags;
			foreach (RecipientRow recipientRow in recipientRows)
			{
				try
				{
					this.recipientTranslator.SetRecipientRow(recipientRow);
				}
				catch (RecipientTranslationException obj)
				{
					if (recipientTranslationFailureObserver != null)
					{
						recipientTranslationFailureObserver(obj);
					}
					this.TryStubRecipient(recipientRow);
				}
			}
		}

		public AttachmentView GetAttachmentTable(Logon logon, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle)
		{
			RopHandler.CheckEnum<TableFlags>(tableFlags);
			TableFlags tableFlags2 = TableFlags.DeferredErrors | TableFlags.NoNotifications | TableFlags.MapiUnicode | TableFlags.SuppressNotifications;
			if ((byte)(tableFlags & ~tableFlags2) != 0)
			{
				throw new RopExecutionException(string.Format("Flags {0} not supported on AttachmentViews.", tableFlags), (ErrorCode)2147746050U);
			}
			return new AttachmentView(logon, this.CoreItemReference, TableFlags.NoNotifications | tableFlags, notificationHandler, returnNotificationHandle);
		}

		public void RemoveAllRecipients()
		{
			this.recipientTranslator.RemoveAllRecipients();
		}

		public bool TryOpenAttachment(OpenMode openMode, uint attachmentNumber, out Attachment attachment)
		{
			if (openMode != OpenMode.ReadOnly && openMode != OpenMode.ReadWrite && openMode != OpenMode.BestAccess)
			{
				throw new RopExecutionException(string.Format("OpenMode {0} not supported.", openMode), (ErrorCode)2147746050U);
			}
			CoreAttachmentCollection attachmentCollection = ((ICoreItem)this.CoreItem).AttachmentCollection;
			foreach (AttachmentHandle attachmentHandle in attachmentCollection)
			{
				if (attachmentHandle.AttachNumber == (int)attachmentNumber)
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						CoreAttachment coreAttachment = attachmentCollection.Open(attachmentHandle);
						disposeGuard.Add<CoreAttachment>(coreAttachment);
						if (ExTraceGlobals.MessageTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							string arg = (this.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string) ?? "<null>";
							ExTraceGlobals.MessageTracer.TraceDebug<uint, string, string>((long)this.GetHashCode(), "TryOpenAttachment. #{0}. OpenMode: {1}, ParentMessage: \"{2}\".", attachmentNumber, openMode.ToString(), arg);
						}
						attachment = new Attachment(coreAttachment, this.CoreItemReference, base.LogonObject, this.string8Encoding);
						if (this.ignorePropertySaveErrors)
						{
							attachment.IgnorePropertySaveErrors();
						}
						disposeGuard.Success();
						return true;
					}
				}
			}
			attachment = null;
			return false;
		}

		public RecipientCollector ReadRecipients(uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, Func<PropertyTag[], RecipientCollector> createRecipientCollectorDelegate)
		{
			Util.ThrowOnNullArgument(extraUnicodePropertyTags, "extraUnicodePropertyTags");
			RecipientCollector result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				this.recipientTranslator.ExtraUnicodePropertyTags = extraUnicodePropertyTags;
				RecipientCollector recipientCollector = createRecipientCollectorDelegate(this.recipientTranslator.ExtraPropertyTags);
				disposeGuard.Add<RecipientCollector>(recipientCollector);
				bool flag = false;
				foreach (RecipientRow row in this.recipientTranslator.GetRecipientRows((int)recipientRowId))
				{
					if (!recipientCollector.TryAddRecipientRow(row))
					{
						if (!flag)
						{
							throw new RopExecutionException("Unable to add even one recipient to the RecipientCollector.", ErrorCode.BufferTooSmall);
						}
						break;
					}
					else
					{
						flag = true;
					}
				}
				disposeGuard.Success();
				result = recipientCollector;
			}
			return result;
		}

		public RecipientCollector ReloadCachedInformation(PropertyTag[] extraUnicodePropertyTags, Func<MessageHeader, PropertyTag[], Encoding, RecipientCollector> createRecipientCollectorDelegate)
		{
			Util.ThrowOnNullArgument(extraUnicodePropertyTags, "extraUnicodePropertyTags");
			MessageHeader messageHeader = Message.GetMessageHeader(this.CoreItem);
			RecipientCollector result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				this.recipientTranslator.ExtraUnicodePropertyTags = extraUnicodePropertyTags;
				RecipientCollector recipientCollector = createRecipientCollectorDelegate(messageHeader, this.recipientTranslator.ExtraPropertyTags, String8Encodings.TemporaryDefault);
				disposeGuard.Add<RecipientCollector>(recipientCollector);
				foreach (RecipientRow row in this.recipientTranslator.GetRecipientRows(0))
				{
					if (!recipientCollector.TryAddRecipientRow(row))
					{
						break;
					}
				}
				disposeGuard.Success();
				result = recipientCollector;
			}
			return result;
		}

		public virtual StoreId SaveChanges(SaveChangesMode saveChangesMode)
		{
			if (ExTraceGlobals.MessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = (this.PropertyBag.TryGetProperty(ItemSchema.Subject) as string) ?? "<null>";
				ExTraceGlobals.MessageTracer.TraceDebug<SaveChangesMode, string>((long)this.GetHashCode(), "SaveChangesMessage. SaveChangesMode = {0}, Subject: \"{1}\".", saveChangesMode, arg);
			}
			if ((byte)(saveChangesMode & (SaveChangesMode.TransportDelivery | SaveChangesMode.IMAPChange | SaveChangesMode.ForceNotificationPublish)) != 0)
			{
				throw new RopExecutionException(string.Format("SaveChangesMode {0} is not supported.", saveChangesMode), (ErrorCode)2147746050U);
			}
			if ((byte)(saveChangesMode & SaveChangesMode.KeepOpenReadOnly) == 1 && (byte)(saveChangesMode & SaveChangesMode.KeepOpenReadWrite) == 2)
			{
				throw new RopExecutionException(string.Format("SaveChangesMode {0} is not supported.", saveChangesMode), (ErrorCode)2147746050U);
			}
			Feature.Stubbed(54480, "SaveChangesMode flags not supported. SaveChangesModes=" + saveChangesMode);
			this.bestBodyCoreObjectProperties.ResetBody();
			if (this.SaveChangesToLinkedDocumentLibraryIfNecessary())
			{
				((MailboxSession)this.Session).TryToSyncSiteMailboxNow();
			}
			else
			{
				SaveMode saveMode = ((byte)(saveChangesMode & SaveChangesMode.ForceSave) == 4) ? SaveMode.NoConflictResolutionForceSave : SaveMode.ResolveConflicts;
				ConflictResolutionResult conflictResolutionResult = this.CoreItem.Save(saveMode);
				if (conflictResolutionResult.SaveStatus == SaveResult.SuccessWithoutSaving)
				{
					return StoreId.Empty;
				}
				if (conflictResolutionResult.SaveStatus != SaveResult.Success && conflictResolutionResult.SaveStatus != SaveResult.SuccessWithConflictResolution)
				{
					throw new RopExecutionException(string.Format("SaveChangesMessage failed due to conflict {0}.", conflictResolutionResult), (ErrorCode)2147746057U);
				}
			}
			this.CoreItem.PropertyBag.Load(null);
			return this.GetMessageIdAfterSave();
		}

		public bool SetReadFlag(SetReadFlagFlags flags)
		{
			if ((byte)(flags & ~(SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)) != 0)
			{
				throw new RopExecutionException(string.Format("SetReadFlagFlags {0} is not supported.", flags), (ErrorCode)2147746050U);
			}
			bool result;
			this.CoreItem.SetReadFlag((int)flags, out result);
			return result;
		}

		public void SubmitMessage(SubmitMessageFlags submitFlags)
		{
			if ((byte)(submitFlags & ~(SubmitMessageFlags.Preprocess | SubmitMessageFlags.NeedsSpooler)) != 0)
			{
				throw new RopExecutionException(string.Format("SubmitMessageFlags {0} is not supported.", submitFlags), (ErrorCode)2147746050U);
			}
			SubmitMessageFlags submitMessageFlags = SubmitMessageFlags.None;
			if ((byte)(submitFlags & SubmitMessageFlags.Preprocess) != 0)
			{
				submitMessageFlags |= SubmitMessageFlags.Preprocess;
			}
			if ((byte)(submitFlags & SubmitMessageFlags.NeedsSpooler) != 0)
			{
				submitMessageFlags |= SubmitMessageFlags.NeedsSpooler;
			}
			this.UpdateSecureSubmitFlags();
			this.bestBodyCoreObjectProperties.ResetBody();
			this.CoreItem.Submit(submitMessageFlags);
		}

		public PropertyValue[] TransportSend()
		{
			this.UpdateSecureSubmitFlags();
			this.bestBodyCoreObjectProperties.ResetBody();
			PropertyDefinition[] properties;
			object[] xsoPropertyValues;
			this.CoreItem.TransportSend(out properties, out xsoPropertyValues);
			bool flag = true;
			ICollection<PropertyTag> propertyTags = MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(this.Session, properties, flag);
			PropertyValue[] array = MEDSPropertyTranslator.TranslatePropertyValues(this.Session, propertyTags, xsoPropertyValues, flag);
			base.PropertyConverter.ConvertPropertyValuesToClientAndSuppressClientSide(this.Session, this.StorageObjectProperties, array, null, base.ClientSideProperties);
			return array;
		}

		internal static MessageHeader GetMessageHeader(ICoreItem coreItem)
		{
			return new MessageHeader(true, true, coreItem.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.SubjectPrefix), coreItem.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.NormalizedSubject), (ushort)coreItem.Recipients.Count);
		}

		internal void IgnorePropertySaveErrors()
		{
			this.ignorePropertySaveErrors = true;
			this.coreItemReference.ReferencedObject.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
		}

		internal bool AddRecipients(RecipientCollector recipientCollector)
		{
			bool result = false;
			foreach (RecipientRow row in this.recipientTranslator.GetRecipientRows(0))
			{
				if (!recipientCollector.TryAddRecipientRow(row))
				{
					break;
				}
				result = true;
			}
			return result;
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyTo()
		{
			this.IgnorePropertySaveErrors();
			FastTransferUpload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessage message = this.CreateUploadMessageAdaptor();
				disposeGuard.Add<IMessage>(message);
				FastTransferMessageCopyTo fastTransferMessageCopyTo = new FastTransferMessageCopyTo(false, message, true);
				disposeGuard.Add<FastTransferMessageCopyTo>(fastTransferMessageCopyTo);
				FastTransferUpload fastTransferUpload = new FastTransferUpload(fastTransferMessageCopyTo, PropertyFilterFactory.IncludeAllFactory, base.LogonObject);
				disposeGuard.Add<FastTransferUpload>(fastTransferUpload);
				disposeGuard.Success();
				result = fastTransferUpload;
			}
			return result;
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyProperties()
		{
			return this.InternalFastTransferDestinationCopyTo();
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyTo(bool isShallowCopy, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludePropertyTags)
		{
			if (ExTraceGlobals.MessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = (this.PropertyBag.TryGetProperty(ItemSchema.Subject) as string) ?? "<null>";
				ExTraceGlobals.MessageTracer.TraceDebug<string>((long)this.GetHashCode(), "CreateSourceCopyToDownload. Subject: \"{0}\"", arg);
			}
			DownloadBodyOption downloadBodyOption = ((flags & FastTransferCopyFlag.BestBody) == FastTransferCopyFlag.BestBody) ? DownloadBodyOption.BestBodyOnly : DownloadBodyOption.AllBodyProperties;
			if (downloadBodyOption == DownloadBodyOption.BestBodyOnly)
			{
				BodyHelper.RemoveBodyProperties(ref excludePropertyTags);
			}
			return this.InternalFastTransferSourceCopy(isShallowCopy, downloadBodyOption, sendOptions, false, excludePropertyTags, false);
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyProperties(bool isShallowCopy, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] includePropertyTags)
		{
			return this.InternalFastTransferSourceCopy(isShallowCopy, DownloadBodyOption.AllBodyProperties, sendOptions, true, includePropertyTags, true);
		}

		protected override PropertyError[] InternalCopyTo(PropertyServerObject destinationPropertyServerObject, CopySubObjects copySubObjects, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] excludeProperties)
		{
			Message message = RopHandler.Downcast<Message>(destinationPropertyServerObject);
			this.PrepareForCopy(message);
			return this.CoreItem.CopyItem(message.CoreItem, copyPropertiesFlags, copySubObjects, excludeProperties);
		}

		protected override PropertyError[] InternalCopyProperties(PropertyServerObject destinationPropertyServerObject, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] properties)
		{
			Message message = RopHandler.Downcast<Message>(destinationPropertyServerObject);
			this.PrepareForCopy(message);
			return this.CoreItem.CopyProperties(message.CoreItem, copyPropertiesFlags, properties);
		}

		protected override void FixBodyPropertiesIfNeeded(PropertyValue[] values)
		{
			this.bestBodyCoreObjectProperties.BodyHelper.FixupProperties(values, FixupMapping.GetProperties);
		}

		protected override bool TryGetOneOffPropertyStream(PropertyTag propertyTag, OpenMode openMode, bool isAppend, out Stream momtStream, out uint length)
		{
			momtStream = null;
			length = 0U;
			if (propertyTag.IsBodyProperty() && (openMode == OpenMode.ReadOnly || openMode == OpenMode.ReadWrite) && this.bestBodyCoreObjectProperties.BodyHelper.IsConversionNeeded(propertyTag))
			{
				try
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						using (StreamSource streamSource = this.GetStreamSource())
						{
							if (openMode == OpenMode.ReadOnly)
							{
								momtStream = Message.BuildBodyConversionStream(base.LogonObject, streamSource, propertyTag, isAppend, this.bestBodyCoreObjectProperties.BodyHelper);
							}
							else
							{
								momtStream = Message.BuildUpgradeableBodyConversionStream(base.LogonObject, streamSource, propertyTag, isAppend, this.String8Encoding, this.bestBodyCoreObjectProperties.BodyHelper);
							}
							disposeGuard.Add<Stream>(momtStream);
						}
						length = momtStream.GetSize();
						disposeGuard.Success();
						return true;
					}
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			return base.TryGetOneOffPropertyStream(propertyTag, openMode, isAppend, out momtStream, out length);
		}

		protected override bool ShouldSkipPropertyChange(StorePropertyDefinition propertyDefinition)
		{
			bool isLinked = TeamMailboxClientOperations.IsLinked(this.PropertyBag);
			return MessagePropertyRestriction.Instance.ShouldBlock(propertyDefinition, isLinked);
		}

		protected virtual StoreId GetMessageIdAfterSave()
		{
			ICoreItem coreItem = this.CoreItem;
			return new StoreId(coreItem.Session.IdConverter.GetMidFromMessageId(coreItem.StoreObjectId));
		}

		protected virtual MessageAdaptor CreateDownloadMessageAdaptor(DownloadBodyOption downloadBodyOption, FastTransferSendOption sendOptions, bool isFastTransferCopyProperties)
		{
			this.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
			return new MessageAdaptor(this.bestBodyCoreObjectProperties, this.ReferenceCoreItem, new MessageAdaptor.Options
			{
				IsReadOnly = true,
				IsEmbedded = false,
				DownloadBodyOption = downloadBodyOption,
				IsUpload = sendOptions.IsUpload(),
				IsFastTransferCopyProperties = isFastTransferCopyProperties
			}, base.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), null);
		}

		protected virtual MessageAdaptor CreateUploadMessageAdaptor()
		{
			return new MessageAdaptor(this.bestBodyCoreObjectProperties, this.ReferenceCoreItem, new MessageAdaptor.Options
			{
				IsReadOnly = false,
				IsEmbedded = false,
				DownloadBodyOption = DownloadBodyOption.AllBodyProperties
			}, base.LogonObject.LogonString8Encoding, true, base.LogonObject);
		}

		protected virtual bool SaveChangesToLinkedDocumentLibraryIfNecessary()
		{
			bool result;
			try
			{
				result = TeamMailboxExecutionHelper.SaveChangesToLinkedDocumentLibraryIfNecessary(this.CoreItem, base.LogonObject);
			}
			catch (StoragePermanentException e)
			{
				TeamMailboxExecutionHelper.LogServerFailures(this.CoreItem, base.LogonObject, e);
				throw;
			}
			catch (StorageTransientException e2)
			{
				TeamMailboxExecutionHelper.LogServerFailures(this.CoreItem, base.LogonObject, e2);
				throw;
			}
			return result;
		}

		protected override StreamSource GetStreamSource()
		{
			return new StreamSource<CoreItem>(this.coreItemReference, (CoreItem coreItem) => coreItem.PropertyBag);
		}

		protected override PropertyTag[] AdditionalPropertiesForGetPropertiesAll(bool useUnicodeType)
		{
			if (useUnicodeType)
			{
				return BodyHelper.AllBodyPropertiesUnicode;
			}
			return BodyHelper.AllBodyPropertiesString8;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Message>(this);
		}

		protected override void InternalDispose()
		{
			ExTraceGlobals.MessageTracer.TraceDebug((long)this.GetHashCode(), "DisposeMessage.");
			this.CoreItem.BeforeFlush -= this.OnBeforeFlush;
			this.coreItemReference.Release();
			base.InternalDispose();
		}

		private static Stream BuildConversionStream(PropertyTag propertyTag, BodyHelper bodyHelper)
		{
			return bodyHelper.GetConversionStream(propertyTag);
		}

		private static BodyConversionStream BuildBodyConversionStream(Logon logon, StreamSource streamSource, PropertyTag propertyTag, bool isAppend, BodyHelper bodyHelper)
		{
			BodyConversionStream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StreamSource streamSource2 = streamSource.Duplicate();
				disposeGuard.Add<StreamSource>(streamSource2);
				BodyConversionStream bodyConversionStream = new BodyConversionStream(new Func<PropertyTag, BodyHelper, Stream>(Message.BuildConversionStream), logon, streamSource2, propertyTag, isAppend, bodyHelper);
				disposeGuard.Add<BodyConversionStream>(bodyConversionStream);
				disposeGuard.Success();
				result = bodyConversionStream;
			}
			return result;
		}

		private static UpgradeableBodyConversionStream BuildUpgradeableBodyConversionStream(Logon logon, StreamSource streamSource, PropertyTag propertyTag, bool isAppend, Encoding string8Encoding, BodyHelper bodyHelper)
		{
			UpgradeableBodyConversionStream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StreamSource streamSource2 = streamSource.Duplicate();
				disposeGuard.Add<StreamSource>(streamSource2);
				UpgradeableBodyConversionStream upgradeableBodyConversionStream = new UpgradeableBodyConversionStream(logon, streamSource2, propertyTag, isAppend, string8Encoding, bodyHelper, new Func<Logon, StreamSource, PropertyTag, bool, BodyHelper, BodyConversionStream>(Message.BuildBodyConversionStream), new Func<Logon, StreamSource, PropertyTag, Encoding, PropertyStream>(Message.BuildPropertyStream));
				disposeGuard.Add<UpgradeableBodyConversionStream>(upgradeableBodyConversionStream);
				disposeGuard.Success();
				result = upgradeableBodyConversionStream;
			}
			return result;
		}

		private static PropertyStream BuildPropertyStream(Logon logon, StreamSource streamSource, PropertyTag propertyTag, Encoding string8Encoding)
		{
			PropertyStream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = streamSource.PropertyBag.OpenPropertyStream(BodyHelper.GetBodyPropertyDefinition(propertyTag.PropertyId), MEDSPropertyTranslator.OpenModeToPropertyOpenMode(OpenMode.Create, (ErrorCode)2147749887U));
				disposeGuard.Add<Stream>(stream);
				if (propertyTag.PropertyType == PropertyType.String8)
				{
					EncodedStream encodedStream = new EncodedStream(stream, string8Encoding, logon.ResourceTracker);
					disposeGuard.Add<EncodedStream>(encodedStream);
					stream = encodedStream;
				}
				StreamSource streamSource2 = streamSource.Duplicate();
				disposeGuard.Add<StreamSource>(streamSource2);
				PropertyStream propertyStream = new PropertyStream(stream, propertyTag.PropertyType, logon, streamSource2);
				disposeGuard.Add<PropertyStream>(propertyStream);
				disposeGuard.Success();
				result = propertyStream;
			}
			return result;
		}

		private static void PrepareMessageForCopy(Message message, SaveMode mode)
		{
			if (message.CoreItem.IsDirty)
			{
				ConflictResolutionResult conflictResolutionResult = message.CoreItem.Flush(mode);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new RopExecutionException(string.Format("Failed to flush properties on message item. {0}", conflictResolutionResult), (ErrorCode)2147746057U);
				}
				message.CoreItem.PropertyBag.Clear();
				message.CoreItem.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
			}
		}

		private static CoreAttachment GetCoreAttachment(CoreItem coreItem)
		{
			AttachmentHandle attachmentHandle = null;
			if (coreItem.AttachmentCollection == null)
			{
				return null;
			}
			using (IEnumerator<AttachmentHandle> enumerator = coreItem.AttachmentCollection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AttachmentHandle attachmentHandle2 = enumerator.Current;
					attachmentHandle = attachmentHandle2;
				}
			}
			if (attachmentHandle == null)
			{
				return null;
			}
			PropertyDefinition[] preloadProperties = new PropertyDefinition[]
			{
				AttachmentSchema.AttachLongFileName
			};
			return coreItem.AttachmentCollection.Open(attachmentHandle, preloadProperties);
		}

		private FastTransferDownload InternalFastTransferSourceCopy(bool isShallowCopy, DownloadBodyOption downloadBodyOption, FastTransferSendOption sendOptions, bool isInclude, PropertyTag[] propertyTags, bool isFastTransferCopyProperties)
		{
			this.PrepareFastTransferSourceForCopy();
			FastTransferDownload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessage message = this.CreateDownloadMessageAdaptor(downloadBodyOption, sendOptions, isFastTransferCopyProperties);
				disposeGuard.Add<IMessage>(message);
				FastTransferMessageCopyTo fastTransferMessageCopyTo = new FastTransferMessageCopyTo(isShallowCopy, message, true);
				disposeGuard.Add<FastTransferMessageCopyTo>(fastTransferMessageCopyTo);
				FastTransferDownload fastTransferDownload = new FastTransferDownload(sendOptions, fastTransferMessageCopyTo, 1U, new PropertyFilterFactory(isShallowCopy, isInclude, propertyTags), base.LogonObject);
				disposeGuard.Success();
				result = fastTransferDownload;
			}
			return result;
		}

		private void TryStubRecipient(RecipientRow recipientRow)
		{
			this.recipientTranslator.TryStubRecipient(recipientRow);
		}

		private void PrepareForCopy(Message destinationMessage)
		{
			Message.PrepareMessageForCopy(this, SaveMode.NoConflictResolution);
			Message.PrepareMessageForCopy(destinationMessage, SaveMode.FailOnAnyConflict);
		}

		private void PrepareFastTransferSourceForCopy()
		{
			Message.PrepareMessageForCopy(this, SaveMode.NoConflictResolution);
		}

		private void UpdateSecureSubmitFlags()
		{
			this.CoreItem.PropertyBag.SetProperty(CoreItemSchema.ClientSubmittedSecurely, base.LogonObject.Connection.IsEncrypted);
		}

		private Stream GetBodyConversionStreamCallback(BodyReadConfiguration readConfiguration)
		{
			return this.CoreItem.Body.OpenReadStream(readConfiguration);
		}

		private readonly RecipientTranslator recipientTranslator;

		private readonly ReferenceCount<CoreItem> coreItemReference;

		private readonly Encoding string8Encoding;

		private readonly BestBodyCoreObjectProperties bestBodyCoreObjectProperties;

		private readonly CoreObjectPropertyDefinitionFactory propertyDefinitionFactory;

		private bool ignorePropertySaveErrors;
	}
}
