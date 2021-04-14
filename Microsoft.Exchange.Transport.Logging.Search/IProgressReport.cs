using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public interface IProgressReport
	{
		void Report(int progress);
	}
}
