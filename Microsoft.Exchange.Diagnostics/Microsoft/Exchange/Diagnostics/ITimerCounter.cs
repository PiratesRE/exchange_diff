using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface ITimerCounter : IDisposable
	{
		void Start();

		long Stop();
	}
}
