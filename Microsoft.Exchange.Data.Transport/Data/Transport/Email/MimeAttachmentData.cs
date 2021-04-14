using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class MimeAttachmentData : AttachmentData
	{
		public MimePart AttachmentPart
		{
			[DebuggerStepThrough]
			get
			{
				return this.attachmentPart;
			}
			[DebuggerStepThrough]
			set
			{
				this.attachmentPart = value;
			}
		}

		public MimePart DataPart
		{
			[DebuggerStepThrough]
			get
			{
				return this.dataPart;
			}
			[DebuggerStepThrough]
			set
			{
				this.dataPart = value;
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

		public bool Referenced
		{
			[DebuggerStepThrough]
			get
			{
				return this.referenced;
			}
			[DebuggerStepThrough]
			set
			{
				this.referenced = value;
			}
		}

		public MimeAttachmentData(MimePart part, MessageImplementation message) : base(message)
		{
			this.attachmentPart = part;
			this.referenced = true;
		}

		public void FlushCache()
		{
			this.fileName = null;
			this.dataPart = null;
		}

		private MimePart attachmentPart;

		private MimePart dataPart;

		private string fileName;

		private EmailMessage embeddedMessage;

		private InternalAttachmentType internalAttachmentType;

		private bool referenced;
	}
}
