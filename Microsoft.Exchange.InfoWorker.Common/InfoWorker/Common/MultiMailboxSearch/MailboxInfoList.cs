using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class MailboxInfoList : List<MailboxInfo>, IMailboxInfoList, IList<MailboxInfo>, ICollection<MailboxInfo>, IEnumerable<MailboxInfo>, IEnumerable
	{
		public MailboxInfoList(MailboxInfo[] mailboxes) : base(mailboxes)
		{
		}

		public MailboxInfoList()
		{
		}

		public MailboxInfoList(int capacity) : base(capacity)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = base.Count - 1;
			for (int i = 0; i < base.Count; i++)
			{
				stringBuilder.Append(base[i].DisplayName);
				stringBuilder.Append(base[i].IsPrimary ? "(Primary)" : "(Archive)");
				if (i != num)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
