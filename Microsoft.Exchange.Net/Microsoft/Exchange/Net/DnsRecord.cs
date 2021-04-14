using System;

namespace Microsoft.Exchange.Net
{
	internal abstract class DnsRecord
	{
		protected DnsRecord(Win32DnsRecordHeader header)
		{
			this.name = header.name;
			this.timeToLive = Math.Max(1U, header.ttl);
			this.section = header.Section;
			this.recordType = header.recordType;
		}

		protected DnsRecord(string value)
		{
			this.name = value;
			this.timeToLive = 1U;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			protected set
			{
				this.name = value;
			}
		}

		public TimeSpan TimeToLive
		{
			get
			{
				return TimeSpan.FromSeconds(this.timeToLive);
			}
			set
			{
				this.timeToLive = (uint)value.TotalSeconds;
			}
		}

		public DnsResponseSection Section
		{
			get
			{
				return this.section;
			}
			set
			{
				this.section = value;
			}
		}

		public DnsRecordType RecordType
		{
			get
			{
				return this.recordType;
			}
			set
			{
				this.recordType = value;
			}
		}

		public override string ToString()
		{
			if (this.name == null)
			{
				return "(null) " + this.recordType.ToString();
			}
			return this.name + " " + this.recordType.ToString();
		}

		private string name;

		private uint timeToLive;

		private DnsResponseSection section;

		private DnsRecordType recordType;
	}
}
