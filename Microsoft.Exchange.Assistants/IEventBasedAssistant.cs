using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal interface IEventBasedAssistant : IAssistantBase
	{
		void OnStart(EventBasedStartInfo startInfo);

		bool IsEventInteresting(MapiEvent mapiEvent);

		void HandleEvent(MapiEvent mapiEvent, MailboxSession itemStore, StoreObject item);
	}
}
