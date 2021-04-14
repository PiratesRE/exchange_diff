using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PolicyAllowedDetailLevel
	{
		public static int GetMaxAllowed(SharingPolicyAction allowedActions)
		{
			EnumValidator.ThrowIfInvalid<SharingPolicyAction>(allowedActions, "allowedActions");
			int num = 0;
			foreach (PolicyAllowedDetailLevel policyAllowedDetailLevel in PolicyAllowedDetailLevel.Rules)
			{
				if ((allowedActions & policyAllowedDetailLevel.action) == policyAllowedDetailLevel.action && num < (int)policyAllowedDetailLevel.maxAllowedDetailLevel)
				{
					num = (int)policyAllowedDetailLevel.maxAllowedDetailLevel;
				}
			}
			return num;
		}

		private PolicyAllowedDetailLevel(SharingPolicyAction action, DetailLevelEnumType maxAllowedDetailLevel)
		{
			this.action = action;
			this.maxAllowedDetailLevel = maxAllowedDetailLevel;
		}

		private static readonly PolicyAllowedDetailLevel[] Rules = new PolicyAllowedDetailLevel[]
		{
			new PolicyAllowedDetailLevel(SharingPolicyAction.CalendarSharingFreeBusySimple, DetailLevelEnumType.AvailabilityOnly),
			new PolicyAllowedDetailLevel(SharingPolicyAction.CalendarSharingFreeBusyDetail, DetailLevelEnumType.LimitedDetails),
			new PolicyAllowedDetailLevel(SharingPolicyAction.CalendarSharingFreeBusyReviewer, DetailLevelEnumType.FullDetails)
		};

		private readonly SharingPolicyAction action;

		private readonly DetailLevelEnumType maxAllowedDetailLevel;
	}
}
