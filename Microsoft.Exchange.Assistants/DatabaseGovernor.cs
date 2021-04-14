using System;
using Microsoft.Exchange.Assistants.EventLog;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class DatabaseGovernor : ThrottleGovernor
	{
		public DatabaseGovernor(string name, Governor parentGovernor, Throttle throttle) : base(parentGovernor, throttle)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "DatabaseGovernor for " + this.name;
		}

		protected override bool IsFailureRelevant(AITransientException exception)
		{
			return exception is TransientDatabaseException;
		}

		protected override void Log30MinuteWarning(AITransientException exception, TimeSpan nextRetryInterval)
		{
			base.LogEvent(AssistantsEventLogConstants.Tuple_DatabaseGovernorFailure, null, new object[]
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
