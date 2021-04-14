using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IMailboxLocationCollection
	{
		IMailboxLocationInfo GetMailboxLocation(MailboxLocationType mailboxLocationType);

		IList<IMailboxLocationInfo> GetMailboxLocations(MailboxLocationType mailboxLocationType);

		IList<IMailboxLocationInfo> GetMailboxLocations();

		IMailboxLocationInfo GetMailboxLocation(Guid mailboxGuid);

		void AddMailboxLocation(IMailboxLocationInfo mailboxLocation);

		void AddMailboxLocation(Guid mailboxGuid, ADObjectId databaseLocation, MailboxLocationType mailboxLocationType);

		void RemoveMailboxLocation(Guid mailboxGuid);

		void Validate(List<ValidationError> errors);
	}
}
