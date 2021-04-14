using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IMAPCSVDataRowProvider : MigrationCSVDataRowProvider, IMigrationDataRowProvider
	{
		internal IMAPCSVDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider) : base(job, dataProvider, new MigrationBatchCsvSchema())
		{
		}

		protected override IMigrationDataRow CreateDataRow(CsvRow row)
		{
			SmtpAddress empty = SmtpAddress.Empty;
			string text = null;
			InvalidDataRow invalidDataRow = base.GetInvalidDataRow(row, MigrationType.IMAP);
			if (invalidDataRow != null)
			{
				return invalidDataRow;
			}
			string emailAddress = row["EmailAddress"];
			if (!MigrationServiceHelper.TryParseSmtpAddress(emailAddress, out empty))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("EmailAddress"), MigrationType.IMAP);
			}
			string text2 = row["Username"];
			if (string.IsNullOrEmpty(text2))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("Username"), MigrationType.IMAP);
			}
			string text3 = row["Password"];
			if (string.IsNullOrEmpty(text3))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("Password"), MigrationType.IMAP);
			}
			if (row.TryGetColumnValue("UserRoot", out text) && string.IsNullOrEmpty(text))
			{
				text = null;
			}
			return new IMAPMigrationDataRow(row.Index, empty, text2, text3, text);
		}
	}
}
