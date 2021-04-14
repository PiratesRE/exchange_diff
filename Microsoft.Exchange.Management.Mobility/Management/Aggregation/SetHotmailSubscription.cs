using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Set", "HotmailSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetHotmailSubscription : SetSubscriptionBase<HotmailSubscriptionProxy>
	{
		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public SecureString Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetHotmailSubscriptionConfirmation(this.Identity);
			}
		}

		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.DeltaSyncMail;
			}
		}

		private HotmailSubscriptionProxy DynamicParameters
		{
			get
			{
				return (HotmailSubscriptionProxy)this.GetDynamicParameters();
			}
		}

		protected override void ValidateWithDataObject(IConfigurable dataObject)
		{
			base.ValidateWithDataObject(dataObject);
			if (this.Password != null)
			{
				DeltaSyncUserAccount account = DeltaSyncUserAccount.CreateDeltaSyncUserForTrustedPartnerAuthWithPassword(((HotmailSubscriptionProxy)dataObject).EmailAddress.ToString(), SyncUtilities.SecureStringToString(this.Password));
				LocalizedException exception;
				if (!DeltaSyncAutoProvision.ValidateUserHotmailAccount(account, CommonLoggingHelper.SyncLogSession, out this.accountSettings, out exception))
				{
					base.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, null);
				}
			}
			base.WriteDebugInfo();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.StampChangesOn(dataObject);
			HotmailSubscriptionProxy hotmailSubscriptionProxy = dataObject as HotmailSubscriptionProxy;
			DeltaSyncAggregationSubscription deltaSyncAggregationSubscription = hotmailSubscriptionProxy.Subscription as DeltaSyncAggregationSubscription;
			if (this.accountSettings != null)
			{
				DeltaSyncAutoProvision.UpdateSubscriptionSettings(this.accountSettings, ref deltaSyncAggregationSubscription);
			}
			TaskLogger.LogExit();
		}

		private DeltaSyncSettings accountSettings;
	}
}
