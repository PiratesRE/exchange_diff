using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Migration
{
	internal sealed class PublicFolderMigrationCsvSchema : MigrationCsvSchemaBase
	{
		public PublicFolderMigrationCsvSchema() : base(1000, PublicFolderMigrationCsvSchema.requiredColumns, null, null)
		{
		}

		public override string GetIdentifier(CsvRow row)
		{
			return row["TargetMailbox"];
		}

		internal const string FolderPathColumnName = "FolderPath";

		internal const string TargetMailboxColumnName = "TargetMailbox";

		private const int InternalMaximumRowCount = 1000;

		private static readonly PropertyDefinitionConstraint[] FolderPathConstraints = new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyConstraint(),
			new NoLeadingOrTrailingWhitespaceConstraint()
		};

		private static readonly ProviderPropertyDefinition FolderPathPropertyDefinition = new SimpleProviderPropertyDefinition("FolderPath", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PublicFolderMigrationCsvSchema.FolderPathConstraints, PublicFolderMigrationCsvSchema.FolderPathConstraints);

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"FolderPath",
				PublicFolderMigrationCsvSchema.FolderPathPropertyDefinition
			},
			{
				"TargetMailbox",
				ADObjectSchema.Name
			}
		};
	}
}
