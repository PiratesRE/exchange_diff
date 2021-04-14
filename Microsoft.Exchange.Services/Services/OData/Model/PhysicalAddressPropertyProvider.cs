using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PhysicalAddressPropertyProvider : SimpleEwsPropertyProvider
	{
		public PhysicalAddressPropertyProvider(ReadOnlyCollection<PropertyInformation> physicalAddressPropertyInformationList) : base(physicalAddressPropertyInformationList)
		{
			base.IsMultiValueProperty = true;
		}

		public override List<PropertyUpdate> GetPropertyUpdateList(Entity entity, PropertyDefinition property, object value)
		{
			PhysicalAddress physicalAddress = entity[property] as PhysicalAddress;
			List<PropertyUpdate> list = new List<PropertyUpdate>();
			ArgumentValidator.ThrowIfNull("GetPropertyUpdateList:address", physicalAddress);
			foreach (PropertyInformation propertyInformation in base.PropertyInformationList)
			{
				string text;
				if (this.TryGetComplexPropertyValue(propertyInformation.PropertyPath, physicalAddress, out text))
				{
					ContactItemType contactItemType = EwsServiceObjectFactory.CreateServiceObject<ContactItemType>(entity);
					contactItemType[propertyInformation] = text;
					PropertyUpdate propertyUpdate;
					if (text != null)
					{
						propertyUpdate = EwsPropertyProvider.SetItemPropertyUpdateDelegate(contactItemType);
					}
					else
					{
						propertyUpdate = EwsPropertyProvider.DeleteItemPropertyUpdateDelegate(contactItemType);
					}
					propertyUpdate.PropertyPath = propertyInformation.PropertyPath;
					list.Add(propertyUpdate);
				}
			}
			return list;
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			PhysicalAddress physicalAddress = entity[property] as PhysicalAddress;
			ArgumentValidator.ThrowIfNull("SetProperty:address", physicalAddress);
			foreach (PropertyInformation propertyInformation in base.PropertyInformationList)
			{
				string text;
				if (this.TryGetComplexPropertyValue(propertyInformation.PropertyPath, physicalAddress, out text) && text != null)
				{
					ewsObject[propertyInformation] = text;
				}
			}
		}

		private bool TryGetComplexPropertyValue(PropertyPath propertypath, PhysicalAddress address, out string propertyValue)
		{
			DictionaryPropertyUriBase dictionaryPropertyUriBase = propertypath as DictionaryPropertyUriBase;
			propertyValue = null;
			if (dictionaryPropertyUriBase == null)
			{
				return false;
			}
			switch (dictionaryPropertyUriBase.FieldUri)
			{
			case DictionaryUriEnum.PhysicalAddressStreet:
				if (address.Properties.Contains(PhysicalAddressFields.Street))
				{
					propertyValue = address.Street;
					return true;
				}
				break;
			case DictionaryUriEnum.PhysicalAddressCity:
				if (address.Properties.Contains(PhysicalAddressFields.City))
				{
					propertyValue = address.City;
					return true;
				}
				break;
			case DictionaryUriEnum.PhysicalAddressState:
				if (address.Properties.Contains(PhysicalAddressFields.State))
				{
					propertyValue = address.State;
					return true;
				}
				break;
			case DictionaryUriEnum.PhysicalAddressCountryOrRegion:
				if (address.Properties.Contains(PhysicalAddressFields.CountryOrRegion))
				{
					propertyValue = address.CountryOrRegion;
					return true;
				}
				break;
			case DictionaryUriEnum.PhysicalAddressPostalCode:
				if (address.Properties.Contains(PhysicalAddressFields.PostalCode))
				{
					propertyValue = address.PostalCode;
					return true;
				}
				break;
			default:
				return false;
			}
			return false;
		}
	}
}
