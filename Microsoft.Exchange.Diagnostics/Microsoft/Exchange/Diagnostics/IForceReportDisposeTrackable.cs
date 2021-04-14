using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IForceReportDisposeTrackable : IDisposeTrackable, IDisposable
	{
		void ForceLeakReport();
	}
}
