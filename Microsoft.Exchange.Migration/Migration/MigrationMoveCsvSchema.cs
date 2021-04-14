using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationMoveCsvSchema : MigrationBatchCsvSchema
	{
		protected MigrationMoveCsvSchema(int maximumRowCount, Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(maximumRowCount, requiredColumns, optionalColumns, prohibitedColumns)
		{
		}

		protected override MigrationBatchError ValidateClearTextPassword(string clearTextPassword, CsvRow row, ProviderPropertyDefinition propertyDefinition)
		{
			return null;
		}

		internal const string TargetDatabaseColumnName = "TargetDatabase";

		internal const string TargetArchiveDatabaseColumnName = "TargetArchiveDatabase";

		internal const string BadItemLimitColumnName = "BadItemLimit";

		internal const string LargeItemLimitColumnName = "LargeItemLimit";

		internal const string MailboxTypeColumnName = "MailboxType";

		internal static readonly SimpleProviderPropertyDefinition BadItemLimit = new SimpleProviderPropertyDefinition("BadItemLimit", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new MigrationMoveCsvSchema.CsvRangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		internal static readonly SimpleProviderPropertyDefinition LargeItemLimit = new SimpleProviderPropertyDefinition("LargeItemLimit", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new MigrationMoveCsvSchema.CsvRangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		internal static readonly ProviderPropertyDefinition MailboxTypePropertyDefinition = new SimpleProviderPropertyDefinition("MailboxType", ExchangeObjectVersion.Exchange2012, typeof(MigrationMailboxType), PropertyDefinitionFlags.None, MigrationMailboxType.PrimaryAndArchive, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(MigrationMailboxType))
		}, PropertyDefinitionConstraint.None);

		private class CsvRangedUnlimitedConstraint<T> : RangedUnlimitedConstraint<T> where T : struct, IComparable
		{
			public CsvRangedUnlimitedConstraint(T minValue, T maxValue) : base(minValue, maxValue)
			{
			}

			public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
			{
				Unlimited<T> unlimited;
				if (value is string)
				{
					unlimited = Unlimited<T>.Parse((string)value);
				}
				else
				{
					unlimited = (Unlimited<T>)value;
				}
				return base.Validate(unlimited, propertyDefinition, propertyBag);
			}
		}
	}
}
