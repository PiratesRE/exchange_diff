using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring
{
	internal interface IEventManager
	{
		bool HasEvents();

		void LogEvent(int eventId, string eventMessage);

		void LogEvents(CheckId checkId, ReplicationCheckResultEnum result, List<MessageInfo> messages);
	}
}
