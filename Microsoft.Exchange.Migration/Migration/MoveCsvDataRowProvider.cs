using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MoveCsvDataRowProvider : MigrationCSVDataRowProvider
	{
		public MoveCsvDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider) : base(job, dataProvider, MigrationCSVDataRowProvider.CreateCsvSchema(job))
		{
		}

		protected override IMigrationDataRow CreateDataRow(CsvRow row)
		{
			InvalidDataRow invalidDataRow = base.GetInvalidDataRow(row, base.MigrationJob.MigrationType);
			if (invalidDataRow != null)
			{
				return invalidDataRow;
			}
			MoveJobSubscriptionSettings moveJobSubscriptionSettings = base.MigrationJob.SubscriptionSettings as MoveJobSubscriptionSettings;
			return new MoveMigrationDataRow(row.Index, row["EmailAddress"], base.MigrationJob.JobDirection, row, moveJobSubscriptionSettings != null && moveJobSubscriptionSettings.ArchiveOnly != null && moveJobSubscriptionSettings.ArchiveOnly.Value);
		}
	}
}
