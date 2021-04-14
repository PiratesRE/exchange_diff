using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IBinaryLogger : IDisposable
	{
		bool IsLoggingEnabled { get; }

		bool IsDisposed { get; }

		void Start();

		void Stop();

		bool TryWrite(TraceBuffer buffer, int retries, TimeSpan timeToWait);

		bool TryWrite(TraceBuffer buffer);
	}
}
