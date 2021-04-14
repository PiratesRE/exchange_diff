using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SpeechRecoContext
	{
		internal SpeechRecoContext(SpeechRecognition recognitionHelper)
		{
			this.recognitionHelper = recognitionHelper;
			this.Event = new ManualResetEvent(false);
		}

		internal SpeechRecognitionProcessor.SpeechHttpStatus Status { get; private set; }

		internal string Results { get; private set; }

		internal ManualResetEvent Event { get; private set; }

		public void AddRecoRequestAsync()
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecoContext.AddRecoRequestAsync");
			this.recognitionHelper.AddRecoRequestAsync(delegate(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
			{
				this.SetAsyncArgsAndSignalEvent(args);
			});
		}

		public void RecognizeAsync(byte[] audioBytes)
		{
			lock (this.thisLock)
			{
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecoContext.RecognizeAsync");
				if (this.Event != null)
				{
					this.Event.Reset();
					this.recognitionHelper.RecognizeAsync(audioBytes, delegate(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
					{
						this.SetAsyncArgsAndSignalEvent(args);
					});
				}
			}
		}

		public void Dispose()
		{
			lock (this.thisLock)
			{
				if (this.Event != null)
				{
					this.Event.Dispose();
					this.Event = null;
				}
			}
		}

		private void SetAsyncArgsAndSignalEvent(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int, string>((long)this.GetHashCode(), "SpeechRecoContext HttpStatus code:{0} ,  ResponseText:{1}", args.HttpStatus.StatusCode, args.ResponseText);
			lock (this.thisLock)
			{
				if (this.Event != null)
				{
					this.Status = args.HttpStatus;
					this.Results = args.ResponseText;
					this.Event.Set();
				}
			}
		}

		private object thisLock = new object();

		private SpeechRecognition recognitionHelper;
	}
}
