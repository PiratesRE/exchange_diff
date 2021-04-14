using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Analysis.Features;

namespace Microsoft.Exchange.Management.Analysis.Builders
{
	internal sealed class RuleBuilder<TParent> : IRuleFeatureBuilder, IFeatureBuilder
	{
		public RuleBuilder(RuleBuildContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.context = context;
		}

		public Rule Condition(Func<Result<TParent>, RuleResult> setFunction)
		{
			this.context.SetFunction = delegate(Result x)
			{
				IEnumerable<Result<bool>> result;
				try
				{
					result = new RuleResult[]
					{
						setFunction((Result<TParent>)x)
					};
				}
				catch (Exception exception)
				{
					result = new RuleResult[]
					{
						new RuleResult(exception)
					};
				}
				return result;
			};
			return (Rule)this.context.Construct();
		}

		void IFeatureBuilder.AddFeature(Feature feature)
		{
			((IFeatureBuilder)this.context).AddFeature(feature);
		}

		private RuleBuildContext context;
	}
}
