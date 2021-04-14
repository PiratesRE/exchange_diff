using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class HomeAddressPropertyProvider : PhysicalAddressPropertyProvider
	{
		public HomeAddressPropertyProvider() : base(HomeAddressPropertyProvider.HomeAddressPropertyInformationList)
		{
			base.IsMultiValueProperty = true;
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			PhysicalAddress value = new PhysicalAddress
			{
				Street = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressHomeStreet),
				State = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressHomeState),
				City = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressHomeCity),
				CountryOrRegion = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressHomeCountryOrRegion),
				PostalCode = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressHomePostalCode)
			};
			entity[property] = value;
		}

		internal static readonly ReadOnlyCollection<PropertyInformation> HomeAddressPropertyInformationList = new ReadOnlyCollection<PropertyInformation>(new List<PropertyInformation>
		{
			ContactSchema.PhysicalAddressHomeStreet,
			ContactSchema.PhysicalAddressHomeState,
			ContactSchema.PhysicalAddressHomeCity,
			ContactSchema.PhysicalAddressHomeCountryOrRegion,
			ContactSchema.PhysicalAddressHomePostalCode
		});
	}
}
