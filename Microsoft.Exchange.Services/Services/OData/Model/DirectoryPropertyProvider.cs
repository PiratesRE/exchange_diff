using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DirectoryPropertyProvider : PropertyProvider
	{
		public DirectoryPropertyProvider(ADPropertyDefinition adPropertyDefinition)
		{
			ArgumentValidator.ThrowIfNull("adPropertyDefinition", adPropertyDefinition);
			this.ADPropertyDefinition = adPropertyDefinition;
		}

		public override void GetPropertyFromDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.GetProperty(entity, property, (ADRawEntry)dataSource);
		}

		public override void SetPropertyToDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.SetProperty(entity, property, (ADRawEntry)dataSource);
		}

		public ADPropertyDefinition ADPropertyDefinition { get; private set; }

		public virtual object GetQueryConstant(object value)
		{
			return value;
		}

		protected virtual void GetProperty(Entity entity, PropertyDefinition property, ADRawEntry adObject)
		{
			if (this.ADPropertyDefinition != null)
			{
				entity[property] = adObject[this.ADPropertyDefinition];
			}
		}

		protected virtual void SetProperty(Entity entity, PropertyDefinition property, ADRawEntry adObject)
		{
			if (this.ADPropertyDefinition != null)
			{
				adObject[this.ADPropertyDefinition] = entity[property];
			}
		}
	}
}
