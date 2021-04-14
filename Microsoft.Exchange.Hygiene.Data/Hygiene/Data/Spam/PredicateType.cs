using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum PredicateType : byte
	{
		Any,
		Match,
		FeatureMatch,
		NumericMatch,
		Exists
	}
}
