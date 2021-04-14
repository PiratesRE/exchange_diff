using System;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public enum InterceptorAgentConditionMatchType
	{
		CaseInsensitive,
		CaseSensitive,
		CaseSensitiveEqual,
		CaseInsensitiveEqual,
		CaseSensitiveNotEqual,
		CaseInsensitiveNotEqual,
		Regex,
		PatternMatch,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual
	}
}
