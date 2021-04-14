using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsSoaRecord : DnsRecord
	{
		internal DnsSoaRecord(string name) : base(name)
		{
		}

		internal DnsSoaRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			DnsSoaRecord.Win32DnsSoaRecord win32DnsSoaRecord = (DnsSoaRecord.Win32DnsSoaRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsSoaRecord.Win32DnsSoaRecord));
			this.primaryServer = win32DnsSoaRecord.namePrimaryServer;
			this.administrator = win32DnsSoaRecord.nameAdministrator;
			this.serialNumber = win32DnsSoaRecord.serialNumber;
			this.refresh = win32DnsSoaRecord.refresh;
			this.retry = win32DnsSoaRecord.retry;
			this.expire = win32DnsSoaRecord.expire;
			this.defaultTimeToLive = win32DnsSoaRecord.defaultTTL;
		}

		public string PrimaryServer
		{
			get
			{
				return this.primaryServer;
			}
		}

		public string Administrator
		{
			get
			{
				return this.administrator;
			}
		}

		public int SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public int Refresh
		{
			get
			{
				return this.refresh;
			}
		}

		public int Retry
		{
			get
			{
				return this.retry;
			}
		}

		public int Expire
		{
			get
			{
				return this.expire;
			}
		}

		public int DefaultTimeToLive
		{
			get
			{
				return this.defaultTimeToLive;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " " + base.Name;
		}

		private readonly string primaryServer;

		private readonly string administrator;

		private readonly int serialNumber;

		private readonly int refresh;

		private readonly int retry;

		private readonly int expire;

		private readonly int defaultTimeToLive;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsSoaRecord
		{
			public readonly string namePrimaryServer;

			public readonly string nameAdministrator;

			public readonly int serialNumber;

			public readonly int refresh;

			public readonly int retry;

			public readonly int expire;

			public readonly int defaultTTL;
		}
	}
}
