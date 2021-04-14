using System;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class AggregatedAccountCommandBase<SingleItemType> : SingleStepServiceCommand<BaseAggregatedAccountRequest, SingleItemType>
	{
		internal Hookable<Func<SmtpAddress, Exception, SyncRequestStatistics>> GetSyncRequestStatisticsForAggregatedAccountGetter
		{
			get
			{
				if (this.getSyncRequestStatisticsForAggregatedAccount == null)
				{
					this.getSyncRequestStatisticsForAggregatedAccount = Hookable<Func<SmtpAddress, Exception, SyncRequestStatistics>>.Create(true, (SmtpAddress smtpAddress, Exception exception) => this.GetSyncRequestStatisticsForAggregatedAccount(smtpAddress, out exception));
				}
				return this.getSyncRequestStatisticsForAggregatedAccount;
			}
		}

		protected AggregatedAccountCommandBase(CallContext callContext, BaseAggregatedAccountRequest request, Microsoft.Exchange.Diagnostics.Trace tracer, Type logMetadataEnumType) : base(callContext2, request)
		{
			this.tracer = tracer;
			ServiceCommandBase.ThrowIfNull(this.tracer, "tracer", "AggregatedAccountCommandBase::ctor");
			this.SetLogMetadata(logMetadataEnumType);
			this.netId = AggregatedAccountCommandBase<SingleItemType>.getNetIdFromCallContext.Value(base.CallContext);
			ServiceCommandBase.ThrowIfNull(this.netId, "NetId", "AggregatedAccountCommandBase::ctor");
			this.getOrganizationId = Hookable<Func<OrganizationId>>.Create(true, new Func<OrganizationId>(this.GetOrganizationId));
			this.organizationId = new Lazy<OrganizationId>(this.getOrganizationId.Value);
			this.organizationId_OrganizationalUnit_Name = Hookable<Func<string>>.Create(true, () => this.organizationId.Value.OrganizationalUnit.Name);
			this.callContextAccessingADUser = Hookable<Func<ADUser>>.Create(true, () => base.CallContext.AccessingADUser);
			this.getTenantResellerId = Hookable<Func<string>>.Create(true, new Func<string>(this.GetTenantResellerId));
		}

		internal override ServiceResult<SingleItemType> Execute()
		{
			ServiceResult<SingleItemType> result = null;
			this.ExecuteWithProtocolLogging(this.totalTimeEnum, delegate
			{
				this.ExecuteWithProtocolLogging(this.verifyEnvironmentTimeEnum, new Action(this.VerifyEnvironment));
				this.ExecuteWithProtocolLogging(this.verifyUserIdentityTypeTimeEnum, new Action(this.VerifyUserIdentityType));
				result = this.InternalExecute();
			});
			return result;
		}

		internal abstract ServiceResult<SingleItemType> InternalExecute();

		internal abstract void SetLogMetadataEnumProperties();

		protected abstract string TypeName { get; }

		protected virtual OrganizationId GetOrganizationId()
		{
			return base.CallContext.AccessingPrincipal.MailboxInfo.OrganizationId;
		}

		protected virtual void VerifyUserIdentityType()
		{
			if (base.CallContext.IsMSAUser || base.CallContext.EffectiveCaller == null || this.callContextAccessingADUser.Value() == null)
			{
				this.TraceError(string.Format("Can't find unified mailbox with NetId {0}.", this.netId));
				throw new CannotFindUnifiedMailboxException(CoreResources.IDs.ErrorCannotFindUnifiedMailbox);
			}
			this.VerifyOpenTenant();
		}

		protected virtual void TraceError(string message)
		{
			this.InternalTraceError(message, (base.CallContext.AccessingPrincipal != null) ? base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString() : null, (base.CallContext.AccessingPrincipal != null) ? base.CallContext.EffectiveCallerSid : null);
		}

		private void SetLogMetadata(Type logMetadataEnumType)
		{
			OwsLogRegistry.Register(this.TypeName, logMetadataEnumType, new Type[0]);
			this.SetLogMetadataEnumProperties();
		}

		protected virtual void TraceSuccess(string message)
		{
			this.InternalTraceSuccess(message, (base.CallContext.AccessingPrincipal != null) ? base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString() : null, (base.CallContext.AccessingPrincipal != null) ? base.CallContext.EffectiveCallerSid : null);
		}

		protected void ExecuteWithProtocolLogging(Enum logMetadata, Action operation)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			operation();
			stopwatch.Stop();
			this.requestDetailsLogger_Set.Value(base.CallContext.ProtocolLog, logMetadata, stopwatch.ElapsedMilliseconds);
		}

		protected SyncRequestStatistics GetSyncRequestStatisticsForAggregatedAccount(SmtpAddress emailAddress, out Exception getSyncRequestStatisticsTaskException)
		{
			getSyncRequestStatisticsTaskException = null;
			SyncRequestStatistics result = null;
			using (PSLocalTask<GetSyncRequestStatistics, SyncRequestStatistics> pslocalTask = CmdletTaskFactory.Instance.CreateGetSyncRequestStatisticsTask(base.CallContext.AccessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Identity = SyncRequestIdParameter.Create(base.CallContext.AccessingADUser, (string)emailAddress);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					this.TraceError(string.Format("Could not get aggregated accounts from unified mailbox with NetId {0}. Exception: {1}", this.netId, pslocalTask.Error.Exception));
					getSyncRequestStatisticsTaskException = pslocalTask.Error.Exception;
				}
				result = pslocalTask.Result;
			}
			return result;
		}

		protected void VerifyOpenTenant()
		{
			if (ExEnvironment.IsSdfDomain && Guid.Parse(this.organizationId.Value.ToExternalDirectoryOrganizationId()) == Constants.ConsumerTenantGuid)
			{
				this.TraceSuccess("Verified open tenant with hard-coded GUID in SDF environment.");
				return;
			}
			string text = this.getTenantResellerId.Value();
			if (!StringComparer.OrdinalIgnoreCase.Equals(text, "MSOnline.BPOS_Unmanaged_Hydrated"))
			{
				this.TraceError(string.Format("{0} service command is not supported for tenants with reseller Id {1}", this.TypeName, text));
				throw new UnifiedMailboxSupportedOnlyWithMicrosoftAccountException(CoreResources.IDs.ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount);
			}
		}

		internal static void AssignIfParameterSet(string parameter, Action<string> assignValue)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				assignValue(parameter);
			}
		}

		internal void InternalTraceError(string message, string mailbox, SecurityIdentifier callerSid)
		{
			this.tracer.TraceError((long)this.GetHashCode(), "{0}: {1} Mailbox: {2}, AccessType: {3}, AccessingAs SID: {4}", new object[]
			{
				this.TypeName,
				message,
				mailbox,
				base.CallContext.MailboxAccessType,
				callerSid
			});
		}

		internal void InternalTraceSuccess(string message, string mailbox, SecurityIdentifier callerSid)
		{
			this.tracer.TraceInformation(this.GetHashCode(), 0L, "{0}: {1} Mailbox: {2}, AccessType: {3}, AccessingAs SID: {4}", new object[]
			{
				this.TypeName,
				message,
				mailbox,
				base.CallContext.MailboxAccessType,
				callerSid
			});
		}

		private void VerifyEnvironment()
		{
			if (!this.isDatacenter.Value())
			{
				this.TraceError("Called in a non-datacenter environment.");
				throw new NotApplicableOutsideOfDatacenterException(CoreResources.IDs.ErrorNotApplicableOutsideOfDatacenter);
			}
		}

		private string GetTenantResellerId()
		{
			string name = this.organizationId.Value.OrganizationalUnit.Name;
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(name), 409, "GetTenantResellerId", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\AggregatedAccountCommandBase.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnitByNameOrAcceptedDomain = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(name);
			return exchangeConfigurationUnitByNameOrAcceptedDomain.ResellerId;
		}

		internal const string OpenTenantResellerId = "MSOnline.BPOS_Unmanaged_Hydrated";

		internal readonly Lazy<OrganizationId> organizationId;

		internal readonly Hookable<Func<string>> organizationId_OrganizationalUnit_Name;

		protected readonly string netId;

		protected Microsoft.Exchange.Diagnostics.Trace tracer;

		protected Enum verifyEnvironmentTimeEnum;

		protected Enum verifyUserIdentityTypeTimeEnum;

		protected Enum totalTimeEnum;

		internal readonly Hookable<Func<bool>> isDatacenter = Hookable<Func<bool>>.Create(true, () => VariantConfiguration.InvariantNoFlightingSnapshot.Ews.CreateUnifiedMailbox.Enabled);

		internal readonly Hookable<Func<OrganizationId>> getOrganizationId;

		internal static Hookable<Func<CallContext, string>> getNetIdFromCallContext = Hookable<Func<CallContext, string>>.Create(true, (CallContext callContext) => callContext.EffectiveCallerNetId);

		internal readonly Hookable<Func<RequestDetailsLogger, Enum, object, string>> requestDetailsLogger_Set = Hookable<Func<RequestDetailsLogger, Enum, object, string>>.Create(true, (RequestDetailsLogger protocolLog, Enum property, object value) => protocolLog.Set(property, value));

		internal readonly Hookable<Func<ADUser>> callContextAccessingADUser;

		internal readonly Hookable<Func<string>> getTenantResellerId;

		internal readonly Hookable<Func<CallContext, MailboxSession>> getMailboxIdentityMailboxSession = Hookable<Func<CallContext, MailboxSession>>.Create(true, (CallContext callContext) => callContext.SessionCache.GetMailboxIdentityMailboxSession(false));

		internal readonly Hookable<Func<MailboxSession, ADUser, AggregatedAccountHelper>> createAggregatedAccountHelper = Hookable<Func<MailboxSession, ADUser, AggregatedAccountHelper>>.Create(true, (MailboxSession session, ADUser adUser) => new AggregatedAccountHelper(session, adUser));

		private Hookable<Func<SmtpAddress, Exception, SyncRequestStatistics>> getSyncRequestStatisticsForAggregatedAccount;
	}
}
