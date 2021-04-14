using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NspiCsvMigrationDataRowProvider : MigrationCSVDataRowProvider
	{
		internal NspiCsvMigrationDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider, bool discoverProvisioning = true) : base(job, dataProvider, new ExchangeMigrationBatchCsvSchema())
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = job.SourceEndpoint as ExchangeOutlookAnywhereEndpoint;
			this.discoverProvisioning = discoverProvisioning;
			if (discoverProvisioning)
			{
				this.nspiDataReader = exchangeOutlookAnywhereEndpoint.GetNspiDataReader(job);
			}
		}

		protected override IMigrationDataRow CreateDataRow(CsvRow row)
		{
			InvalidDataRow invalidDataRow = base.GetInvalidDataRow(row, MigrationType.ExchangeOutlookAnywhere);
			if (invalidDataRow != null)
			{
				return invalidDataRow;
			}
			if (!row.ColumnMap.Contains("EmailAddress"))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("EmailAddress"), MigrationType.ExchangeOutlookAnywhere);
			}
			string text = row["EmailAddress"];
			bool flag = true;
			string text2 = text;
			string text3;
			if (row.TryGetColumnValue("Username", out text3) && !string.IsNullOrEmpty(text3))
			{
				text2 = text3;
				flag = false;
			}
			ExchangeMigrationDataRow exchangeMigrationDataRow;
			if (!this.discoverProvisioning)
			{
				exchangeMigrationDataRow = new ExchangeMigrationDataRow(row.Index, text2, MigrationUserRecipientType.Mailbox);
			}
			else
			{
				IMigrationDataRow recipient = this.nspiDataReader.GetRecipient(text2);
				invalidDataRow = (recipient as InvalidDataRow);
				if (invalidDataRow != null)
				{
					MigrationBatchError error = invalidDataRow.Error;
					error.RowIndex = row.Index;
					return new InvalidDataRow(row.Index, error, MigrationType.ExchangeOutlookAnywhere);
				}
				NspiMigrationDataRow nspiMigrationDataRow = (NspiMigrationDataRow)recipient;
				if (nspiMigrationDataRow.Recipient.RecipientType != MigrationUserRecipientType.Mailbox)
				{
					return base.GetInvalidDataRow(row, Strings.UnsupportedSourceRecipientTypeError(nspiMigrationDataRow.Recipient.RecipientType.ToString()), MigrationType.ExchangeOutlookAnywhere);
				}
				exchangeMigrationDataRow = nspiMigrationDataRow;
			}
			string text4;
			row.TryGetColumnValue("Password", out text4);
			bool forceChangePassword = false;
			string value;
			if (row.TryGetColumnValue("ForceChangePassword", out value) && !bool.TryParse(value, out forceChangePassword))
			{
				forceChangePassword = false;
			}
			exchangeMigrationDataRow.CursorPosition = row.Index;
			exchangeMigrationDataRow.EncryptedPassword = (text4 ?? string.Empty);
			exchangeMigrationDataRow.ForceChangePassword = forceChangePassword;
			if (!flag)
			{
				exchangeMigrationDataRow.TargetIdentifier = text;
			}
			return exchangeMigrationDataRow;
		}

		private readonly NspiMigrationDataReader nspiDataReader;

		private readonly bool discoverProvisioning;
	}
}
