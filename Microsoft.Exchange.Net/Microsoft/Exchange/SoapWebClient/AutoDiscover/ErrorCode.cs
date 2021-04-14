using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum ErrorCode
	{
		NoError,
		RedirectAddress,
		RedirectUrl,
		InvalidUser,
		InvalidRequest,
		InvalidSetting,
		SettingIsNotAvailable,
		ServerBusy,
		InvalidDomain,
		NotFederated,
		InternalServerError
	}
}
