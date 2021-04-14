using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;
using Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	public static class TranslationRuleExtensions
	{
		public static ITranslationRule<TLeft, TRight> OfType<TLeft, TRight, TNewLeft, TNewRight>(this ITranslationRule<TNewLeft, TNewRight> internalRule) where TNewLeft : class, TLeft where TNewRight : class, TRight
		{
			return new OfTypeTranslationRule<TLeft, TRight, TNewLeft, TNewRight>(internalRule);
		}

		public static IEnumerable<ITranslationRule<TLeft, TRight>> EnumerateRules<TLeft, TRight>(this IEnumerable<ITranslationRule<TLeft, TRight>> enumerable)
		{
			foreach (ITranslationRule<TLeft, TRight> translationRule in enumerable)
			{
				IEnumerable<ITranslationRule<TLeft, TRight>> subset = translationRule as IEnumerable<ITranslationRule<!0, !1>>;
				if (subset != null)
				{
					foreach (ITranslationRule<TLeft, TRight> rule in subset.EnumerateRules<TLeft, TRight>())
					{
						yield return rule;
					}
				}
				else
				{
					yield return translationRule;
				}
			}
			yield break;
		}

		public static List<ITranslationRule<TLeft, TRight>> AddRules<TLeft, TRight>(this List<ITranslationRule<TLeft, TRight>> firstRuleSet, IEnumerable<ITranslationRule<TLeft, TRight>> secondRuleSet)
		{
			if (secondRuleSet != null)
			{
				firstRuleSet.AddRange(secondRuleSet);
			}
			return firstRuleSet;
		}

		internal static ITranslationRule<TStoreObject, TEntity> MapTo<TStoreObject, TEntity, TStorageValue, TEntityValue>(this IStoragePropertyAccessor<TStoreObject, TStorageValue> storageAccessor, EntityPropertyAccessor<TEntity, TEntityValue> entityAccessor, IConverter<TStorageValue, TEntityValue> storageToEntityConverter, IConverter<TEntityValue, TStorageValue> entityToStorageConverter) where TStoreObject : IStoreObject where TEntity : IPropertyChangeTracker<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>
		{
			return new PropertyTranslationRule<TStoreObject, TEntity, Microsoft.Exchange.Data.PropertyDefinition, TStorageValue, TEntityValue>(storageAccessor, entityAccessor, storageToEntityConverter, entityToStorageConverter);
		}

		internal static ITranslationRule<TStoreObject, TEntity> MapTo<TStoreObject, TEntity, TStorageValue, TEntityValue, TConverter>(this IStoragePropertyAccessor<TStoreObject, TStorageValue> storageAccessor, EntityPropertyAccessor<TEntity, TEntityValue> entityAccessor, TConverter converter) where TStoreObject : IStoreObject where TEntity : IPropertyChangeTracker<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>
		{
			IConverter<TStorageValue, TEntityValue> storageToEntityConverter = converter as IConverter<TStorageValue, TEntityValue>;
			IConverter<TEntityValue, TStorageValue> entityToStorageConverter = converter as IConverter<TEntityValue, TStorageValue>;
			return storageAccessor.MapTo(entityAccessor, storageToEntityConverter, entityToStorageConverter);
		}

		internal static ITranslationRule<TStoreObject, TEntity> MapTo<TStoreObject, TEntity, TValue>(this IStoragePropertyAccessor<TStoreObject, TValue> storageAccessor, EntityPropertyAccessor<TEntity, TValue> entityAccessor) where TEntity : IPropertyChangeTracker<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>
		{
			return new PassThruPropertyTranslationRule<TStoreObject, TEntity, Microsoft.Exchange.Data.PropertyDefinition, TValue>(storageAccessor, entityAccessor);
		}

		internal static ITranslationRule<TStoreObject, TEntity> MapTo<TStoreObject, TEntity>(this IStoragePropertyAccessor<TStoreObject, ExDateTime> storageTimeAccessor, IStoragePropertyAccessor<TStoreObject, ExTimeZone> storageTimeZoneAccessor, EntityPropertyAccessor<TEntity, ExDateTime> entityTimeAccessor, EntityPropertyAccessor<TEntity, string> entityIntendedTimeZoneIdAccessor, DateTimeHelper dateTimeHelper) where TStoreObject : IStoreObject where TEntity : IPropertyChangeTracker<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>
		{
			return new StorageTimeZoneSensitiveTimeTranslationRule<TStoreObject, TEntity>(storageTimeAccessor, storageTimeZoneAccessor, entityTimeAccessor, entityIntendedTimeZoneIdAccessor, dateTimeHelper);
		}
	}
}
