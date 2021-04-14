using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventLogger
	{
		public EventLogger(string serviceName)
		{
			this.serviceName = serviceName;
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			object[] array = new object[messageArgs.Length + 1];
			array[0] = this.serviceName;
			Array.Copy(messageArgs, 0, array, 1, messageArgs.Length);
			this.logger.LogEvent(tuple, periodicKey, array);
		}

		private ExEventLog logger = new ExEventLog(Globals.ComponentGuid, "MSExchange Assistants");

		private string serviceName;
	}
}
