using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SpeechRecognitionHandler : IHttpAsyncHandler, IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new NotSupportedException();
		}

		public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback callback, object context)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionHandler.BeginProcessRequest");
			SpeechRecognitionHandler.InitializeStatisticsLoggers();
			this.speechRecoProcessor = new SpeechRecognitionProcessor(httpContext);
			return this.speechRecoProcessor.BeginRecognition(callback, context);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionHandler.EndProcessRequest");
			SpeechRecognitionAsyncResult speechRecognitionAsyncResult = result as SpeechRecognitionAsyncResult;
			HttpResponse response = this.speechRecoProcessor.HttpContext.Response;
			response.TrySkipIisCustomErrors = true;
			response.BufferOutput = false;
			response.StatusCode = speechRecognitionAsyncResult.StatusCode;
			if (!string.IsNullOrEmpty(speechRecognitionAsyncResult.StatusDescription))
			{
				response.StatusDescription = speechRecognitionAsyncResult.StatusDescription;
			}
			response.ContentType = "application/json; charset=utf-8";
			response.Write(speechRecognitionAsyncResult.ResponseText);
		}

		private static void InitializeStatisticsLoggers()
		{
			if (SpeechRecognitionHandler.MobileSpeechRequestStatisticsLogger == null)
			{
				lock (SpeechRecognitionHandler.loggerLock)
				{
					if (SpeechRecognitionHandler.MobileSpeechRequestStatisticsLogger == null)
					{
						SpeechRecognitionHandler.MobileSpeechRequestStatisticsLogger = MobileSpeechRequestStatisticsLogger.Instance;
						SpeechRecognitionHandler.MobileSpeechRequestStatisticsLogger.Init();
					}
				}
			}
			if (SpeechRecognitionHandler.SpeechProcessorStatisticsLogger == null)
			{
				lock (SpeechRecognitionHandler.loggerLock)
				{
					if (SpeechRecognitionHandler.SpeechProcessorStatisticsLogger == null)
					{
						SpeechRecognitionHandler.SpeechProcessorStatisticsLogger = SpeechProcessorStatisticsLogger.Instance;
						SpeechRecognitionHandler.SpeechProcessorStatisticsLogger.Init();
					}
				}
			}
			if (SpeechRecognitionHandler.MethodCallLatencyStatisticsLogger == null)
			{
				lock (SpeechRecognitionHandler.loggerLock)
				{
					if (SpeechRecognitionHandler.MethodCallLatencyStatisticsLogger == null)
					{
						SpeechRecognitionHandler.MethodCallLatencyStatisticsLogger = MethodCallLatencyStatisticsLogger.Instance;
						SpeechRecognitionHandler.MethodCallLatencyStatisticsLogger.Init();
					}
				}
			}
		}

		public static MobileSpeechRequestStatisticsLogger MobileSpeechRequestStatisticsLogger;

		public static SpeechProcessorStatisticsLogger SpeechProcessorStatisticsLogger;

		public static MethodCallLatencyStatisticsLogger MethodCallLatencyStatisticsLogger;

		private static object loggerLock = new object();

		private SpeechRecognitionProcessor speechRecoProcessor;
	}
}
