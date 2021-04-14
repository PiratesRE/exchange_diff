using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal interface IOnPremisesHybridDetectionCmdlets
	{
		IEnumerable<AcceptedDomain> GetAcceptedDomain();

		IEnumerable<OrganizationRelationship> GetOrganizationRelationship();
	}
}
