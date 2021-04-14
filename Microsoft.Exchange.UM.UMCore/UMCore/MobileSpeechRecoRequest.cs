using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MobileSpeechRecoRequest : DisposableBase
	{
		public event RequestCompletedEventHandler Completed;

		public MobileSpeechRecoRequest(Guid id, IMobileSpeechRecoRequestBehavior behavior, IPlatformBuilder platformBuilder)
		{
			ValidateArgument.NotNull(behavior, "behavior");
			ValidateArgument.NotNull(platformBuilder, "platformBuilder");
			MobileSpeechRecoTracer.TraceDebug(this, id, "Entering MobileSpeechRecoRequest constructor", new object[0]);
			this.id = id;
			this.behavior = behavior;
			this.platformBuilder = platformBuilder;
		}

		public void PrepareRecoRequestAsync(MobileRecoAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(callback, "callback");
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.PrepareRecoRequestAsync", new object[0]);
			this.timer = new Timer(new TimerCallback(this.OnTimerExpired), this, this.behavior.MaxProcessingTime, -1);
			this.InitializeAsync(MobileSpeechRecoRequest.RequestState.New, MobileSpeechRecoRequest.RequestState.Preparing, callback);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalPrepareRecoRequest), null);
		}

		public void RecognizeAsync(byte[] audioBytes, MobileRecoAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(audioBytes, "audioBytes");
			ValidateArgument.NotNull(callback, "callback");
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.RecognizeAsync", new object[0]);
			this.InitializeAsync(MobileSpeechRecoRequest.RequestState.Prepared, MobileSpeechRecoRequest.RequestState.Recognizing, callback);
			Exception ex = null;
			try
			{
				this.recognizer.RecognizeAsync(audioBytes, new RecognizeCompletedDelegate(this.RecognizeCompleted));
			}
			catch (FormatException ex2)
			{
				ex = new InvalidAudioStreamException(ex2.Message, ex2);
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (ex != null)
				{
					MobileRecoAsyncCompletedArgs completedArgs = new MobileRecoAsyncCompletedArgs(string.Empty, -1, ex);
					this.InvokeCallback(completedArgs, MobileSpeechRecoRequest.RequestState.Recognizing, MobileSpeechRecoRequest.RequestState.RecognizeComplete, true);
				}
			}
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.InternalDispose", new object[0]);
				if (this.recognizer != null)
				{
					this.recognizer.Dispose();
					this.recognizer = null;
				}
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				if (this.grammars != null)
				{
					foreach (UMGrammar umgrammar in this.grammars)
					{
						if (umgrammar.DeleteFileAfterUse)
						{
							MobileSpeechRecoTracer.TraceDebug(this, this.id, "MobileSpeechRecoRequest.InternalDispose -- Deleting grammar '{0}'", new object[]
							{
								umgrammar.Path
							});
							Util.TryDeleteFile(umgrammar.Path);
						}
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MobileSpeechRecoRequest>(this);
		}

		private static IMobileRecognizer CreateRecognizer(Guid requestId, IMobileSpeechRecoRequestBehavior requestBehavior, IPlatformBuilder platformBuilder)
		{
			MobileSpeechRecoTracer.TraceDebug(null, requestId, "Entering MobileSpeechRecoRequest.CreateRecognizer", new object[0]);
			IMobileRecognizer result = null;
			SpeechRecognitionEngineType engineType = requestBehavior.EngineType;
			CultureInfo culture = requestBehavior.Culture;
			if (!platformBuilder.TryCreateMobileRecognizer(requestId, culture, engineType, requestBehavior.MaxAlternates, out result))
			{
				throw new RecognizerNotInstalledException(engineType.ToString(), culture.ToString());
			}
			return result;
		}

		private void InternalPrepareRecoRequest(object state)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.InternalPrepareRecoRequest", new object[0]);
			Exception ex = null;
			try
			{
				this.recognizer = MobileSpeechRecoRequest.CreateRecognizer(this.id, this.behavior, this.platformBuilder);
				this.grammars = this.behavior.PrepareGrammars();
				this.recognizer.LoadGrammars(this.grammars);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				MobileRecoAsyncCompletedArgs completedArgs = new MobileRecoAsyncCompletedArgs(string.Empty, -1, ex);
				this.InvokeCallback(completedArgs, MobileSpeechRecoRequest.RequestState.Preparing, MobileSpeechRecoRequest.RequestState.Preparing, true);
				return;
			}
			MobileRecoAsyncCompletedArgs completedArgs2 = new MobileRecoAsyncCompletedArgs(string.Empty, -1, null);
			this.InvokeCallback(completedArgs2, MobileSpeechRecoRequest.RequestState.Preparing, MobileSpeechRecoRequest.RequestState.Prepared, false);
		}

		private void RecognizeCompleted(List<IMobileRecognitionResult> results, Exception error, bool speechDetected)
		{
			ValidateArgument.NotNull(results, "results");
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.RecognizeCompleted results.Count='{0}', error='{1}', speechDetected='{2}'", new object[]
			{
				results.Count,
				(error == null) ? "<null>" : error.Message,
				speechDetected
			});
			MobileRecoAsyncCompletedArgs completedArgs;
			try
			{
				int resultCount = -1;
				string result;
				if (error != null)
				{
					result = string.Empty;
				}
				else if (!speechDetected)
				{
					result = string.Empty;
					error = new NoSpeechDetectedException();
				}
				else
				{
					result = this.behavior.ProcessRecoResults(results);
					resultCount = results.Count;
				}
				completedArgs = new MobileRecoAsyncCompletedArgs(result, resultCount, error);
			}
			catch (Exception error2)
			{
				completedArgs = new MobileRecoAsyncCompletedArgs(string.Empty, -1, error2);
			}
			this.InvokeCallback(completedArgs, MobileSpeechRecoRequest.RequestState.Recognizing, MobileSpeechRecoRequest.RequestState.RecognizeComplete, true);
		}

		private void OnTimerExpired(object state)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.OnTimerExpired", new object[0]);
			bool flag = false;
			lock (this.lockObj)
			{
				if (!this.syncIsRequestComplete)
				{
					this.syncIsRequestComplete = true;
					flag = true;
				}
			}
			if (flag)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoTimeout, null, new object[]
				{
					this.id
				});
				this.FireCompletedEvent();
			}
		}

		private void InitializeAsync(MobileSpeechRecoRequest.RequestState currentState, MobileSpeechRecoRequest.RequestState newState, MobileRecoAsyncCompletedDelegate callback)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.InitializeAsync Current state='{0}', New state ='{1}'", new object[]
			{
				currentState,
				newState
			});
			lock (this.lockObj)
			{
				this.ValidateAndChangeState(currentState, newState);
				this.syncCallback = callback;
			}
		}

		private void InvokeCallback(MobileRecoAsyncCompletedArgs completedArgs, MobileSpeechRecoRequest.RequestState currentState, MobileSpeechRecoRequest.RequestState newState, bool isRequestComplete)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.InvokeCallback Current state='{0}', New state ='{1}', Request complete = '{2}'", new object[]
			{
				currentState,
				newState,
				isRequestComplete
			});
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "MobileSpeechRecoRequest.InvokeCallback Completed args Result='{0}', Error='{1}'", new object[]
			{
				completedArgs.Result,
				(completedArgs.Error != null) ? completedArgs.Error.ToString() : "<null>"
			});
			bool flag = false;
			lock (this.lockObj)
			{
				if (!this.syncIsRequestComplete)
				{
					MobileSpeechRecoTracer.TraceDebug(this, this.id, "Complete async operation by invoking callback", new object[0]);
					this.ValidateAndChangeState(currentState, newState);
					if (isRequestComplete)
					{
						TimeSpan requestElapsedTime = ExDateTime.UtcNow - this.utcStartTime;
						completedArgs.RequestElapsedTime = requestElapsedTime;
					}
					this.syncCallback(completedArgs);
					this.syncCallback = null;
					if (isRequestComplete)
					{
						MobileSpeechRecoTracer.TraceDebug(this, this.id, "Set flag to fire completed event", new object[0]);
						this.syncIsRequestComplete = true;
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.FireCompletedEvent();
			}
		}

		private void ValidateAndChangeState(MobileSpeechRecoRequest.RequestState requiredCurrentState, MobileSpeechRecoRequest.RequestState newState)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.ValidateAndChangeState Reqd current state='{0}', New state ='{1}'", new object[]
			{
				requiredCurrentState,
				newState
			});
			lock (this.lockObj)
			{
				ExAssert.RetailAssert(this.syncState == requiredCurrentState, "Current state should be '{0}' but was found to be '{1}'", new object[]
				{
					requiredCurrentState,
					this.syncState
				});
				this.syncState = newState;
			}
		}

		private void FireCompletedEvent()
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.id, "Entering MobileSpeechRecoRequest.FireCompletedEvent", new object[0]);
			if (this.Completed != null)
			{
				this.Completed(this, null);
				this.Completed = null;
			}
		}

		private Guid id;

		private IMobileSpeechRecoRequestBehavior behavior;

		private IPlatformBuilder platformBuilder;

		private IMobileRecognizer recognizer;

		private Timer timer;

		private List<UMGrammar> grammars;

		private object lockObj = new object();

		private ExDateTime utcStartTime = ExDateTime.UtcNow;

		private MobileSpeechRecoRequest.RequestState syncState;

		private MobileRecoAsyncCompletedDelegate syncCallback;

		private bool syncIsRequestComplete;

		private enum RequestState
		{
			New,
			Preparing,
			Prepared,
			Recognizing,
			RecognizeComplete
		}
	}
}
