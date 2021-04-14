using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiMigrationDataRow : ExchangeMigrationDataRow
	{
		private NspiMigrationDataRow(int rowIndex, ExchangeMigrationRecipient recipient) : base(rowIndex, recipient.Identifier, MigrationUserRecipientType.Mailbox)
		{
			this.Recipient = recipient;
		}

		public override MigrationUserRecipientType RecipientType
		{
			get
			{
				return this.Recipient.RecipientType;
			}
		}

		public static bool TryCreate(PropRow row, int index, long[] properties, out IMigrationDataRow dataRow, out MigrationBatchError error)
		{
			MigrationUtil.ThrowOnNullArgument(row, "row");
			MigrationUtil.ThrowOnNullArgument(properties, "properties");
			if (row.Properties.Count != properties.Length)
			{
				throw new ArgumentOutOfRangeException("row", "row.Properties.Count != properties.Length");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ExchangeMigrationRecipient exchangeMigrationRecipient = NspiMigrationDataReader.TryCreateRecipient(row, properties, false);
			if (exchangeMigrationRecipient == null)
			{
				error = null;
				dataRow = null;
				return false;
			}
			if (string.IsNullOrEmpty(exchangeMigrationRecipient.Identifier))
			{
				error = new MigrationBatchError
				{
					EmailAddress = null,
					RowIndex = index,
					LocalizedErrorMessage = ServerStrings.MigrationNSPIMissingRequiredField(PropTag.SmtpAddress)
				};
				dataRow = null;
				return false;
			}
			error = null;
			dataRow = new NspiMigrationDataRow(index, exchangeMigrationRecipient);
			return true;
		}

		public readonly ExchangeMigrationRecipient Recipient;
	}
}
