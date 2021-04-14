using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserMailboxBuilder : IMailboxBuilder<UserMailbox>
	{
		public UserMailboxBuilder(UserMailboxLocator locator, IEnumerable<ADObjectId> owners = null)
		{
			ArgumentValidator.ThrowIfNull("locator", locator);
			this.Mailbox = new UserMailbox(locator);
			this.owners = ((owners != null) ? new HashSet<ADObjectId>(owners) : new HashSet<ADObjectId>());
		}

		public UserMailbox Mailbox { get; private set; }

		public IMailboxBuilder<UserMailbox> BuildFromAssociation(MailboxAssociation association)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			this.Mailbox.IsMember = association.IsMember;
			this.Mailbox.JoinDate = association.JoinDate;
			this.Mailbox.LastVisitedDate = association.LastVisitedDate;
			this.Mailbox.ShouldEscalate = association.ShouldEscalate;
			this.Mailbox.IsAutoSubscribed = association.IsAutoSubscribed;
			this.Mailbox.IsPin = association.IsPin;
			return this;
		}

		public IMailboxBuilder<UserMailbox> BuildFromDirectory(ADRawEntry rawEntry)
		{
			ArgumentValidator.ThrowIfNull("rawEntry", rawEntry);
			this.Mailbox.ADObjectId = rawEntry.Id;
			this.Mailbox.Alias = (rawEntry[ADRecipientSchema.Alias] as string);
			this.Mailbox.DisplayName = (rawEntry[ADRecipientSchema.DisplayName] as string);
			this.Mailbox.ImAddress = ADPersonToContactConverter.GetSipUri(rawEntry);
			this.Mailbox.IsOwner = this.owners.Contains(rawEntry.Id);
			this.Mailbox.SmtpAddress = (SmtpAddress)rawEntry[ADRecipientSchema.PrimarySmtpAddress];
			this.Mailbox.Title = (rawEntry[ADOrgPersonSchema.Title] as string);
			return this;
		}

		public static readonly PropertyDefinition[] AllADProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.Alias,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.PrimarySmtpAddress,
			ADUserSchema.RTCSIPPrimaryUserAddress,
			ADOrgPersonSchema.Title
		};

		private readonly ISet<ADObjectId> owners;
	}
}
