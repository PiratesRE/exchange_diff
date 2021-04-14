using System;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class MessagingPoliciesUtils
	{
		public static MessagingPoliciesUtils.JournalVersion CheckJournalReportVersion(HeaderList headerList)
		{
			if (headerList == null)
			{
				return MessagingPoliciesUtils.JournalVersion.None;
			}
			Header header = headerList.FindFirst("X-MS-Journal-Report");
			if (header != null)
			{
				return MessagingPoliciesUtils.JournalVersion.Exchange2007;
			}
			header = headerList.FindFirst("Content-Identifier");
			if (header != null && !string.IsNullOrEmpty(header.Value) && string.Compare(header.Value, "exjournalreport", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return MessagingPoliciesUtils.JournalVersion.Exchange2003;
			}
			header = headerList.FindFirst("Content-Identifer");
			if (header != null && !string.IsNullOrEmpty(header.Value) && string.Compare(header.Value, "exjournalreport", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return MessagingPoliciesUtils.JournalVersion.Exchange2003;
			}
			return MessagingPoliciesUtils.JournalVersion.None;
		}

		public const string E12EnvelopeJournal = "X-MS-Journal-Report";

		public const string ContentIdentifier = "Content-Identifier";

		public const string ContentIdentifierTypo = "Content-Identifer";

		public const string ExchangeJournalReport = "exjournalreport";

		public enum JournalVersion
		{
			None,
			Exchange2003,
			Exchange2007
		}
	}
}
