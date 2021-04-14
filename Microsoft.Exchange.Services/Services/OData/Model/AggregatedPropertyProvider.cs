using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class AggregatedPropertyProvider : PropertyProvider
	{
		public override void GetPropertyFromDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			PropertyProvider propertyProvider = this.SelectProvider(entity.Schema);
			propertyProvider.GetPropertyFromDataSource(entity, property, dataSource);
		}

		public override void SetPropertyToDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			PropertyProvider propertyProvider = this.SelectProvider(entity.Schema);
			propertyProvider.SetPropertyToDataSource(entity, property, dataSource);
		}

		public abstract PropertyProvider SelectProvider(EntitySchema schema);
	}
}
