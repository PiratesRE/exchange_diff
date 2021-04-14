using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsARecord : DnsAddressRecord
	{
		internal DnsARecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			this.address = new IPAddress((long)((ulong)((DnsARecord.Win32DnsARecord)Marshal.PtrToStructure(dataPointer, typeof(DnsARecord.Win32DnsARecord))).address));
		}

		internal DnsARecord(string name, IPAddress address) : base(name)
		{
			this.address = address;
			base.RecordType = DnsRecordType.A;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsARecord
		{
			public uint address;
		}
	}
}
