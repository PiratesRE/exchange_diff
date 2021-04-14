using System;
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

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Attachment : PropertyServerObject
	{
		internal Attachment(CoreAttachment coreAttachment, ReferenceCount<CoreItem> coreItemReference, Logon logon, Encoding string8Encoding) : this(coreAttachment, coreItemReference, logon, string8Encoding, ClientSideProperties.AttachmentInstance, PropertyConverter.Attachment)
		{
		}

		internal Attachment(CoreAttachment coreAttachment, ReferenceCount<CoreItem> coreItemReference, Logon logon, Encoding string8Encoding, ClientSideProperties clientSideProperties, PropertyConverter converter) : base(logon, clientSideProperties, converter)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.coreAttachmentReference = new ReferenceCount<CoreAttachment>(coreAttachment);
				this.coreItemReference = coreItemReference;
				this.coreItemReference.AddRef();
				this.propertyDefinitionFactory = new CoreObjectPropertyDefinitionFactory(coreAttachment.Session, coreAttachment.PropertyBag);
				this.storageObjectProperties = new CoreObjectProperties(coreAttachment.PropertyBag);
				this.string8Encoding = string8Encoding;
				disposeGuard.Success();
			}
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
				return this.storageObjectProperties;
			}
		}

		public ICorePropertyBag PropertyBag
		{
			get
			{
				return this.CoreAttachment.PropertyBag;
			}
		}

		public override StoreSession Session
		{
			get
			{
				return this.CoreAttachment.Session;
			}
		}

		public override Schema Schema
		{
			get
			{
				return AttachmentSchema.Instance;
			}
		}

		internal CoreAttachment CoreAttachment
		{
			get
			{
				return this.coreAttachmentReference.ReferencedObject;
			}
		}

		private CoreItem CoreItem
		{
			get
			{
				return this.coreItemReference.ReferencedObject;
			}
		}

		protected override bool SupportsPropertyProblems
		{
			get
			{
				return false;
			}
		}

		public EmbeddedMessage OpenEmbeddedMessage(OpenMode openMode, Encoding string8Encoding)
		{
			if (openMode == OpenMode.BestAccess)
			{
				if (this.CoreAttachment.IsReadOnly)
				{
					openMode = OpenMode.ReadOnly;
				}
				else
				{
					openMode = OpenMode.ReadWrite;
				}
			}
			PropertyOpenMode openMode2 = MEDSPropertyTranslator.OpenModeToPropertyOpenMode(openMode, (ErrorCode)2147746050U);
			if ((byte)(openMode & OpenMode.Create) == 0 && this.CoreAttachment.AttachmentType != AttachmentType.EmbeddedMessage)
			{
				throw new RopExecutionException("The attachment does not contain an embedded message.", (ErrorCode)2147746063U);
			}
			if (openMode != OpenMode.ReadOnly && this.CoreAttachment.IsReadOnly)
			{
				throw new RopExecutionException("The attachment is opened for read-only.", (ErrorCode)2147942405U);
			}
			EmbeddedMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreItem coreItem = (CoreItem)this.CoreAttachment.OpenEmbeddedItem(openMode2, new PropertyDefinition[0]);
				disposeGuard.Add<CoreItem>(coreItem);
				EmbeddedMessage embeddedMessage = new EmbeddedMessage(coreItem, base.LogonObject, string8Encoding);
				if (this.ignorePropertySaveErrors)
				{
					embeddedMessage.IgnorePropertySaveErrors();
				}
				disposeGuard.Success();
				result = embeddedMessage;
			}
			return result;
		}

		public void SaveChanges(SaveChangesMode saveChangesMode)
		{
			if (ExTraceGlobals.AttachmentTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = (this.PropertyBag.TryGetProperty(AttachmentSchema.DisplayName) as string) ?? "<null>";
				ExTraceGlobals.AttachmentTracer.TraceDebug<int, SaveChangesMode, string>((long)this.GetHashCode(), "SaveChangesAttachment. #{0}. SaveChangesMode = {1}, DisplayName: \"{2}\".", this.CoreAttachment.AttachmentNumber, saveChangesMode, arg);
			}
			if ((byte)(saveChangesMode & (SaveChangesMode.TransportDelivery | SaveChangesMode.IMAPChange | SaveChangesMode.ForceNotificationPublish)) != 0)
			{
				throw new RopExecutionException(string.Format("The mode is not supported. saveChangesMode = {0}.", saveChangesMode), (ErrorCode)2147746050U);
			}
			if ((byte)(saveChangesMode & SaveChangesMode.KeepOpenReadOnly) == 1 && (byte)(saveChangesMode & SaveChangesMode.KeepOpenReadWrite) == 2)
			{
				throw new RopExecutionException(string.Format("The special mode is not supported. saveChangesMode = {0}.", saveChangesMode), (ErrorCode)2147746050U);
			}
			Feature.Stubbed(54480, "SaveChangesMode flags not supported. SaveChangesModes=" + saveChangesMode);
			this.CoreAttachment.Save();
			this.CoreItem.Flush(SaveMode.FailOnAnyConflict);
			this.CoreItem.PropertyBag.Clear();
			this.CoreItem.PropertyBag.Load(null);
		}

		internal void IgnorePropertySaveErrors()
		{
			this.ignorePropertySaveErrors = true;
			this.coreAttachmentReference.ReferencedObject.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyTo()
		{
			this.IgnorePropertySaveErrors();
			FastTransferUpload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				bool isReadOnly = false;
				this.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				AttachmentAdaptor attachmentAdaptor = new AttachmentAdaptor(this.coreAttachmentReference, isReadOnly, base.LogonObject.LogonString8Encoding, true, false);
				disposeGuard.Add<AttachmentAdaptor>(attachmentAdaptor);
				IFastTransferProcessor<FastTransferUploadContext> fastTransferProcessor = FastTransferAttachmentCopyTo.CreateUploadStateMachine(attachmentAdaptor);
				disposeGuard.Add<IFastTransferProcessor<FastTransferUploadContext>>(fastTransferProcessor);
				FastTransferUpload fastTransferUpload = new FastTransferUpload(fastTransferProcessor, PropertyFilterFactory.IncludeAllFactory, base.LogonObject);
				disposeGuard.Success();
				result = fastTransferUpload;
			}
			return result;
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyProperties(bool isShallowCopy, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] includedProperties)
		{
			if (flags != FastTransferCopyPropertiesFlag.None)
			{
				Feature.Stubbed(185369, "Support FastTransferCopyPropertiesFlag.Move, which is the only flag in FastTransferCopyPropertiesFlag.");
			}
			return this.InternalFastTransferSourceCopyOperation(isShallowCopy, FastTransferCopyFlag.None, sendOptions, true, includedProperties);
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyProperties()
		{
			return this.InternalFastTransferDestinationCopyTo();
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyTo(bool isShallowCopy, FastTransferCopyFlag flags, FastTransferSendOption options, PropertyTag[] excludedPropertyTags)
		{
			if (ExTraceGlobals.AttachmentTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = (this.PropertyBag.TryGetProperty(AttachmentSchema.DisplayName) as string) ?? "<null>";
				ExTraceGlobals.AttachmentTracer.TraceDebug<int, string>((long)this.GetHashCode(), "CreateSourceCopyToDownload. #{0}, \"{1}\"", this.CoreAttachment.AttachmentNumber, arg);
			}
			return this.InternalFastTransferSourceCopyOperation(isShallowCopy, flags, options, false, excludedPropertyTags);
		}

		protected override PropertyError[] InternalCopyTo(PropertyServerObject destinationPropertyServerObject, CopySubObjects copySubObjects, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] excludeProperties)
		{
			Attachment attachment = RopHandler.Downcast<Attachment>(destinationPropertyServerObject);
			if (attachment.PropertyBag.IsDirty)
			{
				attachment.FlushAndReload();
			}
			PropertyError[] result = this.CoreAttachment.CopyAttachment(attachment.CoreAttachment, copyPropertiesFlags, copySubObjects, excludeProperties);
			attachment.PropertyBag.Reload();
			return result;
		}

		protected override PropertyError[] InternalCopyProperties(PropertyServerObject destinationPropertyServerObject, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] properties)
		{
			Attachment attachment = RopHandler.Downcast<Attachment>(destinationPropertyServerObject);
			if (attachment.PropertyBag.IsDirty)
			{
				attachment.FlushAndReload();
			}
			PropertyError[] result = this.CoreAttachment.CopyProperties(attachment.CoreAttachment, copyPropertiesFlags, properties);
			attachment.PropertyBag.Reload();
			return result;
		}

		protected override bool ShouldSkipPropertyChange(StorePropertyDefinition propertyDefinition)
		{
			return TeamMailboxClientOperations.IsLinked(this.CoreAttachment) && AttachmentPropertyRestriction.Instance.ShouldBlock(propertyDefinition, true);
		}

		protected override StreamSource GetStreamSource()
		{
			return new StreamSource<CoreAttachment>(this.coreAttachmentReference, (CoreAttachment coreAttachment) => coreAttachment.PropertyBag);
		}

		private FastTransferDownload InternalFastTransferSourceCopyOperation(bool isShallowCopy, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, bool isInclusion, PropertyTag[] propertyTags)
		{
			FastTransferDownload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				bool isReadOnly = true;
				this.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				AttachmentAdaptor attachmentAdaptor = new AttachmentAdaptor(this.coreAttachmentReference, isReadOnly, base.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), sendOptions.IsUpload());
				disposeGuard.Add<AttachmentAdaptor>(attachmentAdaptor);
				IFastTransferProcessor<FastTransferDownloadContext> fastTransferProcessor = FastTransferAttachmentCopyTo.CreateDownloadStateMachine(attachmentAdaptor);
				disposeGuard.Add<IFastTransferProcessor<FastTransferDownloadContext>>(fastTransferProcessor);
				FastTransferDownload fastTransferDownload = new FastTransferDownload(sendOptions, fastTransferProcessor, 1U, new PropertyFilterFactory(isShallowCopy, isInclusion, propertyTags), base.LogonObject);
				disposeGuard.Success();
				result = fastTransferDownload;
			}
			return result;
		}

		private void FlushAndReload()
		{
			this.CoreAttachment.Flush();
			this.PropertyBag.Reload();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Attachment>(this);
		}

		protected override void InternalDispose()
		{
			this.coreAttachmentReference.Release();
			this.coreItemReference.Release();
			base.InternalDispose();
		}

		private readonly ReferenceCount<CoreAttachment> coreAttachmentReference;

		private readonly ReferenceCount<CoreItem> coreItemReference;

		private readonly CoreObjectPropertyDefinitionFactory propertyDefinitionFactory;

		private readonly CoreObjectProperties storageObjectProperties;

		private readonly Encoding string8Encoding;

		private bool ignorePropertySaveErrors;
	}
}
