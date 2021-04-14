using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion.Translators
{
	internal interface IStorageTranslator<in TStorageObject, TEntity>
	{
		void SetPropertiesFromStorageObjectOnEntity(TStorageObject sourceStoreObject, TEntity destinationEntity);

		void SetPropertiesFromEntityOnStorageObject(TEntity sourceEntity, TStorageObject destinationStoreObject);

		void SetPropertiesFromStoragePropertyValuesOnEntity(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, TEntity destinationEntity);

		TEntity ConvertToEntity(TStorageObject sourceStoreObject);

		TEntity ConvertToEntity(TStorageObject sourceStoreObject1, TStorageObject sourceStoreObject2);

		TEntity ConvertToEntity(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session);

		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Map(string entityPropertyName);

		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Map(IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> properties);
	}
}
