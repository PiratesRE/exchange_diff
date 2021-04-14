using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class SimpleDirectoryPropertyProvider : DirectoryPropertyProvider
	{
		public SimpleDirectoryPropertyProvider(ADPropertyDefinition adPropertyDefinition) : base(adPropertyDefinition)
		{
		}

		public Action<Entity, PropertyDefinition, ADRawEntry, ADPropertyDefinition> Getter { get; set; }

		public Action<Entity, PropertyDefinition, ADRawEntry, ADPropertyDefinition> Setter { get; set; }

		public Func<object, object> QueryConstantBuilder { get; set; }

		public override object GetQueryConstant(object value)
		{
			if (this.QueryConstantBuilder != null)
			{
				return this.QueryConstantBuilder(value);
			}
			return base.GetQueryConstant(value);
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ADRawEntry adObject)
		{
			if (this.Getter != null)
			{
				this.Getter(entity, property, adObject, base.ADPropertyDefinition);
				return;
			}
			base.GetProperty(entity, property, adObject);
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ADRawEntry adObject)
		{
			if (this.Setter != null)
			{
				this.Setter(entity, property, adObject, base.ADPropertyDefinition);
				return;
			}
			base.GetProperty(entity, property, adObject);
		}
	}
}
