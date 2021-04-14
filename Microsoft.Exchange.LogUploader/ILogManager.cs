using System;

namespace Microsoft.Exchange.LogUploader
{
	internal interface ILogManager
	{
		string Instance { get; }

		void Start();

		void Stop();

		LogFileInfo GetLogForReaderToProcess();

		void ReaderCompletedProcessingLog(LogFileInfo logFile);

		IWatermarkFile FindWatermarkFileObject(string logFileName);
	}
}
