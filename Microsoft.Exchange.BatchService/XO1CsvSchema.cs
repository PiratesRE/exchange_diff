using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XO1CsvSchema : MigrationCsvSchemaBase
	{
		public XO1CsvSchema() : base(int.MaxValue, XO1CsvSchema.requiredColumns, XO1CsvSchema.InternalOptionalColumns, null)
		{
		}

		public const string PuidColumn = "Puid";

		public const string FirstNameColumn = "FirstName";

		public const string LastNameColumn = "LastName";

		public const string AccountSizeColumn = "AccountSize";

		public const string TimeZoneColumn = "TimeZone";

		public const string LcidColumn = "Lcid";

		public const string AliasesColumn = "Aliases";

		public const char AliasesDelimiter = '\u0001';

		internal static readonly PropertyDefinitionConstraint[] PuidConstraint = new PropertyDefinitionConstraint[]
		{
			new MigrationCsvSchemaBase.CsvRangedValueConstraint<long>(long.MinValue, long.MaxValue)
		};

		internal static readonly PropertyDefinitionConstraint[] AccountSizeConstraint = new PropertyDefinitionConstraint[]
		{
			new MigrationCsvSchemaBase.CsvRangedValueConstraint<long>(long.MinValue, long.MaxValue)
		};

		internal static readonly PropertyDefinitionConstraint[] LcidConstraint = new PropertyDefinitionConstraint[]
		{
			new XO1CsvSchema.InternalLcidConstraint()
		};

		internal static readonly PropertyDefinitionConstraint[] TimeZoneConstraint = new PropertyDefinitionConstraint[]
		{
			new XO1CsvSchema.InternalTimeZoneConstraint()
		};

		internal static readonly PropertyDefinitionConstraint[] AliasesConstraint = new PropertyDefinitionConstraint[]
		{
			new XO1CsvSchema.InternalValidSmtpAddressListConstraint()
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"EmailAddress",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("EmailAddress", MigrationCsvSchemaBase.EmailAddressConstraint)
			},
			{
				"Puid",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition<long>("Puid", XO1CsvSchema.PuidConstraint)
			},
			{
				"AccountSize",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition<long>("AccountSize", XO1CsvSchema.AccountSizeConstraint)
			},
			{
				"Lcid",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Lcid", XO1CsvSchema.LcidConstraint)
			},
			{
				"TimeZone",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("TimeZone", XO1CsvSchema.TimeZoneConstraint)
			}
		};

		private static readonly Dictionary<string, ProviderPropertyDefinition> InternalOptionalColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"FirstName",
				ADUserSchema.FirstName
			},
			{
				"LastName",
				ADUserSchema.LastName
			},
			{
				"Aliases",
				MigrationCsvSchemaBase.GetDefaultPropertyDefinition("Aliases", XO1CsvSchema.AliasesConstraint)
			}
		};

		private class InternalLcidConstraint : PropertyDefinitionConstraint
		{
			public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
			{
				Exception ex = null;
				try
				{
					int culture = Convert.ToInt32(value);
					CultureInfo.GetCultureInfo(culture);
				}
				catch (FormatException ex2)
				{
					ex = ex2;
				}
				catch (InvalidCastException ex3)
				{
					ex = ex3;
				}
				catch (OverflowException ex4)
				{
					ex = ex4;
				}
				catch (CultureNotFoundException)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationEnumValueNotAllowed(value.ToString()), propertyDefinition, value, this);
				}
				if (ex != null)
				{
					return new PropertyConstraintViolationError(DataStrings.PropertyTypeMismatch(value.GetType().ToString(), typeof(int).ToString()), propertyDefinition, value, this);
				}
				return null;
			}
		}

		private class InternalTimeZoneConstraint : PropertyDefinitionConstraint
		{
			public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
			{
				string text = value.ToString();
				if (string.IsNullOrEmpty(text))
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNullOrEmpty, propertyDefinition, value, this);
				}
				try
				{
					if (ExTimeZone.UtcTimeZone.Id.Equals(text, StringComparison.Ordinal))
					{
						return null;
					}
					ExTimeZoneValue.Parse(text);
				}
				catch (FormatException)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationEnumValueNotAllowed(value.ToString()), propertyDefinition, value, this);
				}
				return null;
			}
		}

		private class InternalValidSmtpAddressListConstraint : PropertyDefinitionConstraint
		{
			public InternalValidSmtpAddressListConstraint()
			{
				this.emailAddressConstraint = new ValidSmtpAddressConstraint();
			}

			public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
			{
				string text = value.ToString();
				if (string.IsNullOrEmpty(text))
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNullOrEmpty, propertyDefinition, value, this);
				}
				foreach (string value2 in text.Split(new char[]
				{
					'\u0001'
				}))
				{
					PropertyConstraintViolationError propertyConstraintViolationError = this.emailAddressConstraint.Validate(value2, propertyDefinition, propertyBag);
					if (propertyConstraintViolationError != null)
					{
						return propertyConstraintViolationError;
					}
				}
				return null;
			}

			private ValidSmtpAddressConstraint emailAddressConstraint;
		}
	}
}
