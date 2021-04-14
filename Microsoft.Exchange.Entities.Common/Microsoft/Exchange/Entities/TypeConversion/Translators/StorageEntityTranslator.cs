using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules;

namespace Microsoft.Exchange.Entities.TypeConversion.Translators
{
	internal class StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema> : StorageTranslator<TStoreObject, TEntity> where TStoreObject : IStoreObject where TEntity : StorageEntity<TEntitySchema>, new() where TEntitySchema : StorageEntitySchema, new()
	{
		protected StorageEntityTranslator(IEnumerable<ITranslationRule<TStoreObject, TEntity>> additionalRules = null) : base(StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema>.CreateTranslationRules().AddRules(additionalRules))
		{
		}

		public static StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema> Instance
		{
			get
			{
				return StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema>.SingletonInstance;
			}
		}

		public override void SetPropertiesFromStoragePropertyValuesOnEntity(IDictionary<PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, TEntity destinationEntity)
		{
			StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema>.IdTranslationRule.FromPropertyValues(propertyIndices, values, session, destinationEntity);
			base.SetPropertiesFromStoragePropertyValuesOnEntity(propertyIndices, values, session, destinationEntity);
		}

		protected override TEntity CreateEntity()
		{
			return Activator.CreateInstance<TEntity>();
		}

		private static List<ITranslationRule<TStoreObject, TEntity>> CreateTranslationRules()
		{
			return new List<ITranslationRule<TStoreObject, TEntity>>
			{
				StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema>.IdTranslationRule
			};
		}

		private static readonly StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema> SingletonInstance = new StorageEntityTranslator<TStoreObject, TEntity, TEntitySchema>(null);

		private static readonly StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema> IdTranslationRule = new StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>(IdConverter.Instance);
	}
}
