using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class PropertyProvider
	{
		public abstract void GetPropertyFromDataSource(Entity entity, PropertyDefinition property, object dataSource);

		public abstract void SetPropertyToDataSource(Entity entity, PropertyDefinition property, object dataSource);
	}
}
