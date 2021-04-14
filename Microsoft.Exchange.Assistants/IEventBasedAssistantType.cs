using System;
using Microsoft.Exchange.Data;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal interface IEventBasedAssistantType : IAssistantType
	{
		MapiEventTypeFlags EventMask { get; }

		bool NeedsMailboxSession { get; }

		PropertyDefinition[] PreloadItemProperties { get; }

		bool ProcessesPublicDatabases { get; }

		Guid Identity { get; }

		IEventBasedAssistant CreateInstance(DatabaseInfo databaseInfo);
	}
}
