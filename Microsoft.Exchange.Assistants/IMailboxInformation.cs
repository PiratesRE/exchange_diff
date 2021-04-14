using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Assistants
{
	internal interface IMailboxInformation
	{
		object GetMailboxProperty(PropertyTagPropertyDefinition property);

		Guid MailboxGuid { get; }
	}
}
