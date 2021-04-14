using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationLocalMoveCsvSchema : MigrationMoveCsvSchema
	{
		public MigrationLocalMoveCsvSchema() : base(int.MaxValue, MigrationLocalMoveCsvSchema.requiredColumns, MigrationLocalMoveCsvSchema.optionalColumns, null)
		{
		}

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"EmailAddress",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("EmailAddress", MigrationCsvSchemaBase.EmailAddressConstraint)
			}
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> optionalColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"TargetDatabase",
				ADUserSchema.DatabaseName
			},
			{
				"TargetArchiveDatabase",
				ADUserSchema.DatabaseName
			},
			{
				"BadItemLimit",
				MigrationMoveCsvSchema.BadItemLimit
			},
			{
				"MailboxType",
				MigrationMoveCsvSchema.MailboxTypePropertyDefinition
			}
		};
	}
}
