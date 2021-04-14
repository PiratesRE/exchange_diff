using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal interface IContent
	{
		bool Matches(MultiMatcher matcher, RulesEvaluationContext context);
	}
}
