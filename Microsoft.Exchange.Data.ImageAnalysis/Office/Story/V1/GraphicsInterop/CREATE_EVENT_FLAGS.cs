using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	[Flags]
	internal enum CREATE_EVENT_FLAGS
	{
		CREATE_EVENT_INITIAL_SET = 2,
		CREATE_EVENT_MANUAL_RESET = 1,
		None = 0
	}
}
