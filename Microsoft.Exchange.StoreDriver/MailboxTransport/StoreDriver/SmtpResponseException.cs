using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal class SmtpResponseException : LocalizedException
	{
		public SmtpResponseException(SmtpResponse response) : this(response, null)
		{
		}

		public SmtpResponseException(SmtpResponse response, string source) : base(new LocalizedString(response.ToString()))
		{
			MessageAction action;
			switch (response.SmtpResponseType)
			{
			case SmtpResponseType.Success:
				if (string.IsNullOrEmpty(source))
				{
					throw new ArgumentException("Source must be provided for success smtp response type", "response");
				}
				action = MessageAction.LogProcess;
				goto IL_6E;
			case SmtpResponseType.TransientError:
				action = MessageAction.Retry;
				goto IL_6E;
			case SmtpResponseType.PermanentError:
				action = MessageAction.NDR;
				goto IL_6E;
			}
			throw new ArgumentException("The smtp response type is not supported.", "response");
			IL_6E:
			this.status = new MessageStatus(action, response, this, false, source);
		}

		public SmtpResponseException(SmtpResponse response, MessageAction action) : base(new LocalizedString(response.ToString()))
		{
			if (action == MessageAction.Throw)
			{
				throw new ArgumentException("MessageAction.Throw is not supported.", "action");
			}
			this.status = new MessageStatus(action, response, this);
		}

		public MessageStatus Status
		{
			get
			{
				return this.status;
			}
		}

		private MessageStatus status;
	}
}
