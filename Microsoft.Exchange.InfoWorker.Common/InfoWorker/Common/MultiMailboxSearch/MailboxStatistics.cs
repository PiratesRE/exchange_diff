using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MailboxStatistics : IComparable<MailboxStatistics>, IComparable
	{
		public MailboxStatistics(MailboxInfo mailboxInfo, ulong count, ByteQuantifiedSize size)
		{
			this.mailboxInfo = mailboxInfo;
			this.count = count;
			this.size = size;
		}

		public MailboxInfo MailboxInfo
		{
			get
			{
				return this.mailboxInfo;
			}
		}

		public ulong Count
		{
			get
			{
				return this.count;
			}
		}

		public ByteQuantifiedSize Size
		{
			get
			{
				return this.size;
			}
		}

		public void Merge(MailboxStatistics other)
		{
			if (other == null || !this.mailboxInfo.LegacyExchangeDN.Equals(other.MailboxInfo.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			this.count += other.Count;
			this.size += other.Size;
		}

		public override string ToString()
		{
			return string.Format("[User:{0}, IsPrimary: {1}, Count: {2}, Size: {3} bytes]", new object[]
			{
				this.mailboxInfo.LegacyExchangeDN,
				this.mailboxInfo.IsPrimary,
				this.Count,
				this.Size.ToBytes()
			});
		}

		public override bool Equals(object obj)
		{
			MailboxStatistics mailboxStatistics = obj as MailboxStatistics;
			return mailboxStatistics != null && 0 == this.CompareTo(mailboxStatistics);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public int CompareTo(object obj)
		{
			MailboxStatistics mailboxStatistics = obj as MailboxStatistics;
			if (mailboxStatistics == null)
			{
				throw new ArgumentException("Object is not a MailboxStatistics");
			}
			return this.CompareTo(mailboxStatistics);
		}

		public int CompareTo(MailboxStatistics other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = this.Count.CompareTo(other.Count);
			if (num == 0)
			{
				num = this.Size.CompareTo(other.Size);
				if (num == 0)
				{
					num = string.Compare(this.MailboxInfo.LegacyExchangeDN, other.MailboxInfo.LegacyExchangeDN, CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase);
					if (num == 0)
					{
						num = this.MailboxInfo.IsPrimary.CompareTo(other.MailboxInfo.IsPrimary);
					}
				}
			}
			return num;
		}

		private readonly MailboxInfo mailboxInfo;

		private ulong count;

		private ByteQuantifiedSize size;
	}
}
