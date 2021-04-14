using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public interface IMailboxUrls
	{
		string InboxUrl { get; }

		string CalendarUrl { get; }

		string PeopleUrl { get; }

		string PhotoUrl { get; }

		string OwaUrl { get; }
	}
}
