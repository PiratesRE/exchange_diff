using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class AttachmentData
	{
		public MessageImplementation MessageImplementation
		{
			[DebuggerStepThrough]
			get
			{
				return this.messageImplementation;
			}
		}

		public object Attachment
		{
			[DebuggerStepThrough]
			get
			{
				return this.attachment;
			}
			[DebuggerStepThrough]
			set
			{
				this.attachment = value;
			}
		}

		protected AttachmentData(MessageImplementation messageImplementation)
		{
			this.messageImplementation = messageImplementation;
		}

		public virtual void Invalidate()
		{
			this.messageImplementation = null;
			this.attachment = null;
		}

		private MessageImplementation messageImplementation;

		private object attachment;
	}
}
