using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	internal abstract class EntityNestedProperty : EntityProperty, INestedProperty
	{
		public EntityNestedProperty(EntityPropertyDefinition propertyDefinition, PropertyType type = PropertyType.ReadWrite) : base(propertyDefinition, type, false)
		{
		}

		public virtual INestedData NestedData { get; set; }

		public override void Bind(IItem item)
		{
			this.Unbind();
			base.Bind(item);
		}

		public override void Unbind()
		{
			base.Unbind();
			this.NestedData.Clear();
		}
	}
}
