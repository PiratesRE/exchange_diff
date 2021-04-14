using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy
{
	[XmlInclude(typeof(GetFederationInformationRequest))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[XmlInclude(typeof(GetUserSettingsRequest))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(GetDomainSettingsRequest))]
	[XmlInclude(typeof(GetOrganizationRelationshipSettingsRequest))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class AutodiscoverRequest
	{
	}
}
