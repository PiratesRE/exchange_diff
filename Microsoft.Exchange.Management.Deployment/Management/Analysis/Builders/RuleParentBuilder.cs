using System;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class RuleParentBuilder
	{
		public RuleParentBuilder(RuleBuildContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public RuleInBuilder<TParent> WithParent<TParent>(Func<AnalysisMember<TParent>> parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			this.context.Parent = parent;
			return new RuleInBuilder<TParent>(this.context);
		}

		public RuleInBuilder<object> AsRootRule()
		{
			this.context.Parent = null;
			return new RuleInBuilder<object>(this.context);
		}

		private RuleBuildContext context;
	}
}
