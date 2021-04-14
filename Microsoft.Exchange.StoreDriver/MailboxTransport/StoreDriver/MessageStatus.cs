using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal class MessageStatus
	{
		internal MessageStatus(MessageAction action, SmtpResponse smtpResponse, Exception exception) : this(action, smtpResponse, exception, false)
		{
		}

		internal MessageStatus(MessageAction action, SmtpResponse smtpResponse, Exception exception, bool forAllRecipients) : this(action, smtpResponse, exception, forAllRecipients, null)
		{
		}

		internal MessageStatus(MessageAction action, SmtpResponse smtpResponse, Exception exception, bool forAllRecipients, string source)
		{
			this.action = action;
			this.smtpResponse = smtpResponse;
			this.exception = exception;
			this.forAllRecipients = forAllRecipients;
			this.source = source;
		}

		internal MessageAction Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		internal SmtpResponse Response
		{
			get
			{
				return this.smtpResponse;
			}
			set
			{
				this.smtpResponse = value;
			}
		}

		public TimeSpan? RetryInterval
		{
			get
			{
				return this.retryInterval;
			}
			set
			{
				this.retryInterval = value;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		internal bool NDRForAllRecipients
		{
			get
			{
				return this.forAllRecipients && this.action == MessageAction.NDR;
			}
		}

		internal string Source
		{
			get
			{
				return this.source;
			}
		}

		internal static readonly MessageStatus Success = new MessageStatus(MessageAction.Success, SmtpResponse.Empty, null);

		private MessageAction action;

		private SmtpResponse smtpResponse;

		private Exception exception;

		private bool forAllRecipients;

		private TimeSpan? retryInterval;

		private string source;

		private class MailboxDatabaseOfflineException : StorageTransientException
		{
			public MailboxDatabaseOfflineException(LocalizedString message) : base(message)
			{
			}
		}
	}
}
