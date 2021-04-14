using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MobileSpeechRecoDispatcher : IUMAsyncComponent
	{
		private MobileSpeechRecoDispatcher()
		{
		}

		public static MobileSpeechRecoDispatcher TestHookCreateInstance()
		{
			return new MobileSpeechRecoDispatcher();
		}

		public static MobileSpeechRecoDispatcher Instance
		{
			get
			{
				if (MobileSpeechRecoDispatcher.instance == null)
				{
					lock (MobileSpeechRecoDispatcher.staticLock)
					{
						if (MobileSpeechRecoDispatcher.instance == null)
						{
							MobileSpeechRecoTracer.TraceDebug(null, Guid.Empty, "Creating singleton instance of MobileSpeechRecoDispatcher", new object[0]);
							MobileSpeechRecoDispatcher.instance = new MobileSpeechRecoDispatcher();
						}
					}
				}
				return MobileSpeechRecoDispatcher.instance;
			}
		}

		public bool IsStopping
		{
			get
			{
				bool result;
				lock (this.lockObj)
				{
					result = this.syncIsStopping;
				}
				return result;
			}
		}

		public int NumPendingRequests
		{
			get
			{
				int count;
				lock (this.lockObj)
				{
					count = this.syncSpeechRecoRequestTable.Count;
				}
				return count;
			}
		}

		public AutoResetEvent StoppedEvent
		{
			get
			{
				return this.stoppedEvent;
			}
		}

		public bool IsInitialized
		{
			get
			{
				bool result;
				lock (this.lockObj)
				{
					result = this.syncIsInitialized;
				}
				return result;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public void CleanupAfterStopped()
		{
			if (this.stoppedEvent != null)
			{
				this.stoppedEvent.Close();
			}
		}

		public void StartNow(StartupStage stage)
		{
			if (stage == StartupStage.WPActivation)
			{
				lock (this.lockObj)
				{
					if (this.syncIsInitialized)
					{
						throw new InvalidOperationException();
					}
					this.syncMaxConcurrentRequests = GlobCfg.MaxMobileSpeechRecoRequestsPerCore * Environment.ProcessorCount;
					MobileSpeechRecoTracer.TraceDebug(this, Guid.Empty, "Maximum number of concurrent requests='{0}'", new object[]
					{
						this.syncMaxConcurrentRequests
					});
					this.syncIsInitialized = true;
					MobileSpeechRecoTracer.TraceDebug(this, Guid.Empty, "{0} starting in stage {1}", new object[]
					{
						this.Name,
						stage
					});
				}
			}
		}

		public void StopAsync()
		{
			lock (this.lockObj)
			{
				this.syncIsStopping = true;
				MobileSpeechRecoTracer.TraceDebug(this, Guid.Empty, "{0} stopping", new object[]
				{
					this.Name
				});
				if (this.NumPendingRequests <= 0)
				{
					this.stoppedEvent.Set();
					MobileSpeechRecoTracer.TraceDebug(this, Guid.Empty, "{0} stopped", new object[]
					{
						this.Name
					});
				}
			}
		}

		public void AddRecoRequestAsync(Guid requestId, IMobileSpeechRecoRequestBehavior requestBehavior, IPlatformBuilder platformBuilder, MobileRecoAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(requestBehavior, "requestBehavior");
			ValidateArgument.NotNull(platformBuilder, "platformBuilder");
			ValidateArgument.NotNull(callback, "callback");
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering MobileSpeechRecoDispatcher.AddRecoRequestAsync", new object[0]);
			MobileSpeechRecoRequest mobileSpeechRecoRequest = null;
			Exception ex = null;
			try
			{
				mobileSpeechRecoRequest = new MobileSpeechRecoRequest(requestId, requestBehavior, platformBuilder);
				lock (this.lockObj)
				{
					try
					{
						this.ValidateNewRequest(requestId);
						this.syncSpeechRecoRequestTable.Add(requestId, mobileSpeechRecoRequest);
					}
					catch (MobileRecoRequestCannotBeHandledException ex2)
					{
						ex = ex2;
					}
					catch (MobileRecoInvalidRequestException ex3)
					{
						ex = ex3;
					}
				}
			}
			finally
			{
				if (ex != null)
				{
					mobileSpeechRecoRequest.Dispose();
					mobileSpeechRecoRequest = null;
				}
			}
			if (mobileSpeechRecoRequest != null)
			{
				MobileSpeechRecoTracer.TraceDebug(this, requestId, "Set up completed callback and start PrepareRecoRequestAsync", new object[0]);
				mobileSpeechRecoRequest.Completed += this.OnRequestCompleted;
				mobileSpeechRecoRequest.PrepareRecoRequestAsync(callback);
				return;
			}
			MobileRecoAsyncCompletedArgs mobileRecoAsyncCompletedArgs = new MobileRecoAsyncCompletedArgs(string.Empty, -1, ex);
			MobileSpeechRecoTracer.TraceError(this, requestId, "Request rejected Error:'{0}'", new object[]
			{
				mobileRecoAsyncCompletedArgs.Error
			});
			callback(mobileRecoAsyncCompletedArgs);
		}

		public void RecognizeAsync(Guid requestId, byte[] audioBytes, MobileRecoAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(audioBytes, "audioBytes");
			ValidateArgument.NotNull(callback, "callback");
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering MobileSpeechRecoDispatcher.RecognizeAsync", new object[0]);
			MobileSpeechRecoRequest mobileSpeechRecoRequest = null;
			lock (this.lockObj)
			{
				this.syncSpeechRecoRequestTable.TryGetValue(requestId, out mobileSpeechRecoRequest);
			}
			if (mobileSpeechRecoRequest != null)
			{
				MobileSpeechRecoTracer.TraceDebug(this, requestId, "Start RecognizeAsync", new object[0]);
				mobileSpeechRecoRequest.RecognizeAsync(audioBytes, callback);
				return;
			}
			MobileRecoAsyncCompletedArgs mobileRecoAsyncCompletedArgs = new MobileRecoAsyncCompletedArgs(string.Empty, -1, new InvalidRecoRequestIdException(requestId));
			MobileSpeechRecoTracer.TraceError(this, requestId, "Request rejected Error:'{0}'", new object[]
			{
				mobileRecoAsyncCompletedArgs.Error
			});
			callback(mobileRecoAsyncCompletedArgs);
		}

		private void OnRequestCompleted(object sender, EventArgs e)
		{
			MobileSpeechRecoRequest mobileSpeechRecoRequest = sender as MobileSpeechRecoRequest;
			Guid id = mobileSpeechRecoRequest.Id;
			MobileSpeechRecoTracer.TraceDebug(this, id, "Entering MobileSpeechRecoDispatcher.OnRequestCompleted", new object[0]);
			lock (this.lockObj)
			{
				bool condition = this.syncSpeechRecoRequestTable.Remove(id);
				ExAssert.RetailAssert(condition, "Tried to remove request '{0}' but it was not found in the table", new object[]
				{
					id
				});
				if (this.syncIsStopping && this.NumPendingRequests <= 0)
				{
					this.stoppedEvent.Set();
					MobileSpeechRecoTracer.TraceDebug(this, id, "{0} stopped after last request was completed", new object[]
					{
						this.Name
					});
				}
			}
			MobileSpeechRecoTracer.TraceDebug(this, id, "Disposing request", new object[0]);
			mobileSpeechRecoRequest.Dispose();
		}

		private void ValidateNewRequest(Guid requestId)
		{
			lock (this.lockObj)
			{
				if (!this.syncIsInitialized)
				{
					throw new MobileRecoDispatcherNotInitializedException();
				}
				if (this.syncIsStopping)
				{
					throw new MobileRecoDispatcherStoppingException();
				}
				if (this.syncSpeechRecoRequestTable.Count >= this.syncMaxConcurrentRequests)
				{
					throw new MaxMobileRecoRequestsReachedException(this.syncSpeechRecoRequestTable.Count, this.syncMaxConcurrentRequests);
				}
				if (requestId == Guid.Empty)
				{
					throw new EmptyRecoRequestIdException(requestId);
				}
				if (this.syncSpeechRecoRequestTable.ContainsKey(requestId))
				{
					throw new DuplicateRecoRequestIdException(requestId);
				}
			}
		}

		private static object staticLock = new object();

		private static MobileSpeechRecoDispatcher instance;

		private AutoResetEvent stoppedEvent = new AutoResetEvent(false);

		private object lockObj = new object();

		private bool syncIsInitialized;

		private bool syncIsStopping;

		private Dictionary<Guid, MobileSpeechRecoRequest> syncSpeechRecoRequestTable = new Dictionary<Guid, MobileSpeechRecoRequest>(64);

		private int syncMaxConcurrentRequests;
	}
}
