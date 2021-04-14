using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	[Flags]
	public enum RuleScopeType : byte
	{
		None = 0,
		GlobalSpamRules = 1,
		OptionalSpamRules = 2,
		EnvelopeRules = 4,
		PartitionRules = 8,
		EnvelopeHeaderRules = 16,
		All = 255
	}
}
