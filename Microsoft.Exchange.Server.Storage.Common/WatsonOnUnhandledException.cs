using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class WatsonOnUnhandledException
	{
		internal static bool DisableExceptionFilter
		{
			get
			{
				return WatsonOnUnhandledException.disableExceptionFilter && !WatsonOnUnhandledException.isUnderWatsonSuiteTests;
			}
			set
			{
				WatsonOnUnhandledException.disableExceptionFilter = value;
			}
		}

		internal static bool DisableGenerateDumpFile
		{
			get
			{
				return WatsonOnUnhandledException.disableGenerateDumpFile;
			}
			set
			{
				WatsonOnUnhandledException.disableGenerateDumpFile = value;
			}
		}

		public static bool IsUnderWatsonSuiteTests
		{
			get
			{
				return WatsonOnUnhandledException.isUnderWatsonSuiteTests;
			}
		}

		public static bool ProcessKilled
		{
			get
			{
				return WatsonOnUnhandledException.processKilled;
			}
		}

		public static void Initialize()
		{
			MethodInfo method = typeof(WatsonOnUnhandledException).GetMethod("ExceptionFilter", BindingFlags.Static | BindingFlags.NonPublic);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
			method = typeof(WatsonOnUnhandledException).GetMethod("FailFastOnOutOfMemoryException", BindingFlags.Static | BindingFlags.NonPublic);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
		}

		internal static void ResetForTest(bool testModeEnabled, TimeSpan singleWatsonReportSemaphoreTimeout)
		{
			WatsonOnUnhandledException.isUnderWatsonSuiteTests = testModeEnabled;
			WatsonOnUnhandledException.processKilled = false;
			WatsonOnUnhandledException.singleWatsonReportSemaphore = new Semaphore(1, 1);
			WatsonOnUnhandledException.singleWatsonReportSemaphoreTimeout = singleWatsonReportSemaphoreTimeout;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void FailFastOnOutOfMemoryException()
		{
			Environment.FailFast("OutOfMemoryException");
		}

		private static bool IsFlowControlException(object exGeneric)
		{
			return exGeneric is FailRpcException || exGeneric is RpcException;
		}

		private static void InternalExit()
		{
			Environment.Exit(-559034355);
		}

		private static bool UnhandledExceptionEventHandler(IExecutionDiagnostics executionDiagnostics, object exception)
		{
			if (WatsonOnUnhandledException.DisableExceptionFilter)
			{
				return false;
			}
			if (!WatsonOnUnhandledException.IsExceptionInteresting(exception))
			{
				return false;
			}
			if (executionDiagnostics != null)
			{
				ExWatson.RegisterReportAction(new WatsonOnUnhandledException.DiagnosticWatsonReportAction(executionDiagnostics), WatsonActionScope.Thread);
			}
			Exception ex = exception as Exception;
			Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_UnhandledException, new object[]
			{
				ex
			});
			try
			{
				if (executionDiagnostics != null)
				{
					executionDiagnostics.OnUnhandledException(ex);
				}
			}
			catch (Exception exception2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
			}
			if (WatsonOnUnhandledException.DisableGenerateDumpFile)
			{
				return false;
			}
			ErrorHelper.CheckForDebugger();
			try
			{
				if (!WatsonOnUnhandledException.singleWatsonReportSemaphore.WaitOne(WatsonOnUnhandledException.singleWatsonReportSemaphoreTimeout) && WatsonOnUnhandledException.IsUnderWatsonSuiteTests)
				{
					return false;
				}
				if (ExWatson.KillProcessAfterWatson || WatsonOnUnhandledException.IsUnderWatsonSuiteTests)
				{
					ReportOptions options = ReportOptions.ReportTerminateAfterSend | ReportOptions.DoNotFreezeThreads;
					ExWatson.SendReport(ex, options, null);
				}
				else
				{
					ExWatson.SendReportAndCrashOnAnotherThread(ex);
				}
			}
			finally
			{
				WatsonOnUnhandledException.KillCurrentProcess();
			}
			return false;
		}

		private static bool ExceptionFilter(object exception, IExecutionDiagnostics executionDiagnostics)
		{
			if (exception is OutOfMemoryException)
			{
				WatsonOnUnhandledException.FailFastOnOutOfMemoryException();
			}
			return WatsonOnUnhandledException.UnhandledExceptionEventHandler(executionDiagnostics, exception);
		}

		public static void Guard(IExecutionDiagnostics executionDiagnostics, TryDelegate body)
		{
			ILUtil.DoTryFilterCatch<IExecutionDiagnostics>(body, WatsonOnUnhandledException.filterDelegate, WatsonOnUnhandledException.catchDelegate, executionDiagnostics);
		}

		private static bool IsExceptionInteresting(object exGeneric)
		{
			return !(exGeneric is Exception) || !WatsonOnUnhandledException.IsFlowControlException(exGeneric);
		}

		internal static void KillCurrentProcess()
		{
			WatsonOnUnhandledException.processKilled = true;
			if (WatsonOnUnhandledException.IsUnderWatsonSuiteTests)
			{
				return;
			}
			try
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					currentProcess.Kill();
				}
			}
			catch (Win32Exception exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
			}
			WatsonOnUnhandledException.InternalExit();
		}

		private static bool IsGrayException(Exception exception)
		{
			return GrayException.IsGrayException(exception);
		}

		private static bool disableExceptionFilter = false;

		private static bool disableGenerateDumpFile = false;

		private static bool isUnderWatsonSuiteTests;

		private static bool processKilled;

		private static Semaphore singleWatsonReportSemaphore = new Semaphore(1, 1);

		private static TimeSpan singleWatsonReportSemaphoreTimeout = TimeSpan.FromMilliseconds(-1.0);

		private static GenericFilterDelegate<IExecutionDiagnostics> filterDelegate = new GenericFilterDelegate<IExecutionDiagnostics>(null, (UIntPtr)ldftn(ExceptionFilter));

		private static GenericCatchDelegate<IExecutionDiagnostics> catchDelegate = new GenericCatchDelegate<IExecutionDiagnostics>(null, (UIntPtr)ldftn(<.cctor>b__0));

		private class DiagnosticWatsonReportAction : WatsonReportAction
		{
			internal DiagnosticWatsonReportAction(IExecutionDiagnostics executionDiagnostics) : base(null, true)
			{
				this.executionDiagnostics = executionDiagnostics;
			}

			public override string ActionName
			{
				get
				{
					return "ExecutionDiagnostics";
				}
			}

			public override string Evaluate(WatsonReport watsonReport)
			{
				string result;
				try
				{
					result = (this.executionDiagnostics.DiagnosticInformationForWatsonReport ?? string.Empty);
				}
				catch (Exception ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					result = ex.ToString();
				}
				return result;
			}

			private readonly IExecutionDiagnostics executionDiagnostics;
		}
	}
}
