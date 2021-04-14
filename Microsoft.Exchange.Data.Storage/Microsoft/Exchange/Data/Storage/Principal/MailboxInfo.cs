using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxInfo : IMailboxInfo
	{
		public MailboxInfo(Guid mailboxGuid, ADObjectId databaseId, IGenericADUser adUser, IMailboxConfiguration mailboxConfiguration, IMailboxLocation mailboxLocation)
		{
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfNull("adUser", adUser);
			ArgumentValidator.ThrowIfNull("mailboxConfiguration", mailboxConfiguration);
			ArgumentValidator.ThrowIfNull("mailboxLocation", mailboxLocation);
			MailboxLocationType? mailboxLocationType = adUser.GetMailboxLocationType(mailboxGuid);
			if (mailboxLocationType == null)
			{
				throw new ArgumentException("The provided mailbox guid doesn't match with any of the user's mailbox.");
			}
			this.MailboxType = mailboxLocationType.Value;
			this.DisplayName = adUser.DisplayName;
			this.PrimarySmtpAddress = adUser.PrimarySmtpAddress;
			this.ExternalEmailAddress = adUser.ExternalEmailAddress;
			this.EmailAddresses = (adUser.EmailAddresses ?? Array<ProxyAddress>.Empty);
			this.OrganizationId = adUser.OrganizationId;
			this.MailboxGuid = mailboxGuid;
			this.MailboxDatabase = databaseId;
			if (this.IsArchive)
			{
				this.archiveName = ((adUser.ArchiveName != null) ? (adUser.ArchiveName.FirstOrDefault<string>() ?? string.Empty) : string.Empty);
				this.archiveState = adUser.ArchiveState;
				this.archiveStatus = adUser.ArchiveStatus;
				this.IsRemote = adUser.IsArchiveMailboxRemote();
			}
			else
			{
				this.IsRemote = adUser.IsPrimaryMailboxRemote();
			}
			if (this.IsRemote)
			{
				this.remoteIdentity = this.GetRemoteIdentity(adUser, this.IsArchive);
			}
			else if (this.MailboxDatabase.IsNullOrEmpty())
			{
				throw new ObjectNotFoundException(ServerStrings.MailboxDatabaseRequired(mailboxGuid));
			}
			this.WhenMailboxCreated = adUser.WhenMailboxCreated;
			this.Location = mailboxLocation;
			this.Configuration = mailboxConfiguration;
		}

		public string DisplayName { get; private set; }

		public SmtpAddress PrimarySmtpAddress { get; private set; }

		public ProxyAddress ExternalEmailAddress { get; private set; }

		public IEnumerable<ProxyAddress> EmailAddresses { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public ADObjectId MailboxDatabase { get; private set; }

		public DateTime? WhenMailboxCreated { get; private set; }

		public bool IsArchive
		{
			get
			{
				return this.MailboxType == MailboxLocationType.MainArchive;
			}
		}

		public MailboxLocationType MailboxType { get; private set; }

		public string ArchiveName
		{
			get
			{
				if (!this.IsArchive)
				{
					throw new InvalidOperationException("Not an archive mailbox");
				}
				return this.archiveName;
			}
		}

		public ArchiveStatusFlags ArchiveStatus
		{
			get
			{
				if (!this.IsArchive)
				{
					throw new InvalidOperationException("Not an archive mailbox");
				}
				return this.archiveStatus;
			}
		}

		public ArchiveState ArchiveState
		{
			get
			{
				if (!this.IsArchive)
				{
					throw new InvalidOperationException("Not an archive mailbox");
				}
				return this.archiveState;
			}
		}

		public SmtpAddress? RemoteIdentity
		{
			get
			{
				if (!this.IsRemote)
				{
					throw new InvalidOperationException("Not a remote mailbox");
				}
				return this.remoteIdentity;
			}
		}

		public bool IsRemote { get; private set; }

		public bool IsAggregated
		{
			get
			{
				return this.MailboxType == MailboxLocationType.Aggregated;
			}
		}

		public IMailboxLocation Location { get; private set; }

		public IMailboxConfiguration Configuration { get; private set; }

		public override string ToString()
		{
			return string.Format("Display Name: {0}, Mailbox Guid: {1}, Database: {2}, Location: {3}", new object[]
			{
				this.DisplayName,
				this.MailboxGuid,
				this.MailboxDatabase,
				this.Location
			});
		}

		private SmtpAddress? GetRemoteIdentity(IGenericADUser adUser, bool isArchive)
		{
			if (isArchive)
			{
				if (adUser.ArchiveDomain != null && (adUser.ArchiveStatus & ArchiveStatusFlags.Active) == ArchiveStatusFlags.Active)
				{
					return new SmtpAddress?(new SmtpAddress(SmtpProxyAddress.EncapsulateExchangeGuid(adUser.ArchiveDomain.Domain, adUser.ArchiveGuid)));
				}
			}
			else if (adUser.ExternalEmailAddress.Prefix == ProxyAddressPrefix.Smtp && SmtpAddress.IsValidSmtpAddress(adUser.ExternalEmailAddress.AddressString))
			{
				return new SmtpAddress?(new SmtpAddress(adUser.ExternalEmailAddress.AddressString));
			}
			return null;
		}

		private readonly string archiveName;

		private readonly ArchiveState archiveState;

		private readonly ArchiveStatusFlags archiveStatus;

		private readonly SmtpAddress? remoteIdentity;
	}
}
