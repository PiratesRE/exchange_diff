using System;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public interface IDiagnosticLogger : IDisposable
	{
		void LogError(string format, params object[] arguments);

		void LogInformation(string format, params object[] arguments);

		void LogDebug(string format, params object[] arguments);
	}
}
