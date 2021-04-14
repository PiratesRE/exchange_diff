using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public enum PartnerType
	{
		Min,
		MicrosoftSupport,
		SyndicatePartner,
		BreadthPartner,
		BreadthPartnerDelegatedAdmin,
		OperatingCompany,
		IndependentSoftwareVendor,
		Max
	}
}
