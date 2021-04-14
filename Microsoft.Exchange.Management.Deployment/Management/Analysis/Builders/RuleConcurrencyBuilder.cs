using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class RuleConcurrencyBuilder<TParent>
	{
		public RuleConcurrencyBuilder(RuleBuildContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public RuleBuilder<TParent> AsSync()
		{
			this.context.RunAs = ConcurrencyType.Synchronous;
			return new RuleBuilder<TParent>(this.context);
		}

		public RuleBuilder<TParent> AsAsync()
		{
			this.context.RunAs = ConcurrencyType.ASynchronous;
			return new RuleBuilder<TParent>(this.context);
		}

		private RuleBuildContext context;
	}
}
