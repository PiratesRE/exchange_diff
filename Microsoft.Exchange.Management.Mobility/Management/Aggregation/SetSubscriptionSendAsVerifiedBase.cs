using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	public abstract class SetSubscriptionSendAsVerifiedBase<TSubscription> : SetSubscriptionBase<TSubscription> where TSubscription : IConfigurable, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "DisableSubscriptionAsPoison", ValueFromPipeline = true)]
		[Parameter(Mandatory = true, ParameterSetName = "SubscriptionModification", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ResendVerificationEmail", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ValidateSendAs", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override AggregationSubscriptionIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "ResendVerificationEmail", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "ValidateSendAs", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "DisableSubscriptionAsPoison", ValueFromPipeline = true)]
		public override MailboxIdParameter Mailbox
		{
			get
			{
				return base.Mailbox;
			}
			set
			{
				base.Mailbox = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public SmtpAddress EmailAddress
		{
			get
			{
				return (SmtpAddress)base.Fields["EmailAddress"];
			}
			set
			{
				base.Fields["EmailAddress"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public string IncomingUserName
		{
			get
			{
				return (string)base.Fields["IncomingUserName"];
			}
			set
			{
				base.Fields["IncomingUserName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public Fqdn IncomingServer
		{
			get
			{
				return (Fqdn)base.Fields["IncomingServer"];
			}
			set
			{
				base.Fields["IncomingServer"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public int IncomingPort
		{
			get
			{
				return (int)base.Fields["IncomingPort"];
			}
			set
			{
				base.Fields["IncomingPort"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
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

		[Parameter(Mandatory = false, ParameterSetName = "ValidateSendAs")]
		public string ValidateSecret
		{
			get
			{
				return (string)base.Fields["ValidateSecret"];
			}
			set
			{
				base.Fields["ValidateSecret"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ResendVerificationEmail")]
		public SwitchParameter ResendVerification
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResendVerification"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ResendVerification"] = value;
			}
		}

		protected bool NeedsSendAsCheck
		{
			get
			{
				return this.sendAsCheckNeeded;
			}
			set
			{
				this.sendAsCheckNeeded = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			if (this.ValidateSecret == null && this.ResendVerification == false)
			{
				return configurable;
			}
			PimSubscriptionProxy pimSubscriptionProxy = (PimSubscriptionProxy)configurable;
			AggregationSubscriptionDataProvider dataProvider = (AggregationSubscriptionDataProvider)base.DataSession;
			AggregationTaskUtils.ProcessSendAsSpecificParameters(pimSubscriptionProxy, this.ValidateSecret, this.ResendVerification, dataProvider, new Task.TaskErrorLoggingDelegate(base.WriteDebugInfoAndError));
			base.WriteDebugInfo();
			return pimSubscriptionProxy;
		}

		protected override bool SendAsCheckNeeded()
		{
			return this.NeedsSendAsCheck;
		}

		protected bool ShouldSkipAccountValidation()
		{
			return this.Force || this.ValidateSecret != null || this.ResendVerification || !base.Enabled || base.DisableAsPoison;
		}

		private bool sendAsCheckNeeded;
	}
}
