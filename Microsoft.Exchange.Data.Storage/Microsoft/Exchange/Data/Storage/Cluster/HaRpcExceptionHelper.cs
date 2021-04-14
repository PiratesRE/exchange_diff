using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class HaRpcExceptionHelper
	{
		public static bool TryGetExceptionOrInnerOfType<TException>(this Exception ex, out Exception convertedException) where TException : Exception
		{
			convertedException = null;
			if (ex is TException)
			{
				convertedException = ex;
				return true;
			}
			return ex.TryGetInnerExceptionOfType(out convertedException);
		}

		public static bool TryGetInnerExceptionOfType<TException>(this Exception ex, out Exception innerException) where TException : Exception
		{
			innerException = null;
			while (ex.InnerException != null)
			{
				Exception innerException2 = ex.InnerException;
				if (innerException2 is TException)
				{
					innerException = innerException2;
					return true;
				}
				ex = ex.InnerException;
			}
			return false;
		}

		public static bool TryGetTypedInnerException<TException>(this Exception ex, out TException innerException) where TException : Exception
		{
			innerException = default(TException);
			while (ex.InnerException != null)
			{
				TException ex2 = ex.InnerException as TException;
				if (ex2 != null)
				{
					innerException = ex2;
					return true;
				}
				ex = ex.InnerException;
			}
			return false;
		}

		internal static string GetCompleteExceptionMessage(Exception ex, bool fCalledFromToString)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			bool flag = false;
			while (ex != null)
			{
				if (flag)
				{
					stringBuilder.Append(" ---> ");
				}
				else
				{
					flag = true;
				}
				string text = string.Empty;
				IHaRpcServerBaseExceptionInternal haRpcServerBaseExceptionInternal = ex as IHaRpcServerBaseExceptionInternal;
				if (haRpcServerBaseExceptionInternal != null)
				{
					text = haRpcServerBaseExceptionInternal.MessageInternal;
				}
				else
				{
					text = ex.Message;
				}
				if (fCalledFromToString)
				{
					stringBuilder.AppendFormat("{0}: {1}", ex.GetType(), text);
				}
				else
				{
					stringBuilder.Append(text);
				}
				ex = ex.InnerException;
			}
			return stringBuilder.ToString();
		}

		internal static string GetFullString(IHaRpcServerBaseException ex, IHaRpcServerBaseExceptionInternal exInternal)
		{
			StringBuilder stringBuilder = new StringBuilder(2048);
			stringBuilder.AppendFormat("{0}: {1}", ex.GetType(), exInternal.MessageInternal);
			if (ex.InnerException != null)
			{
				stringBuilder.Append(" ---> ");
				stringBuilder.Append(ex.InnerException.ToString());
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Format("   --- End of inner exception stack trace ({0}) ---", ex.InnerException.GetType()));
			}
			if (!string.IsNullOrEmpty(ex.OriginatingStackTrace))
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(ex.OriginatingStackTrace);
			}
			if (!string.IsNullOrEmpty(ex.OriginatingServer))
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(string.Format("   --- End of stack trace on server ({0}) ---", ex.OriginatingServer));
			}
			if (!string.IsNullOrEmpty(ex.StackTrace))
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(ex.StackTrace);
			}
			return stringBuilder.ToString();
		}

		internal static string GetOriginatingServerString(string originatingServer, string databaseName)
		{
			if (string.IsNullOrEmpty(originatingServer))
			{
				return string.Empty;
			}
			if (string.IsNullOrEmpty(databaseName))
			{
				return string.Format(" [{0}: {1}]", ServerStrings.OriginatingServer, originatingServer);
			}
			return string.Format(" [{0}: {1}, {2}: {3}]", new object[]
			{
				ServerStrings.Database,
				databaseName,
				ServerStrings.OriginatingServer,
				originatingServer
			});
		}

		public static string AppendLastErrorString(string message, string lastError)
		{
			string arg = string.Format(" [{0}: {1}]", ServerStrings.LastErrorMessage, lastError);
			return string.Format("{0}{1}", message, arg);
		}

		internal const string ClientSideExceptionFormatToString = "{0}: {1}";

		internal const string ClientSideInnerSeperator = " ---> ";

		internal const string OriginatingServerFormatString = " [{0}: {1}]";

		internal const string OriginatingServerWithDbFormatString = " [{0}: {1}, {2}: {3}]";

		internal const string LastErrorFormatString = " [{0}: {1}]";

		internal const string HaExceptionMessageFormatString = "{0}{1}";

		internal const string ServerSideStackTraceFormatString = "   --- End of stack trace on server ({0}) ---";

		internal const string InnerExceptionStackTraceFormatString = "   --- End of inner exception stack trace ({0}) ---";
	}
}
