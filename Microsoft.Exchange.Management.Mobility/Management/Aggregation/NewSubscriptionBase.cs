using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	public class NewSubscriptionBase<TSubscription> : NewTenantADTaskBase<TSubscription> where TSubscription : IConfigurable, new()
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public string Name
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

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields["DisplayName"];
			}
			set
			{
				base.Fields["DisplayName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public SmtpAddress EmailAddress
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

		protected virtual AggregationType AggregationType
		{
			get
			{
				return AggregationType.Aggregation;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 97, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\NewSubscriptionBase.cs");
			if (this.Mailbox == null)
			{
				ADObjectId adObjectId;
				if (!base.TryGetExecutingUserId(out adObjectId))
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				this.Mailbox = new MailboxIdParameter(adObjectId);
			}
			ADUser adUser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			IRecipientSession session = AggregationTaskUtils.VerifyIsWithinWriteScopes(tenantOrRootOrgRecipientSession, adUser, new Task.TaskErrorLoggingDelegate(this.WriteDebugInfoAndError));
			AggregationSubscriptionDataProvider result = null;
			try
			{
				result = SubscriptionConfigDataProviderFactory.Instance.CreateSubscriptionDataProvider(this.AggregationType, AggregationTaskType.New, session, adUser);
			}
			catch (MailboxFailureException exception)
			{
				this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, this.Mailbox);
			}
			return result;
		}

		protected override IConfigurable PrepareDataObject()
		{
			PimSubscriptionProxy pimSubscriptionProxy = (PimSubscriptionProxy)base.PrepareDataObject();
			this.EnsureUserDisplayName();
			pimSubscriptionProxy.Name = this.Name;
			pimSubscriptionProxy.DisplayName = this.DisplayName;
			pimSubscriptionProxy.EmailAddress = this.EmailAddress;
			pimSubscriptionProxy.AggregationType = this.AggregationType;
			pimSubscriptionProxy.SendAsCheckNeeded = true;
			return pimSubscriptionProxy;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalBeginProcessing();
				if (this.password != null)
				{
					if (this.password.Length <= 0)
					{
						this.WriteDebugInfoAndError(new SubscriptionPasswordEmptyException(), ErrorCategory.InvalidArgument, null);
					}
					ValidationError validationError = AggregationSubscriptionConstraints.PasswordRangeConstraint.Validate(this.password.Length, new SubscriptionProxyPropertyDefinition("Password", typeof(int)), null);
					if (validationError != null)
					{
						this.WriteDebugInfoAndError(new LocalizedException(Strings.SubscriptionPasswordTooLong(this.password.Length, this.Name)), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			finally
			{
				this.WriteDebugInfo();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				base.InternalProcessRecord();
			}
			finally
			{
				this.WriteDebugInfo();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			AggregationTaskUtils.ValidateEmailAddress(base.DataSession, this.DataObject as PimSubscriptionProxy, new Task.TaskErrorLoggingDelegate(this.WriteDebugInfoAndError));
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is FailedCreateAggregationSubscriptionException || exception is FailedDeleteAggregationSubscriptionException || exception is RedundantPimSubscriptionException || exception is RedundantAccountSubscriptionException || exception is SubscriptionInconsistentException || exception is SubscriptionNameAlreadyExistsException || exception is SubscriptionNumberExceedLimitException;
		}

		protected void WriteDebugInfoAndError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteDebugInfo();
			base.WriteError(exception, category, target);
		}

		protected void WriteDebugInfo()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(CommonLoggingHelper.SyncLogSession.GetBlackBoxText());
			}
			CommonLoggingHelper.SyncLogSession.ClearBlackBox();
		}

		private void EnsureUserDisplayName()
		{
			if (string.IsNullOrEmpty(this.DisplayName))
			{
				string text = this.EmailAddress.ToString();
				if (text.Length >= 256)
				{
					this.DisplayName = text.Substring(0, 255);
					return;
				}
				this.DisplayName = text;
			}
		}

		protected SecureString password;
	}
}
