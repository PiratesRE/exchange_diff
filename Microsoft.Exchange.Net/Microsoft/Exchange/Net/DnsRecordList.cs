using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Net
{
	internal class DnsRecordList : List<DnsRecord>
	{
		public IEnumerable<DnsRecord> AnswerRecords
		{
			get
			{
				foreach (DnsRecord record in this)
				{
					if (record.Section == DnsResponseSection.Answer)
					{
						yield return record;
					}
				}
				yield break;
			}
		}

		public IEnumerable<DnsRecord> AdditionalRecords
		{
			get
			{
				foreach (DnsRecord record in this)
				{
					if (record.Section == DnsResponseSection.Additional)
					{
						yield return record;
					}
				}
				yield break;
			}
		}

		public static IEnumerable<DnsRecord> EnumerateAddresses(IEnumerable<DnsRecord> records)
		{
			foreach (DnsRecord record in records)
			{
				if (record.RecordType == DnsRecordType.A || record.RecordType == DnsRecordType.AAAA)
				{
					yield return record;
				}
			}
			yield break;
		}

		public List<T> ExtractRecords<T>(DnsRecordType type, IComparer<T> comparer) where T : class
		{
			List<T> list = new List<T>();
			foreach (DnsRecord dnsRecord in this)
			{
				if (dnsRecord.RecordType == type)
				{
					T item = dnsRecord as T;
					int num = list.BinarySearch(item, comparer);
					if (num < 0)
					{
						list.Insert(~num, item);
					}
				}
			}
			return list;
		}

		public IEnumerable<DnsRecord> EnumerateAnswers(DnsRecordType type)
		{
			foreach (DnsRecord record in this)
			{
				if (record.Section == DnsResponseSection.Answer && record.RecordType == type)
				{
					yield return record;
				}
			}
			yield break;
		}

		public IEnumerable<DnsRecord> Enumerate(DnsRecordType type)
		{
			foreach (DnsRecord record in this)
			{
				if (record.RecordType == type)
				{
					yield return record;
				}
			}
			yield break;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DnsRecord value in this)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static DnsRecordList Empty = new DnsRecordList();
	}
}
