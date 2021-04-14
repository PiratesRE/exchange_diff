using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InternalInstanceEntry
	{
		public int SpinLock;

		public int InstanceNameHashCode;

		public int InstanceNameOffset;

		public int RefCount;

		public int FirstCounterOffset;

		public int NextInstanceOffset;
	}
}
