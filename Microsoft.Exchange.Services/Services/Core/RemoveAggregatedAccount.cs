using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
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
	internal sealed class RemoveAggregatedAccount : AggregatedAccountCommandBase<ServiceResultNone>
	{
		public RemoveAggregatedAccount(CallContext callContext, RemoveAggregatedAccountRequest request) : base(callContext, request, ExTraceGlobals.RemoveAggregatedAccountTracer, typeof(RemoveAggregatedAccountMetadata))
		{
			this.EmailAddress = SmtpAddress.Parse(request.EmailAddress);
			this.getAggregatedAccountMailboxGuidFromSyncRequestStatistics = Hookable<Func<Guid>>.Create(true, new Func<Guid>(this.GetAggregatedAccountMailboxGuidFromSyncRequestStatistics));
			this.removeAggregatedAccountToADUser = Hookable<Action>.Create(true, new Action(this.RemoveAggregatedAccountFromADUser));
			this.removeSyncRequest = Hookable<Action>.Create(true, new Action(this.RemoveSyncRequest));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new RemoveAggregatedAccountResponse(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<ServiceResultNone> InternalExecute()
		{
			base.ExecuteWithProtocolLogging(RemoveAggregatedAccountMetadata.GetAggregatedMailboxGuidFromSyncRequestStatisticsTime, delegate
			{
				this.MailboxGuid = this.getAggregatedAccountMailboxGuidFromSyncRequestStatistics.Value();
			});
			base.ExecuteWithProtocolLogging(RemoveAggregatedAccountMetadata.RemoveSyncRequestCmdletTime, delegate
			{
				this.removeSyncRequest.Value();
			});
			base.ExecuteWithProtocolLogging(RemoveAggregatedAccountMetadata.RemoveAggregatedMailboxGuidFromADUserTime, delegate
			{
				this.removeAggregatedAccountToADUser.Value();
			});
			this.TraceSuccess("Successfully removed aggregated account.");
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		internal override void SetLogMetadataEnumProperties()
		{
			this.verifyEnvironmentTimeEnum = RemoveAggregatedAccountMetadata.VerifyEnvironmentTime;
			this.verifyUserIdentityTypeTimeEnum = RemoveAggregatedAccountMetadata.VerifyUserIdentityTypeTime;
			this.totalTimeEnum = RemoveAggregatedAccountMetadata.TotalTime;
		}

		protected override string TypeName
		{
			get
			{
				return RemoveAggregatedAccount.RemoveAggregatedAccountName;
			}
		}

		private Guid GetAggregatedAccountMailboxGuidFromSyncRequestStatistics()
		{
			Exception ex = null;
			SyncRequestStatistics syncRequestStatistics = base.GetSyncRequestStatisticsForAggregatedAccountGetter.Value(this.EmailAddress, ex);
			if (syncRequestStatistics == null)
			{
				throw new CannotRemoveAggregatedAccountException((CoreResources.IDs)2834376775U, ex);
			}
			if (syncRequestStatistics.EmailAddress != this.EmailAddress)
			{
				throw new CannotRemoveAggregatedAccountException(CoreResources.IDs.ErrorNoSyncRequestsMatchedSpecifiedEmailAddress);
			}
			if (!syncRequestStatistics.Flags.HasFlag(RequestFlags.TargetIsAggregatedMailbox))
			{
				throw new CannotRemoveAggregatedAccountException(CoreResources.IDs.ErrorFoundSyncRequestForNonAggregatedAccount);
			}
			if (syncRequestStatistics.TargetExchangeGuid == Guid.Empty)
			{
				throw new CannotRemoveAggregatedAccountException((CoreResources.IDs)3504612180U);
			}
			return syncRequestStatistics.TargetExchangeGuid;
		}

		private void RemoveAggregatedAccountFromADUser()
		{
			using (PSLocalTask<SetMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateSetMailboxTask(base.CallContext.AccessingPrincipal, "RemoveAggregatedMailbox"))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.Identity = new MailboxIdParameter(base.CallContext.AccessingADUser.ObjectId);
				pslocalTask.Task.AggregatedMailboxGuid = this.MailboxGuid;
				pslocalTask.Task.RemoveAggregatedAccount = new SwitchParameter(true);
				pslocalTask.Task.ForestWideDomainControllerAffinityByExecutingUser = new SwitchParameter(true);
				pslocalTask.Task.Execute();
				if (pslocalTask.Error != null)
				{
					this.TraceError(string.Format("Could not remove aggregated mailbox {0} from unified mailbox with NetId {1}. Exception: {2}.", this.MailboxGuid, this.netId, pslocalTask.Error.Exception));
					base.CallContext.ProtocolLog.Set(AddAggregatedAccountMetadata.SetMailboxCmdletError, pslocalTask.Error.Exception);
					throw new CannotRemoveAggregatedAccountException(ResponseCodeType.ErrorCannotRemoveAggregatedAccountMailbox, pslocalTask.Error.Exception);
				}
				this.TraceSuccess(string.Format("Successfully removed aggregated mailbox guid {0} from unified mailbox with NetId {1}", this.MailboxGuid, this.netId));
			}
		}

		private void RemoveSyncRequest()
		{
			using (PSLocalTask<RemoveSyncRequest, object> removeSyncRequest = CmdletTaskFactory.Instance.CreateRemoveSyncRequestTask(base.CallContext.AccessingPrincipal))
			{
				removeSyncRequest.CaptureAdditionalIO = true;
				removeSyncRequest.Task.Identity = SyncRequestIdParameter.Create(base.CallContext.AccessingADUser, (string)this.EmailAddress);
				Exception removeSyncRequestException = null;
				FaultInjection.FaultInjectionPoint((FaultInjection.LIDs)2340826429U, delegate
				{
					removeSyncRequest.Task.Execute();
					if (removeSyncRequest.Error != null)
					{
						removeSyncRequestException = removeSyncRequest.Error.Exception;
					}
				}, delegate
				{
					removeSyncRequestException = new Exception("This is an exception from fault injection action");
				});
				if (removeSyncRequestException != null)
				{
					this.TraceError(string.Format("Could not remove aggregated account from unified mailbox with NetId {0}. Exception: {1}.", this.netId, removeSyncRequestException));
					throw new CannotRemoveAggregatedAccountException((CoreResources.IDs)2834376775U, removeSyncRequestException);
				}
				this.TraceSuccess(string.Format("Successfully removed aggregated account from unified mailbox with NetId {0}", this.netId));
			}
		}

		private static readonly string RemoveAggregatedAccountName = "RemoveAggregatedAccount";

		private readonly SmtpAddress EmailAddress;

		private Guid MailboxGuid;

		internal readonly Hookable<Func<Guid>> getAggregatedAccountMailboxGuidFromSyncRequestStatistics;

		internal readonly Hookable<Action> removeAggregatedAccountToADUser;

		internal readonly Hookable<Action> removeSyncRequest;
	}
}
