using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class Answer
	{
		public string Domain { get; private set; }

		public RecordType RecordType { get; private set; }

		public RecordClass RecordClass { get; private set; }

		public int TTL { get; private set; }

		public ARecord Arecord { get; private set; }

		public AaaaRecord AaaaRecord { get; private set; }

		public NSRecord NSrecord { get; private set; }

		public SOARecord SOArecord { get; private set; }

		public int ProcessMessage(byte[] message, int position)
		{
			int num = position;
			this.Domain = DnsHelper.ReadDomain(message, ref num);
			this.RecordType = (RecordType)DnsHelper.GetUShort(message, num);
			num += 2;
			this.RecordClass = (RecordClass)DnsHelper.GetUShort(message, num);
			num += 2;
			this.TTL = DnsHelper.GetInt(message, num);
			num += 4;
			DnsHelper.GetUShort(message, num);
			num += 2;
			RecordType recordType = this.RecordType;
			switch (recordType)
			{
			case RecordType.A:
				this.Arecord = new ARecord();
				num = this.Arecord.ProcessResponse(message, num);
				break;
			case RecordType.NS:
				this.NSrecord = new NSRecord();
				num = this.NSrecord.ProcessResponse(message, num);
				break;
			default:
				if (recordType != RecordType.SOA)
				{
					if (recordType != RecordType.AAAA)
					{
						throw new FormatException(string.Format("Invalid record type for answer, value={0}", this.RecordType));
					}
					this.AaaaRecord = new AaaaRecord();
					num = this.AaaaRecord.ProcessResponse(message, num);
				}
				else
				{
					this.SOArecord = new SOARecord();
					num = this.SOArecord.ProcessResponse(message, num);
				}
				break;
			}
			return num;
		}

		public override string ToString()
		{
			string text = string.Format("Domain={0}, RecordType={1}, RecordClass={2}, TTL={3} ", new object[]
			{
				this.Domain,
				this.RecordType,
				this.RecordClass,
				this.TTL
			});
			RecordType recordType = this.RecordType;
			switch (recordType)
			{
			case RecordType.A:
				text += this.Arecord.ToString();
				break;
			case RecordType.NS:
				text += this.NSrecord.ToString();
				break;
			default:
				if (recordType != RecordType.SOA)
				{
					if (recordType == RecordType.AAAA)
					{
						text += this.AaaaRecord.ToString();
					}
				}
				else
				{
					text += this.SOArecord.ToString();
				}
				break;
			}
			return text;
		}
	}
}
