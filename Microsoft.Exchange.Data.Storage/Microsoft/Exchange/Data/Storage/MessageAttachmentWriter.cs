using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MessageAttachmentWriter : IDisposeTrackable, IDisposable
	{
		internal MessageAttachmentWriter(InboundMessageWriter messageWriter)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.messageWriter = messageWriter;
			this.coreAttachment = this.CoreItem.AttachmentCollection.InternalCreate(null);
			this.attachMethod = null;
		}

		internal MessageAttachmentWriter(InboundMessageWriter messageWriter, int attachMethod)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.messageWriter = messageWriter;
			AttachmentType attachmentType = CoreAttachmentCollection.GetAttachmentType(new int?(attachMethod));
			this.coreAttachment = this.CoreItem.AttachmentCollection.Create(attachmentType);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageAttachmentWriter>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal Stream CreateOleAttachmentDataStream()
		{
			this.CheckDisposed("CreateOleAttachmentDataStream");
			this.ResetAttachMethod(6);
			return this.coreAttachment.PropertyBag.OpenPropertyStream(InternalSchema.AttachDataObj, PropertyOpenMode.Create);
		}

		internal Stream CreatePropertyStream(PropertyDefinition property)
		{
			this.CheckDisposed("CreatePropertyStream");
			if (property.Equals(InternalSchema.AttachDataBin))
			{
				this.ResetAttachMethod(1);
			}
			return this.coreAttachment.PropertyBag.OpenPropertyStream(property, PropertyOpenMode.Create);
		}

		internal ICoreItem CreateAttachmentItem()
		{
			this.CheckDisposed("CreateAttachmentItem");
			this.ResetAttachMethod(5);
			bool noMessageDecoding = this.coreAttachment.ParentCollection.ContainerItem.CharsetDetector.NoMessageDecoding;
			return this.coreAttachment.PropertyBag.OpenAttachedItem(PropertyOpenMode.Create, null, noMessageDecoding);
		}

		internal void SaveAttachment()
		{
			this.CheckDisposed("SaveAttachment");
			if (this.attachMethod == null)
			{
				this.ResetAttachMethod(1);
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<CoreAttachment>(this.coreAttachment);
				using (Attachment attachment = AttachmentCollection.CreateTypedAttachment(this.coreAttachment, null))
				{
					attachment.SaveFlags |= PropertyBagSaveFlags.IgnoreMapiComputedErrors;
					attachment.Save();
				}
				disposeGuard.Success();
			}
		}

		internal void AddProperty(PropertyDefinition propDefinition, object value)
		{
			this.CheckDisposed("AddProperty");
			if (propDefinition.Equals(InternalSchema.AttachMethod))
			{
				int newAttachMethod = (int)value;
				this.ResetAttachMethod(newAttachMethod);
			}
			if (!propDefinition.Equals(InternalSchema.AttachDataBin))
			{
				this.coreAttachment.PropertyBag[propDefinition] = value;
				return;
			}
			int? num = this.attachMethod;
			int valueOrDefault = num.GetValueOrDefault();
			if (num == null)
			{
				this.data = (byte[])value;
				return;
			}
			if (valueOrDefault != 1)
			{
				return;
			}
			this.coreAttachment.PropertyBag.SetProperty(propDefinition, value);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.coreAttachment != null)
				{
					this.coreAttachment.Dispose();
					this.coreAttachment = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
		}

		private void ResetAttachMethod(int newAttachMethod)
		{
			this.CheckDisposed("TransformAttachment");
			if (this.attachMethod == newAttachMethod)
			{
				return;
			}
			if (this.attachMethod == 5 || (this.attachMethod != null && newAttachMethod == 5))
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "MessageAttachmentWriter::TransformAttachment: wrong attachment transformation.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			IDirectPropertyBag propertyBag = this.coreAttachment.PropertyBag;
			this.attachMethod = new int?(newAttachMethod);
			propertyBag.SetValue(InternalSchema.AttachMethod, newAttachMethod);
			if (newAttachMethod == 1 && this.data != null)
			{
				propertyBag.SetValue(InternalSchema.AttachDataBin, this.data);
				this.data = null;
			}
		}

		private ICoreItem CoreItem
		{
			get
			{
				this.CheckDisposed("Item::get");
				return this.messageWriter.CoreItem;
			}
		}

		private bool isDisposed;

		private InboundMessageWriter messageWriter;

		private byte[] data;

		private CoreAttachment coreAttachment;

		private int? attachMethod;

		private readonly DisposeTracker disposeTracker;
	}
}
