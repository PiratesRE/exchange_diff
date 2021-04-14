using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationHelper
	{
		public static string AppendDiagnosticHistory(string history, params string[] entryFields)
		{
			return MigrationHelper.MigrationDiagnosticHistory.AppendDiagnosticHistory(history, entryFields);
		}

		internal static T? GetEnumPropertyOrNull<T>(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition) where T : struct
		{
			object valueOrDefault = item.GetValueOrDefault<object>(propertyDefinition, null);
			if (valueOrDefault == null)
			{
				return null;
			}
			return new T?(MigrationHelper.GetEnumProperty<T>(propertyDefinition, valueOrDefault));
		}

		internal static T GetEnumProperty<T>(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition) where T : struct
		{
			return MigrationHelper.GetEnumProperty<T>(propertyDefinition, item[propertyDefinition]);
		}

		internal static ExTimeZone GetExTimeZoneProperty(IPropertyBag item, StorePropertyDefinition propertyDefinition)
		{
			Exception ex = null;
			object objectValue = item[propertyDefinition];
			ExTimeZone exTimeZoneValue = MigrationHelper.GetExTimeZoneValue(objectValue, ref ex);
			if (ex != null)
			{
				throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, objectValue), ex);
			}
			return exTimeZoneValue;
		}

		internal static ExTimeZone GetExTimeZoneValue(object objectValue, ref Exception ex)
		{
			try
			{
				if (ExTimeZone.UtcTimeZone.Id.Equals((string)objectValue, StringComparison.Ordinal))
				{
					return ExTimeZone.UtcTimeZone;
				}
				return ExTimeZoneValue.Parse((string)objectValue).ExTimeZone;
			}
			catch (FormatException ex2)
			{
				ex = ex2;
			}
			return null;
		}

		internal static CultureInfo GetCultureInfoPropertyOrDefault(IMigrationStoreObject item, PropertyDefinition propertyDefinition)
		{
			string valueOrDefault = item.GetValueOrDefault<string>(propertyDefinition, "en-US");
			CultureInfo result;
			try
			{
				result = new CultureInfo(valueOrDefault);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, valueOrDefault), innerException);
			}
			return result;
		}

		internal static ExDateTime GetExDateTimePropertyOrDefault(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, ExDateTime defaultTime)
		{
			ExDateTime? exDateTimePropertyOrNull = MigrationHelper.GetExDateTimePropertyOrNull(item, propertyDefinition);
			if (exDateTimePropertyOrNull != null)
			{
				return exDateTimePropertyOrNull.Value;
			}
			return defaultTime;
		}

		internal static ExDateTime? GetExDateTimePropertyOrNull(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			object property = MigrationHelper.GetProperty<object>(item, propertyDefinition, false);
			return MigrationHelperBase.GetValidExDateTime(property as ExDateTime?);
		}

		internal static Fqdn GetFqdnProperty(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Exception innerException = null;
			try
			{
				string property = MigrationHelper.GetProperty<string>(item, propertyDefinition, required);
				if (property == null)
				{
					return null;
				}
				return new Fqdn(property);
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static SmtpAddress GetSmtpAddressProperty(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			string property = MigrationHelper.GetProperty<string>(item, propertyDefinition, false);
			if (string.IsNullOrEmpty(property))
			{
				return SmtpAddress.Empty;
			}
			return MigrationHelper.GetSmtpAddress(property, propertyDefinition);
		}

		internal static SmtpAddress GetSmtpAddress(string smtpAddress, StorePropertyDefinition propertyDefinition)
		{
			Exception innerException = null;
			try
			{
				return SmtpAddress.Parse(smtpAddress);
			}
			catch (PropertyErrorException ex)
			{
				innerException = ex;
			}
			catch (FormatException ex2)
			{
				innerException = ex2;
			}
			throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static Guid GetGuidProperty(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Guid valueOrDefault = item.GetValueOrDefault<Guid>(propertyDefinition, Guid.Empty);
			if (required && valueOrDefault == Guid.Empty)
			{
				throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null));
			}
			return valueOrDefault;
		}

		internal static ADObjectId GetADObjectId(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			return MigrationHelper.GetADObjectIdImpl(item, propertyDefinition, false);
		}

		internal static bool TryGetADObjectId(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, out ADObjectId id)
		{
			id = MigrationHelper.GetADObjectIdImpl(item, propertyDefinition, true);
			return id != null;
		}

		internal static StoreObjectId GetObjectIdProperty(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Exception innerException = null;
			try
			{
				byte[] property = MigrationHelper.GetProperty<byte[]>(item, propertyDefinition, required);
				if (property == null)
				{
					return null;
				}
				return MigrationHelperBase.GetStoreObjectId(property);
			}
			catch (CorruptDataException ex)
			{
				innerException = ex;
			}
			throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static Unlimited<int>? ReadUnlimitedProperty(IMigrationStoreObject bag, PropertyDefinition property)
		{
			Unlimited<int>? result;
			try
			{
				string text = bag[property] as string;
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					result = new Unlimited<int>?(Unlimited<int>.Parse(text));
				}
			}
			catch (PropertyErrorException ex)
			{
				if (!ex.PropertyErrors.Any((PropertyError error) => error.PropertyErrorCode == PropertyErrorCode.NotFound))
				{
					throw;
				}
				result = null;
			}
			catch (KeyNotFoundException)
			{
				result = null;
			}
			return result;
		}

		internal static void WriteUnlimitedProperty(IMigrationStoreObject bag, PropertyDefinition property, Unlimited<int>? value)
		{
			if (value != null)
			{
				bag[property] = value.Value.ToString();
				return;
			}
			bag.Delete(property);
		}

		internal static void WriteOrDeleteNullableProperty<T>(IMigrationStoreObject bag, PropertyDefinition property, T value)
		{
			if (value != null)
			{
				bag[property] = value;
				return;
			}
			bag.Delete(property);
		}

		internal static T GetProperty<T>(IMigrationStoreObject item, PropertyDefinition propertyDefinition, bool required) where T : class
		{
			Exception innerException = null;
			try
			{
				if (required)
				{
					return (T)((object)item[propertyDefinition]);
				}
				return item.GetValueOrDefault<T>(propertyDefinition, default(T));
			}
			catch (PropertyErrorException ex)
			{
				innerException = ex;
			}
			catch (InvalidCastException ex2)
			{
				innerException = ex2;
			}
			catch (CorruptDataException ex3)
			{
				innerException = ex3;
			}
			catch (InvalidDataException ex4)
			{
				innerException = ex4;
			}
			throw new MigrationDataCorruptionException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static PersistableDictionary GetDictionaryProperty(IMigrationStoreObject item, PropertyDefinition propertyDefinition, bool required)
		{
			MigrationUtil.ThrowOnNullArgument(item, "item");
			MigrationUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			string property = MigrationHelper.GetProperty<string>(item, propertyDefinition, required);
			if (property == null)
			{
				return null;
			}
			PersistableDictionary result;
			try
			{
				result = PersistableDictionary.Create(property);
			}
			catch (XmlException innerException)
			{
				throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, property), innerException);
			}
			return result;
		}

		internal static void SetDictionaryProperty(IPropertyBag item, PropertyDefinition propertyDefinition, PersistableDictionary persistDictionary)
		{
			MigrationUtil.ThrowOnNullArgument(item, "item");
			MigrationUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			MigrationUtil.ThrowOnNullArgument(persistDictionary, "persistDictionary");
			item[propertyDefinition] = persistDictionary.Serialize();
		}

		internal static string GetPropertyErrorMessage(PropertyDefinition propertyDefinition, object objectValue)
		{
			return string.Format(CultureInfo.InvariantCulture, "Property error: {0}={1}", new object[]
			{
				(propertyDefinition == null) ? "Null" : propertyDefinition.ToString(),
				(objectValue == null) ? "Null" : objectValue.ToString()
			});
		}

		internal static MultiValuedProperty<T> ToMultiValuedProperty<T>(IEnumerable<T> collection)
		{
			if (collection is MultiValuedProperty<T>)
			{
				return (MultiValuedProperty<T>)collection;
			}
			MultiValuedProperty<T> multiValuedProperty = new MultiValuedProperty<T>();
			foreach (T item in collection)
			{
				multiValuedProperty.TryAdd(item);
			}
			return multiValuedProperty;
		}

		internal static IEnumerable<StoreObjectId> FindMessageIds(IMigrationDataProvider dataProvider, MigrationEqualityFilter primaryFilter, MigrationEqualityFilter[] secondaryFilters, SortBy[] additionalSortCriteria, int? maxCount)
		{
			if (secondaryFilters == null)
			{
				secondaryFilters = new MigrationEqualityFilter[0];
			}
			if (additionalSortCriteria == null)
			{
				additionalSortCriteria = new SortBy[0];
			}
			PropertyDefinition[] array = new PropertyDefinition[secondaryFilters.Length + additionalSortCriteria.Length];
			SortBy[] array2 = new SortBy[secondaryFilters.Length + additionalSortCriteria.Length];
			for (int i = 0; i < secondaryFilters.Length; i++)
			{
				array[i] = secondaryFilters[i].Property;
				array2[i] = new SortBy(secondaryFilters[i].Property, SortOrder.Ascending);
			}
			for (int j = 0; j < additionalSortCriteria.Length; j++)
			{
				int num = secondaryFilters.Length + j;
				array[num] = additionalSortCriteria[j].ColumnDefinition;
				array2[num] = additionalSortCriteria[j];
			}
			bool matchRegionStarted = false;
			return dataProvider.FindMessageIds(primaryFilter, array, array2, delegate(IDictionary<PropertyDefinition, object> rowData)
			{
				if (!MigrationHelper.FitsFilter(primaryFilter, rowData[primaryFilter.Property]))
				{
					return MigrationRowSelectorResult.RejectRowStopProcessing;
				}
				foreach (MigrationEqualityFilter migrationEqualityFilter in secondaryFilters)
				{
					if (!MigrationHelper.FitsFilter(migrationEqualityFilter, rowData[migrationEqualityFilter.Property]))
					{
						MigrationRowSelectorResult result;
						if (!matchRegionStarted)
						{
							result = MigrationRowSelectorResult.RejectRowContinueProcessing;
						}
						else
						{
							result = MigrationRowSelectorResult.RejectRowStopProcessing;
						}
						return result;
					}
				}
				matchRegionStarted = true;
				return MigrationRowSelectorResult.AcceptRow;
			}, maxCount);
		}

		internal static bool FitsFilter(MigrationEqualityFilter filter, object xsoValue)
		{
			object value = filter.Value;
			ComparisonFilter comparisonFilter = filter.Filter as ComparisonFilter;
			if (comparisonFilter == null || comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
			{
				return MigrationHelper.IsEqualXsoValues(value, xsoValue);
			}
			ComparisonOperator comparisonOperator = comparisonFilter.ComparisonOperator;
			if (comparisonOperator == ComparisonOperator.NotEqual)
			{
				return !MigrationHelper.IsEqualXsoValues(value, xsoValue);
			}
			if (value == null && xsoValue == null)
			{
				return comparisonOperator == ComparisonOperator.Equal || comparisonOperator == ComparisonOperator.LessThanOrEqual || comparisonOperator == ComparisonOperator.GreaterThanOrEqual;
			}
			if (value == null && xsoValue != null)
			{
				return comparisonOperator == ComparisonOperator.GreaterThan || comparisonOperator == ComparisonOperator.GreaterThanOrEqual;
			}
			if (value != null && xsoValue == null)
			{
				return comparisonOperator == ComparisonOperator.LessThan || comparisonOperator == ComparisonOperator.LessThanOrEqual;
			}
			IComparable obj;
			IComparable comparable;
			if (value is ExDateTime?)
			{
				obj = (value as ExDateTime?).Value;
				comparable = (xsoValue as ExDateTime?).Value;
			}
			else
			{
				obj = (value as IComparable);
				comparable = (xsoValue as IComparable);
			}
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.LessThan:
				return comparable.CompareTo(obj) < 0;
			case ComparisonOperator.LessThanOrEqual:
				return comparable.CompareTo(obj) <= 0;
			case ComparisonOperator.GreaterThan:
				return comparable.CompareTo(obj) > 0;
			case ComparisonOperator.GreaterThanOrEqual:
				return comparable.CompareTo(obj) >= 0;
			default:
				return false;
			}
		}

		internal static bool IsEqualXsoValues(object propertyDefinitionValue, object xsoValue)
		{
			if (object.Equals(propertyDefinitionValue, xsoValue))
			{
				return true;
			}
			if (xsoValue == null)
			{
				return false;
			}
			if (xsoValue is string)
			{
				return string.Equals(propertyDefinitionValue as string, xsoValue as string, StringComparison.OrdinalIgnoreCase);
			}
			if (propertyDefinitionValue is Enum)
			{
				if (Enum.GetUnderlyingType(propertyDefinitionValue.GetType()) != xsoValue.GetType())
				{
					return false;
				}
				propertyDefinitionValue = Convert.ChangeType(propertyDefinitionValue, xsoValue.GetType());
				return object.Equals(propertyDefinitionValue, xsoValue);
			}
			else
			{
				byte[] array = xsoValue as byte[];
				if (array == null)
				{
					return false;
				}
				byte[] array2 = propertyDefinitionValue as byte[];
				if (array2 == null)
				{
					return false;
				}
				if (array2.Length != array.Length)
				{
					return false;
				}
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] != array[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		internal static void VerifyMigrationTypeEquality(MigrationType expected, MigrationType actual)
		{
			if (expected != actual)
			{
				throw new ArgumentException(string.Format("IMAPMigrationJobSettings expects a batch of type {0} but found {1}", expected, actual));
			}
		}

		internal static T[] AggregateArrays<T>(params IList<T>[] itemLists)
		{
			int num = 0;
			foreach (IList<T> list in itemLists)
			{
				if (list != null)
				{
					num += list.Count;
				}
			}
			T[] array = new T[num];
			foreach (IList<T> list2 in itemLists)
			{
				if (list2 != null)
				{
					list2.CopyTo(array, array.Length - num);
					num -= list2.Count;
				}
			}
			return array;
		}

		internal static PropertyDefinition[] AggregateProperties(params IList<PropertyDefinition>[] items)
		{
			return MigrationHelper.AggregateArrays<PropertyDefinition>(items);
		}

		internal static string ToTruncatedString(this string val)
		{
			if (val == null || val.Length <= 15)
			{
				return val;
			}
			return val.Substring(0, 15);
		}

		internal static ExDateTime? GetUniversalDateTime(ExDateTime? dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			return new ExDateTime?(dateTime.Value.ToUtc());
		}

		internal static ExDateTime? GetLocalizedDateTime(ExDateTime? dateTime, ExTimeZone timeZone)
		{
			if (dateTime == null)
			{
				return null;
			}
			if (timeZone == null)
			{
				return dateTime;
			}
			return new ExDateTime?(timeZone.ConvertDateTime(dateTime.Value));
		}

		internal static void RunUpdateOperation(Action updateOperation)
		{
			for (int i = 1; i <= 3; i++)
			{
				try
				{
					updateOperation();
					break;
				}
				catch (TransientException ex)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "Encountered a transient exception: {0}", new object[]
					{
						ex
					});
					if (i == 3)
					{
						throw;
					}
				}
			}
		}

		internal static void SendFriendlyWatson(Exception ex, bool collectMemoryDump, string additionalData = null)
		{
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
			ReportOptions reportOptions = ReportOptions.DeepStackTraceHash;
			if (!collectMemoryDump)
			{
				reportOptions |= ReportOptions.DoNotCollectDumps;
			}
			ExWatson.SendReport(ex, reportOptions, additionalData);
		}

		private static T GetEnumProperty<T>(StorePropertyDefinition propertyDefinition, object objectValue) where T : struct
		{
			Exception innerException = null;
			try
			{
				return (T)((object)objectValue);
			}
			catch (PropertyErrorException ex)
			{
				innerException = ex;
			}
			catch (FormatException ex2)
			{
				innerException = ex2;
			}
			catch (InvalidCastException ex3)
			{
				innerException = ex3;
			}
			throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, objectValue), innerException);
		}

		private static ADObjectId GetADObjectIdImpl(IMigrationStoreObject item, StorePropertyDefinition propertyDefinition, bool useTryGet)
		{
			MigrationUtil.ThrowOnNullArgument(item, "item");
			MigrationUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			Exception innerException = null;
			try
			{
				byte[] array;
				if (useTryGet)
				{
					array = item.GetValueOrDefault<byte[]>(propertyDefinition, null);
					if (array == null)
					{
						return null;
					}
				}
				else
				{
					array = (byte[])item[propertyDefinition];
				}
				return new ADObjectId(array);
			}
			catch (CorruptDataException ex)
			{
				innerException = ex;
			}
			catch (ArgumentNullException ex2)
			{
				innerException = ex2;
			}
			throw new InvalidDataException(MigrationHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal const int MaxNumberOfRowsInOneChunk = 100;

		private const string DefaultLanguage = "en-US";

		private const int MaximumRetryCount = 3;

		internal static readonly PropertyDefinition[] ItemIdProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static class MigrationDiagnosticHistory
		{
			public static string AppendDiagnosticHistory(string history, params string[] entryFields)
			{
				MigrationUtil.ThrowOnNullArgument(history, "history");
				int num = history.Count((char p) => p == ';');
				if (num >= 35)
				{
					int num2 = -1;
					while (num-- >= 35)
					{
						num2 = history.IndexOf(';', num2 + 1);
						if (num2 < 0)
						{
							break;
						}
					}
					if (num2 < 0)
					{
						throw new MigrationDataCorruptionException(string.Format("bad format of history {0}, expected to find delim", history));
					}
					history = history.Substring(num2);
				}
				return history + string.Join(":", entryFields) + ';';
			}

			private const char HistoryEntryDelim = ';';

			private const string HistoryEntryValueDelim = ":";

			private const int MaximumHistoryEntryValue = 35;
		}
	}
}
