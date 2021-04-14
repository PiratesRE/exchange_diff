using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsCNameRecord : DnsRecord
	{
		internal DnsCNameRecord(string name) : base(name)
		{
		}

		internal DnsCNameRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			this.host = ((DnsCNameRecord.Win32DnsCNameRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsCNameRecord.Win32DnsCNameRecord))).nameHost;
		}

		internal DnsCNameRecord(string name, string cname) : base(name)
		{
			this.host = cname;
			base.RecordType = DnsRecordType.CNAME;
		}

		public static IComparer<DnsCNameRecord> Comparer
		{
			get
			{
				if (DnsCNameRecord.comparer == null)
				{
					DnsCNameRecord.comparer = new DnsCNameRecord.CNameComparer();
				}
				return DnsCNameRecord.comparer;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " " + this.host;
		}

		private static IComparer<DnsCNameRecord> comparer;

		private string host;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsCNameRecord
		{
			public string nameHost;
		}

		private class CNameComparer : IComparer<DnsCNameRecord>
		{
			public int Compare(DnsCNameRecord x, DnsCNameRecord y)
			{
				if (x == null)
				{
					if (y != null)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (y == null)
					{
						return 1;
					}
					return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
				}
			}
		}
	}
}
