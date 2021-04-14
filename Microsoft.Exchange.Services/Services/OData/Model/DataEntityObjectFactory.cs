using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class DataEntityObjectFactory
	{
		public static TEntity CreateEntity<TEntity>(object dataEntity) where TEntity : Entity
		{
			ArgumentValidator.ThrowIfNull("dataEntity", dataEntity);
			DataEntityObjectFactory.DataEntityTypeMapEntry dataEntityTypeMapEntry = DataEntityObjectFactory.map.FirstOrDefault((DataEntityObjectFactory.DataEntityTypeMapEntry x) => x.DataEntityType.Equals(dataEntity.GetType()));
			if (dataEntityTypeMapEntry == null)
			{
				throw new NotSupportedException(string.Format("Service object type {0} not suppported", dataEntity.GetType()));
			}
			return (TEntity)((object)dataEntityTypeMapEntry.EntityCreator());
		}

		public static TDataEntity CreateDataEntity<TDataEntity>(Entity entityObject) where TDataEntity : class
		{
			ArgumentValidator.ThrowIfNull("entityObject", entityObject);
			DataEntityObjectFactory.DataEntityTypeMapEntry dataEntityTypeMapEntry = DataEntityObjectFactory.map.FirstOrDefault((DataEntityObjectFactory.DataEntityTypeMapEntry x) => x.EntityType.Equals(entityObject.GetType()));
			if (dataEntityTypeMapEntry == null)
			{
				throw new NotSupportedException(string.Format("Entity type {0} not suppported", entityObject.GetType()));
			}
			return (TDataEntity)((object)dataEntityTypeMapEntry.DataEntityCreator());
		}

		public static TDataEntity CreateAndSetPropertiesOnDataEntityForCreate<TDataEntity>(Entity entityObject) where TDataEntity : class
		{
			TDataEntity tdataEntity = DataEntityObjectFactory.CreateDataEntity<TDataEntity>(entityObject);
			foreach (PropertyDefinition propertyDefinition in entityObject.PropertyBag.GetProperties())
			{
				if (propertyDefinition.Flags.HasFlag(PropertyDefinitionFlags.CanCreate))
				{
					propertyDefinition.DataEntityPropertyProvider.SetPropertyToDataSource(entityObject, propertyDefinition, tdataEntity);
				}
			}
			return tdataEntity;
		}

		public static TDataEntity CreateAndSetPropertiesOnDataEntityForUpdate<TDataEntity>(Entity entityObject) where TDataEntity : class
		{
			TDataEntity tdataEntity = DataEntityObjectFactory.CreateDataEntity<TDataEntity>(entityObject);
			foreach (PropertyDefinition propertyDefinition in entityObject.PropertyBag.GetProperties())
			{
				if (propertyDefinition.Flags.HasFlag(PropertyDefinitionFlags.CanUpdate))
				{
					propertyDefinition.DataEntityPropertyProvider.SetPropertyToDataSource(entityObject, propertyDefinition, tdataEntity);
				}
			}
			return tdataEntity;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DataEntityObjectFactory()
		{
			DataEntityObjectFactory.DataEntityTypeMapEntry[] array = new DataEntityObjectFactory.DataEntityTypeMapEntry[3];
			array[0] = new DataEntityObjectFactory.DataEntityTypeMapEntry(typeof(Calendar), typeof(Calendar), () => new Calendar(), () => new Calendar());
			array[1] = new DataEntityObjectFactory.DataEntityTypeMapEntry(typeof(CalendarGroup), typeof(CalendarGroup), () => new CalendarGroup(), () => new CalendarGroup());
			array[2] = new DataEntityObjectFactory.DataEntityTypeMapEntry(typeof(Event), typeof(Event), () => new Event(), () => new Event());
			DataEntityObjectFactory.map = array;
		}

		private static readonly DataEntityObjectFactory.DataEntityTypeMapEntry[] map;

		private class DataEntityTypeMapEntry
		{
			public DataEntityTypeMapEntry(Type dataEntityType, Type entityType, Func<object> dataEntityCreator, Func<Entity> entityCreator)
			{
				ArgumentValidator.ThrowIfNull("dataEntityType", dataEntityType);
				ArgumentValidator.ThrowIfNull("entityType", entityType);
				ArgumentValidator.ThrowIfNull("dataEntityCreator", dataEntityCreator);
				ArgumentValidator.ThrowIfNull("entityCreator", entityCreator);
				this.DataEntityType = dataEntityType;
				this.EntityType = entityType;
				this.DataEntityCreator = dataEntityCreator;
				this.EntityCreator = entityCreator;
			}

			public Type DataEntityType { get; private set; }

			public Type EntityType { get; private set; }

			public Func<object> DataEntityCreator { get; private set; }

			public Func<Entity> EntityCreator { get; private set; }
		}
	}
}
