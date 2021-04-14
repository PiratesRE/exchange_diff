using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationBatchCsvSchema : MigrationCsvSchemaBase
	{
		public MigrationBatchCsvSchema() : base(50000, MigrationBatchCsvSchema.requiredColumns, MigrationBatchCsvSchema.optionalColumns, null)
		{
		}

		protected MigrationBatchCsvSchema(int maximumRowCount, Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(maximumRowCount, requiredColumns, optionalColumns, prohibitedColumns)
		{
		}

		protected virtual PropertyDefinitionConstraint[] ClearTextPasswordConstraints
		{
			get
			{
				return new PropertyDefinitionConstraint[]
				{
					new StringLengthConstraint(1, 256),
					new NoLeadingOrTrailingWhitespaceConstraint()
				};
			}
		}

		private ProviderPropertyDefinition PasswordPropertyDefinition
		{
			get
			{
				return MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Password", this.ClearTextPasswordConstraints);
			}
		}

		public static PropertyConstraintViolationError ValidatePasswordIsNotBadPasswordValue(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			string a = value as string;
			if (string.Equals(a, "<Invalid Password>?`~! 23aeb1e0-b53e-41be-9565-7ca88c97b5b2\teda6458e-c843-45c3-b97f-73aa3c06ba9c"))
			{
				return new PropertyConstraintViolationError(ServerStrings.MigrationInvalidPassword, propertyDefinition, value, owner);
			}
			return null;
		}

		public override void ProcessRow(CsvRow row, out MigrationBatchError error)
		{
			error = null;
			base.ProcessRow(row, out error);
			string text;
			if (error == null && row.TryGetColumnValue("Password", out text) && !string.IsNullOrEmpty(text))
			{
				error = this.ValidateClearTextPassword(text, row, this.PasswordPropertyDefinition);
			}
		}

		protected virtual MigrationBatchError ValidateClearTextPassword(string clearTextPassword, CsvRow row, ProviderPropertyDefinition propertyDefinition)
		{
			PropertyValidationError propertyValidationError = propertyDefinition.ValidateValue(clearTextPassword, false);
			if (propertyValidationError != null)
			{
				row[propertyDefinition.Name] = "<Invalid Password>?`~! 23aeb1e0-b53e-41be-9565-7ca88c97b5b2\teda6458e-c843-45c3-b97f-73aa3c06ba9c";
				return base.CreateValidationError(row, ServerStrings.ColumnError(propertyDefinition.Name, propertyValidationError.Description));
			}
			row[propertyDefinition.Name] = MigrationServiceFactory.Instance.GetCryptoAdapter().ClearStringToEncryptedString(clearTextPassword);
			return null;
		}

		public const int InternalMaximumRowCount = 50000;

		public const int HeaderRowIndex = 0;

		internal const string UsernameColumnName = "Username";

		internal const string PasswordColumnName = "Password";

		internal const string UserRootFolderName = "UserRoot";

		internal const string BadPasswordValue = "<Invalid Password>?`~! 23aeb1e0-b53e-41be-9565-7ca88c97b5b2\teda6458e-c843-45c3-b97f-73aa3c06ba9c";

		private const int PasswordMaxLength = 256;

		internal static readonly PropertyDefinitionConstraint[] BadPasswordConstraint = new DelegateConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(MigrationBatchCsvSchema.ValidatePasswordIsNotBadPasswordValue))
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"EmailAddress",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("EmailAddress", MigrationCsvSchemaBase.EmailAddressConstraint)
			},
			{
				"Username",
				ADRecipientSchema.DisplayName
			},
			{
				"Password",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Password", MigrationBatchCsvSchema.BadPasswordConstraint)
			}
		};

		private static readonly PropertyDefinitionConstraint[] UserRootFolderConstraints = new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024),
			new NoLeadingOrTrailingWhitespaceConstraint()
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> optionalColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"UserRoot",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Password", MigrationBatchCsvSchema.UserRootFolderConstraints)
			}
		};
	}
}
