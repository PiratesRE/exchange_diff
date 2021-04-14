using System;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MailboxGovernor : ThrottleGovernor
	{
		public MailboxGovernor(Governor parentGovernor, Throttle throttle) : base(parentGovernor, throttle)
		{
		}

		protected override bool IsFailureRelevant(AITransientException exception)
		{
			return exception is TransientMailboxException;
		}
	}
}
