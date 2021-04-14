﻿using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	[Flags]
	internal enum AccessFlags
	{
		DELETE = 65536,
		READ_CONTROL = 131072,
		SYNCHRONIZE = 1048576,
		WRITE_DAC = 262144,
		WRITE_OWNER = 524288,
		EVENT_ALL_ACCESS = 2031619,
		EVENT_MODIFY_STATE = 2,
		MUTEX_ALL_ACCESS = 2031617,
		MUTEX_MODIFY_STATE = 1,
		SEMAPHORE_ALL_ACCESS = 2031619,
		SEMAPHORE_MODIFY_STATE = 2,
		TIMER_ALL_ACCESS = 2031619,
		TIMER_MODIFY_STATE = 2,
		TIMER_QUERY_STATE = 1
	}
}
