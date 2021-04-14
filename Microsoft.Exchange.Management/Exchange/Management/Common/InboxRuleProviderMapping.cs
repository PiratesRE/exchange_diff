using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct InboxRuleProviderMapping
	{
		internal InboxRuleProviderMapping(RuleProviderId id, string name)
		{
			this.RuleProviderId = id;
			this.RuleProviderString = name;
		}

		internal static string GetRuleProviderString(RuleProviderId id)
		{
			for (int i = 0; i < InboxRuleProviderMapping.table.Length; i++)
			{
				if (InboxRuleProviderMapping.table[i].RuleProviderId == id)
				{
					return InboxRuleProviderMapping.table[i].RuleProviderString;
				}
			}
			throw new ArgumentException("RuleProviderId '" + id.ToString() + "' is not supported.");
		}

		internal readonly RuleProviderId RuleProviderId;

		internal readonly string RuleProviderString;

		private static InboxRuleProviderMapping[] table = new InboxRuleProviderMapping[]
		{
			new InboxRuleProviderMapping(RuleProviderId.Unknown, null),
			new InboxRuleProviderMapping(RuleProviderId.OL98Plus, "RuleOrganizer"),
			new InboxRuleProviderMapping(RuleProviderId.Exchange14, "ExchangeMailboxRules14")
		};
	}
}
