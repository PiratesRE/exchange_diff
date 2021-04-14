using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class RuleBuildContext : BuildContext<bool>
	{
		public RuleBuildContext(Func<RuleBuildContext, AnalysisMember<bool>> constructor)
		{
			this.constructor = constructor;
			this.SetFunction = null;
		}

		public Func<Result, IEnumerable<Result<bool>>> SetFunction { get; set; }

		public override AnalysisMember<bool> Construct()
		{
			return this.constructor(this);
		}

		private Func<RuleBuildContext, AnalysisMember<bool>> constructor;
	}
}
