using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EventFlags
	{
		None = 0,
		ReminderPropertiesModified = 1,
		TimerEventFired = 2,
		NonIPMChange = 4
	}
}
