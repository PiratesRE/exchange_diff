using System;
using System.Management.Automation;
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
	public abstract class RemoveSubscriptionBase<TIdentity> : RemoveTenantADTaskBase<AggregationSubscriptionIdParameter, TIdentity> where TIdentity : IConfigurable, new()
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.RemoveSubscriptionConfirmation(this.Identity);
			}
		}

		protected abstract AggregationType AggregationType { get; }

		protected virtual MailboxIdParameter GetMailboxIdParameter()
		{
			MailboxIdParameter result;
			if (this.Identity != null && this.Identity.MailboxIdParameter != null)
			{
				result = this.Identity.MailboxIdParameter;
			}
			else
			{
				ADObjectId adObjectId;
				if (!base.TryGetExecutingUserId(out adObjectId))
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				result = new MailboxIdParameter(adObjectId);
			}
			return result;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 82, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\RemoveSubscriptionBase.cs");
			MailboxIdParameter mailboxIdParameter = this.GetMailboxIdParameter();
			ADUser adUser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(mailboxIdParameter.ToString())));
			IRecipientSession session = AggregationTaskUtils.VerifyIsWithinWriteScopes(tenantOrRootOrgRecipientSession, adUser, new Task.TaskErrorLoggingDelegate(this.WriteDebugInfoAndError));
			AggregationSubscriptionDataProvider result = null;
			try
			{
				AggregationType aggregationType = this.AggregationType;
				if (this.Identity != null && this.Identity.AggregationType != null)
				{
					aggregationType = this.Identity.AggregationType.Value;
				}
				result = SubscriptionConfigDataProviderFactory.Instance.CreateSubscriptionDataProvider(aggregationType, AggregationTaskType.Remove, session, adUser);
			}
			catch (MailboxFailureException exception)
			{
				this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, mailboxIdParameter);
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				base.DataObject
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

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception);
		}

		private void WriteDebugInfoAndError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteDebugInfo();
			base.WriteError(exception, category, target);
		}

		private void WriteDebugInfo()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(CommonLoggingHelper.SyncLogSession.GetBlackBoxText());
			}
			CommonLoggingHelper.SyncLogSession.ClearBlackBox();
		}
	}
}
