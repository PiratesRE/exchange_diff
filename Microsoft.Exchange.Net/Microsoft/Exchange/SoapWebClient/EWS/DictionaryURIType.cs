using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DictionaryURIType
	{
		[XmlEnum("item:InternetMessageHeader")]
		itemInternetMessageHeader,
		[XmlEnum("contacts:ImAddress")]
		contactsImAddress,
		[XmlEnum("contacts:PhysicalAddress:Street")]
		contactsPhysicalAddressStreet,
		[XmlEnum("contacts:PhysicalAddress:City")]
		contactsPhysicalAddressCity,
		[XmlEnum("contacts:PhysicalAddress:State")]
		contactsPhysicalAddressState,
		[XmlEnum("contacts:PhysicalAddress:CountryOrRegion")]
		contactsPhysicalAddressCountryOrRegion,
		[XmlEnum("contacts:PhysicalAddress:PostalCode")]
		contactsPhysicalAddressPostalCode,
		[XmlEnum("contacts:PhoneNumber")]
		contactsPhoneNumber,
		[XmlEnum("contacts:EmailAddress")]
		contactsEmailAddress,
		[XmlEnum("distributionlist:Members:Member")]
		distributionlistMembersMember
	}
}
