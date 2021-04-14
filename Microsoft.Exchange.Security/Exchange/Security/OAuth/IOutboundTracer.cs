using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal interface IOutboundTracer
	{
		void LogInformation(int hashCode, string formatString, params object[] args);

		void LogWarning(int hashCode, string formatString, params object[] args);

		void LogError(int hashCode, string formatString, params object[] args);

		void LogToken(int hashCode, string tokenString);
	}
}
