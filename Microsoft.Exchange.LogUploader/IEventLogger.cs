using System;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal interface IEventLogger
	{
		void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs);
	}
}
