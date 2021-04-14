using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderMigrationRequestCsvSchema : CsvSchema
	{
		public PublicFolderMigrationRequestCsvSchema() : base(1000, PublicFolderMigrationRequestCsvSchema.requiredColumns, null, null)
		{
		}

		internal const string FolderPathColumnName = "FolderPath";

		internal const string TargetMailboxColumnName = "TargetMailbox";

		private const int InternalMaximumRowCount = 1000;

		private static PropertyDefinitionConstraint[] FolderPathConstraints = new PropertyDefinitionConstraint[]
		{
			new NotNullOrEmptyConstraint(),
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new StringLengthConstraint(1, 0)
		};

		private static readonly ProviderPropertyDefinition FolderPathPropertyDefinition = new SimpleProviderPropertyDefinition("FolderPath", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PublicFolderMigrationRequestCsvSchema.FolderPathConstraints, PublicFolderMigrationRequestCsvSchema.FolderPathConstraints);

		private static Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"FolderPath",
				PublicFolderMigrationRequestCsvSchema.FolderPathPropertyDefinition
			},
			{
				"TargetMailbox",
				ADObjectSchema.Name
			}
		};
	}
}
