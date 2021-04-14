using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class SpeechRecognitionScenarioBase
	{
		public SpeechRecognitionScenarioBase(RequestParameters requestParameters, UserContext userContext)
		{
			this.Parameters = requestParameters;
			this.UserContext = userContext;
			this.resultsProcessed = false;
			CultureInfo c = (requestParameters != null) ? requestParameters.Culture : CommonConstants.DefaultCulture;
			this.immediateThreshold = LocConfig.Instance[c].MowaSpeech.MowaVoiceImmediateThreshold;
			this.InitializeSpeechRecognitions(requestParameters);
		}

		public UserContext UserContext { get; private set; }

		public RequestParameters Parameters { get; set; }

		protected Dictionary<MobileSpeechRecoRequestType, SpeechRecognition> RecognitionHelpers { get; set; }

		internal virtual void StartRecoRequestAsync(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionScenariosBase.StartRecoRequestAsync");
			this.resultHandlerCallback = callback;
			foreach (SpeechRecognition speechRecognition in this.RecognitionHelpers.Values)
			{
				speechRecognition.StartRecoRequestAsync(new SpeechRecognitionProcessor.SpeechProcessorResultsCompletedDelegate(this.HandleResults));
			}
		}

		internal virtual void SetAudio(byte[] audioBytes)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionScenariosBase.AudioReadyForRecognition");
			lock (this.thisLock)
			{
				foreach (SpeechRecognition speechRecognition in this.RecognitionHelpers.Values)
				{
					speechRecognition.SignalAudioReady(audioBytes);
				}
			}
		}

		protected static RequestParameters CreateRequestParameters(MobileSpeechRecoRequestType requestType, RequestParameters requestParameters)
		{
			Guid requestId = Guid.NewGuid();
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string, string, string>(0L, "Requested Id Guid:'{0}' created for Request Type:'{1}', Parameter Tag:'{2}'", requestId.ToString(), requestType.ToString(), requestParameters.Tag);
			return new RequestParameters(requestId, requestParameters.Tag, requestType, requestParameters.Culture, requestParameters.TimeZone, requestParameters.UserObjectGuid, requestParameters.TenantGuid, requestParameters.OrgId);
		}

		protected abstract void InitializeSpeechRecognitions(RequestParameters requestParameters);

		protected abstract SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs GetFormattedResultsForHighestConfidenceProcessor(SpeechRecognition recognitionWithHighestConfidence);

		private void HandleResults(SpeechRecognition helper)
		{
			lock (this.thisLock)
			{
				try
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionScenariosBase.HandleResults");
					if (!this.resultsProcessed)
					{
						if (helper.ResultsPriority == SpeechRecognitionResultsPriority.Immediate)
						{
							this.ProcessImmediatePriorityProcessor(helper);
						}
						else
						{
							this.ProcessWaitingPriorityProcessor(helper);
						}
					}
				}
				catch (Exception e)
				{
					this.HandleUnexpectedException(e);
				}
			}
		}

		private void ProcessImmediatePriorityProcessor(SpeechRecognition helper)
		{
			lock (this.thisLock)
			{
				if (helper.Results.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success && helper.HighestConfidenceResult > this.immediateThreshold)
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessImediatePriorityProcessor Recognition: {0} was successful and will be processed immediately", helper.RequestType.ToString());
					this.resultsProcessed = true;
					this.InvokeHandlerCallbackAndDisposeHelpers(helper.Results);
				}
				else
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string, string, double>((long)this.GetHashCode(), "ProcessImediatePriorityProcessor Recognition: {0} was Unsuccessful with error description:{1}, confidence value:{2}", helper.RequestType.ToString(), helper.Results.HttpStatus.StatusDescription, helper.HighestConfidenceResult);
					this.ProcessWaitingPriorityProcessor(helper);
				}
			}
		}

		private void ProcessWaitingPriorityProcessor(SpeechRecognition helper)
		{
			lock (this.thisLock)
			{
				if (!this.resultsProcessed)
				{
					this.resultsProcessed = true;
					if (helper.Results.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.NoSpeechDetected)
					{
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessWaitingPriorityProcessor: No Speech Detected from Speech Recognition:'{0}'", helper.RequestType.ToString());
						this.InvokeHandlerCallbackAndDisposeHelpers(helper.Results);
					}
					else
					{
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "ProcessWaitingPriorityProcessor: Final Results hasnt been processed yet. Initiate processing...");
						SpeechRecognition speechRecognition = helper;
						foreach (KeyValuePair<MobileSpeechRecoRequestType, SpeechRecognition> keyValuePair in this.RecognitionHelpers)
						{
							if (!keyValuePair.Value.ResultsReceived)
							{
								ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "ProcessWaitingPriorityProcessor Recognition: {0} result is not available yet and will be waited on", keyValuePair.Key);
								this.resultsProcessed = false;
								break;
							}
							if (speechRecognition.HighestConfidenceResult < keyValuePair.Value.HighestConfidenceResult)
							{
								speechRecognition = keyValuePair.Value;
							}
						}
						if (this.resultsProcessed)
						{
							SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs formattedResultsForHighestConfidenceProcessor = this.GetFormattedResultsForHighestConfidenceProcessor(speechRecognition);
							this.InvokeHandlerCallbackAndDisposeHelpers(formattedResultsForHighestConfidenceProcessor);
						}
					}
				}
				else
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "ProcessWaitingPriorityProcessor: Final Results already processed. Skip processing stage.");
				}
			}
		}

		private void InvokeHandlerCallbackAndDisposeHelpers(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs argResults)
		{
			this.resultHandlerCallback(argResults);
			this.DisposeRecognitionHelpers();
		}

		private void DisposeRecognitionHelpers()
		{
			lock (this.thisLock)
			{
				foreach (KeyValuePair<MobileSpeechRecoRequestType, SpeechRecognition> keyValuePair in this.RecognitionHelpers)
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "DisposeRecognitionHelpers disposing Recognition: {0}", keyValuePair.Key);
					keyValuePair.Value.Dispose();
				}
				this.RecognitionHelpers.Clear();
			}
		}

		private void HandleUnexpectedException(Exception e)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, Guid>((long)this.GetHashCode(), "SpeechRecognitionScenarioBase - HandleUnexpectedException='{0}' RequestId='{1}'", e, this.Parameters.RequestId);
			ExWatson.SendReport(e, ReportOptions.None, null);
			this.HandleException(e, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError);
		}

		private void HandleException(Exception e, SpeechRecognitionProcessor.SpeechHttpStatus status)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, int, string>((long)this.GetHashCode(), "SpeechRecognitionScenarioBase - Exception='{0}', Status Code='{1}', Status Description='{2}'", e, status.StatusCode, status.StatusDescription);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestFailed, null, new object[]
			{
				this.Parameters.RequestId,
				this.Parameters.UserObjectGuid,
				this.Parameters.TenantGuid,
				CommonUtil.ToEventLogString(e)
			});
			SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs argResults = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, status);
			this.InvokeHandlerCallbackAndDisposeHelpers(argResults);
		}

		private readonly double immediateThreshold;

		private object thisLock = new object();

		private bool resultsProcessed;

		private SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate resultHandlerCallback;
	}
}
