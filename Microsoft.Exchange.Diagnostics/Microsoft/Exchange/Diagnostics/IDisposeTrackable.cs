using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IDisposeTrackable : IDisposable
	{
		DisposeTracker GetDisposeTracker();

		void SuppressDisposeTracker();
	}
}
