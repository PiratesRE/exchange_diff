using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsNsRecord : DnsRecord
	{
		internal DnsNsRecord(string name) : base(name)
		{
		}

		internal DnsNsRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			this.host = ((DnsNsRecord.Win32DnsNsRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsNsRecord.Win32DnsNsRecord))).nameHost;
		}

		internal DnsNsRecord(string name, string ns) : base(name)
		{
			this.host = ns;
			base.RecordType = DnsRecordType.NS;
		}

		public static IComparer<DnsNsRecord> Comparer
		{
			get
			{
				if (DnsNsRecord.comparer == null)
				{
					DnsNsRecord.comparer = new DnsNsRecord.NsComparer();
				}
				return DnsNsRecord.comparer;
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

		private static IComparer<DnsNsRecord> comparer;

		private readonly string host;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsNsRecord
		{
			public string nameHost;
		}

		private class NsComparer : IComparer<DnsNsRecord>
		{
			public int Compare(DnsNsRecord x, DnsNsRecord y)
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
					return string.Compare(x.Host, y.Host, StringComparison.OrdinalIgnoreCase);
				}
			}
		}
	}
}
