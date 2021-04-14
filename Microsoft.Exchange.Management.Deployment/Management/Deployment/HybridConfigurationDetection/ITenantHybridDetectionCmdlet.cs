using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal interface ITenantHybridDetectionCmdlet
	{
		void Connect(PSCredential psCredential, string targetServer, ILogger logger);

		IEnumerable<OrganizationConfig> GetOrganizationConfig();

		void Dispose();
	}
}
