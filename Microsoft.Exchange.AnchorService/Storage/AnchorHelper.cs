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

namespace Microsoft.Exchange.AnchorService.Storage
{
	internal static class AnchorHelper
	{
		public static string AppendDiagnosticHistory(string history, params string[] entryFields)
		{
			return AnchorHelper.MigrationDiagnosticHistory.AppendDiagnosticHistory(history, entryFields);
		}

		internal static T? GetEnumPropertyOrNull<T>(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition) where T : struct
		{
			object valueOrDefault = item.GetValueOrDefault<object>(propertyDefinition, null);
			if (valueOrDefault == null)
			{
				return null;
			}
			return new T?(AnchorHelper.GetEnumProperty<T>(propertyDefinition, valueOrDefault));
		}

		internal static T GetEnumProperty<T>(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition) where T : struct
		{
			return AnchorHelper.GetEnumProperty<T>(propertyDefinition, item[propertyDefinition]);
		}

		internal static ExTimeZone GetExTimeZoneProperty(IPropertyBag item, StorePropertyDefinition propertyDefinition)
		{
			Exception ex = null;
			object objectValue = item[propertyDefinition];
			ExTimeZone exTimeZoneValue = AnchorHelper.GetExTimeZoneValue(objectValue, ref ex);
			if (ex != null)
			{
				throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, objectValue), ex);
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

		internal static CultureInfo GetCultureInfoPropertyOrDefault(IAnchorStoreObject item, PropertyDefinition propertyDefinition)
		{
			string valueOrDefault = item.GetValueOrDefault<string>(propertyDefinition, "en-US");
			CultureInfo result;
			try
			{
				result = new CultureInfo(valueOrDefault);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, valueOrDefault), innerException);
			}
			return result;
		}

		internal static ExDateTime GetExDateTimePropertyOrDefault(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, ExDateTime defaultTime)
		{
			ExDateTime? exDateTimePropertyOrNull = AnchorHelper.GetExDateTimePropertyOrNull(item, propertyDefinition);
			if (exDateTimePropertyOrNull != null)
			{
				return exDateTimePropertyOrNull.Value;
			}
			return defaultTime;
		}

		internal static ExDateTime? GetExDateTimePropertyOrNull(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			object property = AnchorHelper.GetProperty<object>(item, propertyDefinition, false);
			return MigrationHelperBase.GetValidExDateTime(property as ExDateTime?);
		}

		internal static Fqdn GetFqdnProperty(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Exception innerException = null;
			try
			{
				string property = AnchorHelper.GetProperty<string>(item, propertyDefinition, required);
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
			throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static SmtpAddress GetSmtpAddressProperty(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			string property = AnchorHelper.GetProperty<string>(item, propertyDefinition, false);
			if (string.IsNullOrEmpty(property))
			{
				return SmtpAddress.Empty;
			}
			return AnchorHelper.GetSmtpAddress(property, propertyDefinition);
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
			throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static Guid GetGuidProperty(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Guid valueOrDefault = item.GetValueOrDefault<Guid>(propertyDefinition, Guid.Empty);
			if (required && valueOrDefault == Guid.Empty)
			{
				throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null));
			}
			return valueOrDefault;
		}

		internal static ADObjectId GetADObjectId(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition)
		{
			return AnchorHelper.GetADObjectIdImpl(item, propertyDefinition, false);
		}

		internal static bool TryGetADObjectId(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, out ADObjectId id)
		{
			id = AnchorHelper.GetADObjectIdImpl(item, propertyDefinition, true);
			return id != null;
		}

		internal static StoreObjectId GetObjectIdProperty(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, bool required)
		{
			Exception innerException = null;
			try
			{
				byte[] property = AnchorHelper.GetProperty<byte[]>(item, propertyDefinition, required);
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
			throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static Unlimited<int>? ReadUnlimitedProperty(IPropertyBag bag, PropertyDefinition property)
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

		internal static T GetProperty<T>(IAnchorStoreObject item, PropertyDefinition propertyDefinition, bool required) where T : class
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
			throw new MigrationDataCorruptionException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
		}

		internal static PersistableDictionary GetDictionaryProperty(IAnchorStoreObject item, PropertyDefinition propertyDefinition, bool required)
		{
			AnchorUtil.ThrowOnNullArgument(item, "item");
			AnchorUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			string property = AnchorHelper.GetProperty<string>(item, propertyDefinition, required);
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
				throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, property), innerException);
			}
			return result;
		}

		internal static void SetDictionaryProperty(IPropertyBag item, PropertyDefinition propertyDefinition, PersistableDictionary persistDictionary)
		{
			AnchorUtil.ThrowOnNullArgument(item, "item");
			AnchorUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
			AnchorUtil.ThrowOnNullArgument(persistDictionary, "persistDictionary");
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
			return AnchorHelper.AggregateArrays<PropertyDefinition>(items);
		}

		internal static ProviderPropertyDefinition GetDefaultPropertyDefinition(string propertyName, PropertyDefinitionConstraint[] constraints)
		{
			if (constraints == null)
			{
				constraints = PropertyDefinitionConstraint.None;
			}
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, constraints, constraints);
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

		internal static void RunUpdateOperation(AnchorContext context, Action updateOperation)
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
					context.Logger.Log(MigrationEventType.Warning, "Encountered a transient exception", new object[]
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
			throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, objectValue), innerException);
		}

		private static ADObjectId GetADObjectIdImpl(IAnchorStoreObject item, StorePropertyDefinition propertyDefinition, bool useTryGet)
		{
			AnchorUtil.ThrowOnNullArgument(item, "item");
			AnchorUtil.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
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
			throw new InvalidDataException(AnchorHelper.GetPropertyErrorMessage(propertyDefinition, null), innerException);
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
				AnchorUtil.ThrowOnNullArgument(history, "history");
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
