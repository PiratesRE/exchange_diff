using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class InboundMessageWriter : IDisposeTrackable, IDisposable
	{
		internal InboundMessageWriter(ICoreItem item, InboundConversionOptions options, InboundAddressCache addressCache, ConversionLimitsTracker limitsTracker, MimeMessageLevel messageLevel)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.parent = null;
			this.isTopLevelMessage = (messageLevel == MimeMessageLevel.TopLevelMessage);
			this.SetItem(item, options, false);
			this.SetAddressCache(addressCache, false);
			this.SetLimitsTracker(limitsTracker);
			this.componentType = ConversionComponentType.Message;
		}

		internal InboundMessageWriter(ICoreItem item, InboundConversionOptions options, MimeMessageLevel messageLevel)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.parent = null;
			this.isTopLevelMessage = (messageLevel == MimeMessageLevel.TopLevelMessage);
			this.SetItem(item, options, false);
			ConversionLimitsTracker conversionLimitsTracker = new ConversionLimitsTracker(options.Limits);
			InboundAddressCache addressCache = new InboundAddressCache(options, conversionLimitsTracker, messageLevel);
			this.SetAddressCache(addressCache, true);
			this.SetLimitsTracker(conversionLimitsTracker);
			this.componentType = ConversionComponentType.Message;
		}

		private InboundMessageWriter(InboundMessageWriter parent, ICoreItem item)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.parent = parent;
			this.isTopLevelMessage = false;
			this.SetItem(item, parent.ConversionOptions, true);
			ConversionLimitsTracker conversionLimitsTracker = parent.LimitsTracker;
			conversionLimitsTracker.StartEmbeddedMessage();
			InboundAddressCache addressCache = new InboundAddressCache(parent.ConversionOptions, conversionLimitsTracker, MimeMessageLevel.AttachedMessage);
			this.SetAddressCache(addressCache, true);
			this.SetLimitsTracker(conversionLimitsTracker);
			this.componentType = ConversionComponentType.Message;
		}

		internal ICoreItem CoreItem
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_Item");
				return this.coreItem;
			}
		}

		internal InboundConversionOptions ConversionOptions
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_ConversionOptions");
				return this.conversionOptions;
			}
		}

		internal bool IsTopLevelMessage
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_IsTopLevelMessage");
				return this.isTopLevelMessage;
			}
		}

		internal bool IsTopLevelWriter
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_IsTopLevelWriter");
				return this.parent == null;
			}
		}

		internal ConversionComponentType ComponentType
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_ComponentType");
				return this.componentType;
			}
		}

		internal bool ForceParticipantResolution
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_ForceParticipantResolution");
				return this.forceParticipantResolution;
			}
			set
			{
				this.CheckDisposed("InboundMessageWriter::set_ForceParticipantResolution");
				this.forceParticipantResolution = value;
			}
		}

		private ConversionLimitsTracker LimitsTracker
		{
			get
			{
				this.CheckDisposed("InboundMessageWriter::get_LimitsTracker");
				return this.limitsTracker;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<InboundMessageWriter>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void ClearRecipientTable()
		{
			this.CheckDisposed("ClearRecipientTable");
			MessageItem messageItem = this.coreItem as MessageItem;
			if (messageItem != null)
			{
				messageItem.Recipients.Clear();
			}
		}

		internal void ClearAttachmentTable()
		{
			this.CheckDisposed("ClearAttachmentTable");
			this.coreItem.AttachmentCollection.RemoveAll();
		}

		internal void StartNewRecipient()
		{
			this.CheckDisposed("StartNewRecipient");
			this.componentType = ConversionComponentType.Recipient;
			this.currentRecipient = new ConversionRecipientEntry();
		}

		internal void EndRecipient()
		{
			this.CheckDisposed("EndRecipient");
			if (this.currentRecipient != null)
			{
				if (this.currentRecipient.Participant != null && ConvertUtils.IsRecipientTransmittable(this.currentRecipient.RecipientItemType))
				{
					this.conversionAddressCache.AddRecipient(this.currentRecipient);
				}
				else
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "Tnef didn't have enough information to construct a Participant");
				}
				this.currentRecipient = null;
			}
			this.componentType = ConversionComponentType.Message;
		}

		internal void StartNewAttachment()
		{
			this.CheckDisposed("StartNewAttachment");
			this.limitsTracker.CountMessageAttachment();
			this.attachment = new MessageAttachmentWriter(this);
			this.componentType = ConversionComponentType.FileAttachment;
		}

		internal void StartNewAttachment(int attachMethod)
		{
			this.CheckDisposed("StartNewAttachment");
			this.limitsTracker.CountMessageAttachment();
			this.attachment = new MessageAttachmentWriter(this, attachMethod);
			this.componentType = ConversionComponentType.FileAttachment;
		}

		internal void EndAttachment()
		{
			this.CheckDisposed("EndAttachment");
			if (this.attachment == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "InboundMessageWriter::EndAttachment: the attachment is null.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			this.attachment.SaveAttachment();
			this.attachment.Dispose();
			this.attachment = null;
			this.componentType = ConversionComponentType.Message;
		}

		internal void SetProperty(StorePropertyDefinition property, object value)
		{
			this.CheckDisposed("MessageWriter::SetProperty");
			switch (this.componentType)
			{
			case ConversionComponentType.Message:
				this.SetMessageProperty(property, value);
				return;
			case ConversionComponentType.Recipient:
				this.SetRecipientProperty(property, value);
				return;
			case ConversionComponentType.FileAttachment:
				this.SetAttachmentProperty(property, value);
				return;
			default:
				return;
			}
		}

		internal void SetSubjectProperty(NativeStorePropertyDefinition property, string value)
		{
			this.CheckDisposed("SetSubjectProperty");
			SubjectProperty.ModifySubjectProperty(CoreObject.GetPersistablePropertyBag(this.coreItem), property, value);
		}

		internal void SetAddressProperty(StorePropertyDefinition property, object value)
		{
			this.conversionAddressCache.SetProperty((NativeStorePropertyDefinition)property, value);
		}

		internal void DeleteMessageProperty(StorePropertyDefinition property)
		{
			this.CheckDisposed("DeleteMessageProperty");
			this.coreItem.PropertyBag.Delete(property);
		}

		internal Stream OpenPropertyStream(StorePropertyDefinition property)
		{
			this.CheckDisposed("OpenPropertyStream");
			switch (this.componentType)
			{
			case ConversionComponentType.Message:
				return this.OpenMessagePropertyStream(property);
			case ConversionComponentType.FileAttachment:
				return this.OpenAttachmentPropertyStream(property);
			}
			StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "MessageWriter.OpenPropertyStream: can't open stream on the component");
			throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
		}

		internal Stream OpenOleAttachmentDataStream()
		{
			this.CheckDisposed("OpenOleAttachmentDataStream");
			return this.attachment.CreateOleAttachmentDataStream();
		}

		internal InboundMessageWriter OpenAttachedMessageWriter()
		{
			this.CheckDisposed("OpenAttachedMessage");
			ICoreItem coreItem = null;
			InboundMessageWriter inboundMessageWriter = null;
			try
			{
				coreItem = this.attachment.CreateAttachmentItem();
				inboundMessageWriter = new InboundMessageWriter(this, coreItem);
			}
			finally
			{
				if (inboundMessageWriter == null && coreItem != null)
				{
					coreItem.Dispose();
					coreItem = null;
				}
			}
			return inboundMessageWriter;
		}

		internal void SuppressLimitChecks()
		{
			this.limitsTracker.SuppressLimitChecks();
		}

		internal void UndoTnef()
		{
			this.CheckDisposed("UndoTnef");
			this.ClearRecipientTable();
			this.ClearAttachmentTable();
			foreach (PropertyDefinition propertyDefinition in InboundMessageWriter.UndoPropertyList)
			{
				this.coreItem.PropertyBag.Delete(propertyDefinition);
			}
			PersistablePropertyBag persistablePropertyBag = CoreObject.GetPersistablePropertyBag(this.CoreItem);
			IDirectPropertyBag directPropertyBag = persistablePropertyBag;
			directPropertyBag.SetValue(InternalSchema.ItemClass, "IPM.Note");
			MessageItem messageItem = this.coreItem as MessageItem;
			if (messageItem != null)
			{
				messageItem.ReplyTo.Clear();
			}
		}

		internal void Commit()
		{
			if (this.ownsAddressCache)
			{
				if (this.ForceParticipantResolution)
				{
					this.conversionAddressCache.Resolve();
				}
				else
				{
					this.conversionAddressCache.ReplyTo.Resync(true);
				}
				this.conversionAddressCache.CopyDataToItem(this.coreItem);
			}
			if (this.ownsItem)
			{
				this.CoreItem.CharsetDetector.DetectionOptions = this.ConversionOptions.DetectionOptions;
				this.CoreItem.Save(SaveMode.ResolveConflicts);
			}
		}

		private void SetItem(ICoreItem coreItem, InboundConversionOptions options, bool ownsItem)
		{
			this.coreItem = coreItem;
			this.conversionOptions = options;
			this.ownsItem = ownsItem;
		}

		private void SetAddressCache(InboundAddressCache addressCache, bool ownsCache)
		{
			this.conversionAddressCache = addressCache;
			this.ownsAddressCache = ownsCache;
		}

		private void SetLimitsTracker(ConversionLimitsTracker limitsTracker)
		{
			this.limitsTracker = limitsTracker;
			if (this.IsTopLevelMessage)
			{
				this.limitsTracker.CountMessageBody();
			}
		}

		private void SetMessageProperty(StorePropertyDefinition property, object value)
		{
			this.CheckDisposed("SetProperty");
			this.coreItem.PropertyBag.SetProperty(property, value);
		}

		private Stream OpenMessagePropertyStream(StorePropertyDefinition property)
		{
			return this.coreItem.PropertyBag.OpenPropertyStream(property, PropertyOpenMode.Create);
		}

		private void SetRecipientProperty(StorePropertyDefinition property, object value)
		{
			this.CheckDisposed("SetRecipientProperty");
			if (this.currentRecipient != null)
			{
				this.currentRecipient.SetProperty(property, value, true);
			}
		}

		private void SetAttachmentProperty(StorePropertyDefinition property, object value)
		{
			this.attachment.AddProperty(property, value);
		}

		private Stream OpenAttachmentPropertyStream(StorePropertyDefinition property)
		{
			this.CheckDisposed("OpenAttachmentPropertyStream");
			return this.attachment.CreatePropertyStream(property);
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.InternalDispose(disposing);
				this.isDisposed = true;
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.parent != null)
				{
					this.LimitsTracker.EndEmbeddedMessage();
					this.parent = null;
				}
				if (this.ownsItem && this.coreItem != null)
				{
					this.coreItem.Dispose();
					this.coreItem = null;
				}
				if (this.attachment != null)
				{
					this.attachment.Dispose();
					this.attachment = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private static readonly PropertyDefinition[] UndoPropertyList = new PropertyDefinition[]
		{
			InternalSchema.SenderAddressType,
			InternalSchema.SenderDisplayName,
			InternalSchema.SenderEmailAddress,
			InternalSchema.SenderEntryId,
			InternalSchema.SenderSearchKey,
			InternalSchema.SentRepresentingDisplayName,
			InternalSchema.SentRepresentingEmailAddress,
			InternalSchema.SentRepresentingEntryId,
			InternalSchema.SentRepresentingType,
			InternalSchema.SentRepresentingSearchKey,
			InternalSchema.SenderFlags,
			InternalSchema.SentRepresentingFlags,
			InternalSchema.RtfBody,
			InternalSchema.RtfInSync,
			InternalSchema.RtfSyncBodyCount,
			InternalSchema.RtfSyncBodyCrc,
			InternalSchema.RtfSyncBodyTag,
			InternalSchema.RtfSyncPrefixCount,
			InternalSchema.RtfSyncTrailingCount,
			InternalSchema.HtmlBody,
			InternalSchema.TextBody,
			InternalSchema.MapiSubject,
			InternalSchema.NormalizedSubjectInternal,
			InternalSchema.SubjectPrefixInternal,
			InternalSchema.ConversationIndex,
			InternalSchema.ConversationTopic
		};

		private readonly DisposeTracker disposeTracker;

		private ICoreItem coreItem;

		private InboundAddressCache conversionAddressCache;

		private ConversionRecipientEntry currentRecipient;

		private MessageAttachmentWriter attachment;

		private InboundMessageWriter parent;

		private InboundConversionOptions conversionOptions;

		private ConversionLimitsTracker limitsTracker;

		private ConversionComponentType componentType;

		private bool ownsItem;

		private bool ownsAddressCache;

		private bool isTopLevelMessage;

		private bool forceParticipantResolution;

		private bool isDisposed;
	}
}
