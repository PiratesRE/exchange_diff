using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class EwsPropertyProvider : PropertyProvider
	{
		public bool IsMultiValueProperty { get; protected set; }

		public EwsPropertyProvider(PropertyInformation propertyInformation)
		{
			ArgumentValidator.ThrowIfNull("propertyInformation", propertyInformation);
			this.PropertyInformation = propertyInformation;
		}

		public EwsPropertyProvider(ReadOnlyCollection<PropertyInformation> propertyInformationList)
		{
			ArgumentValidator.ThrowIfNull("propertyInformationList", propertyInformationList);
			this.PropertyInformationList = propertyInformationList;
		}

		public override void GetPropertyFromDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.GetProperty(entity, property, (ServiceObject)dataSource);
		}

		public override void SetPropertyToDataSource(Entity entity, PropertyDefinition property, object dataSource)
		{
			ArgumentValidator.ThrowIfNull("entity", entity);
			ArgumentValidator.ThrowIfNull("property", property);
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.SetProperty(entity, property, (ServiceObject)dataSource);
		}

		public PropertyInformation PropertyInformation { get; private set; }

		public ReadOnlyCollection<PropertyInformation> PropertyInformationList { get; private set; }

		public virtual PropertyUpdate GetPropertyUpdate(ServiceObject ewsObject, object value)
		{
			PropertyUpdate propertyUpdate;
			if (this.IsDeletingProperty(value))
			{
				propertyUpdate = EwsPropertyProvider.DeleteItemPropertyUpdateDelegate(ewsObject);
			}
			else
			{
				propertyUpdate = EwsPropertyProvider.SetItemPropertyUpdateDelegate(ewsObject);
			}
			propertyUpdate.PropertyPath = this.PropertyInformation.PropertyPath;
			return propertyUpdate;
		}

		public virtual List<PropertyUpdate> GetPropertyUpdateList(Entity entity, PropertyDefinition Prop, object value)
		{
			throw new NotImplementedException("GetPropertyUpdateList is not implemented for this property provider");
		}

		public virtual string GetQueryConstant(object value)
		{
			if (value != null)
			{
				return value.ToString();
			}
			return null;
		}

		protected virtual bool IsDeletingProperty(object value)
		{
			return value == null;
		}

		protected virtual void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			if (this.PropertyInformation != null)
			{
				if (ewsObject.PropertyBag.Contains(this.PropertyInformation))
				{
					entity[property] = ewsObject[this.PropertyInformation];
					return;
				}
				if (property.EdmType.IsNullable)
				{
					entity[property] = null;
				}
			}
		}

		protected virtual void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			if (this.PropertyInformation != null && entity.PropertyBag.Contains(property))
			{
				ewsObject[this.PropertyInformation] = entity[property];
			}
		}

		public static readonly Func<ServiceObject, PropertyUpdate> SetItemPropertyUpdateDelegate = (ServiceObject s) => new SetItemPropertyUpdate
		{
			Item = (ItemType)s
		};

		public static readonly Func<ServiceObject, PropertyUpdate> DeleteItemPropertyUpdateDelegate = (ServiceObject s) => new DeleteItemPropertyUpdate();

		public static readonly Func<ServiceObject, PropertyUpdate> SetFolderPropertyUpdateDelegate = (ServiceObject s) => new SetFolderPropertyUpdate
		{
			Folder = (BaseFolderType)s
		};

		public static readonly Func<ServiceObject, PropertyUpdate> DeleteFolderPropertyUpdateDelegate = (ServiceObject s) => new DeleteFolderPropertyUpdate();
	}
}
