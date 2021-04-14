using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum CalendarMethod
	{
		None = 0,
		Publish = 1,
		Request = 2,
		Reply = 4,
		Add = 8,
		Cancel = 16,
		Refresh = 32,
		Counter = 64,
		DeclineCounter = 128,
		All = 255
	}
}
