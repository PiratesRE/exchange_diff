using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class BusinessAddressPropertyProvider : PhysicalAddressPropertyProvider
	{
		public BusinessAddressPropertyProvider() : base(BusinessAddressPropertyProvider.BusinessAddressPropertyInformationList)
		{
			base.IsMultiValueProperty = true;
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			PhysicalAddress value = new PhysicalAddress
			{
				Street = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressBusinessStreet),
				State = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressBusinessState),
				City = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressBusinessCity),
				CountryOrRegion = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressBusinessCountryOrRegion),
				PostalCode = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressBusinessPostalCode)
			};
			entity[property] = value;
		}

		private static readonly ReadOnlyCollection<PropertyInformation> BusinessAddressPropertyInformationList = new ReadOnlyCollection<PropertyInformation>(new List<PropertyInformation>
		{
			ContactSchema.PhysicalAddressBusinessStreet,
			ContactSchema.PhysicalAddressBusinessState,
			ContactSchema.PhysicalAddressBusinessCity,
			ContactSchema.PhysicalAddressBusinessCountryOrRegion,
			ContactSchema.PhysicalAddressBusinessPostalCode
		});
	}
}
