using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[XmlType(TypeName = "InvalidRecipientResponseCodeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum InvalidRecipientResponseCodeType
	{
		OtherError,
		RecipientOrganizationNotFederated,
		CannotObtainTokenFromSTS,
		SystemPolicyBlocksSharingWithThisRecipient,
		RecipientOrganizationFederatedWithUnknownTokenIssuer
	}
}
