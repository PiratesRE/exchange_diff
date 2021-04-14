using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Setup.Common
{
	internal class LoggableMonadCommand : MonadCommand
	{
		public LoggableMonadCommand() : this(null, null)
		{
		}

		public LoggableMonadCommand(string cmdText) : this(cmdText, null)
		{
		}

		public LoggableMonadCommand(string cmdText, MonadConnection connection) : base(cmdText, connection)
		{
			this.RegisterListeners();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.StartExecution -= CommandLoggingSession.StartExecution;
				base.EndExecution -= CommandLoggingSession.EndExecution;
				base.ErrorReport -= CommandLoggingSession.ErrorReport;
				base.WarningReport -= CommandLoggingSession.WarningReport;
			}
			base.Dispose(disposing);
		}

		private void RegisterListeners()
		{
			base.StartExecution += CommandLoggingSession.StartExecution;
			base.EndExecution += CommandLoggingSession.EndExecution;
			base.ErrorReport += CommandLoggingSession.ErrorReport;
			base.WarningReport += CommandLoggingSession.WarningReport;
		}
	}
}
