using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal static class AmExceptionHelper
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterTracer;
			}
		}

		public static string GetExceptionMessageOrNoneString(Exception ex)
		{
			return AmExceptionHelper.GetExceptionGenericStringOrNoneString(ex, (Exception exception) => exception.Message);
		}

		public static string GetExceptionToStringOrNoneString(Exception ex)
		{
			return AmExceptionHelper.GetExceptionGenericStringOrNoneString(ex, (Exception exception) => exception.ToString());
		}

		public static string GetExceptionGenericStringOrNoneString(Exception ex, Func<Exception, string> stringExtractor)
		{
			string message = (ex != null) ? stringExtractor(ex) : Strings.NoErrorSpecified;
			return AmExceptionHelper.GetMessageOrNoneString(message);
		}

		public static string GetMessageOrNoneString(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				message = Strings.NoErrorSpecified;
			}
			return message;
		}

		public static uint Win32ErrorCodeFromHresult(uint hresult)
		{
			if ((hresult & 4294901760U) == 2147942400U || (hresult & 4294901760U) == 2147549184U)
			{
				return hresult & 65535U;
			}
			return hresult;
		}

		internal static bool IsKnownClusterException(object task, Exception e)
		{
			if (e is DataSourceTransientException || e is DataSourceOperationException || e is ClusterException || e is TransientException || e is SecurityException || e is UnauthorizedAccessException || e is IOException)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<string>((long)task.GetHashCode(), task.ToString() + " got exception : {0}", e.Message);
				return true;
			}
			return false;
		}

		internal static bool IsImmediateClusRetryException(Exception e)
		{
			return true;
		}

		internal static uint Win32ErrorCodeFromComException(COMException comException)
		{
			uint errorCode = (uint)comException.ErrorCode;
			return AmExceptionHelper.Win32ErrorCodeFromHresult(errorCode);
		}

		internal static bool IsRetryableClusterResourceException(Exception e)
		{
			return true;
		}

		internal static bool CheckExceptionCode(Exception e, uint errCode)
		{
			if (e == null)
			{
				return false;
			}
			COMException ex = e as COMException;
			if (ex != null && ((ulong)errCode == (ulong)((long)(ex.ErrorCode & 65535)) || errCode == (uint)ex.ErrorCode))
			{
				return true;
			}
			ex = (e.InnerException as COMException);
			if (ex != null && ((ulong)errCode == (ulong)((long)(ex.ErrorCode & 65535)) || errCode == (uint)ex.ErrorCode))
			{
				return true;
			}
			Win32Exception ex2 = e as Win32Exception;
			if (ex2 != null)
			{
				if (ex2.NativeErrorCode == (int)errCode)
				{
					return true;
				}
			}
			else
			{
				ex2 = (e.InnerException as Win32Exception);
				if (ex2 != null && ex2.NativeErrorCode == (int)errCode)
				{
					return true;
				}
			}
			return false;
		}

		internal static ClusterApiException ConstructClusterApiException(int errorCode, string methodName, params object[] methodParameters)
		{
			Win32Exception ex = new Win32Exception(errorCode);
			return new ClusterApiException(Strings.ClusterApiErrorMessage(string.Format(methodName, methodParameters), errorCode, ex.Message), ex);
		}

		internal static ClusterApiException ConstructClusterApiExceptionNoErr(string methodName, params object[] methodParameters)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			return AmExceptionHelper.ConstructClusterApiException(lastWin32Error, methodName, methodParameters);
		}

		internal static TException HandleSpecificException<TException>(Action operation) where TException : Exception
		{
			TException result = default(TException);
			try
			{
				operation();
			}
			catch (TException ex)
			{
				TException ex2 = (TException)((object)ex);
				result = ex2;
			}
			return result;
		}

		internal static void HandleRetryIoPendingExceptions(COMException e, ref int count)
		{
			if (AmExceptionHelper.CheckExceptionCode(e, 997U))
			{
				throw new ClusCommonTaskPendingException(e.Message, e);
			}
			AmExceptionHelper.HandleRetryExceptions(e, ref count, null);
		}

		internal static void HandleRetryExceptions(Exception e, ref int count, IAmCluster cluster)
		{
			AmExceptionHelper.HandleRetryExceptions(e, ref count, false, cluster, null);
		}

		internal static void HandleRetryExceptions(Exception e, ref int count, bool dbAccess, IAmCluster cluster)
		{
			AmExceptionHelper.HandleRetryExceptions(e, ref count, dbAccess, cluster, null);
		}

		internal static void HandleRetryExceptions(Exception e, ref int count, bool dbAccess, IAmCluster cluster, int? sleepIntervalMilliSecs)
		{
			if (!AmExceptionHelper.IsImmediateClusRetryException(e))
			{
				if (!AmExceptionHelper.IsRetryableClusterResourceException(e))
				{
					throw new ClusCommonFailException(e.Message, e);
				}
				if (dbAccess)
				{
					throw new ClusterDatabaseTransientException(e.Message, e);
				}
				throw new ClusCommonNonRetryableTransientException(e.Message, e);
			}
			else
			{
				AmExceptionHelper.CheckExceptionCode(e, 2147549448U);
				if (count++ < 3)
				{
					if (sleepIntervalMilliSecs != null)
					{
						Thread.Sleep(sleepIntervalMilliSecs.Value);
					}
					return;
				}
				AmExceptionHelper.Tracer.TraceDebug<string, int>(0L, "HandleRetry throwing error {0}, after {1} retries", e.Message, count);
				if (dbAccess)
				{
					throw new ClusterDatabaseTransientException(e.Message, e);
				}
				throw new ClusCommonRetryableTransientException(e.Message, e);
			}
		}

		internal const int RetryTimes = 3;
	}
}
