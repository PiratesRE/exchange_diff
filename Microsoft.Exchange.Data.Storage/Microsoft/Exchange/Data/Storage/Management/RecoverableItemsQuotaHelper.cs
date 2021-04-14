using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RecoverableItemsQuotaHelper
	{
		private static bool IsIncreasingQuotaEnabled(ADUser user)
		{
			if (user != null)
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(user.GetContext(null), null, null);
				return snapshot.Ipaed.IncreaseQuotaForOnHoldMailboxes.Enabled;
			}
			return false;
		}

		public static void IncreaseRecoverableItemsQuotaIfNeeded(ADUser user)
		{
			if (RecoverableItemsQuotaHelper.IsIncreasingQuotaEnabled(user) && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && user.IsInLitigationHoldOrInplaceHold && ((user.UseDatabaseQuotaDefaults != null && user.UseDatabaseQuotaDefaults.Value) || (!user.RecoverableItemsQuota.IsUnlimited && user.RecoverableItemsQuota.Value.ToGB() < RecoverableItemsQuotaHelper.RecoverableItemsQuotaForMailboxesOnHoldInGB)))
			{
				user.UseDatabaseQuotaDefaults = new bool?(false);
				user.RecoverableItemsQuota = ByteQuantifiedSize.FromGB(RecoverableItemsQuotaHelper.RecoverableItemsQuotaForMailboxesOnHoldInGB);
				user.RecoverableItemsWarningQuota = ByteQuantifiedSize.FromGB(RecoverableItemsQuotaHelper.RecoverableItemsWarningQuotaForMailboxesOnHoldInGB);
			}
		}

		public static readonly ulong RecoverableItemsQuotaForMailboxesOnHoldInGB = 100UL;

		public static readonly ulong RecoverableItemsWarningQuotaForMailboxesOnHoldInGB = 90UL;
	}
}
