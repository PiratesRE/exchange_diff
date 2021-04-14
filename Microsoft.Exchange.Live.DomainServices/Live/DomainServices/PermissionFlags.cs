using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Flags]
	[XmlType(Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[Serializable]
	public enum PermissionFlags
	{
		IsPasswordResetAllowed = 1,
		IsPreProvisionInboxAllowed = 2,
		IsBlockEmailAllowed = 4
	}
}
