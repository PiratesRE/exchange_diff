using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal interface IEventSkipNotification
	{
		void OnSkipEvent(MapiEvent mapiEvent, Exception exception);
	}
}
