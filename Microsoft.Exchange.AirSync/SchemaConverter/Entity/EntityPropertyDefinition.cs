using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	public class EntityPropertyDefinition : Microsoft.Exchange.Data.PropertyDefinition
	{
		public EntityPropertyDefinition(Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition edmProperty) : base(edmProperty.Name, edmProperty.ValueType)
		{
			this.EdmDefinition = edmProperty;
		}

		public EntityPropertyDefinition(Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition edmProperty, Func<IItem, object> getter) : this(edmProperty)
		{
			this.Getter = getter;
		}

		public EntityPropertyDefinition(Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition edmProperty, Func<IItem, object> getter, Func<IItem, object, object> setter) : this(edmProperty, getter)
		{
			this.Setter = setter;
		}

		internal Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition EdmDefinition { get; private set; }

		internal Func<IItem, object> Getter { get; private set; }

		internal Func<IItem, object, object> Setter { get; private set; }

		public override string ToString()
		{
			return string.Format("Property: {0} ({1})", base.Name, base.Type.FullName);
		}
	}
}
