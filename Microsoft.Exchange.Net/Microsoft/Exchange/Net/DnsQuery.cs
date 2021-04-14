using System;

namespace Microsoft.Exchange.Net
{
	internal class DnsQuery
	{
		internal DnsQuery(DnsRecordType type, string question)
		{
			this.type = type;
			this.question = question.ToLowerInvariant();
		}

		internal DnsRecordType Type
		{
			get
			{
				return this.type;
			}
		}

		internal string Question
		{
			get
			{
				return this.question;
			}
		}

		public override int GetHashCode()
		{
			return this.type.GetHashCode() ^ this.question.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DnsQuery dnsQuery = obj as DnsQuery;
			return dnsQuery != null && this.type == dnsQuery.type && this.question == dnsQuery.question;
		}

		public override string ToString()
		{
			return this.type + " " + this.question;
		}

		private DnsRecordType type;

		private string question;
	}
}
