using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public enum RelationOperator : byte
	{
		LessThan,
		LessThanEqual,
		GreaterThan,
		GreaterThanEqual,
		Equal,
		NotEqual,
		Like,
		MemberOfDistributionList = 100
	}
}
