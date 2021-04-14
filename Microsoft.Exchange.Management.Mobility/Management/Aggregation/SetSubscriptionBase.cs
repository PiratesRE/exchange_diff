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
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	public abstract class SetSubscriptionBase<TSubscription> : SetTenantADTaskBase<AggregationSubscriptionIdParameter, TSubscription, TSubscription> where TSubscription : IConfigurable, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "DisableSubscriptionAsPoison", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "SubscriptionModification", ValueFromPipeline = true)]
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

		[Parameter(Mandatory = false, ParameterSetName = "DisableSubscriptionAsPoison", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true)]
		public virtual MailboxIdParameter Mailbox
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

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public SwitchParameter EnablePoisonSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnablePoisonSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EnablePoisonSubscription"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SubscriptionModification")]
		public bool Enabled
		{
			get
			{
				return (bool)(base.Fields["Enabled"] ?? true);
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DisableSubscriptionAsPoison")]
		public SwitchParameter DisableAsPoison
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableAsPoison"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DisableAsPoison"] = value;
			}
		}

		protected abstract AggregationSubscriptionType IdentityType { get; }

		protected virtual AggregationType AggregationType
		{
			get
			{
				return AggregationType.Aggregation;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			base.InternalStateReset();
			this.Identity.SubscriptionType = new AggregationSubscriptionType?(this.IdentityType);
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 149, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\SetSubscriptionBase.cs");
			if (this.Mailbox == null)
			{
				if (this.Identity != null && this.Identity.MailboxIdParameter != null)
				{
					this.Mailbox = this.Identity.MailboxIdParameter;
				}
				else
				{
					ADObjectId adObjectId;
					if (!base.TryGetExecutingUserId(out adObjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					this.Mailbox = new MailboxIdParameter(adObjectId);
				}
			}
			ADUser adUser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			IRecipientSession session = AggregationTaskUtils.VerifyIsWithinWriteScopes(tenantOrRootOrgRecipientSession, adUser, new Task.TaskErrorLoggingDelegate(this.WriteDebugInfoAndError));
			AggregationSubscriptionDataProvider result = null;
			try
			{
				result = SubscriptionConfigDataProviderFactory.Instance.CreateSubscriptionDataProvider(this.AggregationType, AggregationTaskType.Set, session, adUser);
			}
			catch (MailboxFailureException exception)
			{
				this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, this.Mailbox);
			}
			return result;
		}

		protected virtual void ValidateWithDataObject(IConfigurable dataObject)
		{
			PimSubscriptionProxy pimSubscriptionProxy = (PimSubscriptionProxy)dataObject;
			if (this.EnablePoisonSubscription && pimSubscriptionProxy.Status != AggregationStatus.Poisonous)
			{
				this.WriteDebugInfoAndError(new LocalizedException(Strings.SubscriptionCannotBeEnabled), ErrorCategory.InvalidArgument, null);
			}
			if ((pimSubscriptionProxy.Status == AggregationStatus.Poisonous && !this.EnablePoisonSubscription) || pimSubscriptionProxy.Status == AggregationStatus.InvalidVersion)
			{
				this.WriteDebugInfoAndError(new LocalizedException(Strings.SubscriptionCannotBeChanged), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable configurable = this.ResolveDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			this.ValidateWithDataObject(configurable);
			this.StampChangesOn(configurable);
			if (base.HasErrors)
			{
				return null;
			}
			PimSubscriptionProxy pimSubscriptionProxy = (PimSubscriptionProxy)configurable;
			pimSubscriptionProxy.ObjectState = ObjectState.Changed;
			SendAsManager sendAsManager = new SendAsManager();
			sendAsManager.ResetVerificationEmailData(pimSubscriptionProxy.Subscription);
			pimSubscriptionProxy.SendAsCheckNeeded = this.SendAsCheckNeeded();
			TaskLogger.LogExit();
			return pimSubscriptionProxy;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			PimSubscriptionProxy pimSubscriptionProxy = (PimSubscriptionProxy)dataObject;
			if (base.Fields.IsModified("DisplayName"))
			{
				pimSubscriptionProxy.DisplayName = this.DisplayName;
			}
			SubscriptionStateTransitionHelper subscriptionStateTransitionHelper = new SubscriptionStateTransitionHelper(pimSubscriptionProxy.Subscription);
			if (this.DisableAsPoison)
			{
				subscriptionStateTransitionHelper.DisableAsPoisonous();
			}
			else if (this.EnablePoisonSubscription)
			{
				if (this.Enabled)
				{
					subscriptionStateTransitionHelper.EnableFromPoison();
				}
				else
				{
					subscriptionStateTransitionHelper.Disable();
				}
			}
			else if (this.Enabled)
			{
				subscriptionStateTransitionHelper.Enable();
			}
			else
			{
				subscriptionStateTransitionHelper.Disable();
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is SubscriptionInconsistentException;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.password != null)
			{
				if (this.password.Length <= 0)
				{
					this.WriteDebugInfoAndError(new SubscriptionPasswordEmptyException(), ErrorCategory.InvalidArgument, null);
				}
				ValidationError validationError = AggregationSubscriptionConstraints.PasswordRangeConstraint.Validate(this.password.Length, new SubscriptionProxyPropertyDefinition("password", typeof(int)), null);
				if (validationError != null)
				{
					this.WriteDebugInfoAndError(new LocalizedException(Strings.SubscriptionPasswordTooLong(this.password.Length, this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			Exception ex = null;
			try
			{
				base.InternalProcessRecord();
			}
			catch (LocalizedException ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (ex != null)
				{
					this.WriteDebugInfoAndError(ex, ErrorCategory.InvalidArgument, this.Mailbox);
				}
				else
				{
					this.WriteDebugInfo();
				}
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

		protected virtual bool SendAsCheckNeeded()
		{
			return true;
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

		protected SecureString password;
	}
}
