using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Net
{
	internal class DnsTxtRecord : DnsRecord
	{
		internal DnsTxtRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			DnsTxtRecord.Win32DnsTxtRecord win32DnsTxtRecord = (DnsTxtRecord.Win32DnsTxtRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsTxtRecord.Win32DnsTxtRecord));
			if (win32DnsTxtRecord.stringCount > DnsTxtRecord.MaxStrings)
			{
				win32DnsTxtRecord.stringCount = DnsTxtRecord.MaxStrings;
				this.dnsStatus = DnsStatus.ErrorInvalidData;
			}
			StringBuilder stringBuilder = new StringBuilder((int)header.dataLength);
			IntPtr ptr = new IntPtr((long)dataPointer + (long)Marshal.OffsetOf(typeof(DnsTxtRecord.Win32DnsTxtRecord), "arypStringArray"));
			int num = 0;
			while ((long)num < (long)((ulong)win32DnsTxtRecord.stringCount))
			{
				IntPtr ptr2 = Marshal.ReadIntPtr(ptr, num * Marshal.SizeOf(typeof(IntPtr)));
				stringBuilder.Append(Marshal.PtrToStringUni(ptr2));
				num++;
			}
			this.text = stringBuilder.ToString();
		}

		public static uint MaxStrings
		{
			get
			{
				return DnsTxtRecord.maxStrings;
			}
			set
			{
				DnsTxtRecord.maxStrings = value;
			}
		}

		public string Text
		{
			get
			{
				return string.Concat(new string[]
				{
					this.text
				});
			}
		}

		public DnsStatus Status
		{
			get
			{
				return this.dnsStatus;
			}
		}

		public const int DefaultMaxStrings = 4096;

		private static uint maxStrings = 4096U;

		private string text;

		private DnsStatus dnsStatus;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsTxtRecord
		{
			public uint stringCount;

			public IntPtr arypStringArray;
		}
	}
}
