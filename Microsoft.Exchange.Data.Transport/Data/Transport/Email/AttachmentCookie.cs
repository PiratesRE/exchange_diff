using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal struct AttachmentCookie
	{
		public int Index
		{
			[DebuggerStepThrough]
			get
			{
				return this.index;
			}
		}

		public MessageImplementation MessageImplementation
		{
			[DebuggerStepThrough]
			get
			{
				return this.messageImplementation;
			}
		}

		public AttachmentCookie(int index, MessageImplementation messageImplementation)
		{
			this.index = index;
			this.messageImplementation = messageImplementation;
		}

		private int index;

		private MessageImplementation messageImplementation;
	}
}
