using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("New", "PopSubscription", SupportsShouldProcess = true)]
	public sealed class NewPopSubscription : NewSubscriptionBase<PopSubscriptionProxy>
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
		public AuthenticationMechanism IncomingAuth
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
		public SecurityMechanism IncomingSecurity
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

		[Parameter(Mandatory = false)]
		public bool LeaveOnServer
		{
			get
			{
				return this.DataObject.LeaveOnServer;
			}
			set
			{
				this.DataObject.LeaveOnServer = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.CreatePopSubscriptionConfirmation(this.DataObject);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			PopSubscriptionProxy dataObject = this.DataObject;
			if (dataObject.IncomingAuthentication == AuthenticationMechanism.Basic)
			{
				AggregationTaskUtils.ValidateUnicodeInfoOnUserNameAndPassword(dataObject.IncomingUserName, this.password, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			}
			AggregationTaskUtils.ValidateIncomingServerLength(dataObject.IncomingServer, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			base.WriteDebugInfo();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			PopSubscriptionProxy popSubscriptionProxy = (PopSubscriptionProxy)base.PrepareDataObject();
			if (this.Force == false)
			{
				AggregationSubscriptionDataProvider aggregationSubscriptionDataProvider = (AggregationSubscriptionDataProvider)base.DataSession;
				if (base.Mailbox == null)
				{
					ADObjectId adobjectId;
					if (!base.TryGetExecutingUserId(out adobjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
				}
				else
				{
					ADObjectId internalADObjectId = base.Mailbox.InternalADObjectId;
				}
				LocalizedException exception;
				if (!Pop3AutoProvision.ValidatePopSettings(popSubscriptionProxy.LeaveOnServer, popSubscriptionProxy.AggregationType == AggregationType.Mirrored, popSubscriptionProxy.IncomingServer, popSubscriptionProxy.IncomingPort, popSubscriptionProxy.IncomingUserName, this.password, popSubscriptionProxy.IncomingAuthentication, popSubscriptionProxy.IncomingSecurity, aggregationSubscriptionDataProvider.UserLegacyDN, CommonLoggingHelper.SyncLogSession, out exception))
				{
					base.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, this.DataObject);
				}
			}
			popSubscriptionProxy.SetPassword(this.password);
			base.WriteDebugInfo();
			return popSubscriptionProxy;
		}
	}
}
