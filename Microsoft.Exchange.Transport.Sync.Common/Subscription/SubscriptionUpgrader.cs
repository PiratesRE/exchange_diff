using System;
using System.Globalization;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionUpgrader
	{
		public SubscriptionUpgrader(SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
			if (!this.TryReadAllowUpgradeValueFromRegistry(out this.allowUpgrades))
			{
				this.allowUpgrades = true;
			}
		}

		public bool UpgradeSubscription(AggregationSubscription subscription, MessageItem message, SubscriptionMessageHelper messageHelper)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("message", message);
			SyncUtilities.ThrowIfArgumentNull("messageHelper", messageHelper);
			if (!subscription.IsUpgradeRequired())
			{
				this.syncLogSession.LogDebugging((TSLID)1473UL, "No Upgrade required for subscription {0}", new object[]
				{
					subscription.SubscriptionIdentity
				});
				return false;
			}
			if (!this.allowUpgrades)
			{
				this.syncLogSession.LogDebugging((TSLID)1474UL, "Upgrade not permitted. And skipping upgrade for subscription {0}", new object[]
				{
					subscription.SubscriptionIdentity
				});
				return false;
			}
			subscription.UpgradeSubscription();
			this.syncLogSession.LogVerbose((TSLID)1475UL, "In-memory upgrade succeeded for subscription {0}", new object[]
			{
				subscription.SubscriptionIdentity
			});
			bool result;
			try
			{
				message.OpenAsReadWrite();
				messageHelper.SaveSubscriptionToMessage(subscription, message);
				this.syncLogSession.LogVerbose((TSLID)1476UL, "Succeeded to save the in-memory upgraded subscription {0} to subscription message.", new object[]
				{
					subscription.SubscriptionIdentity
				});
				result = true;
			}
			catch (LocalizedException ex)
			{
				this.syncLogSession.LogError((TSLID)1477UL, "Marking subscription {0} as InvalidVersion as it failed to save the in-memory upgraded subscription to subscription message due to {1}.", new object[]
				{
					subscription.SubscriptionIdentity,
					ex
				});
				subscription.MarkSubscriptionInvalidVersion(message);
				result = false;
			}
			return result;
		}

		protected virtual bool TryReadAllowUpgradeValueFromRegistry(out bool allowUpgradeValue)
		{
			allowUpgradeValue = false;
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(SubscriptionUpgrader.baseRegistryLocation, false))
				{
					if (registryKey == null)
					{
						this.syncLogSession.LogVerbose((TSLID)1478UL, "Failed to Read {0}\\{1} as the registry key does not exist", new object[]
						{
							SubscriptionUpgrader.baseRegistryLocation,
							SubscriptionUpgrader.allowSubscriptionUpgradeRegistryKeyName
						});
						return false;
					}
					object value = registryKey.GetValue(SubscriptionUpgrader.allowSubscriptionUpgradeRegistryKeyName);
					if (value != null && value is string && bool.TryParse((string)value, out allowUpgradeValue))
					{
						this.syncLogSession.LogVerbose((TSLID)1479UL, "Read {0}\\{1} config as {2}", new object[]
						{
							SubscriptionUpgrader.baseRegistryLocation,
							SubscriptionUpgrader.allowSubscriptionUpgradeRegistryKeyName,
							allowUpgradeValue
						});
						return true;
					}
					this.syncLogSession.LogVerbose((TSLID)1480UL, "Could not decipher value from {0}\\{1}: '{2}'", new object[]
					{
						SubscriptionUpgrader.baseRegistryLocation,
						SubscriptionUpgrader.allowSubscriptionUpgradeRegistryKeyName,
						value
					});
					return false;
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			this.syncLogSession.LogError((TSLID)1481UL, "Could not read value from {0}\\{1}: due to {2}", new object[]
			{
				SubscriptionUpgrader.baseRegistryLocation,
				SubscriptionUpgrader.allowSubscriptionUpgradeRegistryKeyName,
				ex
			});
			return false;
		}

		private static readonly string baseRegistryLocation = string.Format(CultureInfo.InvariantCulture, "SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Transport\\Sync\\", new object[]
		{
			"v15"
		});

		private static readonly string allowSubscriptionUpgradeRegistryKeyName = "AllowSubscriptionUpgrade";

		private readonly SyncLogSession syncLogSession;

		private readonly bool allowUpgrades;
	}
}
