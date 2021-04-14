using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Set", "RemoteAccountSyncCache", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetRemoteAccountSyncCache : SetTenantADTaskBase<CacheIdParameter, SubscriptionsCache, SubscriptionsCache>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetCacheMessageConfirmation(this.Identity);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings sessionSettings = base.SessionSettings;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.IgnoreInvalid, sessionSettings, 58, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\SetRemoteAccountSyncCache.cs");
			string idStringValue = this.Identity.ToString();
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Identity.MailboxId, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(idStringValue)), new LocalizedString?(Strings.ErrorUserNotUnique(idStringValue)));
			IRecipientSession recipientSession = AggregationTaskUtils.VerifyIsWithinWriteScopes(tenantOrRootOrgRecipientSession, aduser, new Task.TaskErrorLoggingDelegate(this.WriteDebugInfoAndError));
			try
			{
				this.userPrincipal = ExchangePrincipal.FromLegacyDN(recipientSession.SessionSettings, aduser.LegacyExchangeDN, RemotingOptions.AllowCrossSite);
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, this.Identity.MailboxId);
			}
			return new CacheDataProvider(SubscriptionCacheAction.Fix, this.userPrincipal);
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

		private ExchangePrincipal userPrincipal;
	}
}
