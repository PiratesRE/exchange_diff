using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public abstract class EntitySchema : TypeSchema
	{
		protected EntitySchema()
		{
			base.RegisterPropertyDefinition(EntitySchema.StaticIdProperty);
		}

		public TypedPropertyDefinition<string> IdProperty
		{
			get
			{
				return EntitySchema.StaticIdProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticIdProperty = new TypedPropertyDefinition<string>("Entity.Id", null, true);
	}
}
