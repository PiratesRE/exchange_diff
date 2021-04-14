using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class SimpleEwsPropertyProvider : EwsPropertyProvider
	{
		public SimpleEwsPropertyProvider(PropertyInformation propertyInformation) : base(propertyInformation)
		{
		}

		public SimpleEwsPropertyProvider(ReadOnlyCollection<PropertyInformation> propertyInformationList) : base(propertyInformationList)
		{
		}

		public Func<object, bool> DeletingPropertyPedicate { get; set; }

		public Func<ServiceObject, PropertyUpdate> SetPropertyUpdateCreator { get; set; }

		public Func<ServiceObject, PropertyUpdate> DeletePropertyUpdateCreator { get; set; }

		public Action<Entity, PropertyDefinition, ServiceObject, PropertyInformation> Getter { get; set; }

		public Action<Entity, PropertyDefinition, ServiceObject, PropertyInformation> Setter { get; set; }

		public Func<object, string> QueryConstantBuilder { get; set; }

		public override PropertyUpdate GetPropertyUpdate(ServiceObject ewsObject, object value)
		{
			PropertyUpdate propertyUpdate = null;
			if (this.IsDeletingProperty(value))
			{
				if (this.DeletePropertyUpdateCreator != null)
				{
					propertyUpdate = this.DeletePropertyUpdateCreator(ewsObject);
				}
			}
			else if (this.SetPropertyUpdateCreator != null)
			{
				propertyUpdate = this.SetPropertyUpdateCreator(ewsObject);
			}
			if (propertyUpdate != null)
			{
				propertyUpdate.PropertyPath = base.PropertyInformation.PropertyPath;
				return propertyUpdate;
			}
			return base.GetPropertyUpdate(ewsObject, value);
		}

		public override string GetQueryConstant(object value)
		{
			if (this.QueryConstantBuilder != null)
			{
				return this.QueryConstantBuilder(value);
			}
			return base.GetQueryConstant(value);
		}

		protected override bool IsDeletingProperty(object value)
		{
			if (this.DeletingPropertyPedicate != null)
			{
				return this.DeletingPropertyPedicate(value);
			}
			return base.IsDeletingProperty(value);
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			if (this.Getter != null)
			{
				this.Getter(entity, property, ewsObject, base.PropertyInformation);
				return;
			}
			base.GetProperty(entity, property, ewsObject);
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			if (this.Setter != null)
			{
				this.Setter(entity, property, ewsObject, base.PropertyInformation);
				return;
			}
			base.SetProperty(entity, property, ewsObject);
		}
	}
}
