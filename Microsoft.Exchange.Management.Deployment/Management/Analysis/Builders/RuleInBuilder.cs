using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class RuleInBuilder<TParent>
	{
		public RuleInBuilder(RuleBuildContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public RuleConcurrencyBuilder<TParent> In(Analysis analysis)
		{
			if (analysis == null)
			{
				throw new ArgumentNullException("analysis");
			}
			this.context.Analysis = analysis;
			return new RuleConcurrencyBuilder<TParent>(this.context);
		}

		private RuleBuildContext context;
	}
}
