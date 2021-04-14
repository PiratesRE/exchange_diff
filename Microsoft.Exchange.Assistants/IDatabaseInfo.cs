using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Assistants
{
	internal interface IDatabaseInfo
	{
		IEnumerable<IMailboxInformation> GetMailboxTable(ClientType clientType, PropertyTagPropertyDefinition[] properties);

		string DatabaseName { get; }
	}
}
