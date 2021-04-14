using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.AirSync
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IMdmNotificationSettings : ISettings
	{
		Uri EnrollmentUrl { get; }

		Uri ComplianceStatusUrl { get; }

		string ADRegistrationServiceHost { get; }

		Uri EnrollmentUrlWithBasicSteps { get; }

		string ActivationUrlWithBasicSteps { get; }

		TimeSpan DeviceStatusCacheExpirationInternal { get; }

		TimeSpan NegativeDeviceStatusCacheExpirationInterval { get; }

		bool PolicyEvaluationEnabled { get; }
	}
}
