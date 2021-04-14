using System;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
	[FriendAccessAllowed]
	internal enum AsyncCausalityStatus
	{
		Canceled = 2,
		Completed = 1,
		Error = 3,
		Started = 0
	}
}
