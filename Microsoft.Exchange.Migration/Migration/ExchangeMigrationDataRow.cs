using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeMigrationDataRow : IMigrationDataRow
	{
		public ExchangeMigrationDataRow(int rowIndex, string identifier, MigrationUserRecipientType recipientType = MigrationUserRecipientType.Mailbox)
		{
			this.CursorPosition = rowIndex;
			this.RemoteIdentifier = identifier;
			this.RecipientType = recipientType;
		}

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public virtual MigrationUserRecipientType RecipientType { get; private set; }

		public int CursorPosition { get; internal set; }

		public string Identifier
		{
			get
			{
				if (this.TargetIdentifier != null)
				{
					return this.TargetIdentifier;
				}
				return this.RemoteIdentifier;
			}
		}

		public string LocalMailboxIdentifier
		{
			get
			{
				return this.Identifier;
			}
		}

		public string RemoteIdentifier { get; set; }

		public string TargetIdentifier { get; set; }

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return this.TargetIdentifier != null;
			}
		}

		public string EncryptedPassword { get; set; }

		public bool ForceChangePassword { get; set; }
	}
}
