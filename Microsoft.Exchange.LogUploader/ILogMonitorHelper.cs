using System;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal interface ILogMonitorHelper<T> where T : LogDataBatch
	{
		CsvTable GetLogSchema(Version version);

		T CreateBatch(int batchSizeInBytes, long batchBeginOffset, string fullLogName, string logPrefix);

		DatabaseWriter<T> CreateDBWriter(ThreadSafeQueue<T> queue, int id, ConfigInstance config, string instanceName);

		string GetDefaultLogVersion();

		bool ShouldProcessLogFile(string logPrefix, string fileName);

		void Initialize(CancellationToken cancellationToken);
	}
}
