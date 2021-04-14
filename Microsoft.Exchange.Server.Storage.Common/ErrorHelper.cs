using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class ErrorHelper
	{
		private static Queue<Breadcrumb> Breadcrumbs
		{
			get
			{
				if (ErrorHelper.breadcrumbs == null)
				{
					ErrorHelper.breadcrumbs = new Queue<Breadcrumb>(ConfigurationSchema.MaxBreadcrumbs.Value);
				}
				return ErrorHelper.breadcrumbs;
			}
		}

		[DllImport("kernel32.dll")]
		public static extern void OutputDebugString(string str);

		[DllImport("kernel32.dll")]
		private static extern bool DebugBreak();

		internal static void Initialize(Guid? databaseGuid)
		{
			if (databaseGuid != null)
			{
				ErrorHelper.InitializeCrashOnLID(databaseGuid.Value);
			}
		}

		internal static void InitializeCrashOnLID(Guid databaseGuid)
		{
			string crashOnLIDKeyName = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}", Environment.MachineName, databaseGuid);
			int crashOnLIDValue = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, crashOnLIDKeyName, "CrashOnLID", 0);
			if (crashOnLIDValue != 0)
			{
				DiagnosticContext.SetOnLIDCallback(delegate(LID lid)
				{
					if (lid.Value == (uint)crashOnLIDValue)
					{
						RegistryWriter.Instance.DeleteValue(Registry.LocalMachine, crashOnLIDKeyName, "CrashOnLID");
						throw new CrashOnLIDException(lid);
					}
				});
			}
		}

		private static ErrorHelper.AttachedDebuggerType GetAttachedDebuggerType()
		{
			ErrorHelper.AttachedDebuggerType result = ErrorHelper.AttachedDebuggerType.None;
			if (Debugger.IsAttached)
			{
				result = ErrorHelper.AttachedDebuggerType.Managed;
			}
			else if (ErrorHelper.IsDebuggerPresent())
			{
				result = ErrorHelper.AttachedDebuggerType.Native;
			}
			return result;
		}

		[DllImport("kernel32.dll")]
		private static extern bool IsDebuggerPresent();

		public static void ClearBreadcrumbsForTest()
		{
			using (LockManager.Lock(ErrorHelper.Breadcrumbs, LockManager.LockType.Breadcrumbs))
			{
				ErrorHelper.Breadcrumbs.Clear();
			}
		}

		public static IReadOnlyList<Breadcrumb> GetBreadcrumbsHistorySnapshot()
		{
			IReadOnlyList<Breadcrumb> result;
			using (LockManager.Lock(ErrorHelper.Breadcrumbs, LockManager.LockType.Breadcrumbs))
			{
				result = new List<Breadcrumb>(ErrorHelper.Breadcrumbs);
			}
			return result;
		}

		public static void AddBreadcrumb(BreadcrumbKind kind, byte operationSource, byte operationNumber, byte clientType, int databaseNumber, int mailboxNumber, int associatedValue, object associatedObject)
		{
			IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.BreadCrumbs);
			if (associatedObject == null)
			{
				int size = DiagnosticContext.Size;
				byte[] array = new byte[6 + size];
				int num = 6;
				DiagnosticContext.PackInfo(array, ref num, size);
				ParseSerialize.SerializeInt32(size, array, 2);
				associatedObject = array;
			}
			using (LockManager.Lock(ErrorHelper.Breadcrumbs, LockManager.LockType.Breadcrumbs))
			{
				Breadcrumb breadcrumb;
				if (ErrorHelper.Breadcrumbs.Count == ConfigurationSchema.MaxBreadcrumbs.Value)
				{
					breadcrumb = ErrorHelper.Breadcrumbs.Dequeue();
				}
				else
				{
					breadcrumb = new Breadcrumb();
				}
				breadcrumb.Kind = kind;
				breadcrumb.Source = operationSource;
				breadcrumb.Operation = operationNumber;
				breadcrumb.Client = clientType;
				breadcrumb.Mailbox = mailboxNumber;
				breadcrumb.Database = databaseNumber;
				breadcrumb.Time = DateTime.UtcNow;
				breadcrumb.DataValue = associatedValue;
				breadcrumb.DataObject = associatedObject;
				ErrorHelper.Breadcrumbs.Enqueue(breadcrumb);
			}
			if (logger != null && logger.IsLoggingEnabled)
			{
				string strValue = string.Empty;
				string empty = string.Empty;
				string empty2 = string.Empty;
				Exception ex = associatedObject as Exception;
				if (ex == null)
				{
					Tuple<Exception, object> tuple = associatedObject as Tuple<Exception, object>;
					if (tuple != null)
					{
						ex = tuple.Item1;
					}
				}
				if (associatedObject is byte[])
				{
					strValue = "0x" + BitConverter.ToString((byte[])associatedObject).Replace("-", string.Empty);
				}
				else if (ex != null)
				{
					ErrorHelper.GetExceptionSummary(ex, out strValue, out empty, out empty2);
				}
				else if (associatedObject != null)
				{
					strValue = associatedObject.ToString();
				}
				using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.BreadCrumbs, true, false, (byte)kind, operationSource, operationNumber, clientType, databaseNumber, mailboxNumber, associatedValue, strValue, empty, empty2))
				{
					logger.TryWrite(traceBuffer);
				}
			}
		}

		public static bool ShouldSkipBreadcrumb(byte operationSource, byte operationNumber, ErrorCodeValue error, uint lid)
		{
			if (lid <= 46439U)
			{
				if (lid != 33639U && lid != 46439U)
				{
					goto IL_2E;
				}
			}
			else if (lid != 54361U && lid != 62152U)
			{
				goto IL_2E;
			}
			return true;
			IL_2E:
			if (operationSource == 0)
			{
				if (error == ErrorCodeValue.NullObject)
				{
					return true;
				}
				if (operationNumber <= 68)
				{
					if (operationNumber <= 21)
					{
						switch (operationNumber)
						{
						case 0:
							return error == ErrorCodeValue.MdbNotInitialized;
						case 1:
							return false;
						case 2:
							return error == ErrorCodeValue.NoAccess || error == ErrorCodeValue.NotFound;
						case 3:
							return error == ErrorCodeValue.NoAccess || error == ErrorCodeValue.NotFound;
						default:
							if (operationNumber == 12)
							{
								return error == ErrorCodeValue.ObjectChanged;
							}
							switch (operationNumber)
							{
							case 20:
								return error == ErrorCodeValue.NotSupported;
							case 21:
								return error == ErrorCodeValue.NotSupported;
							default:
								return false;
							}
							break;
						}
					}
					else
					{
						if (operationNumber == 43)
						{
							return error == ErrorCodeValue.NotFound || error == ErrorCodeValue.NotSupported;
						}
						switch (operationNumber)
						{
						case 49:
							return error == ErrorCodeValue.NotInitialized;
						case 50:
							return false;
						case 51:
							break;
						default:
							switch (operationNumber)
							{
							case 67:
								return error == ErrorCodeValue.NotFound;
							case 68:
								return error == ErrorCodeValue.InvalidParameter;
							default:
								return false;
							}
							break;
						}
					}
				}
				else if (operationNumber <= 115)
				{
					if (operationNumber == 79)
					{
						return error == ErrorCodeValue.NotFound;
					}
					switch (operationNumber)
					{
					case 97:
						return error == ErrorCodeValue.NotFound;
					case 98:
						return false;
					case 99:
						return error == ErrorCodeValue.NoReplicaHere;
					default:
						switch (operationNumber)
						{
						case 114:
							return error == ErrorCodeValue.SyncIgnore || error == ErrorCodeValue.SyncObjectDeleted;
						case 115:
							return error == ErrorCodeValue.SyncIgnore;
						default:
							return false;
						}
						break;
					}
				}
				else
				{
					if (operationNumber == 120)
					{
						return error == ErrorCodeValue.SyncClientChangeNewer || error == ErrorCodeValue.NotFound || error == ErrorCodeValue.SyncObjectDeleted;
					}
					switch (operationNumber)
					{
					case 156:
						return error == ErrorCodeValue.NoAccess;
					case 157:
						return error == ErrorCodeValue.ShutoffQuotaExceeded;
					case 158:
						return error == ErrorCodeValue.DuplicateDelivery;
					case 159:
						return false;
					case 160:
						break;
					case 161:
						return error == ErrorCodeValue.DuplicateDelivery;
					default:
						if (operationNumber != 254)
						{
							return false;
						}
						return error == ErrorCodeValue.ADPropertyError || error == ErrorCodeValue.AdUnavailable || error == ErrorCodeValue.DisabledMailbox || error == ErrorCodeValue.MailboxInTransit || error == ErrorCodeValue.MailboxQuarantined || error == ErrorCodeValue.MaxObjectsExceeded || error == ErrorCodeValue.WrongServer || error == ErrorCodeValue.LogonFailed || error == ErrorCodeValue.UnknownUser || error == ErrorCodeValue.MdbNotInitialized;
					}
				}
				return error == ErrorCodeValue.ShutoffQuotaExceeded;
			}
			else if (operationSource == 1)
			{
				if (operationNumber == 34)
				{
					return error == ErrorCodeValue.PartialCompletion;
				}
				if (operationNumber == 48)
				{
					return error == ErrorCodeValue.MdbNotInitialized;
				}
			}
			return false;
		}

		public static void OnExceptionCatch(byte operationSource, byte operationNumber, byte clientType, int databaseNumber, int mailboxNumber, Exception exception, object diagnosticData)
		{
			if (exception != null)
			{
				if (ExTraceGlobals.ExceptionHandlerTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.ExceptionHandlerTracer.TraceError<string, string>(0, 0L, "Exception: {0} Text: {1}", exception.GetType().Name, exception.ToString());
				}
				int associatedValue = 0;
				if (exception is StoreException)
				{
					StoreException ex = (StoreException)exception;
					if (ErrorHelper.ShouldSkipBreadcrumb(operationSource, operationNumber, ex.Error, ex.Lid))
					{
						return;
					}
					associatedValue = (int)ex.Error;
				}
				object associatedObject;
				if (diagnosticData == null)
				{
					associatedObject = exception;
				}
				else
				{
					associatedObject = new Tuple<Exception, object>(exception, diagnosticData);
				}
				ErrorHelper.AddBreadcrumb(BreadcrumbKind.Exception, operationSource, operationNumber, clientType, databaseNumber, mailboxNumber, associatedValue, associatedObject);
			}
		}

		public static void TraceException(Microsoft.Exchange.Diagnostics.Trace tracer, LID lid, Exception exception)
		{
			StoreException ex = exception as StoreException;
			if (ex != null)
			{
				DiagnosticContext.TraceStoreError(lid, (uint)ex.Error);
			}
			else
			{
				RopExecutionException ex2 = exception as RopExecutionException;
				if (ex2 != null)
				{
					DiagnosticContext.TraceStoreError(lid, (uint)ex2.ErrorCode);
				}
				else
				{
					DiagnosticContext.TraceStoreError(lid, 5000U);
				}
			}
			if (!ExTraceGlobals.ExceptionHandlerTracer.IsTraceEnabled(TraceType.ErrorTrace) && tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				tracer.TraceError<string, string>((int)lid.Value, 0L, "Exception: {0} Text: {1}", exception.GetType().Name, exception.ToString());
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool assertCondition, string message)
		{
			if (!assertCondition && !ErrorHelper.disableDebugAsserts)
			{
				ErrorHelper.OutputDebugString(message);
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_InternalLogicError, new object[]
				{
					message,
					new StackTrace(true).ToString()
				});
				ErrorHelper.CheckForDebugger();
			}
		}

		public static void AssertRetail(bool assertCondition, string message)
		{
			if (!assertCondition)
			{
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_InternalLogicError, new object[]
				{
					message,
					new StackTrace(true).ToString()
				});
				if (ErrorHelper.CheckForDebugger())
				{
					WatsonOnUnhandledException.KillCurrentProcess();
					return;
				}
				ExAssert.RetailAssert(false, message);
			}
		}

		internal static bool CheckForDebugger()
		{
			bool result = false;
			switch (ErrorHelper.GetAttachedDebuggerType())
			{
			case ErrorHelper.AttachedDebuggerType.None:
				result = false;
				break;
			case ErrorHelper.AttachedDebuggerType.Managed:
				result = true;
				ErrorHelper.OutputDebugString("Forcing managed DebugBreak...");
				Debugger.Break();
				break;
			case ErrorHelper.AttachedDebuggerType.Native:
				result = true;
				ErrorHelper.OutputDebugString("Forcing unmanaged DebugBreak...");
				ErrorHelper.DebugBreak();
				break;
			default:
				Globals.AssertRetail(false, "Unexpected debugger type.");
				break;
			}
			return result;
		}

		internal static bool IsDebuggerAttached()
		{
			bool result = false;
			switch (ErrorHelper.GetAttachedDebuggerType())
			{
			case ErrorHelper.AttachedDebuggerType.None:
				result = false;
				break;
			case ErrorHelper.AttachedDebuggerType.Managed:
			case ErrorHelper.AttachedDebuggerType.Native:
				result = true;
				break;
			default:
				Globals.AssertRetail(false, "Unexpected debugger type.");
				break;
			}
			return result;
		}

		internal static IDisposable DisableDebugAsserts()
		{
			return new ErrorHelper.DisableDebugAssertsFrame();
		}

		internal static void GetExceptionSummary(Exception e, out string exceptionType, out string exceptionMessage, out string exceptionStack)
		{
			exceptionType = string.Empty;
			exceptionMessage = string.Empty;
			exceptionStack = string.Empty;
			if (e != null)
			{
				exceptionType = e.GetType().Name;
				if (e.Message != null && e.Message.Length > 0)
				{
					string text = ErrorHelper.whiteSpaceRegex.Replace(e.Message, " ");
					exceptionMessage = text.Substring(0, Math.Min(256, text.Length));
				}
				if (e.StackTrace != null && e.StackTrace.Length > 0)
				{
					exceptionStack = ErrorHelper.GetCondensedCallStack(e.StackTrace);
				}
			}
		}

		internal static string GetCondensedCallStack(string stackTrace)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (stackTrace != null && stackTrace.Length > 0)
			{
				foreach (object obj in ErrorHelper.thrownFromRegex.Matches(stackTrace))
				{
					Match match = (Match)obj;
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					if (match.Groups.Count == 2)
					{
						int num = match.Groups[1].Captures.Count - 2;
						for (int i = 0; i < match.Groups[1].Captures.Count; i++)
						{
							if (i >= num)
							{
								if (i > 0)
								{
									stringBuilder.Append(".");
								}
								stringBuilder.Append(match.Groups[1].Captures[i].Value);
							}
							else
							{
								string value = match.Groups[1].Captures[i].Value;
								if (value != null && value.Length > 0)
								{
									foreach (object obj2 in ErrorHelper.upperRegex.Matches(value))
									{
										Match match2 = (Match)obj2;
										stringBuilder.Append(match2.Value);
									}
								}
							}
						}
					}
					if (stringBuilder.Length > 256)
					{
						break;
					}
				}
			}
			return stringBuilder.ToString().Substring(0, Math.Min(256, stringBuilder.Length));
		}

		private const string KeyNameFormatCrashOnLIDRoot = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}";

		private const string ValueNameCrashOnLID = "CrashOnLID";

		private static bool disableDebugAsserts = false;

		private static Queue<Breadcrumb> breadcrumbs;

		private static Regex thrownFromRegex = new Regex("at (?:([A-Z][A-Za-z]+)[\\.\\(])+");

		private static Regex upperRegex = new Regex("[A-Z]");

		private static Regex whiteSpaceRegex = new Regex("\\s+");

		private enum AttachedDebuggerType
		{
			None,
			Managed,
			Native
		}

		private class DisableDebugAssertsFrame : IDisposable
		{
			public DisableDebugAssertsFrame()
			{
				ErrorHelper.disableDebugAsserts = true;
			}

			public void Dispose()
			{
				ErrorHelper.disableDebugAsserts = false;
			}
		}
	}
}
