using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public interface IJobProgressTracker
	{
		void Report(ProgressInfo progress);
	}
}
