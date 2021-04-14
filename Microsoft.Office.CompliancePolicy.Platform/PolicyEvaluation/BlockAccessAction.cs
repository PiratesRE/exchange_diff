using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class BlockAccessAction : Action
	{
		public BlockAccessAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
		}

		public override string Name
		{
			get
			{
				return "BlockAccess";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return BlockAccessAction.minVersion;
			}
		}

		protected override ExecutionControl OnExecute(PolicyEvaluationContext context)
		{
			throw new NotImplementedException("The BlockAccessAction can only be used for PS object model serialization. Workloads must implement OnExecute.");
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");
	}
}
