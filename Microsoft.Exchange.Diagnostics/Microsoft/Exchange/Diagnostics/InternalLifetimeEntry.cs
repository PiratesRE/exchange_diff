using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InternalLifetimeEntry
	{
		public int LifetimeType;

		public int ProcessId;

		public long StartupTime;
	}
}
