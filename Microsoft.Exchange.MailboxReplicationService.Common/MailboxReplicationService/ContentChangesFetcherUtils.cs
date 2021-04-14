using System;
using System.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ContentChangesFetcherUtils
	{
		[Conditional("DEBUG")]
		internal static void ValidateEnumeration(IFolder sourceFolder, EnumerateContentChangesFlags flags, IContentChangesFetcher contentChangesFetcher, bool hasMoreChangesPrevPage, bool isPagedEnumeration)
		{
			flags.HasFlag(EnumerateContentChangesFlags.FirstPage);
			flags.HasFlag(EnumerateContentChangesFlags.Catchup);
			if (!isPagedEnumeration || flags.HasFlag(EnumerateContentChangesFlags.FirstPage) || flags.HasFlag(EnumerateContentChangesFlags.Catchup))
			{
				return;
			}
		}

		internal static readonly Restriction ExcludeV40RulesRestriction = Restriction.Not(Restriction.And(new Restriction[]
		{
			Restriction.Content(PropTag.MessageClass, "IPM.Rule.Message", ContentFlags.Prefix),
			Restriction.EQ(PropTag.RuleMsgVersion, 1)
		}));

		internal static readonly Restriction ExcludeAllRulesRestriction = Restriction.Not(Restriction.Or(new Restriction[]
		{
			Restriction.Content(PropTag.MessageClass, "IPM.Rule.Message", ContentFlags.Prefix),
			Restriction.Content(PropTag.MessageClass, "IPM.Rule.Version2.Message", ContentFlags.Prefix),
			Restriction.Content(PropTag.MessageClass, "IPM.ExtendedRule.Message", ContentFlags.Prefix)
		}));
	}
}
