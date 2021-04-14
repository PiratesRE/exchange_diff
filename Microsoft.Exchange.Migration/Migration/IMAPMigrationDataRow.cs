using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IMAPMigrationDataRow : IMigrationDataRow
	{
		public IMAPMigrationDataRow(int rowIndex, SmtpAddress destinationEmail, string imapUserId, string encryptedPassword, string migrationUserRootFolder)
		{
			MigrationUtil.ThrowOnNullArgument(destinationEmail, "destinationEmail");
			MigrationUtil.ThrowOnNullOrEmptyArgument(imapUserId, "imapUserId");
			MigrationUtil.ThrowOnNullOrEmptyArgument(encryptedPassword, "encryptedPassword");
			if (rowIndex < 1)
			{
				throw new ArgumentException("RowIndex should not be less than 1");
			}
			this.CursorPosition = rowIndex;
			this.DestinationEmailAddress = destinationEmail;
			this.ImapUserId = imapUserId;
			this.EncryptedPassword = encryptedPassword;
			this.MigrationUserRootFolder = migrationUserRootFolder;
		}

		public SmtpAddress DestinationEmailAddress { get; private set; }

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.IMAP;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return MigrationUserRecipientType.Mailbox;
			}
		}

		public int CursorPosition { get; private set; }

		public string ImapUserId { get; private set; }

		public string EncryptedPassword { get; private set; }

		public string MigrationUserRootFolder { get; private set; }

		public string Identifier
		{
			get
			{
				return this.DestinationEmailAddress.ToString().ToLowerInvariant();
			}
		}

		public string LocalMailboxIdentifier
		{
			get
			{
				return this.Identifier;
			}
		}

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return false;
			}
		}

		public string RemoteIdentifier
		{
			get
			{
				return null;
			}
		}
	}
}
