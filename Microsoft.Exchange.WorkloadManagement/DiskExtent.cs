using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal struct DiskExtent
	{
		public int DiskNumber;

		public long StartingOffset;

		public long ExtentLength;
	}
}
