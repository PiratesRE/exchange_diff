using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RetentionRecycleAction : Action
	{
		internal RetentionRecycleAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RetentionRecycleAction.argumentTypes;
			}
		}

		public override string Name
		{
			get
			{
				return "RetentionRecycle";
			}
		}

		protected override ExecutionControl OnExecute(PolicyEvaluationContext context)
		{
			throw new NotImplementedException("The RetentionRecycle can only be used for PS object model serialization. Workloads must implement OnExecute.");
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
