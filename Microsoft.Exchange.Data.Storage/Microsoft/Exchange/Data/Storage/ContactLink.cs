using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ContactLink
	{
		protected ContactLink(MailboxInfoForLinking mailboxInfo, IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker)
		{
			Util.ThrowOnNullArgument(mailboxInfo, "mailboxInfo");
			Util.ThrowOnNullArgument(logger, "logger");
			Util.ThrowOnNullArgument(performanceTracker, "performanceTracker");
			this.mailboxInfo = mailboxInfo;
			this.logger = logger;
			this.performanceTracker = performanceTracker;
		}

		protected MailboxInfoForLinking MailboxInfo
		{
			get
			{
				return this.mailboxInfo;
			}
		}

		protected void Commit(IEnumerable<ContactInfoForLinking> contacts)
		{
			foreach (ContactInfoForLinking contact in contacts)
			{
				this.Commit(contact);
			}
		}

		protected IContactLinkingPerformanceTracker PerformanceTracker
		{
			get
			{
				return this.performanceTracker;
			}
		}

		protected void Commit(ContactInfoForLinking contact)
		{
			contact.Commit(this.logger, this.performanceTracker);
		}

		protected void LogEvent(ILogEvent logEvent)
		{
			this.logger.LogEvent(logEvent);
		}

		protected static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private IExtensibleLogger logger;

		private MailboxInfoForLinking mailboxInfo;

		private IContactLinkingPerformanceTracker performanceTracker;
	}
}
