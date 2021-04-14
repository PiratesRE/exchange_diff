using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSTImportCsvSchema : MigrationBatchCsvSchema
	{
		public PSTImportCsvSchema() : base(int.MaxValue, PSTImportCsvSchema.requiredColumns, PSTImportCsvSchema.optionalColumns, null)
		{
		}

		internal const string PSTPathFileNameColumnName = "PSTPathFileName";

		internal const string TargetMailboxIdColumnName = "TargetMailboxId";

		internal const string TargetMailboxTypeColumnName = "TargetMailboxType";

		internal const string SourceRootFolderColumnName = "SourceRootFolderName";

		internal const string TargetRootFolderColumnName = "TargetRootFolderName";

		internal static readonly ProviderPropertyDefinition TargetMailboxTypePropertyDefinition = new SimpleProviderPropertyDefinition("TargetMailboxType", ExchangeObjectVersion.Exchange2012, typeof(MigrationMailboxType), PropertyDefinitionFlags.None, MigrationMailboxType.PrimaryOnly, new PropertyDefinitionConstraint[]
		{
			new ValueDefinedConstraint<MigrationMailboxType>(new MigrationMailboxType[]
			{
				MigrationMailboxType.PrimaryOnly,
				MigrationMailboxType.ArchiveOnly
			}, true)
		}, PropertyDefinitionConstraint.None);

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"PSTPathFileName",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("PSTPathFileName", PSTImportCsvSchema.PSTFilePathNameConstraints)
			},
			{
				"TargetMailboxId",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("TargetMailboxId", MigrationCsvSchemaBase.EmailAddressConstraint)
			}
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> optionalColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"TargetMailboxType",
				PSTImportCsvSchema.TargetMailboxTypePropertyDefinition
			},
			{
				"SourceRootFolderName",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("SourceRootFolderName", PSTImportCsvSchema.RootFolderNameConstraints)
			},
			{
				"TargetRootFolderName",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("TargetRootFolderName", PSTImportCsvSchema.RootFolderNameConstraints)
			}
		};

		private static readonly PropertyDefinitionConstraint[] PSTFilePathNameConstraints = new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 256),
			new NoLeadingOrTrailingWhitespaceConstraint()
		};

		private static readonly PropertyDefinitionConstraint[] RootFolderNameConstraints = new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024),
			new NoLeadingOrTrailingWhitespaceConstraint()
		};
	}
}
