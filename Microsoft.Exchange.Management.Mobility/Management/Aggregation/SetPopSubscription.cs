using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Set", "PopSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPopSubscription : SetSubscriptionSendAsVerifiedBase<PopSubscriptionProxy>
	{
		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public AuthenticationMechanism IncomingAuth
		{
			get
			{
				return (AuthenticationMechanism)base.Fields["IncomingAuth"];
			}
			set
			{
				base.Fields["IncomingAuth"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public SecurityMechanism IncomingSecurity
		{
			get
			{
				return (SecurityMechanism)base.Fields["IncomingSecurity"];
			}
			set
			{
				base.Fields["IncomingSecurity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public bool LeaveOnServer
		{
			get
			{
				return (bool)base.Fields["LeaveOnServer"];
			}
			set
			{
				base.Fields["LeaveOnServer"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetPopSubscriptionConfirmation(this.Identity);
			}
		}

		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.Pop;
			}
		}

		private PopSubscriptionProxy DynamicParameters
		{
			get
			{
				return (PopSubscriptionProxy)this.GetDynamicParameters();
			}
		}

		protected override void ValidateWithDataObject(IConfigurable dataObject)
		{
			base.ValidateWithDataObject(dataObject);
			PopSubscriptionProxy popSubscriptionProxy = (PopSubscriptionProxy)dataObject;
			AuthenticationMechanism authenticationMechanism = base.Fields.IsModified("IncomingAuth") ? this.IncomingAuth : popSubscriptionProxy.IncomingAuthentication;
			SecureString password = this.password ?? popSubscriptionProxy.Subscription.LogonPasswordSecured;
			string text = base.Fields.IsModified("IncomingUserName") ? base.IncomingUserName : popSubscriptionProxy.IncomingUserName;
			AggregationTaskUtils.ValidateUserName(text, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			if (authenticationMechanism == AuthenticationMechanism.Basic)
			{
				AggregationTaskUtils.ValidateUnicodeInfoOnUserNameAndPassword(text, password, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			}
			string text2 = base.Fields.IsModified("IncomingServer") ? base.IncomingServer : popSubscriptionProxy.IncomingServer;
			AggregationTaskUtils.ValidateIncomingServerLength(text2, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			if (!base.ShouldSkipAccountValidation())
			{
				bool leaveOnServer = base.Fields.IsModified("LeaveOnServer") ? this.LeaveOnServer : popSubscriptionProxy.LeaveOnServer;
				int port = base.Fields.IsModified("IncomingPort") ? base.IncomingPort : popSubscriptionProxy.IncomingPort;
				SecurityMechanism security = base.Fields.IsModified("IncomingSecurity") ? this.IncomingSecurity : popSubscriptionProxy.IncomingSecurity;
				LocalizedException exception;
				if (!Pop3AutoProvision.ValidatePopSettings(leaveOnServer, popSubscriptionProxy.AggregationType == AggregationType.Mirrored, text2, port, text, password, authenticationMechanism, security, popSubscriptionProxy.Subscription.UserLegacyDN, CommonLoggingHelper.SyncLogSession, out exception))
				{
					base.WriteDebugInfoAndError(exception, (ErrorCategory)1003, dataObject);
				}
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.StampChangesOn(dataObject);
			base.NeedsSendAsCheck = false;
			PopSubscriptionProxy popSubscriptionProxy = dataObject as PopSubscriptionProxy;
			if (base.Fields.IsModified("EmailAddress"))
			{
				base.NeedsSendAsCheck = true;
				popSubscriptionProxy.EmailAddress = base.EmailAddress;
			}
			if (base.Fields.IsModified("IncomingUserName"))
			{
				base.NeedsSendAsCheck = true;
				popSubscriptionProxy.IncomingUserName = base.IncomingUserName;
			}
			if (base.Fields.IsModified("IncomingServer"))
			{
				base.NeedsSendAsCheck = true;
				popSubscriptionProxy.IncomingServer = base.IncomingServer;
			}
			if (base.Fields.IsModified("IncomingPort"))
			{
				popSubscriptionProxy.IncomingPort = base.IncomingPort;
			}
			if (base.Fields.IsModified("IncomingAuth"))
			{
				popSubscriptionProxy.IncomingAuthentication = this.IncomingAuth;
			}
			if (base.Fields.IsModified("IncomingSecurity"))
			{
				popSubscriptionProxy.IncomingSecurity = this.IncomingSecurity;
			}
			if (base.Fields.IsModified("LeaveOnServer"))
			{
				popSubscriptionProxy.LeaveOnServer = this.LeaveOnServer;
			}
			if (this.password != null)
			{
				popSubscriptionProxy.SetPassword(this.password);
			}
			TaskLogger.LogExit();
		}
	}
}
