using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public class ExDiagnostics
	{
		public static FailFastMode FailFastMode
		{
			get
			{
				return ExDiagnostics.failFastMode;
			}
			set
			{
				ExDiagnostics.failFastMode = value;
			}
		}

		public static ExDiagnostics.FailFastEvent OnFailFast
		{
			get
			{
				return ExDiagnostics.failFastEventHandlers;
			}
			set
			{
				ExDiagnostics.failFastEventHandlers = value;
			}
		}

		public static void FailFast(string message, bool alwaysTerminate)
		{
			bool flag = ExDiagnostics.FailFastMode != FailFastMode.Exception && ExDiagnostics.FailFastMode != FailFastMode.Test;
			flag = (flag || alwaysTerminate);
			try
			{
				ExDiagnostics.failFastEventHandlers(flag);
			}
			catch
			{
			}
			if (ExDiagnostics.FailFastMode == FailFastMode.Test && !flag)
			{
				throw new TestFailFastException(message);
			}
			FailFastException ex = new FailFastException(message, new StackTrace(1, true).ToString());
			ExWatson.SendReport(ex);
			if (flag)
			{
				Environment.Exit(1);
				return;
			}
			throw ex;
		}

		private static ExDiagnostics.FailFastEvent failFastEventHandlers;

		private static FailFastMode failFastMode = FailFastMode.Terminate;

		public delegate void FailFastEvent(bool terminating);
	}
}
