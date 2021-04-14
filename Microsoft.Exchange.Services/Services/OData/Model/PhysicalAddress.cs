using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PhysicalAddress
	{
		internal List<PhysicalAddressFields> Properties { get; set; }

		public PhysicalAddress()
		{
			this.Properties = new List<PhysicalAddressFields>();
		}

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string CountryOrRegion { get; set; }

		public string PostalCode { get; set; }

		internal static readonly LazyMember<EdmComplexType> EdmComplexType = new LazyMember<EdmComplexType>(delegate()
		{
			EdmComplexType edmComplexType = new EdmComplexType(typeof(PhysicalAddress).Namespace, typeof(PhysicalAddress).Name);
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, PhysicalAddressFields.Street.ToString(), EdmCoreModel.Instance.GetString(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, PhysicalAddressFields.City.ToString(), EdmCoreModel.Instance.GetString(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, PhysicalAddressFields.State.ToString(), EdmCoreModel.Instance.GetString(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, PhysicalAddressFields.CountryOrRegion.ToString(), EdmCoreModel.Instance.GetString(true)));
			edmComplexType.AddProperty(new EdmStructuralProperty(edmComplexType, PhysicalAddressFields.PostalCode.ToString(), EdmCoreModel.Instance.GetString(true)));
			return edmComplexType;
		});
	}
}
