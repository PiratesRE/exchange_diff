using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationCSVDataRowProvider : IMigrationDataRowProvider
	{
		internal MigrationCSVDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider, MigrationCsvSchemaBase csvSchema)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "DataProvider");
			this.migrationJob = job;
			this.dataProvider = dataProvider;
			this.csvSchema = csvSchema;
			this.csvSchema.AllowUnknownColumnsInCSV = job.AllowUnknownColumnsInCsv;
		}

		protected IMigrationDataProvider DataProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		protected MigrationJob MigrationJob
		{
			get
			{
				return this.migrationJob;
			}
		}

		public static MigrationCsvSchemaBase CreateCsvSchema(MigrationType type, bool isStaged, bool isTenantOnboarding)
		{
			if (type <= MigrationType.ExchangeRemoteMove)
			{
				switch (type)
				{
				case MigrationType.IMAP:
					return new MigrationBatchCsvSchema();
				case MigrationType.XO1:
					return new XO1CsvSchema();
				case MigrationType.IMAP | MigrationType.XO1:
					break;
				case MigrationType.ExchangeOutlookAnywhere:
					if (!isStaged)
					{
						return null;
					}
					return new ExchangeMigrationBatchCsvSchema();
				default:
					if (type == MigrationType.ExchangeRemoteMove)
					{
						if (!isTenantOnboarding)
						{
							return new MigrationRemoteMoveCsvSchema();
						}
						return new MigrationRemoteMoveOnboardingCsvSchema();
					}
					break;
				}
			}
			else
			{
				if (type == MigrationType.ExchangeLocalMove)
				{
					return new MigrationLocalMoveCsvSchema();
				}
				if (type == MigrationType.PSTImport)
				{
					return new PSTImportCsvSchema();
				}
				if (type == MigrationType.PublicFolder)
				{
					return new PublicFolderMigrationCsvSchema();
				}
			}
			return null;
		}

		public static MigrationCsvSchemaBase CreateCsvSchema(MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			return MigrationCSVDataRowProvider.CreateCsvSchema(job.MigrationType, job.IsStaged, !string.IsNullOrEmpty(job.TenantName) && job.SourceEndpoint != null);
		}

		public virtual IEnumerable<IMigrationDataRow> GetNextBatchItem(string cursorPosition, int maxCountHint)
		{
			int lastProcessedRowIndex = 0;
			if (!string.IsNullOrEmpty(cursorPosition) && !int.TryParse(cursorPosition, out lastProcessedRowIndex))
			{
				throw new ArgumentException("LastProcessedIndex should be an int but was " + cursorPosition);
			}
			using (IMigrationMessageItem message = this.migrationJob.FindMessageItem(this.DataProvider, this.migrationJob.InitializationPropertyDefinitions))
			{
				using (IMigrationAttachment attachment = message.GetAttachment("Request.csv", PropertyOpenMode.ReadOnly))
				{
					Stream csvStream = attachment.Stream;
					foreach (CsvRow row in this.csvSchema.Read(csvStream))
					{
						CsvRow csvRow = row;
						if (csvRow.Index > lastProcessedRowIndex)
						{
							yield return this.CreateDataRow(row);
						}
					}
				}
			}
			yield break;
		}

		internal InvalidDataRow GetInvalidDataRow(CsvRow row, MigrationType migrationType)
		{
			MigrationBatchError rowValidationError = MigrationBatchCsvProcessor.GetRowValidationError(row, this.csvSchema);
			if (rowValidationError == null)
			{
				return null;
			}
			return new InvalidDataRow(row.Index, rowValidationError, migrationType);
		}

		internal InvalidDataRow GetInvalidDataRow(CsvRow row, LocalizedString errorMessage, MigrationType migrationType)
		{
			return new InvalidDataRow(row.Index, this.csvSchema.CreateValidationError(row, errorMessage), migrationType);
		}

		protected static bool TryConvertStringToBool(string boolString, out bool value)
		{
			string key;
			switch (key = boolString.ToLowerInvariant())
			{
			case "$true":
			case "true":
			case "1":
			case "yes":
				value = true;
				return true;
			case "$false":
			case "false":
			case "0":
			case "no":
				value = false;
				return true;
			}
			value = false;
			return false;
		}

		protected abstract IMigrationDataRow CreateDataRow(CsvRow row);

		public const string True1 = "$true";

		public const string True2 = "true";

		public const string True3 = "1";

		public const string True4 = "yes";

		public const string False1 = "$false";

		public const string False2 = "false";

		public const string False3 = "0";

		public const string False4 = "no";

		private readonly IMigrationDataProvider dataProvider;

		private readonly MigrationJob migrationJob;

		private readonly MigrationCsvSchemaBase csvSchema;
	}
}
