using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class CommonFailureLog : FailureObjectLog
	{
		private CommonFailureLog() : base(new SimpleObjectLogConfiguration("CommonFailure", "CommonFailureLogEnabled", "CommonFailureLogMaxDirSize", "CommonFailureLogMaxFileSize"))
		{
		}

		public static void LogCommonFailureEvent(IFailureObjectLoggable failureEvent, Exception failureException)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
				CommonFailureLog.instance.LogFailureEvent(failureEvent, failureException);
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
		}

		public static void LogCommonFailureEvent(string objectType, Exception failureException, Guid objectGuid = default(Guid), int failureFlags = 0, string failureContext = null)
		{
			FailureEvent failureEvent = new FailureEvent(objectGuid, objectType, failureFlags, failureContext);
			CommonFailureLog.LogCommonFailureEvent(failureEvent, failureException);
		}

		public override string ComputeFailureHash(Exception failureException)
		{
			return CommonUtils.ComputeCallStackHash(failureException, 5);
		}

		public override string ExtractExceptionString(Exception failureException)
		{
			return CommonUtils.FullFailureMessageWithCallStack(failureException, 5);
		}

		private static CommonFailureLog instance = new CommonFailureLog();
	}
}
