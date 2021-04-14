using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	internal class EntityDataObject : PropertyBase, IServerDataObject, IPropertyContainer, IProperty, IDataObjectGeneratorContainer
	{
		public EntityDataObject(IList<IProperty> propertyFromSchemaLinkId, AirSyncEntitySchemaState airSyncEntitySchemaState)
		{
			base.State = PropertyState.Modified;
			this.propertyFromSchemaLinkId = propertyFromSchemaLinkId;
			this.SchemaState = airSyncEntitySchemaState;
		}

		public IList<IProperty> Children
		{
			get
			{
				return this.propertyFromSchemaLinkId;
			}
		}

		public IDataObjectGenerator SchemaState
		{
			get
			{
				return this.airSyncEntitySchemaState;
			}
			set
			{
				this.airSyncEntitySchemaState = (AirSyncEntitySchemaState)value;
			}
		}

		public void Bind(object item)
		{
			Item item2 = item as Item;
			IItem item3;
			if (item2 != null)
			{
				item3 = EntitySyncItem.GetItem(item2);
			}
			else
			{
				item3 = (item as IItem);
			}
			if (item == null)
			{
				throw new ArgumentNullException("Item is null!");
			}
			foreach (IProperty property in this.propertyFromSchemaLinkId)
			{
				EntityProperty entityProperty = (EntityProperty)property;
				entityProperty.Bind(item3);
			}
		}

		public bool CanConvertItemClassUsingCurrentSchema(string itemClass)
		{
			SinglePropertyBag propertyBag = new SinglePropertyBag(StoreObjectSchema.ItemClass, itemClass);
			return EvaluatableFilter.Evaluate(this.airSyncEntitySchemaState.SupportedClassFilter, propertyBag);
		}

		public PropertyDefinition[] GetPrefetchProperties()
		{
			return null;
		}

		public void SetChangedProperties(PropertyDefinition[] changedProperties)
		{
		}

		public IProperty GetPropBySchemaLinkId(int id)
		{
			return this.propertyFromSchemaLinkId[id];
		}

		public override void Unbind()
		{
			foreach (IProperty property in this.propertyFromSchemaLinkId)
			{
				EntityProperty entityProperty = (EntityProperty)property;
				entityProperty.Unbind();
			}
		}

		public void SetCopyDestination(IPropertyContainer dstPropertyContainer)
		{
		}

		public override void CopyFrom(IProperty srcRootProperty)
		{
			IPropertyContainer propertyContainer = srcRootProperty as IPropertyContainer;
			if (propertyContainer == null)
			{
				throw new ArgumentNullException("srcPropertyContainer");
			}
			propertyContainer.SetCopyDestination(this);
			foreach (IProperty property in propertyContainer.Children)
			{
				if (property.State != PropertyState.NotSupported && property.State != PropertyState.Unmodified && property.State != PropertyState.SetToDefault)
				{
					EntityProperty entityProperty = (EntityProperty)this.propertyFromSchemaLinkId[property.SchemaLinkId];
					if (entityProperty.Type != PropertyType.ReadOnly && entityProperty.State != PropertyState.NotSupported)
					{
						entityProperty.CopyFrom(property);
					}
				}
			}
		}

		private AirSyncEntitySchemaState airSyncEntitySchemaState;

		private IList<IProperty> propertyFromSchemaLinkId;
	}
}
