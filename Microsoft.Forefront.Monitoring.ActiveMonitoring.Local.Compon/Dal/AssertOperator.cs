using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public enum AssertOperator
	{
		EqualTo,
		NotEqualTo,
		LessThan,
		LessThanOrEqualTo,
		GreaterThan,
		GreaterThanOrEqualTo,
		RegexMatch
	}
}
