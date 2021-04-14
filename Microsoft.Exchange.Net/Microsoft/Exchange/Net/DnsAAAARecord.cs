using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsAAAARecord : DnsAddressRecord
	{
		internal DnsAAAARecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			this.address = new IPAddress(((DnsAAAARecord.AryIp6Address)Marshal.PtrToStructure(dataPointer, typeof(DnsAAAARecord.AryIp6Address))).bytes);
		}

		internal DnsAAAARecord(string name, IPAddress address) : base(name, address)
		{
			this.address = address;
			base.RecordType = DnsRecordType.AAAA;
		}

		private struct AryIp6Address
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] bytes;
		}
	}
}
