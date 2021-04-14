using System;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal interface ILogUploaderPerformanceCounters
	{
		ExPerformanceCounter TotalLogBytesProcessed { get; }

		ExPerformanceCounter TotalParseErrors { get; }

		ExPerformanceCounter BatchQueueLength { get; }

		ExPerformanceCounter InputBufferBatchCounts { get; }

		ExPerformanceCounter InputBufferBackfilledLines { get; }

		ExPerformanceCounter LogsNeverProcessedBefore { get; }

		ExPerformanceCounter AverageDbWriteLatency { get; }

		ExPerformanceCounter AverageDbWriteLatencyBase { get; }

		ExPerformanceCounter AverageInactiveParseLatency { get; }

		ExPerformanceCounter AverageInactiveParseLatencyBase { get; }

		ExPerformanceCounter TotalLogLinesProcessed { get; }

		ExPerformanceCounter TotalIncompleteLogs { get; }

		ExPerformanceCounter TotalIncomingLogs { get; }

		ExPerformanceCounter NumberOfIncomingLogs { get; }

		ExPerformanceCounter TotalCompletedLogs { get; }

		ExPerformanceCounter TotalNewLogsBeginProcessing { get; }

		ExPerformanceCounter TotalOpticsTraces { get; }

		ExPerformanceCounter TotalOpticsTraceExtractionErrors { get; }

		ExPerformanceCounter OpticsTracesPerSecond { get; }

		ExPerformanceCounter OpticsTraceExtractionErrorsPerSecond { get; }

		ExPerformanceCounter TotalInvalidLogLineParseErrors { get; }

		ExPerformanceCounter TotalDBWrite { get; }

		ExPerformanceCounter TotalDBPermanentErrors { get; }

		ExPerformanceCounter TotalDBTransientErrors { get; }

		ExPerformanceCounter TotalUnexpectedWriterErrors { get; }

		ExPerformanceCounter TotalLogReaderUnknownErrors { get; }

		ExPerformanceCounter TotalMessageTracingDualWriteErrors { get; }

		ExPerformanceCounter DirectoryCheck { get; }

		ExPerformanceCounter RawIncompleteBytes { get; }

		ExPerformanceCounter RawTotalLogBytes { get; }

		ExPerformanceCounter RawReaderParsedBytes { get; }

		ExPerformanceCounter RawWrittenBytes { get; }

		ExPerformanceCounter IncompleteBytes { get; }

		ExPerformanceCounter TotalLogBytes { get; }

		ExPerformanceCounter ReaderParsedBytes { get; }

		ExPerformanceCounter DBWriteActiveTime { get; }

		ExPerformanceCounter LogReaderActiveTime { get; }

		ExPerformanceCounter LogMonitorActiveTime { get; }

		ExPerformanceCounter ThreadSafeQueueConsumerSemaphoreCount { get; }

		ExPerformanceCounter ThreadSafeQueueProducerSemaphoreCount { get; }

		ExPerformanceCounter TotalMissingCertificateErrors { get; }

		ExPerformanceCounter TotalMessageTypeMappingSaved { get; }

		ExPerformanceCounter TotalMessageTypeMappingErrors { get; }

		ExPerformanceCounter TotalRobocopyIncompleteLogs { get; }

		ExPerformanceCounter WriterPoisonDataBatch { get; }

		ExPerformanceCounter ReaderPoisonDataBatch { get; }
	}
}
