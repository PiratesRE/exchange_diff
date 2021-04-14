using System;
using Microsoft.Exchange.Assistants.EventLog;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class ServerGovernor : ThrottleGovernor
	{
		public ServerGovernor(string name, Throttle throttle) : base(null, throttle)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "ServerGovernor for " + this.name;
		}

		protected override bool IsFailureRelevant(AITransientException exception)
		{
			return exception is TransientServerException;
		}

		protected override void Log30MinuteWarning(AITransientException exception, TimeSpan nextRetryInterval)
		{
			base.LogEvent(AssistantsEventLogConstants.Tuple_ServerGovernorFailure, null, new object[]
			{
				this,
				base.LastRunTime.ToLocalTime(),
				nextRetryInterval,
				exception
			});
		}

		private string name;
	}
}
