using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IContactsUploaderPerformanceTracker
	{
		int ReceivedContactsCount { get; set; }

		double ExportedDataSize { get; set; }

		void Start();

		void Stop();

		void IncrementContactsRead();

		void IncrementContactsExported();

		string OperationResult { get; set; }

		void AddTimeBookmark(ContactsUploaderPerformanceTrackerBookmarks activity);
	}
}
