using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PhysicalAddressODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			return PhysicalAddressODataConverter.ODataValueToPhysicalAddress((ODataComplexValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			return PhysicalAddressODataConverter.PhysicalAddressToODataValue((PhysicalAddress)rawValue);
		}

		internal static ODataValue PhysicalAddressToODataValue(PhysicalAddress address)
		{
			if (address == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(PhysicalAddress).FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = PhysicalAddressFields.Street.ToString(),
					Value = address.Street
				},
				new ODataProperty
				{
					Name = PhysicalAddressFields.City.ToString(),
					Value = address.City
				},
				new ODataProperty
				{
					Name = PhysicalAddressFields.State.ToString(),
					Value = address.State
				},
				new ODataProperty
				{
					Name = PhysicalAddressFields.CountryOrRegion.ToString(),
					Value = address.CountryOrRegion
				},
				new ODataProperty
				{
					Name = PhysicalAddressFields.PostalCode.ToString(),
					Value = address.PostalCode
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}

		internal static PhysicalAddress ODataValueToPhysicalAddress(ODataComplexValue complexValue)
		{
			if (complexValue == null)
			{
				return null;
			}
			PhysicalAddress physicalAddress = new PhysicalAddress();
			physicalAddress.Street = complexValue.GetPropertyValue(PhysicalAddressFields.Street.ToString(), null);
			physicalAddress.City = complexValue.GetPropertyValue(PhysicalAddressFields.City.ToString(), null);
			physicalAddress.State = complexValue.GetPropertyValue(PhysicalAddressFields.State.ToString(), null);
			physicalAddress.CountryOrRegion = complexValue.GetPropertyValue(PhysicalAddressFields.CountryOrRegion.ToString(), null);
			physicalAddress.PostalCode = complexValue.GetPropertyValue(PhysicalAddressFields.PostalCode.ToString(), null);
			if (complexValue.Properties.Any((ODataProperty x) => x.Name.Equals(PhysicalAddressFields.Street.ToString())))
			{
				physicalAddress.Properties.Add(PhysicalAddressFields.Street);
			}
			if (complexValue.Properties.Any((ODataProperty x) => x.Name.Equals(PhysicalAddressFields.City.ToString())))
			{
				physicalAddress.Properties.Add(PhysicalAddressFields.City);
			}
			if (complexValue.Properties.Any((ODataProperty x) => x.Name.Equals(PhysicalAddressFields.State.ToString())))
			{
				physicalAddress.Properties.Add(PhysicalAddressFields.State);
			}
			if (complexValue.Properties.Any((ODataProperty x) => x.Name.Equals(PhysicalAddressFields.CountryOrRegion.ToString())))
			{
				physicalAddress.Properties.Add(PhysicalAddressFields.CountryOrRegion);
			}
			if (complexValue.Properties.Any((ODataProperty x) => x.Name.Equals(PhysicalAddressFields.PostalCode.ToString())))
			{
				physicalAddress.Properties.Add(PhysicalAddressFields.PostalCode);
			}
			return physicalAddress;
		}
	}
}
