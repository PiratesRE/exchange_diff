using System;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	public enum InjectionComparisonOperator
	{
		Skip,
		Equals,
		NotEquals,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual,
		Contains,
		NotContains,
		IsNull,
		IsNotNull,
		PPM
	}
}
