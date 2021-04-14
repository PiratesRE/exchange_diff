using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsMxRecord : DnsRecord
	{
		internal DnsMxRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			DnsMxRecord.Win32DnsMxRecord win32DnsMxRecord = (DnsMxRecord.Win32DnsMxRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsMxRecord.Win32DnsMxRecord));
			this.preference = (int)win32DnsMxRecord.preference;
			this.nameExchange = win32DnsMxRecord.nameExchange;
		}

		public static IComparer<DnsMxRecord> Comparer
		{
			get
			{
				if (DnsMxRecord.comparer == null)
				{
					DnsMxRecord.comparer = new DnsMxRecord.MxComparer();
				}
				return DnsMxRecord.comparer;
			}
		}

		public string NameExchange
		{
			get
			{
				return this.nameExchange;
			}
		}

		public int Preference
		{
			get
			{
				return this.preference;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				" ",
				this.preference,
				" ",
				this.nameExchange
			});
		}

		private static IComparer<DnsMxRecord> comparer;

		private int preference;

		private string nameExchange;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsMxRecord
		{
			public string nameExchange;

			public ushort preference;

			public ushort pad;
		}

		private class MxComparer : IComparer<DnsMxRecord>
		{
			public int Compare(DnsMxRecord a, DnsMxRecord b)
			{
				return a.Preference.CompareTo(b.Preference);
			}
		}
	}
}
