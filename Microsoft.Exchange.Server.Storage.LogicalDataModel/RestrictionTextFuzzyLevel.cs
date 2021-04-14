using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public enum RestrictionTextFuzzyLevel : ushort
	{
		IgnoreCase = 1,
		IgnoreNonSpace,
		Loose = 4
	}
}
