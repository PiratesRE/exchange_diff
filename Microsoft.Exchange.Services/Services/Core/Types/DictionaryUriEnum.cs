using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DictionaryURIType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DictionaryUriEnum
	{
		[XmlEnum("item:InternetMessageHeader")]
		InternetMessageHeader,
		[XmlEnum("contacts:ImAddress")]
		ImAddress,
		[XmlEnum("contacts:PhysicalAddress")]
		PhysicalAddress,
		[XmlEnum("contacts:PhoneNumber")]
		PhoneNumber,
		[XmlEnum("contacts:EmailAddress")]
		EmailAddress,
		[XmlEnum("contacts:PhysicalAddress:Street")]
		PhysicalAddressStreet,
		[XmlEnum("contacts:PhysicalAddress:City")]
		PhysicalAddressCity,
		[XmlEnum("contacts:PhysicalAddress:State")]
		PhysicalAddressState,
		[XmlEnum("contacts:PhysicalAddress:CountryOrRegion")]
		PhysicalAddressCountryOrRegion,
		[XmlEnum("contacts:PhysicalAddress:PostalCode")]
		PhysicalAddressPostalCode,
		[XmlEnum("distributionlist:Members:Member")]
		DistributionListMembersMember
	}
}
