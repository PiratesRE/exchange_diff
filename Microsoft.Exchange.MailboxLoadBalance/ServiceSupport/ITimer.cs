using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ITimer
	{
		void SetAction(Action newAction, bool startExecution);

		void WaitExecution();

		void WaitExecution(TimeSpan timeout);
	}
}
