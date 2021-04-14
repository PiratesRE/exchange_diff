using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker;
using Microsoft.Exchange.Transport.Sync.Worker.Agents;
using Microsoft.Exchange.Transport.Sync.Worker.Health;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncEngine : IExecutionEngine, ISyncStorageProviderItemRetriever
	{
		private SyncEngine()
		{
		}

		internal static SyncEngine Instance
		{
			get
			{
				return SyncEngine.instance;
			}
		}

		public IAsyncResult BeginGetItem(object itemRetrieverState, SyncChangeEntry item, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			SyncEngineState syncEngineState = (SyncEngineState)itemRetrieverState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)985UL, SyncEngine.Tracer, "Begin GetItem invoked for: {0}, sync engine state: {1}.", new object[]
			{
				item,
				syncEngineState
			});
			SyncPoisonContext syncPoisonContext2 = new SyncPoisonContext(syncEngineState.UserMailboxSubscription.SubscriptionGuid, item.SuspectedSyncPoisonItem);
			AsyncResult<SyncEngineState, SyncChangeEntry> asyncResult = new AsyncResult<SyncEngineState, SyncChangeEntry>(SyncEngine.instance, syncEngineState, callback, callbackState, syncPoisonContext2);
			if (SyncPoisonHandler.IsPoisonItem(syncPoisonContext2, syncEngineState.SubscriptionPoisonStatus, syncEngineState.SyncLogSession))
			{
				syncEngineState.SyncLogSession.LogError((TSLID)986UL, SyncEngine.Tracer, "Poison Item found: {0}, skipping GetItem call to other provider.", new object[]
				{
					syncPoisonContext2
				});
				SyncPoisonItemFoundException innerException = new SyncPoisonItemFoundException(syncPoisonContext2.Item.ToString(), syncPoisonContext2.SubscriptionId);
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(item, SyncPermanentException.CreateItemLevelException(innerException));
				return asyncResult;
			}
			if (item.SyncObject != null)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)987UL, SyncEngine.Tracer, "Sync Object already present for item: {0}, skipping GetItem call to other provider.", new object[]
				{
					item
				});
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(item);
				return asyncResult;
			}
			if (item.Exception != null)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)988UL, SyncEngine.Tracer, "Item: {0} already marked with error: {1}, skipping GetItem call to other provider.", new object[]
				{
					item,
					item.Exception
				});
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(item, item.Exception);
				return asyncResult;
			}
			if (SyncEngine.ShouldTerminateSync(syncEngineState))
			{
				syncEngineState.SetSyncInterrupted();
				item.Exception = SyncEngine.syncTooSlowException;
				syncEngineState.SyncLogSession.LogDebugging((TSLID)1359UL, SyncEngine.Tracer, "Item: {0} to be skipped as the sync is too slow.", new object[]
				{
					item
				});
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(item, item.Exception);
				return asyncResult;
			}
			ISyncStorageProviderItemRetriever cloudProvider = syncEngineState.CloudProvider;
			object cloudProviderState = syncEngineState.CloudProviderState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)990UL, SyncEngine.Tracer, "Redirecting GetItem call to Cloud Provider with state: {0}", new object[]
			{
				syncEngineState.CloudProviderState
			});
			asyncResult.PendingAsyncResult = cloudProvider.BeginGetItem(cloudProviderState, item, new AsyncCallback(SyncEngine.OnGetItemCompleted), asyncResult, syncPoisonContext2);
			return asyncResult;
		}

		public AsyncOperationResult<SyncChangeEntry> EndGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<SyncEngineState, SyncChangeEntry> asyncResult2 = (AsyncResult<SyncEngineState, SyncChangeEntry>)asyncResult;
			asyncResult2.State.SyncLogSession.LogDebugging((TSLID)991UL, SyncEngine.Tracer, "End GetItem, sync engine state: {0}.", new object[]
			{
				asyncResult2.State
			});
			return asyncResult2.WaitForCompletion();
		}

		public void CancelGetItem(IAsyncResult asyncResult)
		{
			AsyncResult<SyncEngineState, SyncChangeEntry> asyncResult2 = (AsyncResult<SyncEngineState, SyncChangeEntry>)asyncResult;
			SyncEngineState state = asyncResult2.State;
			if (asyncResult2.PendingAsyncResult == null)
			{
				return;
			}
			ISyncStorageProviderItemRetriever cloudProvider = state.CloudProvider;
			state.SyncLogSession.LogDebugging((TSLID)993UL, SyncEngine.Tracer, "Invoking Cancel GetItem on Cloud provider, sync engine state: {0}.", new object[]
			{
				state
			});
			cloudProvider.CancelGetItem(asyncResult2.PendingAsyncResult);
		}

		public IAsyncResult BeginExecution(AggregationWorkItem workItem, AsyncCallback callback, object callbackState)
		{
			return this.BeginExecution(workItem, TransportMailSubmitter.Instance, null, null, SubscriptionInformationLoader.Instance, RemoteServerHealthChecker.Instance, callback, callbackState);
		}

		public IAsyncResult BeginExecution(AggregationWorkItem workItem, MailSubmitter mailSubmitter, NativeSyncStorageProvider nativeProvider, ISyncStorageProvider cloudProvider, ISubscriptionInformationLoader subscriptionInformationLoader, IRemoteServerHealthChecker remoteServerHealthChecker, AsyncCallback callback, object callbackState)
		{
			SyncUtilities.ThrowIfArgumentNull("workItem", workItem);
			SyncUtilities.ThrowIfArgumentNull("mailSubmitter", mailSubmitter);
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformationLoader", subscriptionInformationLoader);
			SyncUtilities.ThrowIfArgumentNull("remoteServerHealthChecker", remoteServerHealthChecker);
			mailSubmitter.EnsureRegisteredEventHandler(new EventHandler<EventArgs>(PerfCounterHandler.Instance.OnAggregationMailSubmission));
			FrameworkPerfCounterHandler.Instance.OnSyncStarted();
			if (workItem.SyncEngineState == null)
			{
				workItem.SyncEngineState = SyncEngine.CreateSyncEngineState(workItem, mailSubmitter, nativeProvider, cloudProvider, subscriptionInformationLoader, remoteServerHealthChecker);
				workItem.SyncEngineState.CurrentStep = ((workItem.SyncEngineState.SyncMode == SyncMode.CheckForChangesMode) ? SyncEngineStep.PreSyncStepInCheckForChangesMode : SyncEngineStep.PreSyncStepInEnumerateChangesMode);
			}
			else
			{
				workItem.SyncEngineState.SetSyncUnderRetry();
				if (workItem.SyncEngineState.CurrentStep != SyncEngineStep.PreSyncStepInEnumerateChangesMode)
				{
					workItem.SyncEngineState.ContinutationSyncStep = new SyncEngineStep?(workItem.SyncEngineState.CurrentStep);
					workItem.SyncEngineState.CurrentStep = SyncEngineStep.PreSyncStepInEnumerateChangesMode;
				}
			}
			workItem.SyncEngineState.SyncLogSession.LogInformation((TSLID)307UL, SyncEngine.Tracer, "Starting Sync: SyncMode:{0}, CurrentStep:{1}, ContinutationSyncStep:{2}, UnderRetry:{3}.", new object[]
			{
				workItem.SyncEngineState.SyncMode,
				workItem.SyncEngineState.CurrentStep,
				workItem.SyncEngineState.ContinutationSyncStep,
				workItem.SyncEngineState.UnderRetry
			});
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = new AsyncResult<AggregationWorkItem, SyncEngineResultData>(null, workItem, callback, callbackState, workItem.SubscriptionPoisonContext);
			ThreadPool.QueueUserWorkItem(asyncResult.GetWaitCallbackWithPoisonContext(new WaitCallback(SyncEngine.StartStep)), asyncResult);
			return asyncResult;
		}

		public AsyncOperationResult<SyncEngineResultData> EndExecution(IAsyncResult asyncResult)
		{
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult2 = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)asyncResult;
			AsyncOperationResult<SyncEngineResultData> asyncOperationResult = asyncResult2.WaitForCompletion();
			AggregationWorkItem state = asyncResult2.State;
			FrameworkPerfCounterHandler.Instance.OnSyncCompletion(asyncOperationResult);
			SyncEngine.UpdateSyncHealthData(state.SyncEngineState, asyncOperationResult);
			this.GenerateSubscriptionNotificationIfApplicable(state.SyncEngineState, asyncOperationResult);
			this.DisconnectMailboxSessionIfFailedTransiently(state.SyncEngineState, asyncOperationResult.Exception);
			return asyncOperationResult;
		}

		public void Cancel(IAsyncResult asyncResult)
		{
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult2 = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)asyncResult;
			IAsyncResult asyncResult3 = null;
			lock (asyncResult2.State.SyncEngineState.SyncRoot)
			{
				asyncResult3 = asyncResult2.PendingAsyncResult;
			}
			if (!asyncResult2.State.SyncEngineState.TryCancel)
			{
				asyncResult2.State.SyncEngineState.SetTryCancel();
				if (asyncResult3 == null)
				{
					return;
				}
				if (asyncResult3.IsCompleted || asyncResult3.AsyncState == null)
				{
					return;
				}
				LazyAsyncResult lazyAsyncResult = asyncResult3 as LazyAsyncResult;
				if (lazyAsyncResult != null && lazyAsyncResult.AsyncObject is NativeSyncStorageProvider)
				{
					if (asyncResult2.State.SyncEngineState.NativeProvider != null)
					{
						asyncResult2.State.SyncEngineState.NativeProvider.Cancel(asyncResult3);
						return;
					}
				}
				else if (asyncResult2.State.SyncEngineState.CloudProvider != null)
				{
					asyncResult2.State.SyncEngineState.CloudProvider.Cancel(asyncResult3);
				}
			}
		}

		private static SyncEngineState CreateSyncEngineState(AggregationWorkItem workItem, MailSubmitter mailSubmitter, NativeSyncStorageProvider nativeProvider, ISyncStorageProvider cloudProvider, ISubscriptionInformationLoader subscriptionInformationLoader, IRemoteServerHealthChecker remoteServerHealthChecker)
		{
			SyncMode syncMode = SyncEngine.DetermineSyncMode(workItem, remoteServerHealthChecker);
			SyncEngineState syncEngineState = new SyncEngineState(subscriptionInformationLoader, workItem.SyncLogSession, workItem.IsRecoverySyncMode, workItem.SubscriptionPoisonStatus, workItem.SyncHealthData, workItem.SubscriptionPoisonCallstack, workItem.LegacyDN, mailSubmitter, syncMode, workItem.ConnectionStatistics, workItem.Subscription, remoteServerHealthChecker);
			if (nativeProvider != null)
			{
				syncEngineState.SetNativeProvider(nativeProvider);
			}
			if (cloudProvider != null)
			{
				syncEngineState.SetCloudProvider(cloudProvider);
			}
			return syncEngineState;
		}

		private static void StartStep(object state)
		{
			Exception ex = null;
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)state;
			AggregationWorkItem state2 = asyncResult.State;
			SyncEngineState syncEngineState = state2.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1000UL, SyncEngine.Tracer, "StartStep: SyncMode:{0}, CurrentStep:{1}", new object[]
			{
				syncEngineState.SyncMode,
				syncEngineState.CurrentStep
			});
			lock (syncEngineState.SyncRoot)
			{
				asyncResult.PendingAsyncResult = null;
			}
			try
			{
				switch (syncEngineState.CurrentStep)
				{
				case SyncEngineStep.PreSyncStepInEnumerateChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					OrganizationId organizationId;
					bool invalidState;
					ISyncException ex2;
					if (!syncEngineState.SubscriptionInformationLoader.TryLoadMailboxSession(state2, syncEngineState.SyncMailboxSession, out organizationId, out invalidState, out ex2))
					{
						SyncEngine.SyncFailed(asyncResult, (Exception)ex2, false, invalidState);
						return;
					}
					syncEngineState.SetOrganizationId(organizationId);
					if (syncEngineState.UserMailboxSubscription == null)
					{
						ISyncWorkerData userMailboxSubscription;
						if (!syncEngineState.SubscriptionInformationLoader.TryLoadSubscription(state2, syncEngineState.SyncMailboxSession, out userMailboxSubscription, out ex2, out invalidState))
						{
							SyncEngine.SyncFailed(asyncResult, (Exception)ex2, false, invalidState);
							return;
						}
						syncEngineState.SetUserMailboxSubscription(userMailboxSubscription);
					}
					bool disableSubscription;
					if (!SyncEngine.DoesUserMailboxSubscriptionPermitsSync(syncEngineState, out ex2, out disableSubscription, out invalidState))
					{
						SyncEngine.SyncFailed(asyncResult, (Exception)ex2, disableSubscription, invalidState);
						return;
					}
					if (!SyncEngine.TrySetupStateStorage(state2, syncEngineState, out ex2))
					{
						SyncEngine.SyncFailed(asyncResult, (Exception)ex2, false, false);
						return;
					}
					bool recoverySyncMode = SyncEngine.GetRecoverySyncMode(syncEngineState, state2.SubscriptionType, state2.AggregationType, state2.SyncPhase);
					SyncEngine.SetupCloudProviderAndItsState(syncEngineState, syncEngineState.UserMailboxSubscription, recoverySyncMode);
					syncEngineState.CloudProviderState.Subscription = syncEngineState.UserMailboxSubscription;
					syncEngineState.CloudProviderState.StateStorage = syncEngineState.StateStorage;
					BaseWatermark baseWatermark = SyncEngine.CreateSyncWatermark(syncEngineState.SyncLogSession, syncEngineState.StateStorage, syncEngineState.UserMailboxSubscription.SubscriptionType);
					syncEngineState.CloudProviderState.SetBaseWatermark(baseWatermark);
					SyncEngine.SetupNativeProviderAndItsState(syncEngineState, recoverySyncMode);
					syncEngineState.CurrentStep = (syncEngineState.ContinutationSyncStep ?? SyncEngineStep.AuthenticateCloudInEnumerateChangesMode);
					syncEngineState.ContinutationSyncStep = null;
					SyncEngine.StartStep(asyncResult);
					break;
				}
				case SyncEngineStep.PreSyncStepInCheckForChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					SyncEngine.SetupCloudProviderAndItsState(syncEngineState, syncEngineState.MailboxServerSubscription, false);
					BaseWatermark baseWatermark2 = SyncEngine.CreateSyncWatermark(syncEngineState.SyncLogSession, state2.MailboxServerSyncWatermark, syncEngineState.MailboxServerSubscription.SubscriptionType);
					syncEngineState.CloudProviderState.SetBaseWatermark(baseWatermark2);
					syncEngineState.CurrentStep = SyncEngineStep.AuthenticateCloudInCheckForChangesMode;
					SyncEngine.StartStep(asyncResult);
					break;
				}
				case SyncEngineStep.AuthenticateCloudInCheckForChangesMode:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					syncEngineState.LastSyncOperationResult = null;
					asyncResult.PendingAsyncResult = syncEngineState.CloudProvider.BeginAuthenticate(syncEngineState.CloudProviderState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.AuthenticateCloudInEnumerateChangesMode:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					syncEngineState.LastSyncOperationResult = null;
					asyncResult.PendingAsyncResult = syncEngineState.CloudProvider.BeginAuthenticate(syncEngineState.CloudProviderState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.CheckForChangesInCloud:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					syncEngineState.LastSyncOperationResult = null;
					asyncResult.PendingAsyncResult = syncEngineState.CloudProvider.BeginCheckForChanges(syncEngineState.CloudProviderState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.EnumCloud:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					syncEngineState.LastSyncOperationResult = null;
					asyncResult.PendingAsyncResult = syncEngineState.CloudProvider.BeginEnumerateChanges(syncEngineState.CloudProviderState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.ApplyNative:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					asyncResult.PendingAsyncResult = syncEngineState.NativeProvider.BeginApplyChanges(syncEngineState.NativeProviderState, syncEngineState.LastSyncOperationResult.Data.ChangeList, SyncEngine.Instance, syncEngineState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.AcknowledgeCloud:
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					asyncResult.PendingAsyncResult = syncEngineState.CloudProvider.BeginAcknowledgeChanges(syncEngineState.CloudProviderState, syncEngineState.LastSyncOperationResult.Data.ChangeList, syncEngineState.LastSyncOperationResult.Data.HasPermanentSyncErrors, syncEngineState.LastSyncOperationResult.Data.HasTransientSyncErrors, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					break;
				case SyncEngineStep.PostSyncStepInCheckForChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					ISyncException ex3;
					if (SyncEngine.CheckIfRemoteServerIsSlow(syncEngineState, syncEngineState.MailboxServerSubscription, out ex3))
					{
						syncEngineState.SyncLogSession.LogError((TSLID)1012UL, SyncEngine.Tracer, "Hit Remote Server slow error, will switch to enumerate changes mode. Error: {0}.", new object[]
						{
							ex3
						});
						SyncEngine.SwitchToEnumerateChangesMode(syncEngineState, SyncEngineStep.PostSyncStepInEnumerateChangesMode);
						SyncEngine.StartStep(asyncResult);
						return;
					}
					syncEngineState.CurrentStep = SyncEngineStep.End;
					syncEngineState.SyncLogSession.LogVerbose((TSLID)1006UL, SyncEngine.Tracer, "Sync is finished successully with no changes.", new object[0]);
					SyncEngine.Instance.SyncCompletedSuccessfulyWithNoChanges(asyncResult);
					return;
				}
				case SyncEngineStep.PostSyncStepInEnumerateChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					ISyncException ex4;
					if (SyncEngine.CheckIfRemoteServerIsSlow(syncEngineState, syncEngineState.UserMailboxSubscription, out ex4))
					{
						syncEngineState.SyncLogSession.LogError((TSLID)976UL, SyncEngine.Tracer, "Sync Completed successfully while Remote Server is slow. Error: {0}.", new object[]
						{
							ex4
						});
						SyncEngine.SyncCompletedSuccessfullyWithPostSyncError(asyncResult, (Exception)ex4);
						return;
					}
					syncEngineState.CurrentStep = SyncEngineStep.End;
					syncEngineState.SyncLogSession.LogVerbose((TSLID)975UL, SyncEngine.Tracer, "Sync is finished successully with changes.", new object[0]);
					SyncEngine.SyncCompletedSuccessfullyWithChanges(asyncResult);
					return;
				}
				case SyncEngineStep.GetCloudStatistcsInEnumerateChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					ICloudStatisticsProvider cloudStatisticsProvider = syncEngineState.CloudProvider as ICloudStatisticsProvider;
					if (cloudStatisticsProvider != null)
					{
						syncEngineState.SyncLogSession.LogDebugging((TSLID)9984UL, SyncEngine.Tracer, "Get Cloud statistics from the provider", new object[0]);
						asyncResult.PendingAsyncResult = cloudStatisticsProvider.BeginGetStatistics(syncEngineState.CloudProviderState, new AsyncCallback(SyncEngine.EndStep), asyncResult, asyncResult.SyncPoisonContext);
					}
					else
					{
						syncEngineState.CurrentStep = SyncEngineStep.PostSyncStepInEnumerateChangesMode;
						SyncEngine.StartStep(asyncResult);
					}
					break;
				}
				default:
					syncEngineState.SyncLogSession.LogError((TSLID)997UL, SyncEngine.Tracer, "Invalid Sync Engine State: {0}.", new object[]
					{
						syncEngineState
					});
					syncEngineState.CurrentStep = SyncEngineStep.End;
					SyncEngine.SyncFailed(asyncResult, SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new InvalidSyncEngineStateException()), false, false);
					break;
				}
			}
			catch (SyncPermanentException ex5)
			{
				ex = ex5;
			}
			catch (SyncTransientException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1013UL, SyncEngine.Tracer, "Hit Exception in StartStep: {0}.", new object[]
				{
					ex
				});
				SyncEngine.SyncFailed(asyncResult, ex, false, false);
			}
		}

		private static void EndStep(IAsyncResult ar)
		{
			Exception ex = null;
			AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult = (AsyncResult<AggregationWorkItem, SyncEngineResultData>)ar.AsyncState;
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1014UL, SyncEngine.Tracer, "EndStep: SyncMode:{0}, CurrentStep:{1}.", new object[]
			{
				syncEngineState.SyncMode,
				syncEngineState.CurrentStep
			});
			lock (syncEngineState.SyncRoot)
			{
				asyncResult.PendingAsyncResult = null;
			}
			try
			{
				switch (syncEngineState.CurrentStep)
				{
				case SyncEngineStep.AuthenticateCloudInCheckForChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.CloudProvider.EndAuthenticate(ar);
					if (asyncOperationResult.IsSucceeded)
					{
						syncEngineState.CurrentStep = SyncEngineStep.CheckForChangesInCloud;
						SyncEngine.StartStep(asyncResult);
						return;
					}
					syncEngineState.SyncLogSession.LogError((TSLID)1041UL, SyncEngine.Tracer, "AuthenticateCloudInCheckForChangesMode failed, lets switch to Enumerate Changes Mode. Error: {0}.", new object[]
					{
						asyncOperationResult.Exception
					});
					SyncEngine.SwitchToEnumerateChangesMode(syncEngineState, SyncEngineStep.AuthenticateCloudInEnumerateChangesMode);
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				case SyncEngineStep.AuthenticateCloudInEnumerateChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.CloudProvider.EndAuthenticate(ar);
					if (!SyncEngine.HandleSyncOperationResult(asyncOperationResult, asyncResult))
					{
						return;
					}
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					syncEngineState.CurrentStep = SyncEngineStep.EnumCloud;
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				case SyncEngineStep.CheckForChangesInCloud:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.CheckForChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.CloudProvider.EndCheckForChanges(ar);
					if (asyncOperationResult.IsSucceeded && asyncOperationResult.Data.HasNoChangesOnCloud)
					{
						syncEngineState.SyncLogSession.LogInformation((TSLID)522UL, SyncEngine.Tracer, "CheckForChangesInCloud: Sync is done successfully with no Changes, lets do some post sync checks.", new object[0]);
						syncEngineState.CurrentStep = SyncEngineStep.PostSyncStepInCheckForChangesMode;
						SyncEngine.StartStep(asyncResult);
						return;
					}
					syncEngineState.SyncLogSession.LogVerbose((TSLID)998UL, SyncEngine.Tracer, "CheckForChanges failed ({0}) or have detected changes ({1}). We will switch to Enumerate changes Mode", new object[]
					{
						!asyncOperationResult.IsSucceeded,
						asyncOperationResult.Data != null && !asyncOperationResult.Data.HasNoChangesOnCloud
					});
					SyncEngine.SwitchToEnumerateChangesMode(syncEngineState, SyncEngineStep.EnumCloud);
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				case SyncEngineStep.EnumCloud:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.CloudProvider.EndEnumerateChanges(ar);
					if (!SyncEngine.HandleSyncOperationResult(asyncOperationResult, asyncResult))
					{
						return;
					}
					if (SyncEngine.IsCancelRequested(asyncResult))
					{
						return;
					}
					if (asyncOperationResult.Data.ChangeList == null || asyncOperationResult.Data.ChangeList.Count == 0)
					{
						syncEngineState.SyncLogSession.LogDebugging((TSLID)1020UL, "Short-circuiting sync to AcknowledgeCloud as there are no changes.", new object[0]);
						syncEngineState.CurrentStep = SyncEngineStep.AcknowledgeCloud;
						if (!syncEngineState.StateStorage.InitialSyncDone)
						{
							syncEngineState.StateStorage.MarkInitialSyncDone();
							syncEngineState.SyncLogSession.LogInformation((TSLID)1021UL, "Since there are NO Enumerated Changes from Cloud Provider, marking Initial Sync as Done.", new object[0]);
						}
					}
					else
					{
						syncEngineState.CurrentStep = SyncEngineStep.ApplyNative;
					}
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				case SyncEngineStep.ApplyNative:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.NativeProvider.EndApplyChanges(ar);
					SyncEngine.CheckToPromoteTransientExceptionsToPermanent(asyncOperationResult, syncEngineState);
					SyncEngine.TrackFailedCloudEntries(asyncOperationResult, syncEngineState);
					if (!SyncEngine.HandleSyncOperationResult(asyncOperationResult, asyncResult))
					{
						return;
					}
					syncEngineState.CurrentStep = SyncEngineStep.AcknowledgeCloud;
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				case SyncEngineStep.AcknowledgeCloud:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					AsyncOperationResult<SyncProviderResultData> asyncOperationResult = syncEngineState.CloudProvider.EndAcknowledgeChanges(ar);
					if (!SyncEngine.HandleSyncOperationResult(asyncOperationResult, asyncResult))
					{
						return;
					}
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1024UL, SyncEngine.Tracer, "Performing Checkpoint after AcknowledgeCloud.", new object[0]);
					if (SyncEngine.Checkpoint(asyncOperationResult, syncEngineState, new SyncProgress?(SyncProgress.OneWaySyncDone), new bool?(false), asyncResult))
					{
						syncEngineState.CloudItemsSynced = syncEngineState.LastSyncOperationResult.Data.CloudItemsSynced;
						bool hasTransientSyncErrors = syncEngineState.LastSyncOperationResult.Data.HasTransientSyncErrors;
						if (syncEngineState.CloudProviderState.BaseWatermark.IsSyncWatermarkUpdated)
						{
							string text = syncEngineState.CloudProviderState.BaseWatermark.ToString();
							syncEngineState.SyncLogSession.LogInformation((TSLID)1028UL, SyncEngine.Tracer, "Watermark updated on cloud provider, syncWatermark:{0}.", new object[]
							{
								text
							});
							syncEngineState.UpdatedSyncWatermark = text;
						}
						syncEngineState.UserMailboxSubscription.TotalItemsInSourceMailbox = syncEngineState.CloudProviderState.CloudStatistics.TotalItemsInSourceMailbox;
						syncEngineState.UserMailboxSubscription.TotalSizeOfSourceMailbox = syncEngineState.CloudProviderState.CloudStatistics.TotalSizeOfSourceMailbox;
						if (AggregationConfiguration.Instance.CloudStatisticsCollectionDisabled)
						{
							syncEngineState.CurrentStep = SyncEngineStep.PostSyncStepInEnumerateChangesMode;
							syncEngineState.SyncLogSession.LogDebugging((TSLID)1026UL, SyncEngine.Tracer, "One-Way Sync is done, lets do some post sync checks.", new object[0]);
						}
						else
						{
							syncEngineState.CurrentStep = SyncEngineStep.GetCloudStatistcsInEnumerateChangesMode;
							syncEngineState.SyncLogSession.LogDebugging((TSLID)9980UL, SyncEngine.Tracer, "Get Cloud statistics from the provider", new object[0]);
						}
						SyncEngine.StartStep(asyncResult);
						goto IL_628;
					}
					goto IL_628;
				}
				case SyncEngineStep.GetCloudStatistcsInEnumerateChangesMode:
				{
					SyncEngine.AssertSyncModeOnCurrentStep(SyncMode.EnumerateChangesMode, syncEngineState);
					ICloudStatisticsProvider cloudStatisticsProvider = (ICloudStatisticsProvider)syncEngineState.CloudProvider;
					syncEngineState.SyncLogSession.LogDebugging((TSLID)9981UL, SyncEngine.Tracer, "Performing End GetStatistics operation.", new object[0]);
					AsyncOperationResult<CloudStatistics> asyncOperationResult2 = cloudStatisticsProvider.EndGetStatistics(ar);
					if (asyncOperationResult2.IsSucceeded)
					{
						syncEngineState.UserMailboxSubscription.TotalItemsInSourceMailbox = asyncOperationResult2.Data.TotalItemsInSourceMailbox;
						syncEngineState.UserMailboxSubscription.TotalSizeOfSourceMailbox = asyncOperationResult2.Data.TotalSizeOfSourceMailbox;
						syncEngineState.CloudProviderState.CloudStatistics = asyncOperationResult2.Data;
					}
					else
					{
						syncEngineState.SyncLogSession.LogError((TSLID)9982UL, SyncEngine.Tracer, "Get cloud Statistics call wasn't successful. Hit exception {0}", new object[]
						{
							asyncOperationResult2.Exception
						});
					}
					syncEngineState.CurrentStep = SyncEngineStep.PostSyncStepInEnumerateChangesMode;
					syncEngineState.SyncLogSession.LogDebugging((TSLID)9983UL, SyncEngine.Tracer, "Get cloud Statistics step is done.", new object[0]);
					SyncEngine.StartStep(asyncResult);
					goto IL_628;
				}
				}
				syncEngineState.SyncLogSession.LogError((TSLID)1035UL, SyncEngine.Tracer, "Invalid Sync Engine State: {0}.", new object[]
				{
					syncEngineState
				});
				syncEngineState.CurrentStep = SyncEngineStep.End;
				SyncEngine.SyncFailed(asyncResult, SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new InvalidOperationException("Invalid Sync Engine State."), true), false, false);
				IL_628:;
			}
			catch (SyncPermanentException ex2)
			{
				ex = ex2;
			}
			catch (SyncTransientException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1036UL, SyncEngine.Tracer, "Hit Exception in EndStep: {0}.", new object[]
				{
					ex
				});
				SyncEngine.SyncFailed(asyncResult, ex, false, false);
			}
		}

		private static bool TrySetupStateStorage(AggregationWorkItem workItem, SyncEngineState syncEngineState, out ISyncException syncException)
		{
			syncException = null;
			if (syncEngineState.StateStorage != null)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)1038UL, SyncEngine.Tracer, "State Storage is already there, just reloading it.", new object[0]);
				return syncEngineState.SubscriptionInformationLoader.TryReloadStateStorage(workItem, syncEngineState.StateStorage, out syncException);
			}
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1037UL, SyncEngine.Tracer, "State Storage not there, creating one.", new object[0]);
			IStateStorage stateStorage;
			if (!syncEngineState.SubscriptionInformationLoader.TryLoadStateStorage(workItem, syncEngineState.SyncMailboxSession, syncEngineState.UserMailboxSubscription, out stateStorage, out syncException))
			{
				return false;
			}
			syncEngineState.SetStateStorage(stateStorage);
			syncEngineState.SetPreviousSyncProgress(stateStorage.SyncProgress);
			syncEngineState.StateStorage.SetSyncProgress(SyncProgress.OneWaySyncStarted);
			Exception ex = syncEngineState.CommitStateStorage(false);
			if (ex != null)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1059UL, "Exception encountered while commiting sync progress: {0}", new object[]
				{
					ex
				});
				syncException = (ISyncException)ex;
				return false;
			}
			return true;
		}

		private static bool DoesUserMailboxSubscriptionPermitsSync(SyncEngineState syncEngineState, out ISyncException syncException, out bool disableSubscription, out bool invalidState)
		{
			syncException = null;
			disableSubscription = false;
			invalidState = false;
			return !SyncEngine.IsMailboxOverQuota(syncEngineState, out syncException) && SyncEngine.DoesSubscriptionStatusPermitSync(syncEngineState, out syncException, out disableSubscription) && !SyncEngine.DoesSubscriptionHaveInvalidPassword(syncEngineState.UserMailboxSubscription, syncEngineState.SyncLogSession, out syncException) && SyncEngine.DoesRemoteServerHealthPermitSync(syncEngineState, syncEngineState.UserMailboxSubscription, out syncException, out disableSubscription);
		}

		private static bool GetRecoverySyncMode(SyncEngineState syncEngineState, AggregationSubscriptionType subscriptionType, AggregationType aggregationType, SyncPhase syncPhase)
		{
			if (syncEngineState.PreviousSyncProgress == SyncProgress.OneWaySyncStarted)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)1058UL, SyncEngine.Tracer, "UnderRecovery because PreviousSyncProgress={0}", new object[]
				{
					syncEngineState.PreviousSyncProgress
				});
				return true;
			}
			if (syncEngineState.OriginalIsUnderRecoveryFlag)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)98UL, SyncEngine.Tracer, "UnderRecovery because Mailbox server requested.", new object[0]);
				return true;
			}
			if (syncEngineState.UnderRetry)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)309UL, SyncEngine.Tracer, "UnderRecovery because we are retrying the sync on hub.", new object[0]);
				return true;
			}
			if (AggregationConfiguration.Instance.AggregationTypesToBeSyncedInRecoveryMode.Contains(aggregationType) && AggregationConfiguration.Instance.SubscriptionTypesToBeSyncedInRecoveryMode.Contains(subscriptionType) && AggregationConfiguration.Instance.SyncPhasesToBeSyncedInRecoveryMode.Contains(syncPhase))
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)781UL, SyncEngine.Tracer, "UnderRecovery because of configuration: {0} subscription of type {1} in {2} sync phase.", new object[]
				{
					aggregationType,
					subscriptionType,
					syncPhase
				});
				return true;
			}
			if (syncEngineState.UserMailboxSubscription != null && syncEngineState.UserMailboxSubscription.InitialSyncInRecoveryMode && syncPhase == SyncPhase.Initial)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)311UL, SyncEngine.Tracer, "UnderRecovery because subscription's initial sync has been configured to run in recovery mode;  aggregation type: {0};  subscription type: {1};  sync phase: {2}", new object[]
				{
					aggregationType,
					subscriptionType,
					syncPhase
				});
				return true;
			}
			syncEngineState.SyncLogSession.LogDebugging((TSLID)794UL, SyncEngine.Tracer, "NOT UnderRecovery.", new object[0]);
			return false;
		}

		private static void SetupNativeProviderAndItsState(SyncEngineState syncEngineState, bool nativeUnderRecovery)
		{
			if (syncEngineState.NativeProvider == null)
			{
				NativeSyncStorageProvider nativeProvider = SyncStorageProviderFactory.CreateNativeSyncStorageProvider(syncEngineState.UserMailboxSubscription);
				syncEngineState.SetNativeProvider(nativeProvider);
			}
			if (syncEngineState.NativeProviderState == null)
			{
				NativeSyncStorageProviderState nativeProviderState = syncEngineState.NativeProvider.Bind(syncEngineState.SyncMailboxSession, syncEngineState.UserMailboxSubscription, syncEngineState.StateStorage, syncEngineState.MailSubmitter, syncEngineState.SyncLogSession, nativeUnderRecovery);
				syncEngineState.SetNativeProviderState(nativeProviderState);
				return;
			}
			if (nativeUnderRecovery)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)996UL, "Marking the Native Provider as under recovery.", new object[0]);
				syncEngineState.NativeProviderState.UnderRecovery = true;
			}
		}

		private static void SetupCloudProviderAndItsState(SyncEngineState syncEngineState, ISyncWorkerData subscription, bool cloudUnderRecovery)
		{
			if (syncEngineState.CloudProvider == null)
			{
				ISyncStorageProvider cloudProvider = SyncStorageProviderFactory.CreateCloudSyncStorageProvider(subscription);
				syncEngineState.SetCloudProvider(cloudProvider);
			}
			if (syncEngineState.CloudProviderState == null)
			{
				SyncStorageProviderState syncStorageProviderState = syncEngineState.CloudProvider.Bind(subscription, syncEngineState.SyncLogSession, cloudUnderRecovery);
				syncStorageProviderState.RemoteServerRoundtripCompleteEvent += SyncEngine.OnRemoteServerRoundtripComplete;
				syncEngineState.SetCloudProviderState(syncStorageProviderState);
				return;
			}
			if (cloudUnderRecovery)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)995UL, "Marking the Cloud Provider as under recovery.", new object[0]);
				syncEngineState.CloudProviderState.UnderRecovery = true;
			}
		}

		private static void AssertSyncModeOnCurrentStep(SyncMode expectedSyncMode, SyncEngineState syncEngineState)
		{
		}

		private static BaseWatermark CreateSyncWatermark(SyncLogSession syncLogSession, string mailboxServerSyncWatermark, AggregationSubscriptionType subscriptionType)
		{
			if (subscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (subscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					break;
				case (AggregationSubscriptionType)3:
					goto IL_42;
				case AggregationSubscriptionType.DeltaSyncMail:
					return new DeltaSyncWatermark(syncLogSession, mailboxServerSyncWatermark);
				default:
					if (subscriptionType != AggregationSubscriptionType.IMAP)
					{
						goto IL_42;
					}
					break;
				}
			}
			else if (subscriptionType != AggregationSubscriptionType.Facebook && subscriptionType != AggregationSubscriptionType.LinkedIn)
			{
				goto IL_42;
			}
			return new StringWatermark(syncLogSession, mailboxServerSyncWatermark);
			IL_42:
			throw new InvalidOperationException("CreateSyncWatermark called with an invalid subscriptionType: " + subscriptionType);
		}

		private static BaseWatermark CreateSyncWatermark(SyncLogSession syncLogSession, ISimpleStateStorage stateStorage, AggregationSubscriptionType subscriptionType)
		{
			if (subscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (subscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					break;
				case (AggregationSubscriptionType)3:
					goto IL_42;
				case AggregationSubscriptionType.DeltaSyncMail:
					return new DeltaSyncWatermark(syncLogSession, stateStorage);
				default:
					if (subscriptionType != AggregationSubscriptionType.IMAP)
					{
						goto IL_42;
					}
					break;
				}
			}
			else if (subscriptionType != AggregationSubscriptionType.Facebook && subscriptionType != AggregationSubscriptionType.LinkedIn)
			{
				goto IL_42;
			}
			return new StringWatermark(syncLogSession, stateStorage);
			IL_42:
			throw new InvalidOperationException("CreateSyncWatermark called with an invalid subscriptionType: " + subscriptionType);
		}

		private static void SwitchToEnumerateChangesMode(SyncEngineState syncEngineState, SyncEngineStep continuationStep)
		{
			SyncEngineStep currentStep = syncEngineState.CurrentStep;
			syncEngineState.SwitchToEnumerateChangeMode();
			syncEngineState.CurrentStep = SyncEngineStep.PreSyncStepInEnumerateChangesMode;
			syncEngineState.ContinutationSyncStep = new SyncEngineStep?(continuationStep);
			syncEngineState.SyncLogSession.LogVerbose((TSLID)989UL, SyncEngine.Tracer, "Switching to Enumerate Changes Mode: Old CurrentStep:{0}, New CurrentStep:{1}, ContinuationStep:{2}.", new object[]
			{
				currentStep,
				syncEngineState.CurrentStep,
				syncEngineState.ContinutationSyncStep
			});
		}

		private static bool Checkpoint(AsyncOperationResult<SyncProviderResultData> syncOperationResult, SyncEngineState syncEngineState, SyncProgress? newSyncProgressValue, bool? forceRecoverySyncNext, AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			if (newSyncProgressValue != null)
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)992UL, SyncEngine.Tracer, "SyncProgress = {0}", new object[]
				{
					newSyncProgressValue
				});
				syncEngineState.StateStorage.SetSyncProgress(newSyncProgressValue.Value);
			}
			bool flag = syncEngineState.StateStorage.SyncProgress != SyncProgress.OneWaySyncStarted;
			if (forceRecoverySyncNext != null)
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)994UL, SyncEngine.Tracer, "ForceRecoverySyncNext = {0}", new object[]
				{
					forceRecoverySyncNext
				});
				syncEngineState.StateStorage.SetForceRecoverySyncNext(forceRecoverySyncNext.Value);
			}
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1025UL, SyncEngine.Tracer, "Checkpoint for step: {0}", new object[]
			{
				syncEngineState.CurrentStep
			});
			IList<SyncChangeEntry> changeList = null;
			if (syncOperationResult.Data != null)
			{
				changeList = syncOperationResult.Data.ChangeList;
			}
			if (syncEngineState.StateStorage.SyncProgress == SyncProgress.OneWaySyncDone)
			{
				SyncEngine.LogStatistics(syncEngineState, changeList);
			}
			Exception ex = syncEngineState.CommitStateStorage(flag);
			if (ex != null)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1027UL, SyncEngine.Tracer, "We will enter recovery sync as we hit at exception when committing: {0}", new object[]
				{
					ex
				});
				SyncEngine.SyncFailed(asyncResult, ex, false, false);
				return false;
			}
			if (flag && syncOperationResult.IsSucceeded && !syncEngineState.HasTransientItemLevelErrors)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)1002UL, SyncEngine.Tracer, "Clearing Poison Context for subscription: {0}, with status: {1}", new object[]
				{
					syncEngineState.UserMailboxSubscription.SubscriptionGuid,
					syncEngineState.SubscriptionPoisonStatus
				});
				SyncPoisonHandler.ClearPoisonContext(syncEngineState.UserMailboxSubscription.SubscriptionGuid, syncEngineState.SubscriptionPoisonStatus, syncEngineState.SyncLogSession);
			}
			return true;
		}

		private static void CheckToPromoteTransientExceptionsToPermanent(AsyncOperationResult<SyncProviderResultData> syncOperationResult, SyncEngineState syncEngineState)
		{
			if (syncOperationResult.IsSucceeded && syncOperationResult.Data != null && syncOperationResult.Data.ChangeList != null)
			{
				syncOperationResult.Data.HasTransientSyncErrors = false;
				foreach (SyncChangeEntry syncChangeEntry in syncOperationResult.Data.ChangeList)
				{
					SyncTransientException ex = syncChangeEntry.Exception as SyncTransientException;
					if (ex != null)
					{
						bool flag;
						if (syncChangeEntry.CloudId != null)
						{
							if (syncChangeEntry.SchemaType == SchemaType.Folder)
							{
								flag = syncEngineState.StateStorage.ShouldPromoteFolderTransientException(syncChangeEntry.CloudId, ex);
							}
							else
							{
								flag = syncEngineState.StateStorage.ShouldPromoteItemTransientException(syncChangeEntry.CloudId, ex);
							}
						}
						else if (syncChangeEntry.SchemaType == SchemaType.Folder)
						{
							flag = syncEngineState.StateStorage.ShouldPromoteFolderTransientException(syncChangeEntry.NativeId, ex);
						}
						else
						{
							flag = syncEngineState.StateStorage.ShouldPromoteItemTransientException(syncChangeEntry.NativeId, ex);
						}
						if (flag)
						{
							syncChangeEntry.Exception = SyncPermanentException.CreateItemLevelExceptionPromotedFromTransientException(syncChangeEntry.Exception.InnerException);
							syncEngineState.SyncLogSession.LogError((TSLID)1296UL, SyncEngine.Tracer, "Transient Error promoted to Permanent for change:{0}.", new object[]
							{
								syncChangeEntry
							});
							syncOperationResult.Data.HasPermanentSyncErrors = true;
						}
						else
						{
							syncOperationResult.Data.HasTransientSyncErrors = true;
							syncEngineState.HasTransientItemLevelErrors = true;
						}
					}
				}
			}
		}

		private static void TrackFailedCloudEntries(AsyncOperationResult<SyncProviderResultData> syncOperationResult, SyncEngineState syncEngineState)
		{
			if (syncOperationResult.Data == null || syncOperationResult.Data.ChangeList == null || syncOperationResult.Data.ChangeList.Count == 0)
			{
				return;
			}
			foreach (SyncChangeEntry syncChangeEntry in syncOperationResult.Data.ChangeList)
			{
				if (syncChangeEntry.ChangeType == ChangeType.Delete)
				{
					bool flag;
					if (syncChangeEntry.SchemaType == SchemaType.Folder)
					{
						flag = syncEngineState.StateStorage.TryRemoveFailedFolder(syncChangeEntry.CloudId);
					}
					else
					{
						flag = syncEngineState.StateStorage.TryRemoveFailedItem(syncChangeEntry.CloudId);
					}
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1039UL, SyncEngine.Tracer, "Deleting failed entry with result: {0}, for change: {1}", new object[]
					{
						flag,
						syncChangeEntry
					});
				}
				else if (syncChangeEntry.ChangeType == ChangeType.Add && syncChangeEntry.Exception is SyncPermanentException)
				{
					bool flag;
					if (syncChangeEntry.SchemaType == SchemaType.Folder)
					{
						flag = syncEngineState.StateStorage.TryAddFailedFolder(syncChangeEntry.CloudId, syncChangeEntry.CloudFolderId);
					}
					else
					{
						flag = syncEngineState.StateStorage.TryAddFailedItem(syncChangeEntry.CloudId, syncChangeEntry.CloudFolderId);
					}
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1040UL, SyncEngine.Tracer, "Adding failed entry with result: {0}, for change: {1}", new object[]
					{
						flag,
						syncChangeEntry
					});
				}
			}
		}

		private static void IncrementPermanentItemErrorCountBasedOnException(Exception itemLevelException, SyncHealthData syncHealthData)
		{
			SyncPoisonItemFoundException ex = itemLevelException as SyncPoisonItemFoundException;
			if (ex != null)
			{
				syncHealthData.PoisonItemErrorsCount++;
				return;
			}
			MessageSizeLimitExceededException ex2 = itemLevelException as MessageSizeLimitExceededException;
			if (ex2 != null)
			{
				syncHealthData.OverSizeItemErrorsCount++;
				return;
			}
			UnresolveableFolderNameException ex3 = itemLevelException as UnresolveableFolderNameException;
			if (ex3 != null)
			{
				syncHealthData.UnresolveableFolderNameErrorsCount++;
				return;
			}
			ObjectNotFoundException ex4 = itemLevelException as ObjectNotFoundException;
			if (ex4 != null)
			{
				syncHealthData.ObjectNotFoundErrorsCount++;
				return;
			}
			syncHealthData.OtherItemErrorsCount++;
		}

		private static bool HandleSyncOperationResult(AsyncOperationResult<SyncProviderResultData> syncOperationResult, AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			if (syncOperationResult.Exception != null)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)1003UL, SyncEngine.Tracer, "Sync Operation has failed: {0}.", new object[]
				{
					syncOperationResult.Exception
				});
				if (!(syncOperationResult.Exception is SyncTransientException) && !(syncOperationResult.Exception is SyncPermanentException) && !(syncOperationResult.Exception is OperationCanceledException))
				{
					throw new InvalidOperationException("Every exception should be wrapped or cast into appropriate transient or permanent behavior.", syncOperationResult.Exception);
				}
				bool? forceRecoverySyncNext = null;
				if (syncEngineState.TryCancel || syncOperationResult.Exception is OperationCanceledException)
				{
					forceRecoverySyncNext = new bool?(true);
				}
				if (SyncEngine.Checkpoint(syncOperationResult, syncEngineState, new SyncProgress?(SyncProgress.OneWaySyncDone), forceRecoverySyncNext, asyncResult))
				{
					SyncEngine.SyncFailed(asyncResult, syncOperationResult.Exception, false, false);
				}
				return false;
			}
			else
			{
				if (!SyncEngine.Checkpoint(syncOperationResult, syncEngineState, new SyncProgress?(SyncProgress.OneWaySyncStarted), null, asyncResult))
				{
					return false;
				}
				if (syncOperationResult.Data != null && syncOperationResult.Data.MoreItemsAvailable)
				{
					syncEngineState.SyncLogSession.LogVerbose((TSLID)1043UL, "Setting more items available.", new object[0]);
					syncEngineState.SetMoreItemsAvailable();
				}
				syncEngineState.LastSyncOperationResult = syncOperationResult;
				return true;
			}
		}

		private static void LogStatistics(SyncEngineState syncEngineState, IList<SyncChangeEntry> changeList)
		{
			SyncLogSession syncLogSession = syncEngineState.SyncLogSession;
			if (changeList == null || changeList.Count == 0)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)1045UL, SyncEngine.Tracer, "Cloud to Native: no Changes were processed.", new object[0]);
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			int num22 = 0;
			int num23 = 0;
			int num24 = 0;
			int num25 = 0;
			int num26 = 0;
			int num27 = 0;
			int num28 = 0;
			int num29 = 0;
			int num30 = 0;
			int num31 = 0;
			int num32 = 0;
			int num33 = 0;
			int num34 = 0;
			int num35 = 0;
			int num36 = 0;
			int num37 = 0;
			int num38 = 0;
			int num39 = 0;
			int num40 = 0;
			int num41 = 0;
			int num42 = 0;
			foreach (SyncChangeEntry syncChangeEntry in changeList)
			{
				if (syncChangeEntry.Submitted)
				{
					num42++;
				}
				switch (syncChangeEntry.ChangeType)
				{
				case ChangeType.Add:
					SyncEngine.IncrementCounts(syncChangeEntry, syncEngineState, ref num, ref num2, ref num3, ref num4, ref num5, ref num6, ref num7, ref num8);
					break;
				case ChangeType.Change:
					SyncEngine.IncrementCounts(syncChangeEntry, syncEngineState, ref num25, ref num26, ref num27, ref num28, ref num29, ref num30, ref num31, ref num32);
					break;
				case ChangeType.Delete:
					SyncEngine.IncrementCounts(syncChangeEntry, syncEngineState, ref num9, ref num10, ref num11, ref num12, ref num13, ref num14, ref num15, ref num16);
					break;
				case ChangeType.ReadFlagChange:
					SyncEngine.IncrementCounts(syncChangeEntry, syncEngineState, ref num33, ref num34, ref num35, ref num36, ref num37, ref num38, ref num39, ref num40);
					break;
				case ChangeType.SoftDelete:
					SyncEngine.IncrementCounts(syncChangeEntry, syncEngineState, ref num17, ref num18, ref num19, ref num20, ref num21, ref num22, ref num23, ref num24);
					break;
				}
				if (!syncChangeEntry.Persist)
				{
					num41++;
				}
			}
			syncEngineState.SyncHealthData.TotalItemsSubmittedToTransport = num42;
			syncEngineState.SyncHealthData.TotalItemAddsEnumeratedFromRemoteServer = num5 + num6 + num8 + num7;
			syncEngineState.SyncHealthData.TotalItemAddsAppliedToLocalServer = num8;
			syncEngineState.SyncHealthData.TotalItemAddsPermanentExceptions = num5;
			syncEngineState.SyncHealthData.TotalItemAddsTransientExceptions = num6;
			syncEngineState.SyncHealthData.TotalItemChangesPermanentExceptions = num29 + num37 + num21;
			syncEngineState.SyncHealthData.TotalItemChangesTransientExceptions = num30 + num38 + num22;
			syncEngineState.SyncHealthData.TotalItemChangesEnumeratedFromRemoteServer = num32 + num31 + num40 + num39 + num24 + num23 + syncEngineState.SyncHealthData.TotalItemChangesPermanentExceptions + syncEngineState.SyncHealthData.TotalItemChangesTransientExceptions;
			syncEngineState.SyncHealthData.TotalItemChangesAppliedToLocalServer = num32 + num40 + num24;
			syncEngineState.SyncHealthData.TotalItemDeletesEnumeratedFromRemoteServer = num13 + num14 + num16 + num15;
			syncEngineState.SyncHealthData.TotalItemDeletesAppliedToLocalServer = num16;
			syncEngineState.SyncHealthData.TotalItemDeletesPermanentExceptions = num13;
			syncEngineState.SyncHealthData.TotalItemDeletesTransientExceptions = num14;
			syncEngineState.SyncHealthData.TotalFolderAddsEnumeratedFromRemoteServer = num4 + num3 + num + num2;
			syncEngineState.SyncHealthData.TotalFolderAddsAppliedToLocalServer = num4;
			syncEngineState.SyncHealthData.TotalFolderAddsPermanentExceptions = num;
			syncEngineState.SyncHealthData.TotalFolderAddsTransientExceptions = num2;
			syncEngineState.SyncHealthData.TotalFolderChangesPermanentExceptions = num25 + num33 + num17;
			syncEngineState.SyncHealthData.TotalFolderChangesTransientExceptions = num26 + num34 + num18;
			syncEngineState.SyncHealthData.TotalFolderChangesEnumeratedFromRemoteServer = num28 + num27 + num36 + num35 + num20 + num19 + syncEngineState.SyncHealthData.TotalFolderChangesPermanentExceptions + syncEngineState.SyncHealthData.TotalFolderChangesTransientExceptions;
			syncEngineState.SyncHealthData.TotalFolderChangesAppliedToLocalServer = num28 + num36 + num20;
			syncEngineState.SyncHealthData.TotalFolderDeletesEnumeratedFromRemoteServer = num12 + num11 + num9 + num10;
			syncEngineState.SyncHealthData.TotalFolderDeletesAppliedToLocalServer = num12;
			syncEngineState.SyncHealthData.TotalFolderDeletesPermanentExceptions = num9;
			syncEngineState.SyncHealthData.TotalFolderDeletesTransientExceptions = num10;
			syncEngineState.SyncHealthData.PermanentItemErrorsCount = num5 + num29 + num13 + num21 + num37;
			syncEngineState.SyncHealthData.TransientItemErrorsCount = num6 + num30 + num14 + num22 + num38;
			syncEngineState.SyncHealthData.PermanentFolderErrorsCount = num + num25 + num9 + num17 + num33;
			syncEngineState.SyncHealthData.TransientFolderErrorsCount = num2 + num26 + num10 + num18 + num34;
			syncEngineState.SyncLogSession.LogInformation((TSLID)1051UL, SyncEngine.Tracer, "Cloud to Native: not-persisted changes: {0}.", new object[]
			{
				num41
			});
			syncEngineState.SyncLogSession.LogInformation((TSLID)1052UL, SyncEngine.Tracer, "Cloud to Native: successful Folder Operations: {0} ADDs, {1} CHANGEs, {2} SoftDeletes, {3} HardDeletes.", new object[]
			{
				num4,
				num28,
				num20,
				num12
			});
			syncEngineState.SyncLogSession.LogInformation((TSLID)1053UL, SyncEngine.Tracer, "Cloud to Native: unsuccessful Folder Operations: {0} Transient Failures, {1} Permanent Failures.", new object[]
			{
				syncEngineState.SyncHealthData.TransientFolderErrorsCount,
				syncEngineState.SyncHealthData.PermanentFolderErrorsCount
			});
			syncEngineState.SyncLogSession.LogInformation((TSLID)1054UL, SyncEngine.Tracer, "Cloud to Native: successful Item Operations: {0} ADDs, {1} CHANGEs, {2} READ-FLAGs, {3} SoftDeletes, {4} HardDeletes.", new object[]
			{
				num8,
				num32,
				num40,
				num24,
				num16
			});
			syncEngineState.SyncLogSession.LogInformation((TSLID)1055UL, SyncEngine.Tracer, "Cloud to Native: unsuccessful Item Operations: {0} Transient Failures, {1} Permanent Failures.", new object[]
			{
				syncEngineState.SyncHealthData.TransientItemErrorsCount,
				syncEngineState.SyncHealthData.PermanentItemErrorsCount
			});
			ISyncWorkerData userMailboxSubscription = syncEngineState.UserMailboxSubscription;
			userMailboxSubscription.UpdateItemStatistics((long)num8, (long)num5);
			if (!AggregationConfiguration.Instance.ReportDataLoggingDisabled)
			{
				SyncEngine.UpdateReportData(syncEngineState, changeList);
			}
		}

		private static void UpdateReportData(SyncEngineState syncEngineState, IList<SyncChangeEntry> changeList)
		{
			try
			{
				ReportData reportData = SkippedItemUtilities.GetReportData(syncEngineState.UserMailboxSubscription.SubscriptionGuid);
				reportData.Load(syncEngineState.SyncMailboxSession.MailboxSession.Mailbox.MapiStore);
				bool flag = SyncEngine.AddSkippedItems(changeList, reportData, syncEngineState.SyncLogSession);
				if (flag)
				{
					reportData.Flush(syncEngineState.SyncMailboxSession.MailboxSession.Mailbox.MapiStore);
				}
				reportData.Flush(syncEngineState.SyncMailboxSession.MailboxSession.Mailbox.MapiStore);
			}
			catch (StoragePermanentException ex)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)313UL, SyncEngine.Tracer, "Hit Storage Permanent Exception while updating report data in user mailbox: {0}.", new object[]
				{
					ex
				});
			}
			catch (StorageTransientException ex2)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)314UL, SyncEngine.Tracer, "Hit Storage Transient Exception while updating report data in user mailbox: {0}.", new object[]
				{
					ex2
				});
			}
			catch (MapiRetryableException ex3)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1015UL, SyncEngine.Tracer, "Hit transient MAPI transient Exception while updating report data in user mailbox: {0}.", new object[]
				{
					ex3
				});
			}
			catch (MapiPermanentException ex4)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1016UL, SyncEngine.Tracer, "Hit transient MAPI Permanent Exception while updating report data in user mailbox: {0}.", new object[]
				{
					ex4
				});
			}
		}

		private static bool AddSkippedItems(IList<SyncChangeEntry> changeList, ReportData reportData, SyncLogSession syncLogSession)
		{
			bool result = false;
			using (IEnumerator<SyncChangeEntry> enumerator = changeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SyncChangeEntry changeEntry = enumerator.Current;
					if (changeEntry.HasException)
					{
						SyncPermanentException ex = changeEntry.Exception as SyncPermanentException;
						if (ex != null && changeEntry.SyncReportObject != null)
						{
							ISyncReportObject syncReportObject = changeEntry.SyncReportObject;
							BadMessageRec badMessageRec = BadMessageRec.MissingItem(ex);
							badMessageRec.XmlData = changeEntry.SchemaType.ToString();
							badMessageRec.Sender = syncReportObject.Sender;
							badMessageRec.MessageClass = syncReportObject.MessageClass;
							badMessageRec.MessageSize = syncReportObject.MessageSize;
							badMessageRec.Subject = syncReportObject.Subject;
							badMessageRec.DateReceived = (DateTime?)syncReportObject.DateReceived;
							badMessageRec.DateSent = (DateTime?)syncReportObject.DateSent;
							badMessageRec.CloudId = changeEntry.CloudId;
							if (changeEntry.Properties.ContainsKey(NativeSyncStorageProvider.FolderNameProperty))
							{
								badMessageRec.FolderName = changeEntry.Properties[NativeSyncStorageProvider.FolderNameProperty];
							}
							int num = reportData.Entries.FindIndex((ReportEntry rd) => rd.BadItem != null && rd.BadItem.CloudId == changeEntry.CloudId);
							result = true;
							if (num == -1)
							{
								reportData.Append(badMessageRec.ToLocalizedString(), badMessageRec);
							}
							else
							{
								reportData.Entries[num] = new ReportEntry(badMessageRec.ToLocalizedString());
								syncLogSession.LogInformation((TSLID)1108UL, SyncEngine.Tracer, "Existing Report Entry for Cloud Id {0} was found in Report Data. Updating the entry", new object[]
								{
									changeEntry.CloudId
								});
							}
						}
					}
				}
			}
			return result;
		}

		private static void IncrementCounts(SyncChangeEntry change, SyncEngineState syncEngineState, ref int folderPermanentExceptions, ref int folderTransientExceptions, ref int folderSkipped, ref int folderSuccessful, ref int itemPermanentExceptions, ref int itemTransientExceptions, ref int itemSkipped, ref int itemSuccessful)
		{
			SchemaType schemaType = change.SchemaType;
			if (schemaType == SchemaType.Folder)
			{
				SyncEngine.IncrementCounts(change, syncEngineState, ref folderPermanentExceptions, ref folderTransientExceptions, ref folderSkipped, ref folderSuccessful);
				return;
			}
			SyncEngine.IncrementCounts(change, syncEngineState, ref itemPermanentExceptions, ref itemTransientExceptions, ref itemSkipped, ref itemSuccessful);
		}

		private static void IncrementCounts(SyncChangeEntry change, SyncEngineState syncEngineState, ref int permanentExceptions, ref int transientExceptions, ref int skipped, ref int successful)
		{
			bool flag = change.Exception is SyncPermanentException;
			bool flag2 = change.Exception is SyncTransientException;
			if (flag2)
			{
				transientExceptions++;
				SyncEngine.LogItemLevelError(syncEngineState.SyncLogSession, change, false, syncEngineState.SyncMailboxSession.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId.GetTenantGuid(), syncEngineState.UserMailboxSubscription.AggregationType);
				syncEngineState.SyncHealthData.Exceptions.Add(change.Exception);
				return;
			}
			if (flag)
			{
				permanentExceptions++;
				SyncPermanentException ex = change.Exception as SyncPermanentException;
				SyncEngine.LogItemLevelError(syncEngineState.SyncLogSession, change, ex.IsPromotedFromTransientException, syncEngineState.SyncMailboxSession.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId.GetTenantGuid(), syncEngineState.UserMailboxSubscription.AggregationType);
				if (change.ChangeType == ChangeType.Add && change.SchemaType != SchemaType.Folder)
				{
					SyncEngine.IncrementPermanentItemErrorCountBasedOnException(change.Exception.InnerException, syncEngineState.SyncHealthData);
				}
				syncEngineState.SyncHealthData.Exceptions.Add(change.Exception);
				return;
			}
			if (change.ApplyAttempted)
			{
				successful++;
				return;
			}
			skipped++;
		}

		private static void LogItemLevelError(SyncLogSession syncLogSession, SyncChangeEntry change, bool isTransientPromotedToPermanent, Guid tenantGuid, AggregationType aggregationType)
		{
			string value = (change.Exception.InnerException == null) ? string.Empty : change.Exception.InnerException.GetType().Name;
			syncLogSession.LogItemLevelError((TSLID)1400UL, new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("TenantGuid", tenantGuid.ToString()),
				new KeyValuePair<string, object>("AggregationType", aggregationType.ToString()),
				new KeyValuePair<string, object>("ChangeType", change.ChangeType.ToString()),
				new KeyValuePair<string, object>("SchemaType", change.SchemaType.ToString()),
				new KeyValuePair<string, object>("CloudId", (change.CloudId == null) ? string.Empty : change.CloudId),
				new KeyValuePair<string, object>("NativeId", (change.NativeId == null) ? string.Empty : change.NativeId.ToString()),
				new KeyValuePair<string, object>("CloudFolderId", (change.CloudFolderId == null) ? string.Empty : change.CloudFolderId),
				new KeyValuePair<string, object>("NativeFolderId", (change.NativeFolderId == null) ? string.Empty : change.NativeFolderId.ToString()),
				new KeyValuePair<string, object>("IsTransientPromotedToPermanent", isTransientPromotedToPermanent.ToString()),
				new KeyValuePair<string, object>("ExceptionType", change.Exception.GetType().Name),
				new KeyValuePair<string, object>("InnerExceptionType", value),
				new KeyValuePair<string, object>("ExceptionErrorCode", LocalizedException.GenerateErrorCode(change.Exception)),
				new KeyValuePair<string, object>("ExceptionToString", change.Exception.ToString())
			});
		}

		private static void SyncCompletedSuccessfullyWithChanges(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)946UL, "SyncCompletedSuccessfullyWithChanges.", new object[0]);
			SyncEngine.Instance.AssertUserMailboxSubscriptionExists(syncEngineState);
			SyncEngine.Instance.UpdateUserMailboxSubscriptionAndInvokeMigrationAgent(asyncResult, syncEngineState.MoreItemsAvailable, false, false, null);
		}

		private static void SyncFailedWithoutSubscription(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, Exception syncException, bool disableSubscription, bool invalidState)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1004UL, "SyncFailedWithoutSubscription.", new object[0]);
			SyncEngine.Instance.AssertUserMailboxSubscriptionDoNotExist(syncEngineState);
			SyncEngine.Instance.DoSubscriptionAgentManagerWorkWithoutSubscription(asyncResult.State, syncException);
			SyncEngineResultData syncEngineResultData = new SyncEngineResultData(syncEngineState.StartSyncTime, syncEngineState.CloudItemsSynced, false, disableSubscription, invalidState, null, null, SyncPhase.Initial);
			SyncEngine.ProcessCompleted(asyncResult, syncEngineResultData, syncException);
		}

		private static void SyncFailed(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, Exception exception, bool disableSubscription, bool invalidState)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1005UL, "SyncFailed.", new object[0]);
			if (syncEngineState.UserMailboxSubscription == null)
			{
				SyncEngine.SyncFailedWithoutSubscription(asyncResult, exception, disableSubscription, invalidState);
				return;
			}
			SyncEngine.SyncFailedWithSubscription(asyncResult, exception, disableSubscription, invalidState);
		}

		private static void SyncFailedWithSubscription(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, Exception exception, bool disableSubscription, bool invalidState)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1007UL, "SyncFailedWithSubscription.", new object[0]);
			SyncEngine.Instance.AssertUserMailboxSubscriptionExists(syncEngineState);
			SyncEngine.Instance.UpdateUserMailboxSubscriptionAndInvokeMigrationAgent(asyncResult, false, disableSubscription, invalidState, exception);
		}

		private static void SyncCompletedSuccessfullyWithPostSyncError(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, Exception postSyncError)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1008UL, "SyncCompletedSuccessfullyWithPostSyncError.", new object[0]);
			SyncEngine.Instance.UpdateUserMailboxSubscriptionAndInvokeMigrationAgent(asyncResult, syncEngineState.MoreItemsAvailable, false, false, postSyncError);
		}

		private static bool CheckIfRemoteServerIsSlow(SyncEngineState syncEngineState, ISyncWorkerData subscription, out ISyncException exception)
		{
			RemoteServerTooSlowException innerException;
			if (syncEngineState.RemoteServerHealthChecker.IsRemoteServerSlow(syncEngineState, subscription, out innerException))
			{
				exception = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.RemoteServerIsSlow, innerException, true);
				return true;
			}
			exception = null;
			return false;
		}

		private static void ProcessCompleted(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, SyncEngineResultData syncEngineResultData, Exception exception)
		{
			asyncResult.State.SyncEngineState.SyncLogSession.LogInformation((TSLID)1009UL, "SyncEngine.ProcessCompleted::SyncMode:{0}, SyncEngineResultData:{1}, Exception:{2}.", new object[]
			{
				asyncResult.State.SyncEngineState.SyncMode,
				syncEngineResultData,
				exception
			});
			asyncResult.PendingAsyncResult = null;
			if (exception != null)
			{
				asyncResult.ProcessCompleted(syncEngineResultData, exception);
				return;
			}
			asyncResult.ProcessCompleted(syncEngineResultData);
		}

		private static void OnGetItemCompleted(IAsyncResult asyncResult)
		{
			AsyncResult<SyncEngineState, SyncChangeEntry> asyncResult2 = (AsyncResult<SyncEngineState, SyncChangeEntry>)asyncResult.AsyncState;
			SyncEngineState state = asyncResult2.State;
			state.SyncLogSession.LogDebugging((TSLID)1057UL, SyncEngine.Tracer, "EndGetItem to be called on Cloud Provider, sync engine state: {0}.", new object[]
			{
				state
			});
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = state.CloudProvider.EndGetItem(asyncResult);
			asyncResult2.ProcessCompleted(asyncOperationResult.Data, asyncOperationResult.Exception);
		}

		[Conditional("DEBUG")]
		private static void ValidateSyncOperationResult(AsyncOperationResult<SyncProviderResultData> syncOperationResult)
		{
			if (syncOperationResult.Data != null && syncOperationResult.Data.ChangeList != null && (syncOperationResult.Exception == null || !(syncOperationResult.Exception is SyncTransientException)))
			{
				foreach (SyncChangeEntry syncChangeEntry in syncOperationResult.Data.ChangeList)
				{
					if (syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
					Exception exception = syncChangeEntry.Exception;
				}
			}
		}

		private static bool ShouldTerminateSync(SyncEngineState syncEngineState)
		{
			if ((syncEngineState.UserMailboxSubscription.SyncQuirks & SyncQuirks.DoNotTerminateSlowSyncs) != SyncQuirks.None)
			{
				return false;
			}
			if (AggregationConfiguration.Instance.TerminateSlowSyncEnabled && !syncEngineState.WasSyncInterrupted)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow - syncEngineState.StartSyncTime;
				if (timeSpan >= AggregationConfiguration.Instance.SyncDurationThreshold)
				{
					syncEngineState.SyncLogSession.LogDebugging((TSLID)1362UL, SyncEngine.Tracer, "Requesting termination of sync as it has exceeded sync duration threshold: {0}.", new object[]
					{
						timeSpan
					});
					return true;
				}
			}
			return syncEngineState.WasSyncInterrupted;
		}

		private static bool IsMailboxOverQuota(SyncEngineState syncEngineState, out ISyncException syncException)
		{
			ulong requiredFreeBytes = Math.Min(2UL * AggregationConfiguration.Instance.GetMaxDownloadSizePerConnection(syncEngineState.UserMailboxSubscription.AggregationType).ToBytes(), AggregationConfiguration.Instance.MinFreeSpaceRequired.ToBytes());
			if (syncEngineState.SubscriptionInformationLoader.IsMailboxOverQuota(syncEngineState.SyncMailboxSession, syncEngineState.SyncLogSession, requiredFreeBytes))
			{
				syncException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.LabsMailboxQuotaWarning, new MailboxOverQuotaException(), true);
				return true;
			}
			syncException = null;
			return false;
		}

		private static SyncMode DetermineSyncMode(AggregationWorkItem workItem, IRemoteServerHealthChecker remoteServerHealthChecker)
		{
			if (AggregationConfiguration.Instance.AlwaysLoadSubscription)
			{
				workItem.SyncLogSession.LogVerbose((TSLID)1023UL, "IsSubscriptionLoadRequired:true per configuration: AlwaysLoadSubscription.", new object[0]);
				return SyncMode.EnumerateChangesMode;
			}
			if (workItem.Subscription == null)
			{
				workItem.SyncLogSession.LogVerbose((TSLID)1029UL, "IsSubscriptionLoadRequired:true as subscription was not passed during dispatch.", new object[0]);
				return SyncMode.EnumerateChangesMode;
			}
			if (workItem.Subscription.IsMigration)
			{
				workItem.SyncLogSession.LogVerbose((TSLID)1010UL, "IsSubscriptionLoadRequired:true since we don't do the i/o optimization for migration.", new object[0]);
				return SyncMode.EnumerateChangesMode;
			}
			if (workItem.Subscription.SyncPhase != SyncPhase.Incremental)
			{
				workItem.SyncLogSession.LogVerbose((TSLID)1011UL, "IsSubscriptionLoadRequired:true since we do i/o optimization for Incremental sync only and not for: {0} sync.", new object[]
				{
					workItem.Subscription.SyncPhase
				});
				return SyncMode.EnumerateChangesMode;
			}
			if (!workItem.IsMailboxServerSyncWatermarkAvailable)
			{
				workItem.SyncLogSession.LogVerbose((TSLID)320UL, "IsSubscriptionLoadRequired:true since we don't have sync watermark from the mailbox server.", new object[0]);
				return SyncMode.EnumerateChangesMode;
			}
			bool flag = SyncEngine.WasPreviousSyncSuccessful(workItem.Subscription);
			bool flag2 = workItem.SubscriptionPoisonStatus == SyncPoisonStatus.PoisonousSubscription;
			RemoteServerHealthState remoteServerHealthState = remoteServerHealthChecker.GetRemoteServerHealthState(workItem.Subscription);
			bool flag3 = remoteServerHealthState != RemoteServerHealthState.Clean;
			bool flag4 = SyncEngine.IsSubscriptionPasswordInvalid(workItem.Subscription);
			bool flag5 = !flag || workItem.IsRecoverySyncMode || workItem.IsSyncNow || flag2 || workItem.Subscription.IsMirrored || !workItem.Subscription.IsValid || workItem.Subscription.Inactive || flag3 || flag4;
			workItem.SyncLogSession.LogVerbose((TSLID)1031UL, "IsSubscriptionLoadRequired:{0} = wasPreviousSyncSuccessful:{1},underRecovery:{2},isSyncNow:{3},shouldMarkSubscriptionAsPoisonous:{4},isMirrored:{5},isInValid:{6},Inactive:{7},isRemoteServerUnHealthy:{8},isSubscriptionPasswordInvalid:{9}.", new object[]
			{
				flag5,
				flag,
				workItem.IsRecoverySyncMode,
				workItem.IsSyncNow,
				flag2,
				workItem.Subscription.IsMirrored,
				!workItem.Subscription.IsValid,
				workItem.Subscription.Inactive,
				flag3,
				flag4
			});
			if (!flag5)
			{
				return SyncMode.CheckForChangesMode;
			}
			return SyncMode.EnumerateChangesMode;
		}

		private static bool WasPreviousSyncSuccessful(ISyncWorkerData subscription)
		{
			return subscription.LastSyncTime != null && subscription.LastSuccessfulSyncTime != null && !(subscription.LastSuccessfulSyncTime.Value != subscription.LastSyncTime.Value) && subscription.Status == AggregationStatus.Succeeded && subscription.DetailedAggregationStatus == DetailedAggregationStatus.None && !(subscription.Diagnostics != string.Empty) && !(subscription.LastSuccessfulSyncTime.Value != subscription.AdjustedLastSuccessfulSyncTime) && !(subscription.OutageDetectionDiagnostics != string.Empty);
		}

		private static bool DoesRemoteServerHealthPermitSync(SyncEngineState syncEngineState, ISyncWorkerData subscription, out ISyncException exception, out bool disableSubscription)
		{
			exception = null;
			disableSubscription = false;
			RemoteServerHealthState remoteServerHealthState = syncEngineState.RemoteServerHealthChecker.GetRemoteServerHealthState(subscription);
			if (remoteServerHealthState == RemoteServerHealthState.Clean)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)1433UL, "Remote Server is in clean state, continue with the sync.", new object[0]);
				return true;
			}
			if (remoteServerHealthState == RemoteServerHealthState.BackedOff)
			{
				exception = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.RemoteServerIsBackedOff, new RemoteServerIsBackedOffException(), true);
				syncEngineState.SyncLogSession.LogError((TSLID)1434UL, "Remote Server is in backoff state, failing the sync with error:{0}.", new object[]
				{
					exception
				});
				return false;
			}
			if (remoteServerHealthState == RemoteServerHealthState.Poisonous)
			{
				exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.RemoteServerIsPoisonous, new PoisonousRemoteServerException());
				syncEngineState.SyncLogSession.LogError((TSLID)1435UL, "Remote Server is in Poisonous state, failing the sync with error:{0}.", new object[]
				{
					exception
				});
				disableSubscription = true;
				return false;
			}
			throw new InvalidDataException("Unknown RemoteServerHealthState Value:" + remoteServerHealthState);
		}

		private static bool DoesSubscriptionHaveInvalidPassword(ISyncWorkerData subscription, SyncLogSession syncLogSession, out ISyncException exception)
		{
			exception = null;
			if (SyncEngine.IsSubscriptionPasswordInvalid(subscription))
			{
				syncLogSession.LogError((TSLID)955UL, "Will not process subscription ID {0} as it has an empty password.", new object[]
				{
					subscription.SubscriptionGuid
				});
				exception = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.AuthenticationError, new SubscriptionInvalidPasswordException(), true);
				return true;
			}
			return false;
		}

		private static bool IsSubscriptionPasswordInvalid(ISyncWorkerData subscription)
		{
			PimAggregationSubscription pimAggregationSubscription = subscription as PimAggregationSubscription;
			return pimAggregationSubscription != null && pimAggregationSubscription.PasswordRequired && (pimAggregationSubscription.LogonPasswordSecured == null || pimAggregationSubscription.LogonPasswordSecured.Length == 0);
		}

		private static bool DoesSubscriptionStatusPermitSync(SyncEngineState syncEngineState, out ISyncException exception, out bool disableSubscription)
		{
			disableSubscription = false;
			ISyncWorkerData userMailboxSubscription = syncEngineState.UserMailboxSubscription;
			if (!userMailboxSubscription.IsValid)
			{
				if (userMailboxSubscription.Status == AggregationStatus.InvalidVersion)
				{
					syncEngineState.SyncLogSession.LogError((TSLID)950UL, "Will not process subscription ID {0} as its status is marked as {1}.", new object[]
					{
						userMailboxSubscription.SubscriptionGuid,
						userMailboxSubscription.Status
					});
					exception = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.None, new SubscriptionInvalidVersionException(), true);
					return false;
				}
				if (userMailboxSubscription.Status == AggregationStatus.Poisonous && userMailboxSubscription.DetailedAggregationStatus == DetailedAggregationStatus.Corrupted)
				{
					syncEngineState.SyncLogSession.LogError((TSLID)951UL, "Will not process subscription ID {0} as its status is marked as {1} and detailed status as {2}.", new object[]
					{
						userMailboxSubscription.SubscriptionGuid,
						userMailboxSubscription.Status,
						userMailboxSubscription.DetailedAggregationStatus
					});
					exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.Corrupted, new CorruptSubscriptionException(userMailboxSubscription.SubscriptionGuid));
					disableSubscription = true;
					return false;
				}
			}
			if (syncEngineState.SubscriptionPoisonStatus == SyncPoisonStatus.PoisonousSubscription && !SyncEngine.Instance.TryMarkSubscriptionAsPoisonous(syncEngineState, out exception))
			{
				return false;
			}
			if (userMailboxSubscription.Status == AggregationStatus.Poisonous || userMailboxSubscription.Status == AggregationStatus.Disabled)
			{
				syncEngineState.SyncLogSession.LogError((TSLID)952UL, "Subscription: {0} with Aggregation Status: {1} is found, skipping sync and completing operation right away.", new object[]
				{
					userMailboxSubscription.SubscriptionGuid,
					userMailboxSubscription.Status
				});
				exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.None, new SubscriptionSyncException(userMailboxSubscription.Name));
				disableSubscription = true;
				return false;
			}
			exception = null;
			return true;
		}

		private static void OnRemoteServerRoundtripComplete(object sender, RemoteServerRoundtripCompleteEventArgs remoteServerRoundtripCompleteEventArgs)
		{
			SyncUtilities.ThrowIfArgumentNull("remoteServerRoundtripCompleteEventArgs", remoteServerRoundtripCompleteEventArgs);
			double totalMilliseconds = remoteServerRoundtripCompleteEventArgs.RoundtripTime.TotalMilliseconds;
			long remoteServerLatency = SyncUtilities.ConvertToLong(totalMilliseconds);
			AggregationComponent.RecordRemoteServerLatency(remoteServerRoundtripCompleteEventArgs.ServerName, remoteServerLatency);
		}

		private static bool HasStatusForUrgentSyncStatusPrompt(SyncTransientException syncTransientException)
		{
			return syncTransientException.DetailedAggregationStatus == DetailedAggregationStatus.LabsMailboxQuotaWarning || syncTransientException.DetailedAggregationStatus == DetailedAggregationStatus.RemoteMailboxQuotaWarning;
		}

		private static bool IsNonPromotableException(SyncTransientException syncTransientException)
		{
			return syncTransientException.InnerException is NonPromotableTransientException;
		}

		private static bool HasStatusForExtendedDisabledThreshold(SyncTransientException syncTransientException)
		{
			return syncTransientException.DetailedAggregationStatus == DetailedAggregationStatus.LabsMailboxQuotaWarning || syncTransientException.DetailedAggregationStatus == DetailedAggregationStatus.RemoteMailboxQuotaWarning;
		}

		private static bool IsCancelRequested(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			if (syncEngineState.TryCancel)
			{
				syncEngineState.SyncLogSession.LogInformation((TSLID)1001UL, SyncEngine.Tracer, "Stopping due to canceling at {0}.", new object[]
				{
					syncEngineState.CurrentStep
				});
				asyncResult.ProcessCanceled();
				return true;
			}
			return false;
		}

		private static void UpdateSyncHealthData(SyncEngineState syncEngineState, AsyncOperationResult<SyncEngineResultData> syncEngineResult)
		{
			if (syncEngineState != null)
			{
				if (syncEngineResult.Data != null)
				{
					TimeSpan syncDuration = ExDateTime.UtcNow - syncEngineResult.Data.StartSyncTime;
					syncEngineState.SyncHealthData.SyncDuration = syncDuration;
				}
				syncEngineState.SyncHealthData.IsPermanentSyncError = (syncEngineResult.Exception is SyncPermanentException);
				syncEngineState.SyncHealthData.IsTransientSyncError = (syncEngineResult.Exception is SyncTransientException);
				syncEngineState.SyncHealthData.SyncEngineException = syncEngineResult.Exception;
				if (syncEngineState.CloudProviderState != null)
				{
					syncEngineState.SyncHealthData.TotalBytesEnumeratedFromRemoteServer = syncEngineState.CloudProviderState.BytesDownloaded;
					syncEngineState.SyncHealthData.TotalSuccessfulRemoteRoundtrips = syncEngineState.CloudProviderState.TotalSuccessfulRoundtrips;
					syncEngineState.SyncHealthData.AverageSuccessfulRemoteRoundtripTime = syncEngineState.CloudProviderState.AverageSuccessfulRoundtripTime;
					syncEngineState.SyncHealthData.TotalUnsuccessfulRemoteRoundtrips = syncEngineState.CloudProviderState.TotalUnsuccessfulRoundtrips;
					syncEngineState.SyncHealthData.AverageUnsuccessfulRemoteRoundtripTime = syncEngineState.CloudProviderState.AverageUnsuccessfulRoundtripTime;
					syncEngineState.SyncHealthData.CloudStatistics = syncEngineState.CloudProviderState.CloudStatistics;
				}
				if (syncEngineState.NativeProviderState != null)
				{
					syncEngineState.SyncHealthData.TotalSuccessfulNativeRoundtrips = syncEngineState.NativeProviderState.TotalSuccessfulRoundtrips;
					syncEngineState.SyncHealthData.AverageSuccessfulNativeRoundtripTime = syncEngineState.NativeProviderState.AverageSuccessfulRoundtripTime;
					syncEngineState.SyncHealthData.TotalUnsuccessfulNativeRoundtrips = syncEngineState.NativeProviderState.TotalUnsuccessfulRoundtrips;
					syncEngineState.SyncHealthData.AverageUnsuccessfulNativeRoundtripTime = syncEngineState.NativeProviderState.AverageUnsuccessfulRoundtripTime;
					syncEngineState.SyncHealthData.AverageNativeBackoffTime = syncEngineState.NativeProviderState.AverageBackoffTime;
					syncEngineState.SyncHealthData.RecoverySync = syncEngineState.NativeProviderState.UnderRecovery;
					syncEngineState.SyncHealthData.ThrottlingStatistics = syncEngineState.NativeProviderState.ThrottlingStatistics;
				}
				syncEngineState.SyncHealthData.TotalSuccessfulEngineRoundtrips = syncEngineState.TotalSuccessfulRoundtrips;
				syncEngineState.SyncHealthData.AverageSuccessfulEngineRoundtripTime = syncEngineState.AverageSuccessfulRoundtripTime;
				syncEngineState.SyncHealthData.TotalUnsuccessfulEngineRoundtrips = syncEngineState.TotalUnsuccessfulRoundtrips;
				syncEngineState.SyncHealthData.AverageUnsuccessfulEngineRoundtripTime = syncEngineState.AverageUnsuccessfulRoundtripTime;
				syncEngineState.SyncHealthData.AverageEngineBackoffTime = syncEngineState.AverageBackoffTime;
				if (syncEngineState.SyncHealthData.ThrottlingStatistics == null)
				{
					syncEngineState.SyncHealthData.ThrottlingStatistics = syncEngineState.ThrottlingStatistics;
					return;
				}
				syncEngineState.SyncHealthData.ThrottlingStatistics.Update(syncEngineState.ThrottlingStatistics);
			}
		}

		private bool TryMarkSubscriptionAsPoisonous(SyncEngineState syncEngineState, out ISyncException exception)
		{
			ISyncWorkerData userMailboxSubscription = syncEngineState.UserMailboxSubscription;
			syncEngineState.SyncLogSession.LogError((TSLID)953UL, "Marking subscription: {0} as Poisonous.", new object[]
			{
				userMailboxSubscription.SubscriptionGuid
			});
			userMailboxSubscription.LastSyncTime = new DateTime?(DateTime.UtcNow);
			userMailboxSubscription.Status = AggregationStatus.Poisonous;
			userMailboxSubscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
			userMailboxSubscription.PoisonCallstack = syncEngineState.SubscriptionPoisonCallstack;
			Exception ex;
			syncEngineState.SubscriptionInformationLoader.TrySaveSubscription(syncEngineState.SyncMailboxSession, userMailboxSubscription, new EventHandler<RoundtripCompleteEventArgs>(syncEngineState.OnRoundtripComplete), out ex);
			if (ex != null)
			{
				exception = (ISyncException)ex;
				syncEngineState.SyncLogSession.LogError((TSLID)954UL, "Failed to update poison subscription: {0}, error:{1}.", new object[]
				{
					userMailboxSubscription.SubscriptionGuid,
					exception
				});
				return false;
			}
			AggregationComponent.EventLogger.LogEvent(TransportSyncWorkerEventLogConstants.Tuple_PoisonSubscriptionDetected, null, new object[]
			{
				userMailboxSubscription.SubscriptionGuid,
				syncEngineState.LegacyDN,
				syncEngineState.SubscriptionPoisonCallstack
			});
			SyncPoisonHandler.ClearPoisonContext(userMailboxSubscription.SubscriptionGuid, syncEngineState.SubscriptionPoisonStatus, syncEngineState.SyncLogSession);
			exception = null;
			return true;
		}

		private void GenerateSubscriptionNotificationIfApplicable(SyncEngineState syncEngineState, AsyncOperationResult<SyncEngineResultData> workItemResultData)
		{
			if (workItemResultData.IsSucceeded)
			{
				return;
			}
			if (syncEngineState.HasConnectedMailboxSession() && syncEngineState.UserMailboxSubscription != null && !syncEngineState.SubscriptionNotificationSent)
			{
				bool flag = SubscriptionNotificationHelper.Instance.ShouldGenerateSubscriptionNotification(syncEngineState.UserMailboxSubscriptionStatusBeforeSync, syncEngineState.UserMailboxSubscription, workItemResultData.Exception, syncEngineState.SyncLogSession);
				if (flag)
				{
					syncEngineState.SyncLogSession.LogDebugging((TSLID)943UL, SyncEngine.Tracer, (long)this.GetHashCode(), "Trying to send a subscription notification for status {0}, detailed status {1}.", new object[]
					{
						syncEngineState.UserMailboxSubscription.Status,
						syncEngineState.UserMailboxSubscription.DetailedAggregationStatus
					});
					bool flag3;
					bool flag2 = syncEngineState.SubscriptionInformationLoader.TrySendSubscriptionNotificationEmail(syncEngineState.SyncMailboxSession, syncEngineState.UserMailboxSubscription, syncEngineState.SyncLogSession, out flag3);
					syncEngineState.SyncLogSession.LogVerbose((TSLID)944UL, SyncEngine.Tracer, (long)this.GetHashCode(), "Subscription Notification sent {0}, retry {1}.", new object[]
					{
						flag2,
						flag3
					});
					if (flag2 || !flag3)
					{
						syncEngineState.SetSubscriptionNotificationSent();
					}
				}
			}
		}

		private void UpdateUserMailboxSubscriptionAndInvokeMigrationAgent(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult, bool moreDataAvailable, bool disableSubscription, bool invalidState, Exception ex)
		{
			AggregationWorkItem state = asyncResult.State;
			SyncEngineState syncEngineState = state.SyncEngineState;
			this.AssertUserMailboxSubscriptionExists(syncEngineState);
			bool flag2;
			bool flag3;
			bool flag = this.UpdateUserMailboxSubscriptionStatus(ex, moreDataAvailable, state, out flag2, out flag3);
			if (!flag3)
			{
				disableSubscription = (disableSubscription || flag2);
				bool flag4;
				bool flag5;
				this.DoSubscriptionAgentManagerWorkWithUserMailboxSubscription(state, ex, ref disableSubscription, out flag4, out flag5);
				syncEngineState.SyncLogSession.LogVerbose((TSLID)974UL, SyncEngine.Tracer, "Checking to Update/Delete Subscription in User Mailbox. statusChangeRequiresSubscriptionUpdate:{0}, agentManagerRequestsSubscriptionUpdate:{1}, agentManagerRequestsSubscriptionDelete:{2}", new object[]
				{
					flag,
					flag4,
					flag5
				});
				if (flag || flag4 || flag5)
				{
					if (flag5)
					{
						this.DeleteSubscriptionFromUserMailbox(syncEngineState);
					}
					else
					{
						this.UpdateSubscriptionInUserMailbox(syncEngineState);
					}
				}
			}
			SyncEngineResultData syncEngineResultData = new SyncEngineResultData(state.SyncEngineState.StartSyncTime, state.SyncEngineState.CloudItemsSynced, moreDataAvailable, disableSubscription, invalidState, state.SyncEngineState.UpdatedSyncWatermark, state.SyncEngineState.UserMailboxSubscription, state.SyncEngineState.SyncPhaseBeforeSync);
			SyncEngine.ProcessCompleted(asyncResult, syncEngineResultData, ex);
		}

		private void UpdateSubscriptionInUserMailbox(SyncEngineState syncEngineState)
		{
			this.AssertUserMailboxSubscriptionExists(syncEngineState);
			if (!syncEngineState.HasConnectedMailboxSession())
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)321UL, "Skipped update subscription as we didn't have a connected mailbox session.", new object[0]);
				return;
			}
			Exception ex;
			syncEngineState.SubscriptionInformationLoader.TrySaveSubscription(syncEngineState.SyncMailboxSession, syncEngineState.UserMailboxSubscription, new EventHandler<RoundtripCompleteEventArgs>(syncEngineState.ConnectionStatistics.OnRoundtripComplete), out ex);
			if (ex != null)
			{
				if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException is ObjectNotFoundException)
				{
					syncEngineState.SyncLogSession.LogError((TSLID)959UL, SyncEngine.Tracer, "Subscription not found. Removing Sync state.", new object[0]);
					if (!StateStorage.TryDelete(syncEngineState.SyncMailboxSession.MailboxSession, syncEngineState.UserMailboxSubscription, new EventHandler<RoundtripCompleteEventArgs>(syncEngineState.ConnectionStatistics.OnRoundtripComplete)))
					{
						syncEngineState.SyncLogSession.LogError((TSLID)960UL, SyncEngine.Tracer, "Could not delete the sync state for this subscription.", new object[0]);
						return;
					}
				}
				else
				{
					syncEngineState.SyncLogSession.LogError((TSLID)961UL, SyncEngine.Tracer, "Failed to update subscription: {0}.", new object[]
					{
						ex
					});
				}
			}
		}

		private void DeleteSubscriptionFromUserMailbox(SyncEngineState syncEngineState)
		{
			this.AssertUserMailboxSubscriptionExists(syncEngineState);
			syncEngineState.SyncLogSession.LogVerbose((TSLID)973UL, SyncEngine.Tracer, "The agents have marked the subscription to be deleted. Attempting to delete it.", new object[0]);
			if (!syncEngineState.HasConnectedMailboxSession())
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)1017UL, "Skipped delete subscription request as we didn't have a connected mailbox session or protocol handler state.", new object[0]);
				return;
			}
			if (!syncEngineState.SubscriptionInformationLoader.TryDeleteSubscription(syncEngineState.SyncMailboxSession, syncEngineState.UserMailboxSubscription, new EventHandler<RoundtripCompleteEventArgs>(syncEngineState.ConnectionStatistics.OnRoundtripComplete)))
			{
				syncEngineState.SyncLogSession.LogError((TSLID)1018UL, "Failed to Delete subscription in your mailbox.", new object[0]);
			}
		}

		private void SyncCompletedSuccessfulyWithNoChanges(AsyncResult<AggregationWorkItem, SyncEngineResultData> asyncResult)
		{
			SyncEngineState syncEngineState = asyncResult.State.SyncEngineState;
			syncEngineState.SyncLogSession.LogDebugging((TSLID)1019UL, "SyncCompletedSuccessfulyWithNoChanges.", new object[0]);
			this.AssertMailboxServerSubscriptionExists(syncEngineState);
			syncEngineState.MailboxServerSubscription.LastSyncTime = new DateTime?(DateTime.UtcNow);
			syncEngineState.MailboxServerSubscription.LastSuccessfulSyncTime = syncEngineState.MailboxServerSubscription.LastSyncTime;
			syncEngineState.MailboxServerSubscription.AdjustedLastSuccessfulSyncTime = syncEngineState.MailboxServerSubscription.LastSuccessfulSyncTime.Value;
			this.DoSubscriptionAgentManagerWorkWithMailboxServerSubscription(asyncResult.State, null);
			SyncEngineResultData syncEngineResultData = new SyncEngineResultData(syncEngineState.StartSyncTime, 0, false, false, false, null, syncEngineState.MailboxServerSubscription, syncEngineState.SyncPhaseBeforeSync);
			SyncEngine.ProcessCompleted(asyncResult, syncEngineResultData, null);
		}

		private bool UpdateUserMailboxSubscriptionStatus(Exception exception, bool moreDataAvailable, AggregationWorkItem workItem, out bool disableSubscription, out bool stillInRetry)
		{
			SyncEngineState syncEngineState = workItem.SyncEngineState;
			this.AssertUserMailboxSubscriptionExists(syncEngineState);
			disableSubscription = false;
			stillInRetry = false;
			syncEngineState.UserMailboxSubscription.LastSyncTime = new DateTime?(DateTime.UtcNow);
			this.UpdateSubscriptionSyncPhase(syncEngineState, moreDataAvailable, exception);
			if (exception == null)
			{
				syncEngineState.UserMailboxSubscription.LastSuccessfulSyncTime = syncEngineState.UserMailboxSubscription.LastSyncTime;
				syncEngineState.UserMailboxSubscription.AdjustedLastSuccessfulSyncTime = syncEngineState.UserMailboxSubscription.LastSyncTime.Value;
				syncEngineState.UserMailboxSubscription.OutageDetectionDiagnostics = string.Empty;
				if (moreDataAvailable)
				{
					syncEngineState.UserMailboxSubscription.Status = AggregationStatus.InProgress;
				}
				else
				{
					syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Succeeded;
				}
				syncEngineState.UserMailboxSubscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
			}
			else
			{
				if (exception is OperationCanceledException)
				{
					syncEngineState.SyncLogSession.LogDebugging((TSLID)964UL, SyncEngine.Tracer, "Skipping updating the subscription as the operation was canceled.", new object[0]);
					return false;
				}
				SyncTransientException ex = exception as SyncTransientException;
				if (ex == null || ex.NeedsBackoff)
				{
					workItem.MaxOutRetryCount();
				}
				else if (!workItem.IsMaximumNumberOfAttemptsReached)
				{
					syncEngineState.SyncLogSession.LogVerbose((TSLID)965UL, SyncEngine.Tracer, "Skipping updating the subscription with a transient error since we're in retry number {0}.", new object[]
					{
						workItem.CurrentRetryCount
					});
					stillInRetry = true;
					return false;
				}
				if (!syncEngineState.UserMailboxSubscription.IsValid || syncEngineState.UserMailboxSubscription.Inactive)
				{
					syncEngineState.SyncLogSession.LogVerbose((TSLID)966UL, "The subscription will not be updated due to its status: {0}.", new object[]
					{
						syncEngineState.UserMailboxSubscription.Status
					});
					return false;
				}
				SyncPermanentException ex2 = exception as SyncPermanentException;
				EventLogEntry eventLogEntry;
				if (ex2 == null)
				{
					OutageAdjuster outageAdjuster = new OutageAdjuster();
					outageAdjuster.Execute(syncEngineState.UserMailboxSubscription, syncEngineState.UserMailboxSubscriptionCachedLastSyncTime, AggregationConfiguration.Instance.OutageDetectionThreshold, syncEngineState.SyncLogSession, SyncEngine.Tracer, Environment.MachineName, workItem.DatabaseGuid);
					TimeSpan t = syncEngineState.UserMailboxSubscription.LastSyncTime.Value - syncEngineState.UserMailboxSubscription.AdjustedLastSuccessfulSyncTime;
					bool flag = SyncEngine.HasStatusForExtendedDisabledThreshold(ex);
					bool flag2 = SyncEngine.HasStatusForUrgentSyncStatusPrompt(ex);
					if (!SyncEngine.IsNonPromotableException(ex) && (t >= AggregationConfiguration.Instance.ExtendedDisableSubscriptionThreshold || (!flag && t >= AggregationConfiguration.Instance.DisableSubscriptionThreshold)))
					{
						if (flag)
						{
							syncEngineState.SyncLogSession.LogError((TSLID)967UL, SyncEngine.Tracer, "The subscription with a transient exception crossed the extended disable threshold {0}.", new object[]
							{
								AggregationConfiguration.Instance.ExtendedDisableSubscriptionThreshold
							});
						}
						else
						{
							syncEngineState.SyncLogSession.LogError((TSLID)968UL, SyncEngine.Tracer, "The subscription with a transient exception crossed the disable threshold {0}.", new object[]
							{
								AggregationConfiguration.Instance.DisableSubscriptionThreshold
							});
						}
						disableSubscription = true;
						syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Disabled;
						syncEngineState.UserMailboxSubscription.DetailedAggregationStatus = ex.DetailedAggregationStatus;
					}
					else if (flag2 || t >= AggregationConfiguration.Instance.DelayedSubscriptionThreshold)
					{
						if (flag2)
						{
							syncEngineState.SyncLogSession.LogError((TSLID)969UL, SyncEngine.Tracer, "The subscription has a transient exception that needs an urgent sync status prompt.", new object[0]);
						}
						else
						{
							syncEngineState.SyncLogSession.LogError((TSLID)970UL, SyncEngine.Tracer, "The subscription with a transient exception crossed the delayed threshold {0}.", new object[]
							{
								AggregationConfiguration.Instance.DelayedSubscriptionThreshold
							});
						}
						syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Delayed;
						syncEngineState.UserMailboxSubscription.DetailedAggregationStatus = ex.DetailedAggregationStatus;
					}
					eventLogEntry = ex.EventLogEntry;
				}
				else
				{
					disableSubscription = true;
					syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Disabled;
					syncEngineState.UserMailboxSubscription.DetailedAggregationStatus = ex2.DetailedAggregationStatus;
					eventLogEntry = ex2.EventLogEntry;
				}
				if (eventLogEntry != null)
				{
					FrameworkAggregationConfiguration.EventLogger.LogEvent(eventLogEntry.Tuple, eventLogEntry.PeriodicKey, eventLogEntry.MessageArgs);
				}
				syncEngineState.SyncLogSession.LogVerbose((TSLID)80UL, SyncEngine.Tracer, "Exception :{0}", new object[]
				{
					exception
				});
			}
			this.diagnosticBuilder.UpdateSubscriptionDiagnosticMessage(syncEngineState.UserMailboxSubscription, syncEngineState.SyncLogSession, exception);
			return true;
		}

		private void UpdateSubscriptionSyncPhase(SyncEngineState syncEngineState, bool moreDataAvailable, Exception workItemException)
		{
			if ((workItemException == null || (workItemException is SyncTransientException && workItemException.InnerException is RemoteServerTooSlowException)) && !moreDataAvailable && syncEngineState.UserMailboxSubscription.SyncPhase == SyncPhase.Initial)
			{
				syncEngineState.UserMailboxSubscription.SyncPhase = SyncPhase.Incremental;
			}
		}

		private void DoSubscriptionAgentManagerWorkWithoutSubscription(AggregationWorkItem workItem, Exception exception)
		{
			SyncEngineState syncEngineState = workItem.SyncEngineState;
			this.AssertUserMailboxSubscriptionDoNotExist(syncEngineState);
			SubscriptionAgentManager.Instance.ProcessOnWorkItemFailedLoadSubscriptionEvent(syncEngineState.SyncLogSession, workItem.SubscriptionPoisonContext, workItem.SubscriptionId, workItem.AggregationType, exception, workItem.SubscriptionMessageId, workItem.UserMailboxGuid, workItem.LegacyDN, workItem.TenantGuid, syncEngineState.OrganizationId);
		}

		private void DoSubscriptionAgentManagerWorkWithUserMailboxSubscription(AggregationWorkItem workItem, Exception exception, ref bool disableSubscription, out bool updateSubscription, out bool deleteSubscription)
		{
			SyncEngineState syncEngineState = workItem.SyncEngineState;
			this.AssertUserMailboxSubscriptionExists(syncEngineState);
			updateSubscription = false;
			deleteSubscription = false;
			SubscriptionWorkItemCompletedEventResult subscriptionWorkItemCompletedEventResult = SubscriptionAgentManager.Instance.ProcessOnWorkItemCompletedEvent(syncEngineState.SyncLogSession, workItem.SubscriptionPoisonContext, syncEngineState.UserMailboxSubscription.SubscriptionGuid, (AggregationSubscription)syncEngineState.UserMailboxSubscription, workItem.IsSyncNow, exception, workItem.SubscriptionMessageId, workItem.UserMailboxGuid, syncEngineState.LegacyDN, workItem.TenantGuid, syncEngineState.OrganizationId, syncEngineState.SyncMailboxSession.MailboxSession);
			if (subscriptionWorkItemCompletedEventResult.DeleteSubscription)
			{
				deleteSubscription = true;
			}
			else if (subscriptionWorkItemCompletedEventResult.KeepSubscriptionEnabled && syncEngineState.UserMailboxSubscription.Status == AggregationStatus.Disabled)
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)971UL, SyncEngine.Tracer, "The agents have marked the subscription to be kept enabled. Setting the status as delayed.", new object[0]);
				disableSubscription = false;
				updateSubscription = true;
				syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Delayed;
			}
			else if (subscriptionWorkItemCompletedEventResult.DisableSubscription)
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)972UL, SyncEngine.Tracer, "The agents have marked the subscription to be disabled. Setting the status as disabled.", new object[0]);
				disableSubscription = true;
				syncEngineState.UserMailboxSubscription.Status = AggregationStatus.Disabled;
				syncEngineState.UserMailboxSubscription.DetailedAggregationStatus = subscriptionWorkItemCompletedEventResult.DetailedAggregationStatus;
				updateSubscription = true;
			}
			if (subscriptionWorkItemCompletedEventResult.SyncPhaseCompleted)
			{
				syncEngineState.SyncLogSession.LogVerbose((TSLID)1353UL, SyncEngine.Tracer, "The agents have marked the subscription to be syncphase.completed", new object[0]);
				syncEngineState.UserMailboxSubscription.SyncPhase = SyncPhase.Completed;
				updateSubscription = true;
			}
		}

		private void DoSubscriptionAgentManagerWorkWithMailboxServerSubscription(AggregationWorkItem workItem, Exception exception)
		{
			SyncEngineState syncEngineState = workItem.SyncEngineState;
			this.AssertMailboxServerSubscriptionExists(syncEngineState);
			SubscriptionAgentManager.Instance.ProcessOnWorkItemCompletedEvent(syncEngineState.SyncLogSession, workItem.SubscriptionPoisonContext, workItem.SubscriptionId, (AggregationSubscription)syncEngineState.MailboxServerSubscription, workItem.IsSyncNow, exception, workItem.SubscriptionMessageId, workItem.UserMailboxGuid, workItem.LegacyDN, workItem.TenantGuid, syncEngineState.OrganizationId, syncEngineState.SyncMailboxSession.MailboxSession);
		}

		private void DisconnectMailboxSessionIfFailedTransiently(SyncEngineState syncEngineState, Exception exception)
		{
			if (syncEngineState.HasConnectedMailboxSession() && exception is SyncTransientException)
			{
				syncEngineState.SyncLogSession.LogDebugging((TSLID)945UL, "Disconnecting the mailbox session.", new object[0]);
				syncEngineState.SyncMailboxSession.MailboxSession.Disconnect();
			}
		}

		private void AssertUserMailboxSubscriptionExists(SyncEngineState syncEngineState)
		{
		}

		private void AssertUserMailboxSubscriptionDoNotExist(SyncEngineState syncEngineState)
		{
		}

		private void AssertMailboxServerSubscriptionExists(SyncEngineState syncEngineState)
		{
		}

		internal static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.SyncEngineTracer;

		private static readonly SyncEngine instance = new SyncEngine();

		private static readonly SyncTransientException syncTooSlowException = SyncTransientException.CreateOperationLevelException(DetailedAggregationStatus.None, new SyncTooSlowException(AggregationConfiguration.Instance.SyncDurationThreshold), true);

		private readonly DiagnosticBuilder diagnosticBuilder = new DiagnosticBuilder();
	}
}
