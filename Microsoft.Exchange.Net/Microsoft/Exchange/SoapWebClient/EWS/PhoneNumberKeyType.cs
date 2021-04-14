using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum PhoneNumberKeyType
	{
		AssistantPhone,
		BusinessFax,
		BusinessPhone,
		BusinessPhone2,
		Callback,
		CarPhone,
		CompanyMainPhone,
		HomeFax,
		HomePhone,
		HomePhone2,
		Isdn,
		MobilePhone,
		OtherFax,
		OtherTelephone,
		Pager,
		PrimaryPhone,
		RadioPhone,
		Telex,
		TtyTddPhone
	}
}
