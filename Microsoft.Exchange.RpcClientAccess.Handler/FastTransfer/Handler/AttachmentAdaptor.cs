using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AttachmentAdaptor : BaseObject, IAttachment, IDisposable
	{
		internal AttachmentAdaptor(ReferenceCount<CoreAttachment> referenceAttachment, bool isReadOnly, Encoding string8Encoding, bool wantUnicode, bool isUpload)
		{
			this.isReadOnly = isReadOnly;
			this.string8Encoding = string8Encoding;
			this.wantUnicode = wantUnicode;
			this.isUpload = isUpload;
			this.referenceAttachment = referenceAttachment;
			this.referenceAttachment.AddRef();
		}

		protected override void InternalDispose()
		{
			this.referenceAttachment.Release();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AttachmentAdaptor>(this);
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				base.CheckDisposed();
				if (this.attachmentPropertyBag == null)
				{
					this.attachmentPropertyBag = new CoreAttachmentPropertyBagAdaptor(this.referenceAttachment.ReferencedObject.PropertyBag, this.referenceAttachment.ReferencedObject.Session.Mailbox.CoreObject, this.string8Encoding, this.wantUnicode, this.isUpload);
				}
				return this.attachmentPropertyBag;
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				base.CheckDisposed();
				return this.referenceAttachment.ReferencedObject.AttachmentType == AttachmentType.EmbeddedMessage;
			}
		}

		public IMessage GetEmbeddedMessage()
		{
			base.CheckDisposed();
			if (this.isReadOnly && !this.IsEmbeddedMessage)
			{
				throw new InvalidOperationException("GetEmbeddedMessage() should not be called on readonly attachments that do not represent an embedded message");
			}
			PropertyOpenMode openMode = this.isReadOnly ? PropertyOpenMode.ReadOnly : PropertyOpenMode.Create;
			ReferenceCount<CoreItem> referenceCount = ReferenceCount<CoreItem>.Assign((CoreItem)this.referenceAttachment.ReferencedObject.OpenEmbeddedItem(openMode, CoreObjectSchema.AllPropertiesOnStore));
			IMessage result;
			try
			{
				result = new MessageAdaptor(referenceCount, new MessageAdaptor.Options
				{
					IsReadOnly = this.isReadOnly,
					IsEmbedded = true,
					DownloadBodyOption = DownloadBodyOption.RtfOnly,
					IsUpload = this.isUpload
				}, this.string8Encoding, this.wantUnicode, null);
			}
			finally
			{
				referenceCount.Release();
			}
			return result;
		}

		public void Save()
		{
			base.CheckDisposed();
			this.referenceAttachment.ReferencedObject.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
			this.referenceAttachment.ReferencedObject.Save();
		}

		public int AttachmentNumber
		{
			get
			{
				base.CheckDisposed();
				return this.referenceAttachment.ReferencedObject.AttachmentNumber;
			}
		}

		private readonly ReferenceCount<CoreAttachment> referenceAttachment;

		private readonly bool isReadOnly;

		private readonly Encoding string8Encoding;

		private readonly bool wantUnicode;

		private readonly bool isUpload;

		private CoreAttachmentPropertyBagAdaptor attachmentPropertyBag;
	}
}
