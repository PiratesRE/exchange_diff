using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Resolver
{
	internal interface ITargetResolver
	{
		IEnumerable<ComplianceMessage> Resolve(IEnumerable<ComplianceMessage> sources);
	}
}
