using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal class DnsSrvRecord : DnsRecord
	{
		internal DnsSrvRecord(string name) : base(name)
		{
		}

		internal DnsSrvRecord(Win32DnsRecordHeader header, IntPtr dataPointer) : base(header)
		{
			DnsSrvRecord.Win32DnsSrvRecord win32DnsSrvRecord = (DnsSrvRecord.Win32DnsSrvRecord)Marshal.PtrToStructure(dataPointer, typeof(DnsSrvRecord.Win32DnsSrvRecord));
			this.target = win32DnsSrvRecord.nameTarget;
			this.priority = (int)win32DnsSrvRecord.priority;
			this.weight = (int)win32DnsSrvRecord.weight;
			this.port = (int)win32DnsSrvRecord.port;
		}

		internal DnsSrvRecord(string name, string target) : base(name)
		{
			this.target = target;
			base.RecordType = DnsRecordType.SRV;
		}

		public static IComparer<DnsSrvRecord> Comparer
		{
			get
			{
				if (DnsSrvRecord.comparer == null)
				{
					DnsSrvRecord.comparer = new DnsSrvRecord.SrvComparer();
				}
				return DnsSrvRecord.comparer;
			}
		}

		public string NameTarget
		{
			get
			{
				return this.target;
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		public int Weight
		{
			get
			{
				return this.weight;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public override string ToString()
		{
			return base.ToString() + " " + this.target;
		}

		private static IComparer<DnsSrvRecord> comparer;

		private string target;

		private int priority;

		private int weight;

		private int port;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Win32DnsSrvRecord
		{
			public string nameTarget;

			public ushort priority;

			public ushort weight;

			public ushort port;

			public ushort pad;
		}

		private class SrvComparer : IComparer<DnsSrvRecord>
		{
			public int Compare(DnsSrvRecord a, DnsSrvRecord b)
			{
				if (a.priority == b.priority)
				{
					return a.weight.CompareTo(b.weight);
				}
				return a.priority.CompareTo(b.priority);
			}
		}
	}
}
