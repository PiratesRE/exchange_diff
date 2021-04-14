using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[Serializable]
	public enum CompanyProfileStateChangeReason
	{
		Other,
		Lifecycle,
		UserRequest,
		Fraud
	}
}
