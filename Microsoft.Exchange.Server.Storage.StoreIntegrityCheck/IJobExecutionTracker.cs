using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IJobExecutionTracker : IProgress<short>
	{
		void OnCorruptionDetected(Corruption corruption);
	}
}
