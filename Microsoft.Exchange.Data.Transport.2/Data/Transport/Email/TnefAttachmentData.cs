using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class TnefAttachmentData : AttachmentData
	{
		public TnefPropertyBag Properties
		{
			[DebuggerStepThrough]
			get
			{
				return this.properties;
			}
		}

		public EmailMessage EmbeddedMessage
		{
			[DebuggerStepThrough]
			get
			{
				return this.embeddedMessage;
			}
			[DebuggerStepThrough]
			set
			{
				this.embeddedMessage = value;
			}
		}

		public InternalAttachmentType InternalAttachmentType
		{
			[DebuggerStepThrough]
			get
			{
				return this.internalAttachmentType;
			}
			[DebuggerStepThrough]
			set
			{
				this.internalAttachmentType = value;
			}
		}

		public int OriginalIndex
		{
			[DebuggerStepThrough]
			get
			{
				return this.originalIndex;
			}
		}

		public string FileName
		{
			[DebuggerStepThrough]
			get
			{
				return this.fileName;
			}
			[DebuggerStepThrough]
			set
			{
				this.fileName = value;
			}
		}

		public AttachmentMethod AttachmentMethod
		{
			[DebuggerStepThrough]
			get
			{
				return this.attachmentMethod;
			}
			[DebuggerStepThrough]
			set
			{
				this.attachmentMethod = value;
			}
		}

		public TnefAttachmentData(int attachmentIndex, MessageImplementation message) : base(message)
		{
			this.properties = new TnefPropertyBag(this);
			this.originalIndex = attachmentIndex;
		}

		public override void Invalidate()
		{
			this.originalIndex = int.MinValue;
			base.Invalidate();
		}

		private TnefPropertyBag properties;

		private EmailMessage embeddedMessage;

		private InternalAttachmentType internalAttachmentType;

		private int originalIndex;

		private AttachmentMethod attachmentMethod;

		private string fileName;
	}
}
