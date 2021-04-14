using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IEventHandler
	{
		void Consume(Event newEvent);

		void HandleException(Exception exception);
	}
}
