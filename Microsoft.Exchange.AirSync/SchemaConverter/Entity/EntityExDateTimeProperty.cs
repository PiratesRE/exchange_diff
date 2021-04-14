using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityExDateTimeProperty : EntityProperty, IDateTimeProperty, IProperty
	{
		public EntityExDateTimeProperty(EntityPropertyDefinition propertyDefinition, bool syncForOccurenceItem = false) : base(propertyDefinition, PropertyType.ReadWrite, syncForOccurenceItem)
		{
		}

		public EntityExDateTimeProperty(EntityPropertyDefinition propertyDefinition, PropertyType type, bool syncForOccurenceItem = false) : base(propertyDefinition, type, syncForOccurenceItem)
		{
		}

		public virtual ExDateTime DateTime
		{
			get
			{
				if (base.EntityPropertyDefinition.Getter == null)
				{
					throw new ConversionException("Unable to retrieve data of type " + base.EntityPropertyDefinition.Type.FullName);
				}
				return (ExDateTime)base.EntityPropertyDefinition.Getter(base.Item);
			}
		}

		public override void Bind(IItem item)
		{
			base.Bind(item);
			if (base.State == PropertyState.Modified && this.DateTime == ExDateTime.MinValue)
			{
				base.State = PropertyState.SetToDefault;
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			if (base.EntityPropertyDefinition.Setter == null)
			{
				throw new ConversionException("Unable to set data of type " + base.EntityPropertyDefinition.Type.FullName);
			}
			IDateTimeProperty dateTimeProperty = srcProperty as IDateTimeProperty;
			if (dateTimeProperty != null)
			{
				base.EntityPropertyDefinition.Setter(base.Item, dateTimeProperty.DateTime);
			}
		}
	}
}
