using System;
using System.Collections.Generic;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogUploaderDefaultCommonPerfCountersInstance : ILogUploaderPerformanceCounters
	{
		public LogUploaderDefaultCommonPerfCountersInstance(string instanceName, LogUploaderDefaultCommonPerfCountersInstance autoUpdateTotalInstance = null)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes processed/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalLogBytesProcessed = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes processed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalLogBytesProcessed, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalLogBytesProcessed);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log parse errors/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalParseErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log parse errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalParseErrors, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalParseErrors);
				this.BatchQueueLength = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log batch current queue length", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BatchQueueLength, new ExPerformanceCounter[0]);
				list.Add(this.BatchQueueLength);
				this.InputBufferBatchCounts = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Input buffer batch count", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InputBufferBatchCounts, new ExPerformanceCounter[0]);
				list.Add(this.InputBufferBatchCounts);
				this.InputBufferBackfilledLines = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Input buffer backfilled lines/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InputBufferBackfilledLines, new ExPerformanceCounter[0]);
				list.Add(this.InputBufferBackfilledLines);
				this.LogsNeverProcessedBefore = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Number of logs that have never been processed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LogsNeverProcessedBefore, new ExPerformanceCounter[0]);
				list.Add(this.LogsNeverProcessedBefore);
				this.AverageDbWriteLatency = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Average DB write time in seconds", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageDbWriteLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageDbWriteLatency);
				this.AverageDbWriteLatencyBase = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Average DB write time in seconds Base", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageDbWriteLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageDbWriteLatencyBase);
				this.AverageInactiveParseLatency = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Average inactive log parse time in seconds", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageInactiveParseLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageInactiveParseLatency);
				this.AverageInactiveParseLatencyBase = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Average inactive log parse time in seconds Base", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageInactiveParseLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageInactiveParseLatencyBase);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log lines written to database/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalLogLinesProcessed = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total log lines written to database", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalLogLinesProcessed, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalLogLinesProcessed);
				this.TotalIncompleteLogs = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of incomplete logs", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalIncompleteLogs, new ExPerformanceCounter[0]);
				list.Add(this.TotalIncompleteLogs);
				this.TotalIncomingLogs = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of incoming logs", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalIncomingLogs, new ExPerformanceCounter[0]);
				list.Add(this.TotalIncomingLogs);
				this.NumberOfIncomingLogs = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Number of new logs generated in the last directory scan interval", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfIncomingLogs, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfIncomingLogs);
				this.TotalCompletedLogs = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Number of logs that are completely processed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalCompletedLogs, new ExPerformanceCounter[0]);
				list.Add(this.TotalCompletedLogs);
				this.TotalNewLogsBeginProcessing = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Number of new logs begin to be processed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalNewLogsBeginProcessing, new ExPerformanceCounter[0]);
				list.Add(this.TotalNewLogsBeginProcessing);
				this.TotalOpticsTraces = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of traces sent to optics pipelines", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalOpticsTraces, new ExPerformanceCounter[0]);
				list.Add(this.TotalOpticsTraces);
				this.TotalOpticsTraceExtractionErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Optics trace data extraction errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalOpticsTraceExtractionErrors, new ExPerformanceCounter[0]);
				list.Add(this.TotalOpticsTraceExtractionErrors);
				this.OpticsTracesPerSecond = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Rate of traces sent to optics pipelines in seconds.", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OpticsTracesPerSecond, new ExPerformanceCounter[0]);
				list.Add(this.OpticsTracesPerSecond);
				this.OpticsTraceExtractionErrorsPerSecond = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Rate of optics traces extraction errors.", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OpticsTraceExtractionErrorsPerSecond, new ExPerformanceCounter[0]);
				list.Add(this.OpticsTraceExtractionErrorsPerSecond);
				this.TotalInvalidLogLineParseErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of log parse errors because of invalid log line", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalInvalidLogLineParseErrors, new ExPerformanceCounter[0]);
				list.Add(this.TotalInvalidLogLineParseErrors);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "DB writes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TotalDBWrite = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of database writes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalDBWrite, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TotalDBWrite);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "DAL permanent errors/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.TotalDBPermanentErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of database permanent errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalDBPermanentErrors, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TotalDBPermanentErrors);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "database transient errors/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.TotalDBTransientErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of database transient errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalDBTransientErrors, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.TotalDBTransientErrors);
				this.TotalUnexpectedWriterErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of unexpected writer errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalUnexpectedWriterErrors, new ExPerformanceCounter[0]);
				list.Add(this.TotalUnexpectedWriterErrors);
				this.TotalLogReaderUnknownErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of unexpected log reader errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalLogReaderUnknownErrors, new ExPerformanceCounter[0]);
				list.Add(this.TotalLogReaderUnknownErrors);
				this.TotalMessageTracingDualWriteErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of MessageTracing dual write errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalMessageTracingDualWriteErrors, new ExPerformanceCounter[0]);
				list.Add(this.TotalMessageTracingDualWriteErrors);
				this.DirectoryCheck = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Directory check numbers", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DirectoryCheck, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryCheck);
				this.RawIncompleteBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data unprocessed bytes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RawIncompleteBytes, new ExPerformanceCounter[0]);
				list.Add(this.RawIncompleteBytes);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data input/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.RawTotalLogBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data input bytes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RawTotalLogBytes, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.RawTotalLogBytes);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data parsed bytes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.RawReaderParsedBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data parsed bytes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RawReaderParsedBytes, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.RawReaderParsedBytes);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data processed bytes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.RawWrittenBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Combo raw data processed bytes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RawWrittenBytes, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.RawWrittenBytes);
				this.IncompleteBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes unprocessed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.IncompleteBytes, new ExPerformanceCounter[0]);
				list.Add(this.IncompleteBytes);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes input/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				this.TotalLogBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes input", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalLogBytes, new ExPerformanceCounter[]
				{
					exPerformanceCounter10
				});
				list.Add(this.TotalLogBytes);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes parsed/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.ReaderParsedBytes = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Log bytes parsed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReaderParsedBytes, new ExPerformanceCounter[]
				{
					exPerformanceCounter11
				});
				list.Add(this.ReaderParsedBytes);
				this.DBWriteActiveTime = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "DB Write Active Time", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DBWriteActiveTime, new ExPerformanceCounter[0]);
				list.Add(this.DBWriteActiveTime);
				this.LogReaderActiveTime = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "LogReader Active Time", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LogReaderActiveTime, new ExPerformanceCounter[0]);
				list.Add(this.LogReaderActiveTime);
				this.LogMonitorActiveTime = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "LogMonitor Active Time", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LogMonitorActiveTime, new ExPerformanceCounter[0]);
				list.Add(this.LogMonitorActiveTime);
				this.ThreadSafeQueueConsumerSemaphoreCount = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "queue consumer semaphore count", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ThreadSafeQueueConsumerSemaphoreCount, new ExPerformanceCounter[0]);
				list.Add(this.ThreadSafeQueueConsumerSemaphoreCount);
				this.ThreadSafeQueueProducerSemaphoreCount = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "queue producer semaphore count", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ThreadSafeQueueProducerSemaphoreCount, new ExPerformanceCounter[0]);
				list.Add(this.ThreadSafeQueueProducerSemaphoreCount);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Missing certificate errors/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.TotalMissingCertificateErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of missing certificate errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalMissingCertificateErrors, new ExPerformanceCounter[]
				{
					exPerformanceCounter12
				});
				list.Add(this.TotalMissingCertificateErrors);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Saving MessageTypeMapping Saved/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.TotalMessageTypeMappingSaved = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of MessageTypeMapping saved", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalMessageTypeMappingSaved, new ExPerformanceCounter[]
				{
					exPerformanceCounter13
				});
				list.Add(this.TotalMessageTypeMappingSaved);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Saving MessageTypeMappingerrors/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.TotalMessageTypeMappingErrors = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of saving MessageTypeMapping errors", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalMessageTypeMappingErrors, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.TotalMessageTypeMappingErrors);
				this.TotalRobocopyIncompleteLogs = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of robo copy incomplete logs", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalRobocopyIncompleteLogs, new ExPerformanceCounter[0]);
				list.Add(this.TotalRobocopyIncompleteLogs);
				this.WriterPoisonDataBatch = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of writing poison batches", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriterPoisonDataBatch, new ExPerformanceCounter[0]);
				list.Add(this.WriterPoisonDataBatch);
				this.ReaderPoisonDataBatch = new ExPerformanceCounter("Microsoft Forefront Message Tracing service counters.", "Total number of parsing poison batches", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReaderPoisonDataBatch, new ExPerformanceCounter[0]);
				list.Add(this.ReaderPoisonDataBatch);
				long num = this.TotalLogBytesProcessed.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter15 in list)
					{
						exPerformanceCounter15.Close();
					}
				}
			}
		}

		public ExPerformanceCounter TotalLogBytesProcessed { get; private set; }

		public ExPerformanceCounter TotalParseErrors { get; private set; }

		public ExPerformanceCounter BatchQueueLength { get; private set; }

		public ExPerformanceCounter InputBufferBatchCounts { get; private set; }

		public ExPerformanceCounter InputBufferBackfilledLines { get; private set; }

		public ExPerformanceCounter LogsNeverProcessedBefore { get; private set; }

		public ExPerformanceCounter AverageDbWriteLatency { get; private set; }

		public ExPerformanceCounter AverageDbWriteLatencyBase { get; private set; }

		public ExPerformanceCounter AverageInactiveParseLatency { get; private set; }

		public ExPerformanceCounter AverageInactiveParseLatencyBase { get; private set; }

		public ExPerformanceCounter TotalLogLinesProcessed { get; private set; }

		public ExPerformanceCounter TotalIncompleteLogs { get; private set; }

		public ExPerformanceCounter TotalIncomingLogs { get; private set; }

		public ExPerformanceCounter NumberOfIncomingLogs { get; private set; }

		public ExPerformanceCounter TotalCompletedLogs { get; private set; }

		public ExPerformanceCounter TotalNewLogsBeginProcessing { get; private set; }

		public ExPerformanceCounter TotalOpticsTraces { get; private set; }

		public ExPerformanceCounter TotalOpticsTraceExtractionErrors { get; private set; }

		public ExPerformanceCounter OpticsTracesPerSecond { get; private set; }

		public ExPerformanceCounter OpticsTraceExtractionErrorsPerSecond { get; private set; }

		public ExPerformanceCounter TotalInvalidLogLineParseErrors { get; private set; }

		public ExPerformanceCounter TotalDBWrite { get; private set; }

		public ExPerformanceCounter TotalDBPermanentErrors { get; private set; }

		public ExPerformanceCounter TotalDBTransientErrors { get; private set; }

		public ExPerformanceCounter TotalUnexpectedWriterErrors { get; private set; }

		public ExPerformanceCounter TotalLogReaderUnknownErrors { get; private set; }

		public ExPerformanceCounter TotalMessageTracingDualWriteErrors { get; private set; }

		public ExPerformanceCounter DirectoryCheck { get; private set; }

		public ExPerformanceCounter RawIncompleteBytes { get; private set; }

		public ExPerformanceCounter RawTotalLogBytes { get; private set; }

		public ExPerformanceCounter RawReaderParsedBytes { get; private set; }

		public ExPerformanceCounter RawWrittenBytes { get; private set; }

		public ExPerformanceCounter IncompleteBytes { get; private set; }

		public ExPerformanceCounter TotalLogBytes { get; private set; }

		public ExPerformanceCounter ReaderParsedBytes { get; private set; }

		public ExPerformanceCounter DBWriteActiveTime { get; private set; }

		public ExPerformanceCounter LogReaderActiveTime { get; private set; }

		public ExPerformanceCounter LogMonitorActiveTime { get; private set; }

		public ExPerformanceCounter ThreadSafeQueueConsumerSemaphoreCount { get; private set; }

		public ExPerformanceCounter ThreadSafeQueueProducerSemaphoreCount { get; private set; }

		public ExPerformanceCounter TotalMissingCertificateErrors { get; private set; }

		public ExPerformanceCounter TotalMessageTypeMappingSaved { get; private set; }

		public ExPerformanceCounter TotalMessageTypeMappingErrors { get; private set; }

		public ExPerformanceCounter TotalRobocopyIncompleteLogs { get; private set; }

		public ExPerformanceCounter WriterPoisonDataBatch { get; private set; }

		public ExPerformanceCounter ReaderPoisonDataBatch { get; private set; }

		private const string CategoryName = "Microsoft Forefront Message Tracing service counters.";
	}
}
