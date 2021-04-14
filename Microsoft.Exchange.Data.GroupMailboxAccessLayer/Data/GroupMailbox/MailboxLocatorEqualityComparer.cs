using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	internal static class MailboxLocatorEqualityComparer
	{
		public static readonly IEqualityComparer<MailboxLocator> ByLegacyDn = new MailboxLocatorEqualityComparer.LegacyDnComparer();

		public static readonly IEqualityComparer<MailboxLocator> ByExternalIdAndLegacyDn = new MailboxLocatorEqualityComparer.ExternalIdAndLegacyDnComparer();

		private sealed class LegacyDnComparer : IEqualityComparer<MailboxLocator>
		{
			public bool Equals(MailboxLocator a, MailboxLocator b)
			{
				return StringComparer.OrdinalIgnoreCase.Equals(a.LegacyDn, b.LegacyDn);
			}

			public int GetHashCode(MailboxLocator mailboxLocator)
			{
				return StringComparer.OrdinalIgnoreCase.GetHashCode(mailboxLocator.LegacyDn);
			}
		}

		private sealed class ExternalIdAndLegacyDnComparer : IEqualityComparer<MailboxLocator>
		{
			public bool Equals(MailboxLocator a, MailboxLocator b)
			{
				if (a.ExternalId != null && b.ExternalId != null)
				{
					return StringComparer.OrdinalIgnoreCase.Equals(a.ExternalId, a.ExternalId);
				}
				return StringComparer.OrdinalIgnoreCase.Equals(a.LegacyDn, b.LegacyDn);
			}

			public int GetHashCode(MailboxLocator mailboxLocator)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
