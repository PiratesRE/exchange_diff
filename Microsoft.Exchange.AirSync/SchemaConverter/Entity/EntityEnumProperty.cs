using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityEnumProperty : EntityProperty, IIntegerProperty, IProperty
	{
		public EntityEnumProperty(EntityPropertyDefinition propertyDefinition, bool syncForOccurenceItem = false) : base(propertyDefinition, PropertyType.ReadWrite, syncForOccurenceItem)
		{
		}

		public EntityEnumProperty(EntityPropertyDefinition propertyDefinition, PropertyType type, bool syncForOccurenceItem = false) : base(propertyDefinition, type, syncForOccurenceItem)
		{
		}

		public virtual int IntegerData
		{
			get
			{
				if (base.EntityPropertyDefinition.Getter == null)
				{
					throw new ConversionException("Unable to retrieve data of type " + base.EntityPropertyDefinition.Type.FullName);
				}
				return (int)base.EntityPropertyDefinition.Getter(base.Item);
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			IIntegerProperty integerProperty = srcProperty as IIntegerProperty;
			if (integerProperty != null)
			{
				int integerData = integerProperty.IntegerData;
				if (!Enum.IsDefined(base.EntityPropertyDefinition.Type, integerData))
				{
					throw new ConversionException(string.Format("EntityEnumProperty.CopyFrom Type {0} does not have value {1} defined.", base.EntityPropertyDefinition.Type.FullName, integerData));
				}
				base.EntityPropertyDefinition.Setter(base.Item, integerData);
			}
		}
	}
}
