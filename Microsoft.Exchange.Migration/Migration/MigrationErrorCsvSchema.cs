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
	internal class MigrationErrorCsvSchema : CsvSchema
	{
		public MigrationErrorCsvSchema() : base(int.MaxValue, MigrationErrorCsvSchema.requiredColumns, null, null)
		{
		}

		public static IEnumerable<MigrationBatchError> ReadErrors(Stream sourceStream)
		{
			MigrationErrorCsvSchema csvSchema = new MigrationErrorCsvSchema();
			foreach (CsvRow csvRow in csvSchema.Read(sourceStream))
			{
				CsvRow csvRow2 = csvRow;
				if (csvRow2.Index != 0)
				{
					MigrationBatchError error = new MigrationBatchError();
					int rowIndex = 0;
					CsvRow csvRow3 = csvRow;
					int.TryParse(csvRow3["RowIndex"], out rowIndex);
					error.RowIndex = rowIndex;
					MigrationError migrationError = error;
					CsvRow csvRow4 = csvRow;
					migrationError.EmailAddress = csvRow4["EmailAddress"];
					if (string.IsNullOrEmpty(error.EmailAddress))
					{
						error.EmailAddress = ServerStrings.EmailAddressMissing;
					}
					MigrationError migrationError2 = error;
					CsvRow csvRow5 = csvRow;
					migrationError2.LocalizedErrorMessage = new LocalizedString(csvRow5["ErrorMessage"]);
					yield return error;
				}
			}
			yield break;
		}

		public static int WriteHeaderAndErrors(StreamWriter streamWriter, IEnumerable<MigrationBatchError> errorCollection)
		{
			streamWriter.WriteCsvLine(new string[]
			{
				"RowIndex",
				"EmailAddress",
				"ErrorMessage"
			});
			return MigrationErrorCsvSchema.WriteErrors(streamWriter, errorCollection);
		}

		public static int WriteErrors(StreamWriter streamWriter, IEnumerable<MigrationBatchError> errorCollection)
		{
			int num = 0;
			if (errorCollection != null)
			{
				foreach (MigrationBatchError migrationBatchError in errorCollection)
				{
					streamWriter.WriteCsvLine(new string[]
					{
						migrationBatchError.RowIndex.ToString(),
						migrationBatchError.EmailAddress,
						migrationBatchError.LocalizedErrorMessage
					});
					num++;
				}
			}
			return num;
		}

		public const string RowIndexColumnName = "RowIndex";

		public const string EmailColumnName = "EmailAddress";

		public const string ErrorMessageColumnName = "ErrorMessage";

		private const int InternalMaximumRowCount = 2147483647;

		private static Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"RowIndex",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("RowIndex", null)
			},
			{
				"EmailAddress",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("EmailAddress", null)
			},
			{
				"ErrorMessage",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("ErrorMessage", null)
			}
		};
	}
}
