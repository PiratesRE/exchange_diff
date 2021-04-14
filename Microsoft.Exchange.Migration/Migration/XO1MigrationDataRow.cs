using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XO1MigrationDataRow : IMigrationDataRow
	{
		public XO1MigrationDataRow(int rowIndex, string email, long puid, string firstName, string lastName, ExTimeZone timeZone, int localeId, string[] emailAddresses, long accountSize)
		{
			if (rowIndex < 1)
			{
				throw new ArgumentException("RowIndex should not be less than 1");
			}
			this.CursorPosition = rowIndex;
			this.Identifier = email;
			this.Puid = puid;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.TimeZone = timeZone;
			this.LocaleId = localeId;
			this.EmailAddresses = emailAddresses;
			this.AccountSize = accountSize;
		}

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.XO1;
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

		public string Identifier { get; private set; }

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

		public long Puid { get; private set; }

		public string FirstName { get; private set; }

		public string LastName { get; private set; }

		public ExTimeZone TimeZone { get; private set; }

		public int LocaleId { get; private set; }

		public string[] EmailAddresses { get; private set; }

		public long AccountSize { get; private set; }
	}
}
