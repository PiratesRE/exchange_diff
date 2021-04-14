using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRequestQueueManager
	{
		IRequestQueue MainProcessingQueue { get; }

		IRequestQueue GetInjectionQueue(DirectoryDatabase database);

		IRequestQueue GetProcessingQueue(LoadEntity loadObject);

		IRequestQueue GetProcessingQueue(DirectoryObject directoryObject);

		QueueManagerDiagnosticData GetDiagnosticData(bool includeRequestDetails, bool includeRequestVerboseData);

		void Clean();
	}
}
