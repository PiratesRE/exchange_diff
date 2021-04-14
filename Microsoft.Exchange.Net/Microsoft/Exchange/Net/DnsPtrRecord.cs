using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsPtrRecord : DnsRecord
	{
		internal DnsPtrRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			this.host = ((DnsPtrRecord.Win32DnsPtrRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsPtrRecord.Win32DnsPtrRecord))).nameHost;
		}

		internal DnsPtrRecord(IPAddress address, string host) : base(PtrRequest.ConstructPTRQuery(address))
		{
			this.host = host;
			base.RecordType = DnsRecordType.PTR;
		}

		internal DnsPtrRecord(string name, string host) : base(name)
		{
			this.host = host;
			base.RecordType = DnsRecordType.PTR;
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

		private string host;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsPtrRecord
		{
			public string nameHost;
		}
	}
}
