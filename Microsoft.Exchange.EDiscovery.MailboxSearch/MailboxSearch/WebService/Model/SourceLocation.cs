using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	[Flags]
	public enum SourceLocation
	{
		PrimaryOnly = 1,
		ArchiveOnly = 2,
		All = 3
	}
}
