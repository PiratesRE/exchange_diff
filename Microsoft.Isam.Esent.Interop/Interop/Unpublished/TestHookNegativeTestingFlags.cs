using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public enum TestHookNegativeTestingFlags
	{
		DeletingLogFiles = 1,
		CorruptingLogFiles,
		LockingCheckpointFile = 4,
		CorruptingDbHeaders = 8,
		CorruptingPagePgnos = 16,
		LeakStuff = 32,
		CorruptingWithLostFlush = 64,
		DisableTimeoutDeadlockDetection = 128,
		CorruptingPages = 256,
		DiskIoError = 512,
		InvalidApiUsage = 1024,
		InvalidUsage = 2048
	}
}
