using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Net
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct Win32DnsRecordHeader
	{
		public DnsResponseSection Section
		{
			get
			{
				return (DnsResponseSection)(this.flags & 3U);
			}
		}

		public Encoding Encoding
		{
			get
			{
				switch (this.flags >> 3 & 3U)
				{
				case 1U:
					return Encoding.Unicode;
				case 2U:
					return Encoding.UTF8;
				case 3U:
					return Encoding.ASCII;
				default:
					return null;
				}
			}
		}

		public static readonly int MarshalSize = Marshal.SizeOf(typeof(Win32DnsRecordHeader));

		public IntPtr nextRecord;

		public string name;

		public DnsRecordType recordType;

		public ushort dataLength;

		private uint flags;

		public uint ttl;

		public uint reserved;
	}
}
