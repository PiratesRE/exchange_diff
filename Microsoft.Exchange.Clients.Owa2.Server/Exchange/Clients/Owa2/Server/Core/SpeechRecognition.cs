using System;
using System.IO;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class SpeechRecognition
	{
		public SpeechRecognition(RequestParameters requestParameters, SpeechRecognitionResultsPriority resultsPriority)
		{
			ValidateArgument.NotNull(requestParameters, "requestParameters is null");
			this.Parameters = requestParameters;
			this.audioReady = false;
			this.addRecoRequestReady = false;
			this.ResultsPriority = resultsPriority;
			this.Results = null;
			this.ResultsReceived = false;
			this.HighestConfidenceResult = 0.0;
			this.ResultType = MobileSpeechRecoResultType.None;
			this.audioReadyEvent = new ManualResetEvent(false);
			this.audioBuffer = null;
			this.recoContext = new SpeechRecoContext(this);
		}

		public SpeechRecognitionResultsPriority ResultsPriority { get; private set; }

		public string RequestId
		{
			get
			{
				return this.Parameters.RequestId.ToString();
			}
		}

		public MobileSpeechRecoRequestType RequestType
		{
			get
			{
				return this.Parameters.RequestType;
			}
		}

		public SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs Results { get; private set; }

		public bool ResultsReceived
		{
			get
			{
				bool result;
				lock (this.receivedLock)
				{
					result = this.resultReceived;
				}
				return result;
			}
			private set
			{
				lock (this.receivedLock)
				{
					this.resultReceived = value;
				}
			}
		}

		public double HighestConfidenceResult { get; set; }

		public MobileSpeechRecoResultType ResultType { get; set; }

		protected RequestParameters Parameters { get; set; }

		public void StartRecoRequestAsync(SpeechRecognitionProcessor.SpeechProcessorResultsCompletedDelegate resultsCompletedCallback)
		{
			ValidateArgument.NotNull(resultsCompletedCallback, " callback is null");
			this.scenarioCallback = resultsCompletedCallback;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartRecoRequest));
		}

		public void SignalAudioReady(byte[] audioBuffer)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "SpeechRecognition.SignalAudioReady for Request Type:'{0}'", this.RequestType);
			lock (this.thisLock)
			{
				this.audioBuffer = audioBuffer;
				this.audioReadyEvent.Set();
			}
		}

		public void Dispose()
		{
			lock (this.thisLock)
			{
				if (this.audioReadyEvent != null)
				{
					this.audioReadyEvent.Dispose();
					this.audioReadyEvent = null;
				}
				this.recoContext.Dispose();
			}
		}

		public abstract void AddRecoRequestAsync(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback);

		public abstract void RecognizeAsync(byte[] audioBytes, SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback);

		protected void CollectAndLogStatisticsInformation(MobileSpeechRecoRequestStepLogId requestStepId, int audioLength)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestStepLogId, int>((long)this.GetHashCode(), "CollectAndLogStatisticsInformation with RequestStepId='{0}', audioLength:'{1}'", requestStepId, audioLength);
			MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow row = this.CollectStatisticsLog(requestStepId, audioLength);
			SpeechRecognitionHandler.MobileSpeechRequestStatisticsLogger.Append(row);
		}

		protected void CollectAndLogMethodCallStatisticInformation(string methodName, ExDateTime startTime, TimeSpan latency)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "CollectAndLogMethodCallStatisticInformation with methodName='{0}', startTime:'{1}', latency:'{2}'", methodName, startTime.ToString(), latency.TotalSeconds.ToString());
			MethodCallLatencyStatisticsLogger.MethodCallLatencyStatisticsLogRow row = this.CollectMethodCallStatisticsLog(methodName, startTime, latency);
			SpeechRecognitionHandler.MethodCallLatencyStatisticsLogger.Append(row);
		}

		protected void GetHighestConfidenceValueAndResultTypeFromResult(string results)
		{
			double highestConfidenceResult = 0.0;
			this.ResultType = SpeechRecognitionUtils.ParseMobileScenarioXML(results);
			if (this.ResultType != MobileSpeechRecoResultType.None)
			{
				highestConfidenceResult = SpeechRecognition.GetHighestConfidence(results);
			}
			this.HighestConfidenceResult = highestConfidenceResult;
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType, double, MobileSpeechRecoResultType>((long)this.GetHashCode(), "HighestConfidence value for Request Type:'{0}', HighestConfidence:'{1}', Result Type: '{2}'", this.RequestType, this.HighestConfidenceResult, this.ResultType);
		}

		private static double GetHighestConfidence(string results)
		{
			double num = 0.0;
			try
			{
				if (!string.IsNullOrEmpty(results))
				{
					using (XmlReader xmlReader = XmlReader.Create(new StringReader(results)))
					{
						if (xmlReader.ReadToFollowing("MobileReco"))
						{
							using (XmlReader xmlReader2 = xmlReader.ReadSubtree())
							{
								while (xmlReader2.ReadToFollowing("Alternate"))
								{
									if (xmlReader2.MoveToAttribute("confidence"))
									{
										double num2 = (double)xmlReader2.ReadContentAsFloat();
										if (num2 > num)
										{
											num = num2;
										}
									}
								}
							}
						}
					}
				}
			}
			catch (FormatException obj)
			{
				ExTraceGlobals.SpeechRecognitionTracer.TraceError<string>(0L, "Format exception while getting confidence value Exception: {0}", CommonUtil.ToEventLogString(obj));
			}
			return num;
		}

		private void StartRecoRequest(object state)
		{
			this.recoContext.AddRecoRequestAsync();
			this.HandleTimeoutAndAsyncRecoComplete(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate(this.OnAddRecoRequestCompleted), "AddRecoRequestAsync");
			lock (this.thisLock)
			{
				if (this.audioReadyEvent != null)
				{
					ThreadPool.RegisterWaitForSingleObject(this.audioReadyEvent, delegate(object objectstate, bool eventTimeOut)
					{
						if (eventTimeOut)
						{
							ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "Timeout occurs while waiting for the audio, sending an empty buffer to complete the request, RequestType:'{0}'", this.RequestType);
							MemoryStream memoryStream = new MemoryStream();
							this.audioBuffer = memoryStream.ToArray();
							memoryStream.Close();
						}
						lock (this.thisLock)
						{
							this.audioReady = true;
							this.ConsumeAudioIfReady();
						}
					}, null, TimeSpan.FromMilliseconds(30000.0), true);
				}
			}
		}

		private MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow CollectStatisticsLog(MobileSpeechRecoRequestStepLogId requestStepId, int audioLength)
		{
			return new MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogRow
			{
				RequestId = this.Parameters.RequestId,
				StartTime = ExDateTime.UtcNow,
				RequestType = new MobileSpeechRecoRequestType?(this.Parameters.RequestType),
				Tag = this.Parameters.Tag,
				TenantGuid = new Guid?(this.Parameters.TenantGuid),
				UserObjectGuid = new Guid?(this.Parameters.UserObjectGuid),
				TimeZone = this.Parameters.TimeZone.Id,
				RequestStepId = new MobileSpeechRecoRequestStepLogId?(requestStepId),
				AudioLength = audioLength,
				LogOrigin = new MobileSpeechRecoLogStatisticOrigin?(MobileSpeechRecoLogStatisticOrigin.CAS)
			};
		}

		private MethodCallLatencyStatisticsLogger.MethodCallLatencyStatisticsLogRow CollectMethodCallStatisticsLog(string methodName, ExDateTime startTime, TimeSpan latency)
		{
			return new MethodCallLatencyStatisticsLogger.MethodCallLatencyStatisticsLogRow
			{
				RequestId = this.Parameters.RequestId,
				StartTime = startTime,
				Tag = this.Parameters.Tag,
				TenantGuid = new Guid?(this.Parameters.TenantGuid),
				UserObjectGuid = new Guid?(this.Parameters.UserObjectGuid),
				MethodName = methodName,
				Latency = latency
			};
		}

		private void ConsumeAudioIfReady()
		{
			lock (this.thisLock)
			{
				if (this.audioReady && this.addRecoRequestReady)
				{
					this.recoContext.RecognizeAsync(this.audioBuffer);
					this.HandleTimeoutAndAsyncRecoComplete(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate(this.OnRecognizeCompleted), "RecognizeAsync");
				}
			}
		}

		private void HandleTimeoutAndAsyncRecoComplete(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback, string asyncRecoType)
		{
			lock (this.thisLock)
			{
				if (this.recoContext.Event == null)
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceWarning<string>(0L, "The RecoContext for RecoType:'{0}' has already been disposed of, sending internal error to complete the reco request loop", asyncRecoType);
					callback(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError));
				}
				else
				{
					ThreadPool.RegisterWaitForSingleObject(this.recoContext.Event, delegate(object state, bool timedOut)
					{
						SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args;
						if (timedOut)
						{
							ExTraceGlobals.SpeechRecognitionTracer.TraceError<string, string>(0L, "The Async call:'{0}' for Recognition:'{1}' Timed out", asyncRecoType, this.RequestType.ToString());
							UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoClientAsyncCallTimedOut, null, new object[]
							{
								this.RequestId,
								asyncRecoType,
								this.RequestType.ToString()
							});
							args = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError);
						}
						else
						{
							ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string, string>(0L, "The {0} for Recognition:{1} did not time out", asyncRecoType, this.RequestType.ToString());
							args = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(this.recoContext.Results, this.recoContext.Status);
						}
						callback(args);
					}, null, TimeSpan.FromMilliseconds(30000.0), true);
				}
			}
		}

		private void OnAddRecoRequestCompleted(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "Entering SpeechRecognition.OnAddRecoRequestCompleted request type:{0}", this.RequestType);
			try
			{
				if (args.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success)
				{
					lock (this.thisLock)
					{
						this.addRecoRequestReady = true;
						this.ConsumeAudioIfReady();
						goto IL_A3;
					}
				}
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int, string>((long)this.GetHashCode(), "SpeechRecognition.OnAddRecoRequestCompleted not successful,  HttpStatus Code:{0} HttpStatus Description:{1}", args.HttpStatus.StatusCode, args.HttpStatus.StatusDescription);
				SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs state = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, args.HttpStatus);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleResults), state);
				IL_A3:;
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e);
			}
		}

		private void OnRecognizeCompleted(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "Entering SpeechRecognition.OnRecognizeCompleted request type:{0}", this.RequestType);
			try
			{
				if (args.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success)
				{
					this.GetHighestConfidenceValueAndResultTypeFromResult(args.ResponseText);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleResults), args);
				}
				else
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int, string>((long)this.GetHashCode(), "SpeechRecognition.OnRecognizeCompleted not successful,  HttpStatus Code:{0} HttpStatus Description:{1}", args.HttpStatus.StatusCode, args.HttpStatus.StatusDescription);
					SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs state = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, args.HttpStatus);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleResults), state);
				}
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e);
			}
		}

		private void HandleResults(object state)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<MobileSpeechRecoRequestType>((long)this.GetHashCode(), "Entering SpeechRecognition.HandleResults request type:{0}", this.RequestType);
			try
			{
				this.ResultsReceived = true;
				this.Results = (SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs)state;
				this.scenarioCallback(this);
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e);
			}
		}

		private void HandleUnexpectedException(Exception e)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, MobileSpeechRecoRequestType>((long)this.GetHashCode(), "SpeechRecognition - HandleUnexpectedException='{0}' RequestType='{1}'", e, this.RequestType);
			ExWatson.SendReport(e, ReportOptions.None, null);
			this.HandleException(e, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError);
		}

		private void HandleException(Exception e, SpeechRecognitionProcessor.SpeechHttpStatus status)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, int, string>((long)this.GetHashCode(), "SpeechRecognition - Exception='{0}', Status Code='{1}', Status Description='{2}'", e, status.StatusCode, status.StatusDescription);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestFailed, null, new object[]
			{
				this.RequestId,
				this.Parameters.UserObjectGuid,
				this.Parameters.TenantGuid,
				CommonUtil.ToEventLogString(e)
			});
			this.Results = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, status);
			this.ResultsReceived = true;
			this.scenarioCallback(this);
		}

		private const int MaxTimeoutForAsyncCallback = 30000;

		private object thisLock = new object();

		private bool audioReady;

		private bool addRecoRequestReady;

		private ManualResetEvent audioReadyEvent;

		private byte[] audioBuffer;

		private SpeechRecoContext recoContext;

		private bool resultReceived;

		private object receivedLock = new object();

		private SpeechRecognitionProcessor.SpeechProcessorResultsCompletedDelegate scenarioCallback;
	}
}
