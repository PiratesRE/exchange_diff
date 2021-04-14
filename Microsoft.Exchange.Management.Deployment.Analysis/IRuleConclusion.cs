using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public interface IRuleConclusion
	{
		bool IsConditionMet { get; set; }

		Severity Severity { get; set; }
	}
}
