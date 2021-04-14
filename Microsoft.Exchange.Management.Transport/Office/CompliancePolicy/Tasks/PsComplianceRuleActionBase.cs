using System;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class PsComplianceRuleActionBase
	{
		internal abstract Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action ToEngineAction();

		internal static PsComplianceRuleActionBase FromEngineAction(Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action action)
		{
			if (action is HoldAction)
			{
				return PsHoldContentAction.FromEngineAction(action as HoldAction);
			}
			if (action is BlockAccessAction)
			{
				return PsBlockAccessAction.FromEngineAction(action as BlockAccessAction);
			}
			throw new UnexpectedConditionOrActionDetectedException();
		}
	}
}
