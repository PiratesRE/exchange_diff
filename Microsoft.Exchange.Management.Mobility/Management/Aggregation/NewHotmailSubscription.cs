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
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("New", "HotmailSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "AggregationParameterSet")]
	public sealed class NewHotmailSubscription : NewSubscriptionBase<HotmailSubscriptionProxy>
	{
		[Parameter(Mandatory = true, ParameterSetName = "AggregationParameterSet")]
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

		[Parameter(Mandatory = true, ParameterSetName = "AggregationParameterSet")]
		public new SmtpAddress EmailAddress
		{
			get
			{
				return (SmtpAddress)(base.Fields["EmailAddress"] ?? SmtpAddress.Empty);
			}
			set
			{
				base.Fields["EmailAddress"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "AggregationParameterSet")]
		public new string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.CreateHotmailSubscriptionConfirmation(this.DataObject);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			HotmailSubscriptionProxy hotmailSubscriptionProxy = base.PrepareDataObject() as HotmailSubscriptionProxy;
			if (this.accountSettings != null)
			{
				DeltaSyncAggregationSubscription deltaSyncAggregationSubscription = hotmailSubscriptionProxy.Subscription as DeltaSyncAggregationSubscription;
				DeltaSyncAutoProvision.UpdateSubscriptionSettings(this.accountSettings, ref deltaSyncAggregationSubscription);
			}
			return hotmailSubscriptionProxy;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			DeltaSyncUserAccount deltaSyncUserAccount = DeltaSyncUserAccount.CreateDeltaSyncUserForTrustedPartnerAuthWithPassword(this.EmailAddress.ToString(), SyncUtilities.SecureStringToString(this.Password));
			LocalizedException exception;
			if (!DeltaSyncAutoProvision.ValidateUserHotmailAccount(deltaSyncUserAccount, CommonLoggingHelper.SyncLogSession, out this.accountSettings, out exception))
			{
				base.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, null);
			}
			this.DataObject.SetLiveAccountPuid(deltaSyncUserAccount.AuthToken.PUID);
			base.WriteDebugInfo();
			TaskLogger.LogExit();
		}

		private DeltaSyncSettings accountSettings;
	}
}
