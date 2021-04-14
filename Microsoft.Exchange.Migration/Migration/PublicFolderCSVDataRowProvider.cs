using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderCSVDataRowProvider : IMigrationDataRowProvider
	{
		public PublicFolderCSVDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			this.migrationJob = job;
			this.dataProvider = dataProvider;
			this.csvSchema = new PublicFolderMigrationCsvSchema();
		}

		public IEnumerable<IMigrationDataRow> GetNextBatchItem(string cursorPosition, int maxCountHint)
		{
			this.BootstrapUniqueMailboxesCollection();
			int lastProcessedRowIndex = 0;
			if (!string.IsNullOrEmpty(cursorPosition) && !int.TryParse(cursorPosition, out lastProcessedRowIndex))
			{
				throw new ArgumentException("cursorPosition is not an integer value: " + cursorPosition);
			}
			int rowIndex = lastProcessedRowIndex;
			while (rowIndex < this.uniqueTargetMailboxes.Count && maxCountHint > 0)
			{
				yield return new PublicFolderMigrationDataRow(rowIndex + 1, this.uniqueTargetMailboxes[rowIndex]);
				maxCountHint--;
				rowIndex++;
			}
			yield break;
		}

		private void BootstrapUniqueMailboxesCollection()
		{
			if (this.uniqueTargetMailboxes != null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			List<string> list = new List<string>();
			using (IMigrationMessageItem migrationMessageItem = this.migrationJob.FindMessageItem(this.dataProvider, this.migrationJob.InitializationPropertyDefinitions))
			{
				using (IMigrationAttachment attachment = migrationMessageItem.GetAttachment("Request.csv", PropertyOpenMode.ReadOnly))
				{
					Stream stream = attachment.Stream;
					foreach (CsvRow csvRow in this.csvSchema.Read(stream))
					{
						string item;
						if (csvRow.Index != 0 && csvRow.TryGetColumnValue("TargetMailbox", out item) && hashSet.Add(item))
						{
							list.Add(item);
						}
					}
				}
			}
			this.uniqueTargetMailboxes = list;
		}

		private readonly IMigrationDataProvider dataProvider;

		private readonly MigrationJob migrationJob;

		private readonly MigrationCsvSchemaBase csvSchema;

		private List<string> uniqueTargetMailboxes;
	}
}
