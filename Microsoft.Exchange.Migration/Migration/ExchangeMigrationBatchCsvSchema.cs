using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeMigrationBatchCsvSchema : MigrationBatchCsvSchema
	{
		public ExchangeMigrationBatchCsvSchema() : base(ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationSourceStagedExchangeCSVMailboxMaximumCount"), ExchangeMigrationBatchCsvSchema.requiredColumns, ExchangeMigrationBatchCsvSchema.optionalColumns, null)
		{
		}

		protected override PropertyDefinitionConstraint[] ClearTextPasswordConstraints
		{
			get
			{
				return new PropertyDefinitionConstraint[]
				{
					new StringLengthConstraint(6, 16),
					new NoLeadingOrTrailingWhitespaceConstraint()
				};
			}
		}

		public override void ValidateRow(CsvRow row)
		{
			base.ValidateRow(row);
			string text;
			bool flag;
			if (row.TryGetColumnValue("ForceChangePassword", out text) && !string.IsNullOrEmpty(text) && !bool.TryParse(text, out flag))
			{
				PropertyValidationError error = new PropertyValidationError(DataStrings.ConstraintViolationValueIsNotAllowed("true, false", text), ExchangeMigrationBatchCsvSchema.ForceChangePasswordPropertyDefinition, null);
				base.OnPropertyValidationError(row, "ForceChangePassword", error);
			}
		}

		public const string ForceChangePasswordColumnName = "ForceChangePassword";

		private const int PasswordMaxLength = 16;

		private const int PasswordMinLength = 6;

		private static readonly ProviderPropertyDefinition ForceChangePasswordPropertyDefinition = ADUserSchema.ResetPasswordOnNextLogon;

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
				"Password",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Password", MigrationBatchCsvSchema.BadPasswordConstraint)
			},
			{
				"ForceChangePassword",
				ExchangeMigrationBatchCsvSchema.ForceChangePasswordPropertyDefinition
			},
			{
				"Username",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Username", MigrationCsvSchemaBase.EmailAddressConstraint)
			}
		};
	}
}
