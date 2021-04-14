using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LookupMessagesParams
	{
		public LookupMessagesParams(List<uint> fetchUidList)
		{
			this.fetchUidList = fetchUidList;
		}

		public FetchMessagesFlags FetchMessagesFlags
		{
			get
			{
				return FetchMessagesFlags.FetchByUid | FetchMessagesFlags.IncludeExtendedData;
			}
		}

		public string GetUidFetchString()
		{
			if (this.fetchUidList.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (uint num2 in this.fetchUidList)
			{
				stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
				num++;
				if (num < this.fetchUidList.Count)
				{
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}

		private const FetchMessagesFlags EnumerateFlags = FetchMessagesFlags.FetchByUid | FetchMessagesFlags.IncludeExtendedData;

		private const string FetchUidStringDelimiter = ",";

		private readonly List<uint> fetchUidList;
	}
}
