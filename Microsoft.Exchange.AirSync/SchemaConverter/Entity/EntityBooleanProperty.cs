using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityBooleanProperty : EntityProperty, IBooleanProperty, IProperty
	{
		public EntityBooleanProperty(EntityPropertyDefinition propertyDefinition) : base(propertyDefinition, PropertyType.ReadWrite, false)
		{
		}

		public EntityBooleanProperty(EntityPropertyDefinition propertyDefinition, PropertyType type) : base(propertyDefinition, type, false)
		{
		}

		public virtual bool BooleanData
		{
			get
			{
				if (base.EntityPropertyDefinition.Getter == null)
				{
					throw new ConversionException("Unable to retrieve data of type " + base.EntityPropertyDefinition.Type.FullName);
				}
				return (bool)base.EntityPropertyDefinition.Getter(base.Item);
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			IBooleanProperty booleanProperty = srcProperty as IBooleanProperty;
			if (booleanProperty != null)
			{
				base.EntityPropertyDefinition.Setter(base.Item, booleanProperty.BooleanData);
			}
		}
	}
}
