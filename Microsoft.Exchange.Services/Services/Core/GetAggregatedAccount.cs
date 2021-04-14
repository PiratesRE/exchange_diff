using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetAggregatedAccount : AggregatedAccountCommandBase<SyncRequestStatistics[]>
	{
		public GetAggregatedAccount(CallContext callContext, GetAggregatedAccountRequest request) : base(callContext, request, ExTraceGlobals.GetAggregatedAccountTracer, typeof(GetAggregatedAccountMetadata))
		{
			this.getSyncRequests = Hookable<Func<IList<SyncRequest>>>.Create(true, new Func<IList<SyncRequest>>(this.GetSyncRequests));
			this.getSyncRequestStatistics = Hookable<Func<IList<SyncRequest>, IList<SyncRequestStatistics>>>.Create(true, new Func<IList<SyncRequest>, IList<SyncRequestStatistics>>(this.GetSyncRequestStatistics));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			AggregatedAccountType[] array = null;
			if (base.Result.Value != null)
			{
				array = new AggregatedAccountType[base.Result.Value.Length];
				int num = 0;
				foreach (SyncRequestStatistics syncRequestStatistics in from acct in base.Result.Value
				orderby (string)acct.EmailAddress
				select acct)
				{
					ConnectionSettings connectionSettings = ConnectionSettingsConverter.BuildConnectionSettings(syncRequestStatistics);
					array[num++] = new AggregatedAccountType(syncRequestStatistics.TargetExchangeGuid, syncRequestStatistics.RequestGuid, (string)syncRequestStatistics.EmailAddress, syncRequestStatistics.RemoteCredentialUsername, ConnectionSettingsConverter.BuildPublicRepresentation(connectionSettings));
				}
			}
			return new GetAggregatedAccountResponse(base.Result.Code, base.Result.Error, array);
		}

		internal override ServiceResult<SyncRequestStatistics[]> InternalExecute()
		{
			IList<SyncRequest> syncRequests = null;
			base.ExecuteWithProtocolLogging(GetAggregatedAccountMetadata.GetSyncRequestCmdletTime, delegate
			{
				syncRequests = this.getSyncRequests.Value();
			});
			if (syncRequests == null)
			{
				this.TraceError(string.Format("Could not get aggregated accounts from unified mailbox with NetId {0}.", this.netId));
				throw new CannotGetAggregatedAccountException(CoreResources.IDs.ErrorCannotGetAggregatedAccount);
			}
			IList<SyncRequestStatistics> syncRequestStatistics = null;
			base.ExecuteWithProtocolLogging(GetAggregatedAccountMetadata.GetSyncRequestStatisticsCmdletTime, delegate
			{
				syncRequestStatistics = this.getSyncRequestStatistics.Value(syncRequests);
			});
			this.TraceSuccess("Successfully retrieved aggregated accounts.");
			return new ServiceResult<SyncRequestStatistics[]>(syncRequestStatistics.ToArray<SyncRequestStatistics>());
		}

		internal override void SetLogMetadataEnumProperties()
		{
			this.verifyEnvironmentTimeEnum = GetAggregatedAccountMetadata.VerifyEnvironmentTime;
			this.verifyUserIdentityTypeTimeEnum = GetAggregatedAccountMetadata.VerifyUserIdentityTypeTime;
			this.totalTimeEnum = GetAggregatedAccountMetadata.TotalTime;
		}

		protected override string TypeName
		{
			get
			{
				return GetAggregatedAccount.GetAggregatedAccountName;
			}
		}

		private IList<SyncRequestStatistics> GetSyncRequestStatistics(IList<SyncRequest> syncRequests)
		{
			IList<SyncRequestStatistics> list = new List<SyncRequestStatistics>();
			if (syncRequests != null)
			{
				foreach (SyncRequest syncRequest in syncRequests)
				{
					using (PSLocalTask<GetSyncRequestStatistics, SyncRequestStatistics> pslocalTask = CmdletTaskFactory.Instance.CreateGetSyncRequestStatisticsTask(base.CallContext.AccessingPrincipal))
					{
						pslocalTask.CaptureAdditionalIO = true;
						pslocalTask.Task.Identity = new SyncRequestIdParameter(syncRequest);
						pslocalTask.Task.Execute();
						if (pslocalTask.Error != null)
						{
							this.TraceError(string.Format("Could not get sync request statistics for sync request {0}. Exception: {1}", syncRequest.Identity, pslocalTask.Error.Exception));
							throw new CannotGetAggregatedAccountException(CoreResources.IDs.ErrorCannotGetAggregatedAccount, pslocalTask.Error.Exception);
						}
						if (pslocalTask.Result == null)
						{
							this.TraceError(string.Format("Got NULL sync request statistics for sync request {0}.", syncRequest.Identity));
							base.CallContext.ProtocolLog.Set(GetAggregatedAccountMetadata.GetSyncRequestStatisticsCmdletError, string.Format("Got NULL sync request statistics for sync request {0}.", syncRequest.Identity));
							throw new CannotGetAggregatedAccountException(CoreResources.IDs.ErrorCannotGetAggregatedAccount);
						}
						if (!pslocalTask.Result.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox))
						{
							this.TraceError(string.Format("Got a sync request statistics for a non-aggregated account. Sync request {0}. RequestFlags {1}", syncRequest.Identity, pslocalTask.Result.Flags));
							base.CallContext.ProtocolLog.Set(GetAggregatedAccountMetadata.GetSyncRequestStatisticsCmdletNonAggregatedAccountError, string.Format("Got a sync request statistics for a non-aggregated account. Sync request {0}. RequestFlags {1}", syncRequest.Identity, pslocalTask.Result.Flags));
							throw new CannotGetAggregatedAccountException(CoreResources.IDs.ErrorFoundSyncRequestForNonAggregatedAccount);
						}
						list.Add(pslocalTask.Result);
					}
				}
			}
			this.TraceSuccess("Successfully retrieved sync request statistics");
			return list;
		}

		private IList<SyncRequest> GetSyncRequests()
		{
			IList<SyncRequest> allResults;
			using (PSLocalTask<GetSyncRequest, SyncRequest> pslocalTask = CmdletTaskFactory.Instance.CreateGetSyncRequestTask(base.CallContext.AccessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Mailbox = new MailboxOrMailUserIdParameter(base.CallContext.AccessingADUser.ObjectId);
				pslocalTask.Task.Organization = new OrganizationIdParameter(base.CallContext.AccessingADUser.OrganizationId);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null || pslocalTask.AllResults == null)
				{
					this.TraceError(string.Format("Could not get aggregated accounts from unified mailbox with NetId {0}. Exception: {1}", this.netId, pslocalTask.Error.Exception));
					throw new CannotGetAggregatedAccountException(CoreResources.IDs.ErrorCannotGetAggregatedAccount, pslocalTask.Error.Exception);
				}
				this.TraceSuccess("Successfully retrieved sync requests");
				allResults = pslocalTask.AllResults;
			}
			return allResults;
		}

		private static readonly string GetAggregatedAccountName = "GetAggregatedAccount";

		internal readonly Hookable<Func<IList<SyncRequest>>> getSyncRequests;

		internal readonly Hookable<Func<IList<SyncRequest>, IList<SyncRequestStatistics>>> getSyncRequestStatistics;
	}
}
