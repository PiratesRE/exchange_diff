using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeleteEngine : ExecutionEngine
	{
		private DeleteEngine()
		{
			this.connectSubscriptionCleanup = new ConnectSubscriptionCleanup(SubscriptionManager.Instance);
		}

		internal static DeleteEngine Instance
		{
			get
			{
				return DeleteEngine.instance;
			}
		}

		public override IAsyncResult BeginExecution(AggregationWorkItem workItem, AsyncCallback callback, object callbackState)
		{
			FrameworkPerfCounterHandler.Instance.OnDeleteStarted();
			if (workItem.SyncEngineState == null)
			{
				workItem.SyncEngineState = this.CreateSyncEngineState(workItem);
			}
			string format = string.Format("Entering DeleteEngine.BeginExecution: SubscriptionType:{0}", workItem.SubscriptionType);
			workItem.SyncEngineState.SyncLogSession.LogInformation((TSLID)1128UL, ExecutionEngine.Tracer, format, new object[0]);
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = new AsyncResult<AggregationWorkItem, SyncEngineResultData>(null, workItem, callback, callbackState, workItem.SubscriptionPoisonContext);
			asyncResult.SetCompletedSynchronously();
			this.DeleteSubscription(asyncResult);
			return asyncResult;
		}

		public override AsyncOperationResult<SyncEngineResultData> EndExecution(IAsyncResult asyncResultParameter)
		{
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)asyncResultParameter;
			AggregationWorkItem state = asyncResult.State;
			SyncLogSession syncLogSession = state.SyncEngineState.SyncLogSession;
			syncLogSession.LogDebugging((TSLID)1143UL, ExecutionEngine.Tracer, "EndExecution: WaitForCompletion enter...", new object[0]);
			AsyncOperationResult<SyncEngineResultData> asyncOperationResult = asyncResult.WaitForCompletion();
			syncLogSession.LogDebugging((TSLID)1154UL, ExecutionEngine.Tracer, "EndExecution: WaitForCompletion exit, result={0}", new object[]
			{
				asyncOperationResult
			});
			FrameworkPerfCounterHandler.Instance.OnDeleteCompletion(asyncOperationResult);
			syncLogSession.LogInformation((TSLID)1155UL, ExecutionEngine.Tracer, "Exiting DeleteEngine.EndExecution", new object[0]);
			return asyncOperationResult;
		}

		private SyncEngineState CreateSyncEngineState(AggregationWorkItem workItem)
		{
			return new SyncEngineState(SubscriptionInformationLoader.Instance, workItem.SyncLogSession, workItem.IsRecoverySyncMode, workItem.SubscriptionPoisonStatus, workItem.SyncHealthData, workItem.SubscriptionPoisonCallstack, workItem.LegacyDN, TransportMailSubmitter.Instance, SyncMode.DeleteSubscriptionMode, workItem.ConnectionStatistics, workItem.Subscription, RemoteServerHealthChecker.Instance);
		}

		private void DeleteSubscription(object context)
		{
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)context;
			AggregationWorkItem state = asyncResult.State;
			SyncEngineState syncEngineState = state.SyncEngineState;
			SyncLogSession syncLogSession = syncEngineState.SyncLogSession;
			syncLogSession.LogInformation((TSLID)1156UL, ExecutionEngine.Tracer, "Entering DeleteEngine.DeleteSubscription", new object[0]);
			try
			{
				if (this.TryLoadMailboxSessionAndSubscription(asyncResult))
				{
					this.TryCleanupSubscription(asyncResult);
				}
			}
			finally
			{
				syncLogSession.LogDebugging((TSLID)1157UL, ExecutionEngine.Tracer, "Exiting DeleteEngine.DeleteSubscription", new object[0]);
			}
		}

		private void TryCleanupSubscription(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			AggregationWorkItem state = asyncResult.State;
			SyncEngineState syncEngineState = state.SyncEngineState;
			MailboxSession mailboxSession = syncEngineState.SyncMailboxSession.MailboxSession;
			ISyncWorkerData userMailboxSubscription = syncEngineState.UserMailboxSubscription;
			IConnectSubscriptionCleanup connectSubscriptionCleanup = this.CleanupAssociatedWith(userMailboxSubscription.SubscriptionType);
			try
			{
				connectSubscriptionCleanup.Cleanup(mailboxSession, (IConnectSubscription)userMailboxSubscription, false);
			}
			catch (LocalizedException ex)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1158UL, ExecutionEngine.Tracer, "Hit exception in cleanup: {0}.", new object[]
				{
					ex
				});
				this.IndicateFailure(syncEngineState.StartSyncTime, asyncResult, ex);
			}
			this.IndicateSuccess(syncEngineState.StartSyncTime, asyncResult);
		}

		private bool TryLoadMailboxSessionAndSubscription(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			return this.TryLoadMailboxSessionAndSetOrgId(asyncResult) && this.TryLoadAndSetUserMailboxSubscription(asyncResult);
		}

		private bool TryLoadMailboxSessionAndSetOrgId(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			AggregationWorkItem state = asyncResult.State;
			SyncEngineState syncEngineState = state.SyncEngineState;
			SyncMailboxSession syncMailboxSession = syncEngineState.SyncMailboxSession;
			OrganizationId organizationId;
			bool flag;
			ISyncException ex;
			if (!syncEngineState.SubscriptionInformationLoader.TryLoadMailboxSession(state, syncMailboxSession, out organizationId, out flag, out ex))
			{
				this.IndicateFailure(syncEngineState.StartSyncTime, asyncResult, (Exception)ex);
				return false;
			}
			syncEngineState.SetOrganizationId(organizationId);
			return true;
		}

		private bool TryLoadAndSetUserMailboxSubscription(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			AggregationWorkItem state = asyncResult.State;
			SyncEngineState syncEngineState = state.SyncEngineState;
			SyncMailboxSession syncMailboxSession = syncEngineState.SyncMailboxSession;
			ISyncWorkerData userMailboxSubscription;
			ISyncException ex;
			bool flag;
			if (!syncEngineState.SubscriptionInformationLoader.TryLoadSubscription(state, syncMailboxSession, out userMailboxSubscription, out ex, out flag))
			{
				this.IndicateFailure(syncEngineState.StartSyncTime, asyncResult, (Exception)ex);
				return false;
			}
			syncEngineState.SetUserMailboxSubscription(userMailboxSubscription);
			return true;
		}

		internal IConnectSubscriptionCleanup CleanupAssociatedWith(AggregationSubscriptionType subscriptionType)
		{
			if (subscriptionType == AggregationSubscriptionType.Facebook || subscriptionType == AggregationSubscriptionType.LinkedIn)
			{
				return this.connectSubscriptionCleanup;
			}
			string message = string.Format("Invalid SubscriptionType:{0}, only Facebook and LinkedIn subscriptions are supported", subscriptionType);
			throw new ArgumentException(message);
		}

		private void IndicateSuccess(ExDateTime startSyncTime, AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			SyncEngineResultData result = new SyncEngineResultData(startSyncTime, true);
			asyncResult.ProcessCompleted(result);
		}

		private void IndicateFailure(ExDateTime startSyncTime, AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, Exception exception)
		{
			SyncEngineResultData result = new SyncEngineResultData(startSyncTime, false);
			asyncResult.ProcessCompleted(result, exception);
		}

		private static readonly DeleteEngine instance = new DeleteEngine();

		private readonly ConnectSubscriptionCleanup connectSubscriptionCleanup;
	}
}
