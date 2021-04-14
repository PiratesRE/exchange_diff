using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public abstract class Entity<TSchema> : SchematizedObject<TSchema>, IEntity, IPropertyChangeTracker<PropertyDefinition> where TSchema : EntitySchema, new()
	{
		protected Entity()
		{
		}

		protected Entity(string id)
		{
			this.Id = id;
		}

		public string Id
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.IdProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.IdProperty, value);
			}
		}

		public static class Accessors
		{
			// Note: this type is marked as 'beforefieldinit'.
			static Accessors()
			{
				TSchema schemaInstance = SchematizedObject<TSchema>.SchemaInstance;
				Entity<TSchema>.Accessors.Id = new EntityPropertyAccessor<IEntity, string>(schemaInstance.IdProperty, (IEntity entity) => entity.Id, delegate(IEntity entity, string s)
				{
					entity.Id = s;
				});
			}

			public static readonly EntityPropertyAccessor<IEntity, string> Id;
		}
	}
}
