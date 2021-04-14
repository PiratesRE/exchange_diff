using System;

namespace Windows.Foundation.Diagnostics
{
	internal enum AsyncCausalityStatus
	{
		Canceled = 2,
		Completed = 1,
		Error = 3,
		Started = 0
	}
}
