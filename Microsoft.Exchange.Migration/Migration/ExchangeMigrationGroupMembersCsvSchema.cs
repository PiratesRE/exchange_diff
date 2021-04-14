using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExchangeMigrationGroupMembersCsvSchema
	{
		public static IEnumerable<string> Read(Stream sourceStream)
		{
			CsvSchema csvSchema = new CsvSchema(50000, ExchangeMigrationGroupMembersCsvSchema.requiredColumns, null, null);
			foreach (CsvRow csvRow in csvSchema.Read(sourceStream))
			{
				CsvRow csvRow2 = csvRow;
				if (csvRow2.Index != 0)
				{
					CsvRow csvRow3 = csvRow;
					string smtpAddress = csvRow3["EmailAddress"];
					if (!string.IsNullOrEmpty(smtpAddress))
					{
						yield return smtpAddress;
					}
				}
			}
			yield break;
		}

		public static void Write(StreamWriter streamWriter, IEnumerable<string> members)
		{
			streamWriter.WriteCsvLine(new string[]
			{
				"EmailAddress"
			});
			foreach (string text in members)
			{
				streamWriter.WriteCsvLine(new string[]
				{
					text
				});
			}
		}

		public const string AttachmentName = "GroupMembers.csv";

		private const int MaximumRowCount = 50000;

		private const string EmailColumnName = "EmailAddress";

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"EmailAddress",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("EmailAddress", MigrationCsvSchemaBase.EmailAddressConstraint)
			}
		};
	}
}
