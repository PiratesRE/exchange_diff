using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LoggingContext
	{
		public Guid MailboxGuid { get; private set; }

		public Guid TransactionId { get; private set; }

		public string Context { get; private set; }

		public string User { get; private set; }

		public Stream LoggingStream { get; set; }

		public LoggingContext(Guid mailboxGuid, string context, string user, Stream loggingStream)
		{
			this.TransactionId = Guid.NewGuid();
			this.MailboxGuid = mailboxGuid;
			this.Context = context;
			this.User = user;
			this.LoggingStream = loggingStream;
		}
	}
}
