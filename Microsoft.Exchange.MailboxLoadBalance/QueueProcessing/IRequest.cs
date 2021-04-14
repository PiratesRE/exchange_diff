using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRequest
	{
		bool IsBlocked { get; }

		IRequestQueue Queue { get; }

		IEnumerable<ResourceKey> Resources { get; }

		void Abort();

		void AssignQueue(IRequestQueue queue);

		RequestDiagnosticData GetDiagnosticData(bool verbose);

		void Process();

		bool ShouldCancel(TimeSpan queueTimeout);

		bool WaitExecution();

		bool WaitExecution(TimeSpan timeout);

		bool WaitExecutionAndThrowOnFailure(TimeSpan timeout);
	}
}
