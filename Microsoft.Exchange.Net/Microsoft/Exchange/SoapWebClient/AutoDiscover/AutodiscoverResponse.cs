using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[XmlInclude(typeof(DomainResponse))]
	[XmlInclude(typeof(GetDomainSettingsResponse))]
	[XmlInclude(typeof(UserResponse))]
	[XmlInclude(typeof(GetUserSettingsResponse))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlInclude(typeof(GetOrganizationRelationshipSettingsResponse))]
	[XmlInclude(typeof(GetFederationInformationResponse))]
	[Serializable]
	public class AutodiscoverResponse
	{
		public ErrorCode ErrorCode;

		[XmlIgnore]
		public bool ErrorCodeSpecified;

		[XmlElement(IsNullable = true)]
		public string ErrorMessage;
	}
}
