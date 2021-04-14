using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Set", "ImapSubscription", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetImapSubscription : SetSubscriptionSendAsVerifiedBase<IMAPSubscriptionProxy>
	{
		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public IMAPAuthenticationMechanism IncomingAuth
		{
			get
			{
				return (IMAPAuthenticationMechanism)base.Fields["IncomingAuth"];
			}
			set
			{
				base.Fields["IncomingAuth"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public IMAPSecurityMechanism IncomingSecurity
		{
			get
			{
				return (IMAPSecurityMechanism)base.Fields["IncomingSecurity"];
			}
			set
			{
				base.Fields["IncomingSecurity"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetImapSubscriptionConfirmation(this.Identity);
			}
		}

		protected override AggregationSubscriptionType IdentityType
		{
			get
			{
				return AggregationSubscriptionType.IMAP;
			}
		}

		private IMAPSubscriptionProxy DynamicParameters
		{
			get
			{
				return (IMAPSubscriptionProxy)this.GetDynamicParameters();
			}
		}

		protected override void ValidateWithDataObject(IConfigurable dataObject)
		{
			base.ValidateWithDataObject(dataObject);
			IMAPSubscriptionProxy imapsubscriptionProxy = dataObject as IMAPSubscriptionProxy;
			if (imapsubscriptionProxy == null)
			{
				throw new InvalidCastException("Expected dataObject of IMAPSubscriptionProxy type");
			}
			IMAPSubscriptionProxy imapsubscriptionProxy2 = IMAPSubscriptionProxy.ShallowCopy(imapsubscriptionProxy);
			if (base.Fields.IsModified("EmailAddress"))
			{
				imapsubscriptionProxy2.EmailAddress = base.EmailAddress;
			}
			if (base.Fields.IsModified("IncomingUserName"))
			{
				imapsubscriptionProxy2.IncomingUserName = base.IncomingUserName;
			}
			if (base.Fields.IsModified("IncomingServer"))
			{
				imapsubscriptionProxy2.IncomingServer = base.IncomingServer;
			}
			if (base.Fields.IsModified("IncomingPort"))
			{
				imapsubscriptionProxy2.IncomingPort = base.IncomingPort;
			}
			if (base.Fields.IsModified("IncomingAuth"))
			{
				imapsubscriptionProxy2.IncomingAuthentication = this.IncomingAuth;
			}
			if (base.Fields.IsModified("IncomingSecurity"))
			{
				imapsubscriptionProxy2.IncomingSecurity = this.IncomingSecurity;
			}
			AggregationTaskUtils.ValidateUserName(imapsubscriptionProxy2.IncomingUserName, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			SecureString password = this.password ?? imapsubscriptionProxy.Subscription.LogonPasswordSecured;
			if (imapsubscriptionProxy2.IncomingAuthentication == IMAPAuthenticationMechanism.Basic)
			{
				AggregationTaskUtils.ValidateUnicodeInfoOnUserNameAndPassword(imapsubscriptionProxy2.IncomingUserName, password, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			}
			AggregationTaskUtils.ValidateIncomingServerLength(imapsubscriptionProxy2.IncomingServer, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			bool skipAccountVerification = base.ShouldSkipAccountValidation();
			Exception ex = NewImapSubscription.ValidateSubscription(imapsubscriptionProxy2, password, skipAccountVerification);
			if (ex != null)
			{
				base.WriteDebugInfoAndError(ex, (ErrorCategory)1003, imapsubscriptionProxy2);
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.StampChangesOn(dataObject);
			base.NeedsSendAsCheck = false;
			IMAPSubscriptionProxy imapsubscriptionProxy = dataObject as IMAPSubscriptionProxy;
			if (base.Fields.IsModified("EmailAddress"))
			{
				base.NeedsSendAsCheck = true;
				imapsubscriptionProxy.EmailAddress = base.EmailAddress;
			}
			if (base.Fields.IsModified("IncomingUserName"))
			{
				base.NeedsSendAsCheck = true;
				imapsubscriptionProxy.IncomingUserName = base.IncomingUserName;
			}
			if (base.Fields.IsModified("IncomingServer"))
			{
				base.NeedsSendAsCheck = true;
				imapsubscriptionProxy.IncomingServer = base.IncomingServer;
			}
			if (base.Fields.IsModified("IncomingPort"))
			{
				imapsubscriptionProxy.IncomingPort = base.IncomingPort;
			}
			if (base.Fields.IsModified("IncomingAuth"))
			{
				imapsubscriptionProxy.IncomingAuthentication = this.IncomingAuth;
			}
			if (base.Fields.IsModified("IncomingSecurity"))
			{
				imapsubscriptionProxy.IncomingSecurity = this.IncomingSecurity;
			}
			if (this.password != null)
			{
				imapsubscriptionProxy.SetPassword(this.password);
			}
			TaskLogger.LogExit();
		}
	}
}
