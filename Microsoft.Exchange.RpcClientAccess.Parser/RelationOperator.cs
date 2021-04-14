using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum RelationOperator : uint
	{
		LessThan,
		LessThanEquals,
		GreaterThan,
		GreaterThanEquals,
		Equals,
		NotEquals,
		RegularExpression,
		MemberOfDistributionList = 100U
	}
}
