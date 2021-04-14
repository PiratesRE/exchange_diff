using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class MessageTypeEntry
	{
		public MessageType MessageType
		{
			[DebuggerStepThrough]
			get
			{
				return this.messageType;
			}
		}

		public MessageFlags MessageFlags
		{
			[DebuggerStepThrough]
			get
			{
				return this.messageFlags;
			}
		}

		public MessageSecurityType MessageSecurityType
		{
			[DebuggerStepThrough]
			get
			{
				return this.messageSecurityType;
			}
		}

		internal MessageTypeEntry(MessageType type, MessageFlags flags)
		{
			this.messageType = type;
			this.messageFlags = flags;
			this.messageSecurityType = MessageSecurityType.None;
		}

		internal MessageTypeEntry(MessageType type, MessageFlags flags, MessageSecurityType security)
		{
			this.messageType = type;
			this.messageFlags = flags;
			this.messageSecurityType = security;
		}

		private MessageType messageType;

		private MessageFlags messageFlags;

		private MessageSecurityType messageSecurityType;
	}
}
