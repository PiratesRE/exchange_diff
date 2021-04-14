using System;

namespace Microsoft.Exchange.Connections.Common
{
	[Flags]
	public enum LogLevel
	{
		LogNone = 0,
		LogVerbose = 1,
		LogDebug = 2,
		LogTrace = 4,
		LogInfo = 8,
		LogWarn = 16,
		LogError = 32,
		LogFatal = 64,
		LogDefault = 126,
		LogAll = 127,
		LogSerious = 112
	}
}
