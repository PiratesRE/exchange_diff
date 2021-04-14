using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
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
