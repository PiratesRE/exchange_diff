using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContactInfoForLinkingFromPropertyBag : ContactInfoForLinking
	{
		protected ContactInfoForLinkingFromPropertyBag(PropertyBagAdaptor propertyBagAdaptor, MailboxSession mailboxSession, IStorePropertyBag propertyBag) : base(propertyBagAdaptor)
		{
			this.mailboxSession = mailboxSession;
		}

		public static ContactInfoForLinkingFromPropertyBag Create(MailboxSession mailboxSession, IStorePropertyBag propertyBag)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			PropertyBagAdaptor propertyBagAdaptor = PropertyBagAdaptor.Create(propertyBag);
			return new ContactInfoForLinkingFromPropertyBag(propertyBagAdaptor, mailboxSession, propertyBag);
		}

		protected override void UpdateContact(IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker)
		{
			ContactInfoForLinking.Tracer.TraceDebug<VersionedId, string>((long)this.GetHashCode(), "ContactInfoForLinkingFromPropertyBag.UpdateContact: setting link properties AND saving contact with id = {0}; given-name: {1}", base.ItemId, base.GivenName);
			base.RetryOnTransientExceptionCatchObjectNotFoundException(logger, "update of contact Id=" + base.ItemId, delegate
			{
				using (Contact contact = Contact.Bind(this.mailboxSession, base.ItemId, new PropertyDefinition[]
				{
					ContactSchema.PersonId
				}))
				{
					base.SetLinkingProperties(PropertyBagAdaptor.Create(contact));
					AutomaticLink.DisableAutomaticLinkingForItem(contact);
					contact.Save(SaveMode.NoConflictResolution);
				}
			});
			performanceTracker.IncrementContactsUpdated();
		}

		private readonly MailboxSession mailboxSession;
	}
}
