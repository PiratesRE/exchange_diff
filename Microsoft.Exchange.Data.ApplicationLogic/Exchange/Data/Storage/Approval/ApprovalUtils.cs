using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Approval.Common;

namespace Microsoft.Exchange.Data.Storage.Approval
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ApprovalUtils
	{
		public static bool TryGetDecisionMakers(string decisionMakers, out RoutingAddress[] addresses)
		{
			addresses = null;
			if (string.IsNullOrEmpty(decisionMakers))
			{
				ApprovalUtils.diag.TraceDebug(0L, "null or empty decisionMakers string");
				return false;
			}
			if (decisionMakers.Length > 4096)
			{
				ApprovalUtils.diag.TraceDebug<string>(0L, "decision makers too long {0}", decisionMakers);
				return false;
			}
			string[] array = decisionMakers.Split(ApprovalUtils.decisionMakerSeparator, 26, StringSplitOptions.RemoveEmptyEntries);
			addresses = new RoutingAddress[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				addresses[i] = (RoutingAddress)array[i].Trim();
				if (!addresses[i].IsValid || addresses[i] == RoutingAddress.NullReversePath)
				{
					addresses = null;
					return false;
				}
			}
			return true;
		}

		public static IList<RetentionPolicyTag> GetDefaultRetentionPolicyTag(IConfigurationSession scopedSession, ApprovalApplicationId appType, int resultSize)
		{
			SortBy sortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, RetentionPolicyTagSchema.IsDefaultAutoGroupPolicyTag, true);
			QueryFilter filter2 = new ComparisonFilter(ComparisonOperator.Equal, RetentionPolicyTagSchema.IsDefaultModeratedRecipientsPolicyTag, true);
			switch (appType)
			{
			case ApprovalApplicationId.AutoGroup:
				return ApprovalUtils.GetDefaultRetentionPolicyTag(scopedSession, filter, sortBy, resultSize);
			case ApprovalApplicationId.ModeratedRecipient:
				return ApprovalUtils.GetDefaultRetentionPolicyTag(scopedSession, filter2, sortBy, resultSize);
			default:
				return null;
			}
		}

		public static IList<RetentionPolicyTag> GetDefaultRetentionPolicyTag(IConfigurationSession scopedSession, QueryFilter filter, SortBy sortBy, int resultSize)
		{
			return scopedSession.Find<RetentionPolicyTag>(null, QueryScope.SubTree, filter, sortBy, resultSize);
		}

		public const int MaxSupportedDecisionMakers = 25;

		private const int DecisionMakerStringLimit = 4096;

		private static readonly char[] decisionMakerSeparator = new char[]
		{
			';'
		};

		private static readonly Trace diag = ExTraceGlobals.GeneralTracer;
	}
}
