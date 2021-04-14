using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface INotificationSource
	{
		void Unadvise(object notificationHandle);

		bool IsDisposedOrDead { get; }
	}
}
