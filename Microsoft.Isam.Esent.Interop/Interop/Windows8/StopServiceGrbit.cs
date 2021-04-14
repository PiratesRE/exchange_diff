using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	[Flags]
	public enum StopServiceGrbit
	{
		All = 0,
		BackgroundUserTasks = 2,
		QuiesceCaches = 4,
		Resume = -2147483648
	}
}
