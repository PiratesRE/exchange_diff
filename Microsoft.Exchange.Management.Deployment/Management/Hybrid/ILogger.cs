using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	public interface ILogger : IDisposable
	{
		void Log(LocalizedString text);

		void Log(Exception e);

		void LogError(string text);

		void LogWarning(string text);

		void LogInformation(string text);
	}
}
