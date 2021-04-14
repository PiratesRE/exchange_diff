using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.LogUploader
{
	internal interface IWatermarkFile : IDisposable
	{
		string WatermarkFileFullName { get; }

		string LogFileFullName { get; }

		long ProcessedSize { get; }

		bool IsDisposed { get; }

		void InMemoryCountDecrease();

		void InMemoryCountIncrease();

		void WriteWatermark(List<LogFileRange> logfileRanges);

		LogFileRange GetBlockToReprocess();

		LogFileRange GetNewBlockToProcess();

		void UpdateLastReaderParsedEndOffset(long newEndOffset);

		bool ReaderHasBytesToParse();

		bool IsLogCompleted();

		void CreateDoneFile();
	}
}
