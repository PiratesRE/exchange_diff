using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RetentionExpireAction : Action
	{
		internal RetentionExpireAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
		}

		public override string Name
		{
			get
			{
				return "RetentionExpire";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RetentionExpireAction.argumentTypes;
			}
		}

		protected override ExecutionControl OnExecute(PolicyEvaluationContext context)
		{
			throw new NotImplementedException("The RetentionExpireAction can only be used for PS object model serialization. Workloads must implement OnExecute.");
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
