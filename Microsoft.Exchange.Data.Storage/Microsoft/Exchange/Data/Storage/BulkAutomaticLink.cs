using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class BulkAutomaticLink : DisposeTrackableBase
	{
		public BulkAutomaticLink(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			MailboxInfoForLinking mailboxInfo = MailboxInfoForLinking.CreateFromMailboxSession(session);
			this.logger = new ContactLinkingLogger("BulkAutomaticLink", mailboxInfo);
			this.performanceTracker = new ContactLinkingPerformanceTracker(session);
			this.contactStore = new ContactStoreForBulkContactLinking(session, this.performanceTracker);
			this.automaticLink = new AutomaticLink(mailboxInfo, this.logger, this.performanceTracker, new DirectoryPersonSearcher(session.MailboxOwner), this.contactStore);
		}

		internal BulkAutomaticLink(MailboxInfoForLinking mailboxInfo, ContactLinkingLogger logger, IContactLinkingPerformanceTracker performanceTracker, IDirectoryPersonSearcher directoryPersonSearcher, ContactStoreForBulkContactLinking contactStoreForBulkContactLinking)
		{
			Util.ThrowOnNullArgument(mailboxInfo, "mailboxInfo");
			Util.ThrowOnNullArgument(logger, "logger");
			Util.ThrowOnNullArgument(performanceTracker, "performanceTracker");
			Util.ThrowOnNullArgument(directoryPersonSearcher, "directoryPersonSearcher");
			Util.ThrowOnNullArgument(contactStoreForBulkContactLinking, "contactStoreForBulkContactLinking");
			this.logger = logger;
			this.performanceTracker = performanceTracker;
			this.contactStore = contactStoreForBulkContactLinking;
			this.automaticLink = new AutomaticLink(mailboxInfo, this.logger, this.performanceTracker, directoryPersonSearcher, this.contactStore);
		}

		public void Link(Contact contact)
		{
			base.CheckDisposed();
			if (!AutomaticLinkConfiguration.IsBulkEnabled)
			{
				BulkAutomaticLink.Tracer.TraceDebug((long)this.GetHashCode(), "BulkAutomaticLink::Link. Suppressing Automatic Linking based on registry key value.");
				return;
			}
			this.automaticLink.LinkNewOrUpdatedContactBeforeSave(contact.CoreItem, new Func<ContactInfoForLinking, IContactStoreForContactLinking, IEnumerable<ContactInfoForLinking>>(this.GetOtherContactsEnumeratorForBulk));
		}

		public void NotifyContactSaved(Contact contact)
		{
			base.CheckDisposed();
			if (!AutomaticLinkConfiguration.IsBulkEnabled)
			{
				BulkAutomaticLink.Tracer.TraceDebug((long)this.GetHashCode(), "BulkAutomaticLink::NotifyContactSaved. Suppressing Automatic Linking based on registry key value.");
				return;
			}
			Util.ThrowOnNullArgument(contact, "contact");
			Util.ThrowOnNullArgument(contact.Id, "contact.Id");
			this.performanceTracker.Start();
			try
			{
				this.PushContactOntoWorkingSet(contact);
			}
			finally
			{
				this.performanceTracker.Stop();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BulkAutomaticLink>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.performanceTracker != null)
			{
				this.logger.LogEvent(this.performanceTracker.GetLogEvent());
				this.performanceTracker = null;
			}
		}

		private void PushContactOntoWorkingSet(Contact contact)
		{
			contact.PropertyBag.Load(ContactInfoForLinking.Properties);
			this.contactStore.PushContactOntoWorkingSet(contact);
		}

		private IEnumerable<ContactInfoForLinking> GetOtherContactsEnumeratorForBulk(ContactInfoForLinking contactInfoContactBeingSaved, IContactStoreForContactLinking contactStoreForContactLinking)
		{
			return contactStoreForContactLinking.GetAllContacts().Take(AutomaticLink.MaximumNumberOfContactsToProcess.Value);
		}

		internal static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private readonly ContactLinkingLogger logger;

		private readonly AutomaticLink automaticLink;

		private ContactStoreForBulkContactLinking contactStore;

		private IContactLinkingPerformanceTracker performanceTracker;
	}
}
