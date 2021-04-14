using System;
using System.Web;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal interface IE4eLogger
	{
		void LogInfo(HttpContext context, string methodName, string messageID, string messageFormat, params object[] args);

		void LogError(HttpContext context, string methodName, string messageID, string messageFormat, params object[] args);
	}
}
