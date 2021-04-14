using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface ILog
	{
		bool IsEnabled(LogLevel level);

		void Trace(string formatString, params object[] args);

		void Debug(string formatString, params object[] args);

		void Info(string formatString, params object[] args);

		void Warn(string formatString, params object[] args);

		void Error(string formatString, params object[] args);

		void Fatal(string formatString, params object[] args);

		void Fatal(Exception exception, string message = "");

		void Assert(bool condition, string formatString, params object[] args);

		void RetailAssert(bool condition, string formatString, params object[] args);
	}
}
