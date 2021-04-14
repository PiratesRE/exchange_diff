using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GenericPropertyProvider<T> : PropertyProvider
	{
		public override void GetPropertyFromDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.GetProperty(entity, property, (T)((object)dataSource));
		}

		public override void SetPropertyToDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.SetProperty(entity, property, (T)((object)dataSource));
		}

		public Action<Entity, PropertyDefinition, T> Getter { get; set; }

		public Action<Entity, PropertyDefinition, T> Setter { get; set; }

		protected virtual void GetProperty(Entity entity, PropertyDefinition property, T dataObject)
		{
			if (this.Getter != null)
			{
				this.Getter(entity, property, dataObject);
			}
		}

		protected virtual void SetProperty(Entity entity, PropertyDefinition property, T dataObject)
		{
			if (this.Setter != null)
			{
				this.Setter(entity, property, dataObject);
			}
		}
	}
}
