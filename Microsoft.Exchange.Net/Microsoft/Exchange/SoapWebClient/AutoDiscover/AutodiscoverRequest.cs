using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[XmlInclude(typeof(GetOrganizationRelationshipSettingsRequest))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(GetUserSettingsRequest))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(GetFederationInformationRequest))]
	[XmlInclude(typeof(GetDomainSettingsRequest))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class AutodiscoverRequest
	{
	}
}
