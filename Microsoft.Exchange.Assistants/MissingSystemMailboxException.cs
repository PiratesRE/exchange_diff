using System;
using Microsoft.Exchange.Assistants.EventLog;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MissingSystemMailboxException : MissingObjectException
	{
		public MissingSystemMailboxException(string databaseName, EventLogger logger) : this(databaseName, null, logger)
		{
		}

		public MissingSystemMailboxException(string databaseName, Exception innerException, EventLogger logger) : base(Strings.descMissingSystemMailbox(databaseName), innerException)
		{
			logger.LogEvent(AssistantsEventLogConstants.Tuple_MissingSystemMailbox, databaseName, new object[]
			{
				databaseName,
				(innerException != null) ? innerException.ToString() : string.Empty
			});
		}
	}
}
