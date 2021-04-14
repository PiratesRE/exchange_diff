using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class InvalidDataRow : IMigrationDataRow
	{
		public InvalidDataRow(int cursorPosition, MigrationBatchError error, MigrationType migrationType = MigrationType.None)
		{
			this.cursorPosition = cursorPosition;
			this.error = error;
			this.migrationType = migrationType;
		}

		public MigrationType MigrationType
		{
			get
			{
				return this.migrationType;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return MigrationUserRecipientType.Mailbox;
			}
		}

		public string Identifier
		{
			get
			{
				return this.error.EmailAddress;
			}
		}

		public string LocalMailboxIdentifier
		{
			get
			{
				return this.Identifier;
			}
		}

		public int CursorPosition
		{
			get
			{
				return this.cursorPosition;
			}
		}

		public MigrationBatchError Error
		{
			get
			{
				return this.error;
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

		private readonly int cursorPosition;

		private readonly MigrationBatchError error;

		private readonly MigrationType migrationType;
	}
}
