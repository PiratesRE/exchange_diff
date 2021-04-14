using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct JET_INDEXID : IEquatable<JET_INDEXID>
	{
		internal static uint SizeOfIndexId
		{
			[DebuggerStepThrough]
			get
			{
				return JET_INDEXID.TheSizeOfIndexId;
			}
		}

		public static bool operator ==(JET_INDEXID lhs, JET_INDEXID rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_INDEXID lhs, JET_INDEXID rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_INDEXID)obj);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEXID(0x{0:x}:0x{1:x}:0x{2:x})", new object[]
			{
				this.IndexId1,
				this.IndexId2,
				this.IndexId3
			});
		}

		public override int GetHashCode()
		{
			return this.CbStruct.GetHashCode() ^ this.IndexId1.GetHashCode() ^ this.IndexId2.GetHashCode() ^ this.IndexId3.GetHashCode();
		}

		public bool Equals(JET_INDEXID other)
		{
			return this.CbStruct == other.CbStruct && this.IndexId1 == other.IndexId1 && this.IndexId2 == other.IndexId2 && this.IndexId3 == other.IndexId3;
		}

		internal uint CbStruct;

		internal IntPtr IndexId1;

		internal uint IndexId2;

		internal uint IndexId3;

		private static readonly uint TheSizeOfIndexId = (uint)Marshal.SizeOf(typeof(JET_INDEXID));
	}
}
