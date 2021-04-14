using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal interface IHybridConfigurationDetection
	{
		bool RunTenantHybridTest(PSCredential psCredential, string organizationConfigHash);

		bool RunTenantHybridTest(string pathToConfigFile);

		bool RunOnPremisesHybridTest();
	}
}
