using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class JournalingRuleConstants
	{
		internal static bool TryParseGccType(string gccTypeString, out GccType gccType)
		{
			if (gccTypeString != null)
			{
				if (gccTypeString == "none")
				{
					gccType = GccType.None;
					return true;
				}
				if (gccTypeString == "full")
				{
					gccType = GccType.Full;
					return true;
				}
				if (gccTypeString == "prtt")
				{
					gccType = GccType.Prtt;
					return true;
				}
			}
			gccType = GccType.None;
			return false;
		}

		internal static string StringFromGccType(GccType type)
		{
			switch (type)
			{
			case GccType.Full:
				return "full";
			case GccType.Prtt:
				return "prtt";
			}
			return "none";
		}

		internal const string Journal = "Journal";

		internal const string JournalAndReconcile = "JournalAndReconcile";

		internal const char ReconciliationAccountTupleSeparator = '+';

		internal const char ReconciliationAccountDisabledPrefix = '!';

		internal const string AttributeGccType = "gccType";

		internal const string GccTypeNone = "none";

		internal const string GccTypeFull = "full";

		internal const string GccTypePrtt = "prtt";

		internal const string JournalAgentName = "JA";

		internal const string OriginalMessageGroup = "ORIG";

		internal const string JournalReportGroup = "JR";

		internal const string RuleType = "type";

		internal const string TenantRuleType = "tenant";

		internal const string LawfulInterceptRuleType = "LI";

		internal const string RuleId = "ruleid";

		internal const string MessageId = "mid";

		internal const string Destination = "dest";

		internal const string OriginalMessageId = "orig";

		internal const string JournalRecipsProperty = "Microsoft.Exchange.JournalTargetRecips";

		internal const string JournalReconciliationAccounts = "Microsoft.Exchange.JournalReconciliationAccounts";

		internal const string JournalRuleIdsProperty = "Microsoft.Exchange.JournalRuleIds";
	}
}
