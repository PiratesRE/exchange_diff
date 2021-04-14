using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public class PsBlockAccessAction : PsComplianceRuleActionBase
	{
		internal override Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action ToEngineAction()
		{
			return new BlockAccessAction(new List<Argument>(), null);
		}

		internal static PsBlockAccessAction FromEngineAction(BlockAccessAction action)
		{
			return new PsBlockAccessAction();
		}
	}
}
