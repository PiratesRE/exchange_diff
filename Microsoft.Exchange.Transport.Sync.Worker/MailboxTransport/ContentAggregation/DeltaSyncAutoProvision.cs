using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeltaSyncAutoProvision : IAutoProvision
	{
		public string[] Hostnames
		{
			get
			{
				return this.hostnames;
			}
		}

		public int[] ConnectivePorts
		{
			get
			{
				return this.connectivePorts;
			}
		}

		public DeltaSyncAutoProvision(SmtpAddress emailAddress, SecureString password)
		{
			if (emailAddress == SmtpAddress.Empty)
			{
				throw new ArgumentNullException("emailAddress");
			}
			this.emailAddress = emailAddress;
			this.password = password;
		}

		public static bool ValidateUserHotmailAccount(DeltaSyncUserAccount account, SyncLogSession syncLogSession, out DeltaSyncSettings accountSettings, out LocalizedException validationException)
		{
			AsyncOperationResult<DeltaSyncResultData> asyncOperationResult = null;
			validationException = null;
			accountSettings = null;
			using (DeltaSyncClient deltaSyncClient = new DeltaSyncClient(account, 30000, WebRequest.DefaultWebProxy, long.MaxValue, null, syncLogSession, null))
			{
				IAsyncResult asyncResult = deltaSyncClient.BeginVerifyAccount(null, null, null);
				asyncOperationResult = deltaSyncClient.EndVerifyAccount(asyncResult);
			}
			bool flag = false;
			Exception ex = null;
			if (!asyncOperationResult.IsSucceeded)
			{
				ex = asyncOperationResult.Exception;
			}
			else
			{
				DeltaSyncResultData data = asyncOperationResult.Data;
				if (data.IsTopLevelOperationSuccessful)
				{
					flag = true;
					Exception ex2;
					DeltaSyncResultData.TryGetSettings(data.SettingsResponse, out accountSettings, out ex2);
				}
				else
				{
					ex = data.GetStatusException();
				}
			}
			if (!flag)
			{
				if (ex is AuthenticationException || ex is UserAccessException)
				{
					validationException = new LocalizedException(Strings.HotmailAccountVerificationFailedException);
				}
				else
				{
					validationException = new LocalizedException(Strings.RetryLaterException);
				}
			}
			return flag;
		}

		public static void UpdateSubscriptionSettings(DeltaSyncSettings deltaSyncSettings, ref DeltaSyncAggregationSubscription subscription)
		{
			subscription.MaxNumberOfEmailAdds = deltaSyncSettings.MaxNumberOfEmailAdds;
			subscription.MaxNumberOfFolderAdds = deltaSyncSettings.MaxNumberOfFolderAdds;
			subscription.MaxObjectInSync = deltaSyncSettings.MaxObjectsInSync;
			subscription.MinSettingPollInterval = deltaSyncSettings.MinSettingsPollInterval;
			subscription.MinSyncPollInterval = deltaSyncSettings.MinSyncPollInterval;
			subscription.SyncMultiplier = deltaSyncSettings.SyncMultiplier;
			subscription.MaxAttachments = deltaSyncSettings.MaxAttachments;
			subscription.MaxMessageSize = deltaSyncSettings.MaxMessageSize;
			subscription.MaxRecipients = deltaSyncSettings.MaxRecipients;
			switch (deltaSyncSettings.AccountStatus)
			{
			case AccountStatusType.OK:
				subscription.AccountStatus = DeltaSyncAccountStatus.Normal;
				return;
			case AccountStatusType.Blocked:
				subscription.AccountStatus = DeltaSyncAccountStatus.Blocked;
				return;
			case AccountStatusType.RequiresHIP:
				subscription.AccountStatus = DeltaSyncAccountStatus.HipRequired;
				return;
			default:
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "unknown AccountStatus: {0}", new object[]
				{
					deltaSyncSettings.AccountStatus
				}));
			}
		}

		public DiscoverSettingsResult DiscoverSetting(SyncLogSession syncLogSession, bool testOnlyInsecure, Dictionary<Authority, bool> connectiveAuthority, AutoProvisionProgress progressCallback, out PimSubscriptionProxy sub)
		{
			sub = null;
			if (this.password == null)
			{
				throw new InvalidOperationException("Password not set");
			}
			if (testOnlyInsecure)
			{
				return DiscoverSettingsResult.InsecureSettingsNotSupported;
			}
			progressCallback(Strings.AutoProvisionTestHotmail, new LocalizedString(this.emailAddress.ToString()));
			HotmailSubscriptionProxy hotmailSubscriptionProxy = new HotmailSubscriptionProxy();
			DeltaSyncUserAccount deltaSyncUserAccount = DeltaSyncUserAccount.CreateDeltaSyncUserForTrustedPartnerAuthWithPassword(this.emailAddress.ToString(), SyncUtilities.SecureStringToString(this.password));
			DeltaSyncSettings deltaSyncSettings;
			LocalizedException ex;
			if (DeltaSyncAutoProvision.ValidateUserHotmailAccount(deltaSyncUserAccount, syncLogSession.OpenWithContext(hotmailSubscriptionProxy.Subscription.SubscriptionGuid), out deltaSyncSettings, out ex))
			{
				hotmailSubscriptionProxy.SetLiveAccountPuid(deltaSyncUserAccount.AuthToken.PUID);
				if (deltaSyncSettings != null)
				{
					DeltaSyncAggregationSubscription deltaSyncAggregationSubscription = hotmailSubscriptionProxy.Subscription as DeltaSyncAggregationSubscription;
					DeltaSyncAutoProvision.UpdateSubscriptionSettings(deltaSyncSettings, ref deltaSyncAggregationSubscription);
				}
				sub = hotmailSubscriptionProxy;
				return DiscoverSettingsResult.Succeeded;
			}
			return DiscoverSettingsResult.SettingsNotFound;
		}

		internal const int DeltaSyncVerificationTimeout = 30000;

		private readonly SmtpAddress emailAddress;

		private readonly SecureString password;

		private readonly int[] connectivePorts = new int[0];

		private readonly string[] hostnames = new string[0];
	}
}
