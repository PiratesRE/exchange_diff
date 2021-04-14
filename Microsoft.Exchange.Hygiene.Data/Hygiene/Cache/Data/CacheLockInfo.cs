using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class CacheLockInfo
	{
		internal CacheLockInfo(int numLockAttempts, TimeSpan lockTimeout, TimeSpan lockSleepTime)
		{
			this.NumLockAttempts = numLockAttempts;
			this.LockTimeout = lockTimeout;
			this.LockSleepTime = lockSleepTime;
		}

		internal int NumLockAttempts { get; private set; }

		internal TimeSpan LockTimeout { get; private set; }

		internal TimeSpan LockSleepTime { get; private set; }
	}
}
