using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum RuleType : byte
	{
		Spam,
		URI,
		IPListSnapshot,
		IPListUpdate,
		IPList
	}
}
