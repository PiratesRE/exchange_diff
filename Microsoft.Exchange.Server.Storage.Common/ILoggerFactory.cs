using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface ILoggerFactory : IDisposable
	{
		IBinaryLogger GetLoggerInstance(LoggerType type);

		bool IsTracingEnabled(LoggerType type);
	}
}
