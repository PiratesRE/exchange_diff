using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class OtherAddressPropertyProvider : PhysicalAddressPropertyProvider
	{
		public OtherAddressPropertyProvider() : base(OtherAddressPropertyProvider.OtherAddressPropertyInformationList)
		{
			base.IsMultiValueProperty = true;
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			PhysicalAddress value = new PhysicalAddress
			{
				Street = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressOtherStreet),
				State = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressOtherState),
				City = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressOtherCity),
				CountryOrRegion = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressOtherCountryOrRegion),
				PostalCode = ewsObject.GetValueOrDefault<string>(ContactSchema.PhysicalAddressOtherPostalCode)
			};
			entity[property] = value;
		}

		internal static readonly ReadOnlyCollection<PropertyInformation> OtherAddressPropertyInformationList = new ReadOnlyCollection<PropertyInformation>(new List<PropertyInformation>
		{
			ContactSchema.PhysicalAddressOtherStreet,
			ContactSchema.PhysicalAddressOtherState,
			ContactSchema.PhysicalAddressOtherCity,
			ContactSchema.PhysicalAddressOtherCountryOrRegion,
			ContactSchema.PhysicalAddressOtherPostalCode
		});
	}
}
