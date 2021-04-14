using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IContactLinkingPerformanceTracker : IPerformanceTracker
	{
		void IncrementContactsCreated();

		void IncrementContactsUpdated();

		void IncrementContactsRead();

		void IncrementContactsProcessed();

		ILogEvent GetLogEvent();
	}
}
