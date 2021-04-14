using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.WorkloadManagement
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct DiskPerformanceStructure
	{
		public static bool operator ==(DiskPerformanceStructure lhs, DiskPerformanceStructure rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DiskPerformanceStructure lhs, DiskPerformanceStructure rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override bool Equals(object obj)
		{
			return obj is DiskPerformanceStructure && this.Equals((DiskPerformanceStructure)obj);
		}

		public override int GetHashCode()
		{
			return this.BytesRead.GetHashCode() ^ this.BytesWritten.GetHashCode() ^ this.ReadTime.GetHashCode() ^ this.WriteTime.GetHashCode() ^ this.IdleTime.GetHashCode() ^ this.ReadCount.GetHashCode() ^ this.WriteCount.GetHashCode() ^ this.QueueDepth.GetHashCode() ^ this.SplitCount.GetHashCode() ^ this.QueryTime.GetHashCode() ^ this.StorageDeviceNumber.GetHashCode() ^ this.StorageManagerName.GetHashCode();
		}

		public bool Equals(DiskPerformanceStructure obj)
		{
			return this.BytesRead == obj.BytesRead && this.BytesWritten == obj.BytesWritten && this.ReadTime == obj.ReadTime && this.WriteTime == obj.WriteTime && this.IdleTime == obj.IdleTime && this.ReadCount == obj.ReadCount && this.WriteCount == obj.WriteCount && this.QueueDepth == obj.QueueDepth && this.SplitCount == obj.SplitCount && this.QueryTime == obj.QueryTime && this.StorageDeviceNumber == obj.StorageDeviceNumber && this.StorageManagerName == obj.StorageManagerName;
		}

		public readonly long BytesRead;

		public readonly long BytesWritten;

		public readonly long ReadTime;

		public readonly long WriteTime;

		public readonly long IdleTime;

		public readonly int ReadCount;

		public readonly int WriteCount;

		public readonly int QueueDepth;

		public readonly int SplitCount;

		public readonly long QueryTime;

		public readonly int StorageDeviceNumber;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		public readonly string StorageManagerName;
	}
}
