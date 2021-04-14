using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTCSVDataRowProvider : MigrationCSVDataRowProvider, IMigrationDataRowProvider
	{
		internal PSTCSVDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider) : base(job, dataProvider, new PSTImportCsvSchema())
		{
		}

		protected override IMigrationDataRow CreateDataRow(CsvRow row)
		{
			InvalidDataRow invalidDataRow = base.GetInvalidDataRow(row, MigrationType.PSTImport);
			if (invalidDataRow != null)
			{
				return invalidDataRow;
			}
			string text = row["PSTPathFileName"];
			if (string.IsNullOrEmpty(text))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("PSTPathFileName"), MigrationType.PSTImport);
			}
			SmtpAddress empty = SmtpAddress.Empty;
			string emailAddress = row["TargetMailboxId"];
			if (!MigrationServiceHelper.TryParseSmtpAddress(emailAddress, out empty))
			{
				return base.GetInvalidDataRow(row, Strings.ValueNotProvidedForColumn("TargetMailboxId"), MigrationType.PSTImport);
			}
			MigrationMailboxType targetMailboxType = MigrationMailboxType.PrimaryOnly;
			string value = null;
			if (row.TryGetColumnValue("TargetMailboxType", out value))
			{
				Enum.TryParse<MigrationMailboxType>(value, out targetMailboxType);
			}
			string text2 = null;
			if (row.TryGetColumnValue("SourceRootFolderName", out text2) && string.IsNullOrEmpty(text2))
			{
				text2 = null;
			}
			string text3 = null;
			if (row.TryGetColumnValue("TargetRootFolderName", out text3) && string.IsNullOrEmpty(text3))
			{
				text3 = null;
			}
			return new PSTMigrationDataRow(row.Index, text, empty, targetMailboxType, text2, text3);
		}
	}
}
