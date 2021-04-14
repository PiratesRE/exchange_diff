using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public class PsHoldContentAction : PsComplianceRuleActionBase
	{
		public PsHoldContentAction(int holdDurationDays, HoldDurationHint holdDurationDisplayHint)
		{
			this.HoldDurationDays = holdDurationDays;
			this.HoldDurationDisplayHint = holdDurationDisplayHint;
		}

		public int HoldDurationDays { get; private set; }

		public HoldDurationHint HoldDurationDisplayHint { get; private set; }

		internal override Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action ToEngineAction()
		{
			return new HoldAction(new List<Argument>
			{
				new Value(this.HoldDurationDays.ToString(CultureInfo.InvariantCulture)),
				new Value(this.HoldDurationDisplayHint.ToString())
			}, null);
		}

		internal static PsHoldContentAction FromEngineAction(HoldAction action)
		{
			return new PsHoldContentAction(action.HoldDurationDays, action.HoldDurationDisplayHint);
		}
	}
}
