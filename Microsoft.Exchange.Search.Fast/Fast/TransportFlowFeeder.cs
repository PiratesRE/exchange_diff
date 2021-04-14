using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Performance;

namespace Microsoft.Exchange.Search.Fast
{
	internal class TransportFlowFeeder : ITransportFlowFeeder
	{
		public TransportFlowFeeder(IStreamManager streamManager, ISubmitDocument feeder)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("IndexDeliveryAgent", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.TransportFlowFeederTracer, (long)this.GetHashCode());
			this.streamManager = streamManager;
			this.feeder = feeder;
			TransportFlowFeeder.InitPerformanceCounters();
		}

		internal TransportFlowFeeder.PopulateFastDocumentProperties PopulateFastDocumentPropertiesHandler { get; set; }

		public static void InitPerformanceCounters()
		{
			if (TransportFlowFeeder.instanceCounters == null)
			{
				lock (TransportFlowFeeder.lockObject)
				{
					if (TransportFlowFeeder.instanceCounters == null)
					{
						TransportFlowFeeder.instanceCounters = TransportCtsFlowCounters.GetInstance(Process.GetCurrentProcess().ProcessName);
						TransportFlowFeeder.instanceCounters.Reset();
					}
				}
			}
		}

		public static void ReportTimings(TransportFlowOperatorTimings transportFlowOperatorTimings, long messageProcessingTimeInMsec, bool languageDetectionFailed)
		{
			if (TransportFlowFeeder.instanceCounters == null)
			{
				return;
			}
			TransportFlowFeeder.instanceCounters.TotalDocuments.Increment();
			TransportFlowFeeder.instanceCounters.NumberOfProcessedDocuments.Increment();
			TransportFlowFeeder.instanceCounters.TotalTimeProcessingMessageInMsec.IncrementBy(messageProcessingTimeInMsec);
			if (languageDetectionFailed)
			{
				TransportFlowFeeder.instanceCounters.LanguageDetectionFailures.Increment();
			}
			if (transportFlowOperatorTimings != null)
			{
				TransportFlowFeeder.instanceCounters.TimeInDocParserInMsec.IncrementBy(transportFlowOperatorTimings.TimeInDocParserInMsec);
				TransportFlowFeeder.instanceCounters.TimeInNLGSubflowInMsec.IncrementBy(transportFlowOperatorTimings.TimeInNLGSubflowInMsec);
				TransportFlowFeeder.instanceCounters.TimeInQueueInMsec.IncrementBy(transportFlowOperatorTimings.TimeInQueueInMsec);
				TransportFlowFeeder.instanceCounters.TimeInTransportRetrieverInMsec.IncrementBy(transportFlowOperatorTimings.TimeInTransportRetrieverInMsec);
				TransportFlowFeeder.instanceCounters.TimeInWordbreakerInMsec.IncrementBy(transportFlowOperatorTimings.TimeInWordbreakerInMsec);
			}
			if (messageProcessingTimeInMsec < 250L)
			{
				TransportFlowFeeder.instanceCounters.ProcessedUnder250ms.Increment();
				return;
			}
			if (messageProcessingTimeInMsec < 500L)
			{
				TransportFlowFeeder.instanceCounters.ProcessedUnder500ms.Increment();
				return;
			}
			if (messageProcessingTimeInMsec < 1000L)
			{
				TransportFlowFeeder.instanceCounters.ProcessedUnder1000ms.Increment();
				return;
			}
			if (messageProcessingTimeInMsec < 2000L)
			{
				TransportFlowFeeder.instanceCounters.ProcessedUnder2000ms.Increment();
				return;
			}
			if (messageProcessingTimeInMsec < 5000L)
			{
				TransportFlowFeeder.instanceCounters.ProcessedUnder5000ms.Increment();
				return;
			}
			TransportFlowFeeder.instanceCounters.ProcessedOver5000ms.Increment();
		}

		public static void ReportClientSideTimings(ClientSideTimings clientSideTimings)
		{
			if (TransportFlowFeeder.instanceCounters == null)
			{
				return;
			}
			if (clientSideTimings != null)
			{
				TransportFlowFeeder.instanceCounters.TimeInGetConnectionInMsec.IncrementBy((long)clientSideTimings.TimeInGetConnection.TotalMilliseconds);
				TransportFlowFeeder.instanceCounters.TimeInPropertyBagLoadInMsec.IncrementBy((long)clientSideTimings.TimeInPropertyBagLoad.TotalMilliseconds);
				TransportFlowFeeder.instanceCounters.TimeInMessageItemConversionInMsec.IncrementBy((long)clientSideTimings.TimeInMessageItemConversion.TotalMilliseconds);
				TransportFlowFeeder.instanceCounters.TimeDeterminingAgeOfItemInMsec.IncrementBy((long)clientSideTimings.TimeDeterminingAgeOfItem.TotalMilliseconds);
				TransportFlowFeeder.instanceCounters.TimeInMimeConversionInMsec.IncrementBy((long)clientSideTimings.TimeInMimeConversion.TotalMilliseconds);
				TransportFlowFeeder.instanceCounters.TimeInShouldAnnotateMessageInMsec.IncrementBy((long)clientSideTimings.TimeInShouldAnnotateMessage.TotalMilliseconds);
			}
		}

		public static void ReportSkippedDocument()
		{
			if (TransportFlowFeeder.instanceCounters == null)
			{
				return;
			}
			TransportFlowFeeder.instanceCounters.TotalSkippedDocuments.Increment();
		}

		public void ProcessMessage(Stream mimeStream, Stream propertyStream, TransportFlowMessageFlags transportFlowMessageFlags)
		{
			if ((transportFlowMessageFlags & TransportFlowMessageFlags.ShouldDiscardToken) == TransportFlowMessageFlags.ShouldDiscardToken && (transportFlowMessageFlags & TransportFlowMessageFlags.ShouldBypassNlg) == TransportFlowMessageFlags.ShouldBypassNlg)
			{
				return;
			}
			TransportFlowFeeder.Context context = new TransportFlowFeeder.Context();
			IFastDocument fastDocument = this.feeder.CreateFastDocument(DocumentOperation.Insert);
			if (this.PopulateFastDocumentPropertiesHandler != null)
			{
				this.PopulateFastDocumentPropertiesHandler(mimeStream, fastDocument, context);
			}
			else
			{
				fastDocument.Port = this.streamManager.ListenPort;
				fastDocument.TransportContextId = context.ContextId.ToString();
				fastDocument.MessageFlags = (int)transportFlowMessageFlags;
			}
			if ((transportFlowMessageFlags & TransportFlowMessageFlags.ShouldBypassNlg) == TransportFlowMessageFlags.ShouldBypassNlg)
			{
				TransportFlowFeeder.instanceCounters.NumberOfSkippedNlg.Increment();
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				context.ConnectAsyncResult = this.streamManager.BeginWaitForConnection(context.ContextId, new AsyncCallback(this.WaitForConnectionComplete), context);
				using (WaitHandle asyncWaitHandle = context.ConnectAsyncResult.AsyncWaitHandle)
				{
					context.SubmitAsyncResult = this.feeder.BeginSubmitDocument(fastDocument, new AsyncCallback(this.SubmitDocumentComplete), context);
					this.diagnosticsSession.TraceDebug("Wait for connection", new object[0]);
					asyncWaitHandle.WaitOne();
					if (context.SubmitException != null)
					{
						throw context.SubmitException;
					}
					if (context.ConnectException != null)
					{
						throw context.ConnectException;
					}
				}
				long num = 0L;
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				long num2 = elapsedMilliseconds - num;
				TransportFlowFeeder.instanceCounters.TimeSpentWaitingForConnectInMsec.IncrementBy(num2);
				this.diagnosticsSession.TraceDebug<long>("Connected: {0} ms. Sending MIME", num2);
				num = elapsedMilliseconds;
				mimeStream.CopyTo(context.RemoteStream);
				context.RemoteStream.Flush();
				elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				TransportFlowFeeder.instanceCounters.BytesSent.IncrementBy(mimeStream.Position);
				this.diagnosticsSession.TraceDebug<long, long>("Mime sent: {0} ms, {1} bytes. Read properties", elapsedMilliseconds - num, mimeStream.Position);
				num = elapsedMilliseconds;
				context.RemoteStream.CopyTo(propertyStream);
				elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				this.diagnosticsSession.TraceDebug<long, long>("Read properties: {0} ms, {1} bytes. Tell FAST we're done", elapsedMilliseconds - num, propertyStream.Length);
				TransportFlowFeeder.instanceCounters.BytesReceived.IncrementBy(propertyStream.Length);
				context.RemoteStream.Flush();
				this.feeder.TryCompleteSubmitDocument(context.SubmitAsyncResult);
			}
			catch (Exception arg)
			{
				this.diagnosticsSession.TraceError<Exception, long>("Exception processing message: {0}, elapsed {1} ms", arg, stopwatch.ElapsedMilliseconds);
				TransportFlowFeeder.instanceCounters.NumberOfFailedDocuments.Increment();
				TransportFlowFeeder.instanceCounters.TotalTimeProcessingFailedMessageInMsec.IncrementBy(stopwatch.ElapsedMilliseconds);
				context.CancelConnect();
				context.CancelSubmit();
				throw;
			}
			this.diagnosticsSession.TraceDebug<long>("Document processed, elapsed {0} ms", stopwatch.ElapsedMilliseconds);
		}

		private void WaitForConnectionComplete(IAsyncResult connectAsyncResult)
		{
			TransportFlowFeeder.Context context = (TransportFlowFeeder.Context)connectAsyncResult.AsyncState;
			try
			{
				context.RemoteStream = this.streamManager.EndWaitForConnection(connectAsyncResult);
			}
			catch (Exception ex)
			{
				context.ConnectException = ex;
				this.diagnosticsSession.TraceError<Exception>("Exception waiting for connection: {0}", ex);
				context.CancelSubmit();
			}
		}

		private void SubmitDocumentComplete(IAsyncResult submitAsyncResult)
		{
			TransportFlowFeeder.Context context = (TransportFlowFeeder.Context)submitAsyncResult.AsyncState;
			try
			{
				this.feeder.EndSubmitDocument(submitAsyncResult);
			}
			catch (Exception ex)
			{
				context.SubmitException = ex;
				this.diagnosticsSession.TraceError<Exception>("Exception submitting document: {0}", ex);
			}
			context.CancelConnect();
			context.CloseRemoteStream();
		}

		private static TransportCtsFlowCountersInstance instanceCounters;

		private static object lockObject = new object();

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly IStreamManager streamManager;

		private readonly ISubmitDocument feeder;

		internal delegate void PopulateFastDocumentProperties(Stream mimeStream, IFastDocument fastDocument, TransportFlowFeeder.Context context);

		internal class Context
		{
			public Guid ContextId
			{
				get
				{
					return this.contextId;
				}
			}

			public ICancelableAsyncResult ConnectAsyncResult { get; set; }

			public ICancelableAsyncResult SubmitAsyncResult { get; set; }

			public Stream RemoteStream { get; set; }

			public Exception ConnectException { get; set; }

			public Exception SubmitException { get; set; }

			public void CancelConnect()
			{
				if (this.ConnectAsyncResult != null)
				{
					this.ConnectAsyncResult.Cancel();
				}
			}

			public void CancelSubmit()
			{
				if (this.SubmitAsyncResult != null)
				{
					this.SubmitAsyncResult.Cancel();
				}
			}

			public void CloseRemoteStream()
			{
				Stream remoteStream = this.RemoteStream;
				if (remoteStream != null)
				{
					remoteStream.Close();
				}
			}

			private readonly Guid contextId = Guid.NewGuid();
		}
	}
}
