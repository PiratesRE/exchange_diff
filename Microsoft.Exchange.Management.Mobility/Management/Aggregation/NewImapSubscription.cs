using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("New", "ImapSubscription", SupportsShouldProcess = true)]
	public sealed class NewImapSubscription : NewSubscriptionBase<IMAPSubscriptionProxy>
	{
		[Parameter(Mandatory = true)]
		public Fqdn IncomingServer
		{
			get
			{
				return this.DataObject.IncomingServer;
			}
			set
			{
				this.DataObject.IncomingServer = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int IncomingPort
		{
			get
			{
				return this.DataObject.IncomingPort;
			}
			set
			{
				this.DataObject.IncomingPort = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string IncomingUserName
		{
			get
			{
				return this.DataObject.IncomingUserName;
			}
			set
			{
				this.DataObject.IncomingUserName = value;
			}
		}

		[Parameter(Mandatory = true)]
		public SecureString IncomingPassword
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

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IMAPAuthenticationMechanism IncomingAuth
		{
			get
			{
				return this.DataObject.IncomingAuthentication;
			}
			set
			{
				this.DataObject.IncomingAuthentication = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IMAPSecurityMechanism IncomingSecurity
		{
			get
			{
				return this.DataObject.IncomingSecurity;
			}
			set
			{
				this.DataObject.IncomingSecurity = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.CreateIMAPSubscriptionConfirmation(this.DataObject);
			}
		}

		public static LocalizedException ValidateSubscription(IMAPSubscriptionProxy subscription, SecureString password, bool skipAccountVerification)
		{
			ICollection<ValidationError> collection = IMAPSubscriptionValidator.Validate(subscription);
			if (collection != null && collection.Count > 0)
			{
				return new LocalizedException(Strings.IMAPAccountVerificationFailedException);
			}
			if (!skipAccountVerification)
			{
				Exception ex = IMAPAutoProvision.VerifyAccount(subscription.IncomingServer.ToString(), subscription.IncomingPort, subscription.IncomingUserName, password, subscription.IncomingAuthentication, subscription.IncomingSecurity, subscription.AggregationType, CommonLoggingHelper.SyncLogSession);
				if (ex != null)
				{
					SyncPermanentException ex2 = ex as SyncPermanentException;
					if (ex2 != null && ex2.DetailedAggregationStatus == DetailedAggregationStatus.CommunicationError && ex2.InnerException is IMAPGmailNotSupportedException)
					{
						return (LocalizedException)ex2.InnerException;
					}
					return new LocalizedException(Strings.IMAPAccountVerificationFailedException);
				}
			}
			return null;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			IMAPSubscriptionProxy dataObject = this.DataObject;
			if (dataObject.IncomingAuthentication == IMAPAuthenticationMechanism.Basic)
			{
				AggregationTaskUtils.ValidateUnicodeInfoOnUserNameAndPassword(dataObject.IncomingUserName, this.password, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			}
			AggregationTaskUtils.ValidateIncomingServerLength(dataObject.IncomingServer, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			Exception ex = NewImapSubscription.ValidateSubscription(dataObject, this.password, this.Force);
			if (ex != null)
			{
				base.WriteDebugInfoAndError(ex, (ErrorCategory)1003, this.DataObject);
			}
			this.DataObject.SetPassword(this.password);
			base.WriteDebugInfo();
			TaskLogger.LogExit();
		}
	}
}
