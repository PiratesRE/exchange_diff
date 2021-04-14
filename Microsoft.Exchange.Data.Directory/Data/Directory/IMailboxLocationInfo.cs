using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IMailboxLocationInfo
	{
		Guid MailboxGuid { get; }

		ADObjectId DatabaseLocation { get; }

		MailboxLocationType MailboxLocationType { get; }
	}
}
