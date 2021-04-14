using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationPreexistingBatchCsvDataRowProvider : MigrationCSVDataRowProvider
	{
		public MigrationPreexistingBatchCsvDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider) : base(job, dataProvider, new MigrationPreexistingBatchCsvSchema())
		{
		}

		protected override IMigrationDataRow CreateDataRow(CsvRow row)
		{
			string text = row["JobItemGuid"];
			Guid identity;
			if (Guid.TryParse(text, out identity))
			{
				MigrationJobItem byGuid = MigrationJobItem.GetByGuid(base.DataProvider, identity);
				if (byGuid != null)
				{
					if ((base.MigrationJob.BatchFlags & MigrationBatchFlags.DisableOnCopy) == MigrationBatchFlags.DisableOnCopy && byGuid.State != MigrationState.Disabled)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "disabling old migration job item {0}", new object[]
						{
							byGuid
						});
						byGuid.SetStatus(base.DataProvider, MigrationUserStatus.Failed, MigrationState.Disabled, null, null, null, null, null, null, false, new MigrationUserMovedToAnotherBatchException(base.MigrationJob.JobName));
					}
					return new MigrationPreexistingDataRow(row.Index, byGuid);
				}
			}
			return new InvalidDataRow(row.Index, new MigrationBatchError
			{
				RowIndex = row.Index,
				EmailAddress = text,
				LocalizedErrorMessage = Strings.MigrationJobItemNotFound(text)
			}, base.MigrationJob.MigrationType);
		}
	}
}
