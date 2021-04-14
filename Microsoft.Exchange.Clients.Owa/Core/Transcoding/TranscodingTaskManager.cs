using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal class TranscodingTaskManager : DisposeTrackableBase
	{
		private TranscodingTaskManager(int maxConversionTime, int maxConversionPerProcess, string rootCachePath, int totalSizeQuota, int rowNumberInExcel, int maxInputSize, int maxOutputSize, bool isImageMode, HtmlFormat htmlFormat, int memoryLimit)
		{
			if (maxConversionTime <= 0)
			{
				throw new ArgumentException("Invalid maximum conversion time", "maxConversionTime");
			}
			if (maxConversionPerProcess <= 0)
			{
				throw new ArgumentException("Invalid maximum conversion number per process", "maxConversionPerProcess");
			}
			if (string.IsNullOrEmpty(rootCachePath))
			{
				throw new ArgumentException("The root path for cache system can not be null or empty", "rootCachePath");
			}
			if (totalSizeQuota <= 0)
			{
				throw new ArgumentException("Invalid cache quota", "totalSizeQuota");
			}
			if (rowNumberInExcel <= 0)
			{
				throw new ArgumentException("Invalid maximum row/page of excel documents", "rowNumberInExcel");
			}
			if (maxInputSize <= 0)
			{
				throw new ArgumentException("Invalid input data threshold", "maxInputSize");
			}
			if (maxOutputSize <= 0)
			{
				throw new ArgumentException("Invalid output data threshold", "maxOutputSize");
			}
			if (memoryLimit <= 0)
			{
				throw new ArgumentException("Invalid memory limit", "memoryLimit");
			}
			this.rowNumberInExcel = rowNumberInExcel;
			this.maxOutputSize = maxOutputSize;
			this.isImageMode = isImageMode;
			this.htmlFormat = htmlFormat;
			try
			{
				this.transcodingActiveMutex = new Mutex(false, "F7CC98B4-F6F5-489D-98EB-000689C721DE");
			}
			catch (UnauthorizedAccessException e)
			{
				this.CreateMutexFailed(e);
			}
			catch (IOException e2)
			{
				this.CreateMutexFailed(e2);
			}
			catch (ApplicationException e3)
			{
				this.CreateMutexFailed(e3);
			}
			OwaSingleCounters.TotalConversions.RawValue = 0L;
			OwaSingleCounters.ActiveConversions.RawValue = 0L;
			OwaSingleCounters.QueuedConversionRequests.RawValue = 0L;
			OwaSingleCounters.AverageConvertingTime.RawValue = 0L;
			OwaSingleCounters.AverageConversionQueuingTime.RawValue = 0L;
			OwaSingleCounters.TotalRejectedConversions.RawValue = 0L;
			OwaSingleCounters.TotalTimeoutConversions.RawValue = 0L;
			OwaSingleCounters.TotalErrorConversions.RawValue = 0L;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format("SOFTWARE\\Classes\\CLSID\\{{{0}}}\\LocalServer32", "F7CC98B4-F6F5-489D-98EB-000689C721DE")))
			{
				if (registryKey == null)
				{
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerApplicationNotRegistered, string.Empty, new object[0]);
					throw new TranscodingFatalFaultException("TranscodingService is not registered.", null, this);
				}
				this.workProcessPath = ((string)registryKey.GetValue(null)).Trim(new char[]
				{
					'"'
				});
			}
			if (string.IsNullOrEmpty(this.workProcessPath) || !File.Exists(this.workProcessPath))
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerApplicationNotFound, string.Empty, new object[0]);
				throw new TranscodingFatalFaultException("Unable to find the transcoding service application.", null, this);
			}
			Cache.CacheOption option = new Cache.CacheOption(rootCachePath, totalSizeQuota, maxInputSize, maxOutputSize);
			this.cache = new Cache(option);
			this.blockList = new BlockList(3000, TimeSpan.FromSeconds(86400.0));
			ComWorkerConfiguration workerConfiguration = new ComWorkerConfiguration(this.workProcessPath, null, new Guid("F7CC98B4-F6F5-489D-98EB-000689C721DE"), ComWorkerConfiguration.RunAsFlag.RunAsLocalService, this.transcodingActiveMutex, memoryLimit, 7200000, 30000, maxConversionPerProcess, maxConversionTime * 1000, 0);
			try
			{
				this.transcodingProcessManager = new ComProcessManager<ITranscoder>(TranscodingTaskManager.maxWorkProcessNumber, workerConfiguration, ExTraceGlobals.TranscodingTracer);
			}
			catch (ComProcessManagerInitializationException innerException)
			{
				throw new TranscodingFatalFaultException("Initialize transcoding process manager failed.", innerException, this);
			}
			ComProcessManager<ITranscoder> comProcessManager = this.transcodingProcessManager;
			comProcessManager.CreateWorkerCallback = (ComProcessManager<ITranscoder>.OnCreateWorker)Delegate.Combine(comProcessManager.CreateWorkerCallback, new ComProcessManager<ITranscoder>.OnCreateWorker(this.OnCreateWorkerDelegate));
			ComProcessManager<ITranscoder> comProcessManager2 = this.transcodingProcessManager;
			comProcessManager2.ExecuteRequestCallback = (ComProcessManager<ITranscoder>.OnExecuteRequest)Delegate.Combine(comProcessManager2.ExecuteRequestCallback, new ComProcessManager<ITranscoder>.OnExecuteRequest(this.OnExecuteRequestDelegate));
		}

		public static bool IsInitialized
		{
			get
			{
				return TranscodingTaskManager.isInitialized;
			}
		}

		public static int CacheSize
		{
			get
			{
				TranscodingTaskManager.CheckInitialize();
				return TranscodingTaskManager.transcodingTaskManager.cache.CacheSize;
			}
		}

		public static int XCRootPathMaxLength
		{
			get
			{
				return Cache.XCRootPathMaxLength;
			}
		}

		public static bool Initialize(int maxConversionTime, int maxConversionPerProcess, string rootPath, int totalSizeQuota, int rowNumberPerExcelPage, int maxInputSize, int maxOutputSize, bool isImageMode, HtmlFormat htmlFormat, int memoryLimitInMB)
		{
			if (!TranscodingTaskManager.isInitialized)
			{
				lock (TranscodingTaskManager.initLockObj)
				{
					if (!TranscodingTaskManager.isInitialized)
					{
						ExTraceGlobals.TranscodingTracer.TraceDebug(0L, "Start Initialization");
						TranscodingTaskManager.transcodingTaskManager = new TranscodingTaskManager(maxConversionTime, maxConversionPerProcess, rootPath, totalSizeQuota * 1024 * 1024, rowNumberPerExcelPage, maxInputSize * 1024, maxOutputSize * 1024, isImageMode, htmlFormat, memoryLimitInMB * 1024 * 1024);
						TranscodingTaskManager.isInitialized = true;
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingStartSuccessfully, string.Empty, new object[0]);
						ExTraceGlobals.TranscodingTracer.TraceDebug(0L, "Initialization finished");
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static void Transcode(string docId, string sessionId, Stream inputStream, string sourceDocType, int currentPageNumber, out int totalPageNumber, HttpResponse response)
		{
			TranscodingTaskManager.CheckInitialize();
			if (string.IsNullOrEmpty(docId))
			{
				throw new ArgumentNullException("docId");
			}
			if (string.IsNullOrEmpty(sessionId))
			{
				throw new ArgumentNullException("sessionId");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (sourceDocType == null)
			{
				throw new ArgumentNullException("sourceDocType");
			}
			if (currentPageNumber < 0)
			{
				throw new ArgumentException("Invalid current page number", "currentPageNumber");
			}
			TranscodingTaskManager.transcodingTaskManager.TranscodeWorker(docId, sessionId, inputStream, sourceDocType, currentPageNumber, out totalPageNumber, response);
		}

		public static void TransmitFile(string sessionId, string documentId, string fileName, HttpResponse response)
		{
			TranscodingTaskManager.CheckInitialize();
			if (string.IsNullOrEmpty(documentId))
			{
				throw new ArgumentNullException("documentId");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			TranscodingTaskManager.transcodingTaskManager.cache.TransmitFile(sessionId, documentId, fileName, response);
		}

		public static void RemoveSession(string sessionId)
		{
			if (TranscodingTaskManager.isInitialized)
			{
				TranscodingTaskManager.transcodingTaskManager.cache.RemoveSession(sessionId);
			}
		}

		public static void StopTranscoding()
		{
			if (!TranscodingTaskManager.isInitialized)
			{
				return;
			}
			lock (TranscodingTaskManager.initLockObj)
			{
				TranscodingTaskManager.transcodingTaskManager.Dispose();
				TranscodingTaskManager.transcodingTaskManager = null;
				TranscodingTaskManager.isInitialized = false;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.transcodingProcessManager.Dispose();
				this.transcodingActiveMutex.Close();
				this.cache.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TranscodingTaskManager>(this);
		}

		private static void CheckInitialize()
		{
			if (!TranscodingTaskManager.isInitialized)
			{
				throw new TranscodingFatalFaultException("TranscodingTaskManager is uninitialized!");
			}
		}

		private void OnCreateWorkerDelegate(IComWorker<ITranscoder> worker, object requestParameters)
		{
			ITranscoder worker2 = worker.Worker;
			if (worker2 == null)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerInitializationFailed, string.Empty, new object[]
				{
					string.Empty
				});
				throw new TranscodingFatalFaultException("TranscodingTaskManager failed to get ITranscoder interface", null, this);
			}
			TranscodingInitOption initOption = default(TranscodingInitOption);
			initOption.MaxOutputSize = this.maxOutputSize;
			initOption.RowNumberPerExcelPage = this.rowNumberInExcel;
			initOption.HtmlOutputFormat = this.htmlFormat;
			initOption.IsImageMode = this.isImageMode;
			TranscodeErrorCode transcodeErrorCode = TranscodeErrorCode.Succeeded;
			try
			{
				transcodeErrorCode = worker2.Initialize(initOption);
			}
			catch (NullReferenceException innerException)
			{
				throw new TranscodingFatalFaultException("Worker has been terminated by some reason", innerException, this);
			}
			catch (COMException ex)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug((long)this.GetHashCode(), "Work object initialize failed!");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerInitializationFailed, string.Empty, new object[]
				{
					ex.Message
				});
				throw new TranscodingFatalFaultException("TranscodingTaskManager call ITranscoder.Initialize() failed!", ex, this);
			}
			if (transcodeErrorCode != TranscodeErrorCode.Succeeded)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerInitializationFailed, string.Empty, new object[]
				{
					string.Empty
				});
				throw new TranscodingFatalFaultException(string.Format("Initializalize Transcoding service failed with error code : {0}.", transcodeErrorCode), null, this);
			}
		}

		private bool OnExecuteRequestDelegate(IComWorker<ITranscoder> worker, object requestParameters)
		{
			TranscodingParameters transcodingParameters = (TranscodingParameters)requestParameters;
			ITranscoder transcoder = null;
			try
			{
				transcoder = worker.Worker;
			}
			catch (ObjectDisposedException innerException)
			{
				throw new TranscodingFatalFaultException("Worker object has been disposed", innerException);
			}
			this.UpdatePerformanceCounterAfterLeftQueue(transcodingParameters);
			this.UpdatePerformanceCounterBeforeConversion(transcodingParameters);
			int totalPageNumber = 0;
			int num = 0;
			TranscodeErrorCode? transcodeErrorCode = null;
			string text = null;
			string text2 = null;
			bool result;
			try
			{
				try
				{
					ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Start to cache input document {0}", transcodingParameters.DocumentId);
					this.cache.CacheInputDocument(transcodingParameters.SessionId, transcodingParameters.DocumentId, transcodingParameters.SourceStream, out text, out text2);
					using (FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						transcodingParameters.SourceDocSize = (int)fileStream.Length;
					}
					ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Start to transcode {0}", transcodingParameters.DocumentId);
					transcodeErrorCode = new TranscodeErrorCode?(transcoder.Convert(text, text2, transcodingParameters.SourceDocType, transcodingParameters.CurrentPageNumber, out totalPageNumber, out num));
					ExTraceGlobals.TranscodingTracer.TraceDebug<string, TranscodeErrorCode?>((long)this.GetHashCode(), "Document {0} transcoding finished with return code {1}", transcodingParameters.DocumentId, transcodeErrorCode);
					transcodingParameters.TotalPageNumber = totalPageNumber;
				}
				finally
				{
					this.UpdatePerformanceCounterAfterConversion(transcodingParameters);
					transcodingParameters.ErrorCode = transcodeErrorCode;
					if (transcodeErrorCode == TranscodeErrorCode.Succeeded)
					{
						OwaSingleCounters.TotalConvertingResponseRate.IncrementBy((long)(num / 1024));
					}
					string rewrittenHtmlFileName = null;
					if (!string.IsNullOrEmpty(text2))
					{
						this.cache.NotifyTranscodingFinish(transcodingParameters.SessionId, transcodingParameters.DocumentId, Path.GetFileName(text2), out rewrittenHtmlFileName, transcodeErrorCode == TranscodeErrorCode.Succeeded);
						transcodingParameters.RewrittenHtmlFileName = rewrittenHtmlFileName;
					}
				}
				if (transcodeErrorCode == TranscodeErrorCode.Succeeded || transcodeErrorCode == TranscodeErrorCode.WrongFileTypeError || transcodeErrorCode == TranscodeErrorCode.InvalidPageNumberError)
				{
					result = true;
				}
				else
				{
					this.blockList.AddNew(transcodingParameters.DocumentId);
					result = false;
				}
			}
			catch (IOException ex)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Document {0} can not be transcoded because of an IO error. Exception message: {1}", transcodingParameters.DocumentId, ex.Message);
				throw new TranscodingUnconvertibleFileException(string.Format("Document {0} can not be transcoded because of an IO error.", transcodingParameters.DocumentId), ex, this);
			}
			return result;
		}

		private void CreateMutexFailed(Exception e)
		{
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingManagerInitializationFailed, string.Empty, new object[]
			{
				e.Message
			});
			throw new TranscodingFatalFaultException("Create Mutex for active transcoding service com object failed.", e, this);
		}

		private void TranscodeWorker(string docId, string sessionId, Stream inputStream, string sourceDocType, int currentPageNumber, out int totalPageNumber, HttpResponse response)
		{
			ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Start to process document {0}", docId);
			if (this.blockList.CheckItem(docId))
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Document {0} has been blocked", docId);
				OwaSingleCounters.TotalRejectedConversions.Increment();
				OwaSingleCounters.TotalConversions.Increment();
				throw new TranscodingUnconvertibleFileException("Input document has been in block list.", null, this);
			}
			TranscodingParameters transcodingParameters = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				transcodingParameters = new TranscodingParameters(sessionId, docId, inputStream, sourceDocType, currentPageNumber);
				this.UpdatePerformanceCounterBeforeEnterQueue(transcodingParameters);
				flag = this.transcodingProcessManager.ExecuteRequest(transcodingParameters);
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147023170)
				{
					this.blockList.AddNew(transcodingParameters.DocumentId);
					throw new TranscodingCrashException("Worker process crashes when doing convension", ex, this);
				}
				throw new TranscodingFatalFaultException("ComException thrown from transcoding service", ex, this);
			}
			catch (ComInterfaceInitializeException ex2)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Document {0} can not be transcoded because of server initialize failed.", docId);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_TranscodingWorkerInitializationFailed, string.Empty, new object[]
				{
					ex2.Message
				});
				throw new TranscodingFatalFaultException("Initizlie transcoding service failed.", ex2, this);
			}
			catch (ComProcessTimeoutException innerException)
			{
				flag2 = true;
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Document {0} can not be transcoded because it takes too long to convert.", docId);
				this.blockList.AddNew(docId);
				throw new TranscodingTimeoutException("Takes too long to convert " + docId, innerException, this);
			}
			catch (ComProcessBusyException innerException2)
			{
				flag2 = true;
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Document {0} can not be transcoded because of server busy.", docId);
				throw new TranscodingServerBusyException("Server to busy to accept the request.", innerException2, this);
			}
			catch (ComProcessBeyondMemoryLimitException innerException3)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Document {0} can not be transcoded because of it causes transcodingservice consuming too much memory.", docId);
				this.blockList.AddNew(docId);
				throw new TranscodingUnconvertibleFileException("Document " + docId + " causes transcodingservice too memory to transcode it.", innerException3, this);
			}
			catch (UnauthorizedAccessException ex3)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Document {0} can not be transcoded because of the cache folder cannot be accessed. Exception message: {1}", docId, ex3.Message);
				throw new TranscodingFatalFaultException(string.Format("Document {0} can not be transcoded because of the cache folder cannot be accessed", docId), ex3, this);
			}
			finally
			{
				OwaSingleCounters.TotalConvertingRequestsRate.IncrementBy((long)(transcodingParameters.SourceDocSize / 1024));
				this.UpdatePerformanceCounterAfterLeftQueue(transcodingParameters);
				OwaSingleCounters.TotalConversions.Increment();
				if (!flag || transcodingParameters == null || transcodingParameters.ErrorCode != TranscodeErrorCode.Succeeded)
				{
					if (flag2)
					{
						OwaSingleCounters.TotalTimeoutConversions.Increment();
					}
					else
					{
						OwaSingleCounters.TotalErrorConversions.Increment();
					}
				}
			}
			if (transcodingParameters.ErrorCode != TranscodeErrorCode.Succeeded)
			{
				this.HandleTranscoderErrorCode(transcodingParameters.ErrorCode);
			}
			OwaSingleCounters.SuccessfulConversionRequestRate.IncrementBy((long)(transcodingParameters.SourceDocSize / 1024));
			totalPageNumber = transcodingParameters.TotalPageNumber;
			if (response != null)
			{
				this.cache.TransmitFile(sessionId, docId, transcodingParameters.RewrittenHtmlFileName, response);
			}
		}

		private void UpdatePerformanceCounterBeforeEnterQueue(TranscodingParameters parameters)
		{
			ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Transaction {0} enter the queue.", parameters.DocumentId);
			OwaSingleCounters.QueuedConversionRequests.Increment();
			parameters.Stopwatch.Start();
		}

		private void UpdatePerformanceCounterAfterLeftQueue(TranscodingParameters parameters)
		{
			if (parameters != null && !parameters.IsLeftQueueHandled)
			{
				ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Transaction {0} left the queue.", parameters.DocumentId);
				OwaSingleCounters.QueuedConversionRequests.Decrement();
				parameters.Stopwatch.Stop();
				long num = (long)Interlocked.Add(ref this.totalQueuedTime, (int)parameters.Stopwatch.ElapsedMilliseconds);
				long num2 = (long)Interlocked.Increment(ref this.totalRequestCount);
				OwaSingleCounters.AverageConversionQueuingTime.RawValue = (long)((int)(num / num2));
				parameters.IsLeftQueueHandled = true;
			}
		}

		private void UpdatePerformanceCounterBeforeConversion(TranscodingParameters parameters)
		{
			ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Transaction {0} start to transcoding.", parameters.DocumentId);
			OwaSingleCounters.ActiveConversions.Increment();
			parameters.Stopwatch.Reset();
			parameters.Stopwatch.Start();
		}

		private void UpdatePerformanceCounterAfterConversion(TranscodingParameters parameters)
		{
			ExTraceGlobals.TranscodingTracer.TraceDebug<string>((long)this.GetHashCode(), "Transaction {0} finished.", parameters.DocumentId);
			parameters.Stopwatch.Stop();
			long num = Interlocked.Add(ref this.totalConversionTime, (long)((int)parameters.Stopwatch.ElapsedMilliseconds));
			long num2 = Interlocked.Increment(ref this.totalConversionCount);
			OwaSingleCounters.AverageConvertingTime.RawValue = (long)((int)(num / num2));
			OwaSingleCounters.ActiveConversions.Decrement();
		}

		private void HandleTranscoderErrorCode(TranscodeErrorCode? error)
		{
			TranscodeErrorCode valueOrDefault = error.GetValueOrDefault();
			if (error != null)
			{
				switch (valueOrDefault)
				{
				case TranscodeErrorCode.FatalFaultError:
					throw new TranscodingFatalFaultException("Fatal fault returns from transcoding service", null, this);
				case TranscodeErrorCode.UnconvertibleError:
					throw new TranscodingUnconvertibleFileException("Transcoding service returns unconvertible error.", null, this);
				case TranscodeErrorCode.WrongFileTypeError:
					throw new TranscodingErrorFileException("Wrong input document type", null, this);
				case TranscodeErrorCode.InvalidPageNumberError:
					throw new TranscodingUnconvertibleFileException("Invalid page number", null, this);
				}
			}
			throw new TranscodingUnconvertibleFileException("Transcoding service returns unknown value.", null, this);
		}

		private const int MaxWaitTimeForWorker = 30;

		private const int MaxBlockListSize = 3000;

		private const int MaxBlockTime = 86400;

		private const int WorkProcessRecycleInterval = 7200;

		private const string TranscodingServiceGuid = "F7CC98B4-F6F5-489D-98EB-000689C721DE";

		private const int ComCallFailed = -2147023170;

		private static TranscodingTaskManager transcodingTaskManager;

		private static bool isInitialized;

		private static object initLockObj = new object();

		private static int maxWorkProcessNumber = Environment.ProcessorCount * 3;

		private Cache cache;

		private BlockList blockList;

		private ComProcessManager<ITranscoder> transcodingProcessManager;

		private long totalConversionTime;

		private int totalQueuedTime;

		private long totalConversionCount;

		private int totalRequestCount;

		private int rowNumberInExcel;

		private int maxOutputSize;

		private bool isImageMode;

		private HtmlFormat htmlFormat;

		private string workProcessPath;

		private Mutex transcodingActiveMutex;
	}
}
