using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Entities.TypeConversion.Translators
{
	internal abstract class StorageTranslator<TStoreObject, TEntity> : IStorageTranslator<TStoreObject, TEntity>
	{
		protected StorageTranslator(IList<ITranslationRule<TStoreObject, TEntity>> additionalRules)
		{
			this.Strategy = new TranslationStrategy<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, TEntity>(additionalRules);
			IEnumerable<IStorageTranslationRule<TStoreObject, TEntity>> enumerable = StorageTranslator<TStoreObject, TEntity>.BuildMappings(this.Strategy);
			this.edmPropertyNameToStorageDependencies = new Dictionary<string, IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>>();
			List<Tuple<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>>> list = new List<Tuple<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>>>();
			foreach (IStorageTranslationRule<TStoreObject, TEntity> storageTranslationRule in enumerable)
			{
				Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] array = storageTranslationRule.EntityProperties.ToArray<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>();
				IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> dependencies = storageTranslationRule.StorageDependencies ?? ((IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>)new Microsoft.Exchange.Data.PropertyDefinition[0]);
				this.edmPropertyNameToStorageDependencies.AddRange(from entityProperty in array
				select new KeyValuePair<string, IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>>(entityProperty.Name, dependencies));
				if (storageTranslationRule.StoragePropertyGroup != null)
				{
					Tuple<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>> item = new Tuple<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>>(storageTranslationRule.StoragePropertyGroup, array);
					list.Add(item);
				}
			}
			this.StoragePropertyGroupsToEdmProperties = SimpleMappingConverter<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>>.CreateRelaxedConverter(list);
		}

		private protected SimpleMappingConverter<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>> StoragePropertyGroupsToEdmProperties { protected get; private set; }

		private protected TranslationStrategy<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, TEntity> Strategy { protected get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Map(string entityPropertyName)
		{
			IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> result;
			if (!this.edmPropertyNameToStorageDependencies.TryGetValue(entityPropertyName, out result))
			{
				return null;
			}
			return result;
		}

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Map(IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> properties)
		{
			HashSet<Microsoft.Exchange.Data.PropertyDefinition> hashSet = new HashSet<Microsoft.Exchange.Data.PropertyDefinition>();
			foreach (Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition propertyDefinition in properties)
			{
				IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> enumerable = this.Map(propertyDefinition.Name);
				if (enumerable == null)
				{
					throw new ArgumentNotSupportedException(string.Format("properties:{0}", propertyDefinition.Name), "Mapped Properties");
				}
				hashSet.AddRange(enumerable);
			}
			return hashSet;
		}

		public void SetPropertiesFromStorageObjectOnEntity(TStoreObject sourceStoreObject, TEntity destinationEntity)
		{
			this.Strategy.FromLeftToRightType(sourceStoreObject, destinationEntity);
		}

		public void SetPropertiesFromEntityOnStorageObject(TEntity sourceEntity, TStoreObject destinationStoreObject)
		{
			this.Strategy.FromRightToLeftType(destinationStoreObject, sourceEntity);
		}

		public virtual void SetPropertiesFromStoragePropertyValuesOnEntity(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, TEntity destinationEntity)
		{
			this.Strategy.FromPropertyValues(propertyIndices, values, destinationEntity);
		}

		public TEntity ConvertToEntity(TStoreObject sourceStoreObject)
		{
			TEntity tentity = this.CreateEntity();
			this.SetPropertiesFromStorageObjectOnEntity(sourceStoreObject, tentity);
			return tentity;
		}

		public TEntity ConvertToEntity(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session)
		{
			TEntity tentity = this.CreateEntity();
			this.SetPropertiesFromStoragePropertyValuesOnEntity(propertyIndices, values, session, tentity);
			return tentity;
		}

		public virtual TEntity ConvertToEntity(TStoreObject sourceStoreObject1, TStoreObject sourceStoreObject2)
		{
			throw new NotImplementedException();
		}

		protected abstract TEntity CreateEntity();

		private static IEnumerable<IStorageTranslationRule<TStoreObject, TEntity>> BuildMappings(IEnumerable<ITranslationRule<TStoreObject, TEntity>> ruleSet)
		{
			return from rule in ruleSet.EnumerateRules<TStoreObject, TEntity>()
			select rule as IStorageTranslationRule<!0, !1> into storageRule
			where storageRule != null && storageRule.EntityProperties != null
			select storageRule;
		}

		private readonly Dictionary<string, IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>> edmPropertyNameToStorageDependencies;
	}
}
