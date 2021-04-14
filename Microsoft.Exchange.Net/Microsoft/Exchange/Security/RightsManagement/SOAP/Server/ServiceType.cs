using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Server
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://microsoft.com/DRM/ServerService")]
	[Serializable]
	public enum ServiceType
	{
		EnrollmentService,
		LicensingService,
		PublishingService,
		CertificationService,
		ActivationService,
		PrecertificationService,
		ServerService,
		DrmRemoteDirectoryServices,
		GroupExpansionService,
		LicensingInternalService,
		CertificationInternalService,
		ServerLicensingWSService,
		CertificationWSService,
		PreLicensingWSService,
		PublishingWSService,
		TemplateDistributionWSService,
		ServerLicensingMexService,
		CertificationMexService,
		PreLicensingMexService,
		PublishingMexService,
		TemplateDistributionMexService
	}
}
