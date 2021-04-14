using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class HttpLogger
	{
		internal static bool LoggerNotDisposed
		{
			get
			{
				if (!LoggerSettings.LogEnabled)
				{
					return false;
				}
				if (LoggerSettings.IsPowerShellWebService)
				{
					PswsLogger pswsLogger = RequestDetailsLoggerBase<PswsLogger>.Current;
					return pswsLogger != null && !pswsLogger.IsDisposed;
				}
				RpsHttpLogger rpsHttpLogger = RequestDetailsLoggerBase<RpsHttpLogger>.Current;
				return rpsHttpLogger != null && !rpsHttpLogger.IsDisposed;
			}
		}

		internal static IActivityScope ActivityScope
		{
			get
			{
				if (!LoggerSettings.LogEnabled)
				{
					return null;
				}
				if (LoggerSettings.IsPowerShellWebService)
				{
					if (RequestDetailsLoggerBase<PswsLogger>.Current != null)
					{
						return RequestDetailsLoggerBase<PswsLogger>.Current.ActivityScope;
					}
					return null;
				}
				else
				{
					if (RequestDetailsLoggerBase<RpsHttpLogger>.Current != null)
					{
						return RequestDetailsLoggerBase<RpsHttpLogger>.Current.ActivityScope;
					}
					return null;
				}
			}
		}

		internal static void InitializeRequestLogger()
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				RequestDetailsLoggerBase<PswsLogger>.InitializeRequestLogger();
				return;
			}
			RequestDetailsLoggerBase<RpsHttpLogger>.InitializeRequestLogger();
		}

		internal static void SafeSetLogger(Enum key, object value)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				RequestDetailsLoggerBase<PswsLogger>.SafeSetLogger(RequestDetailsLoggerBase<PswsLogger>.Current, key, value);
				return;
			}
			RequestDetailsLoggerBase<RpsHttpLogger>.SafeSetLogger(RequestDetailsLoggerBase<RpsHttpLogger>.Current, key, value);
		}

		internal static void SafeAppendGenericError(string key, Exception ex, Func<Exception, bool> funcToVerifyException)
		{
			HttpLogger.SafeAppendGenericError(key, (ex == null) ? null : ex.ToString(), funcToVerifyException(ex));
		}

		internal static void SafeAppendGenericError(string key, string value, bool isUnhandledException)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			string genericErrorKey = Diagnostics.GetGenericErrorKey(key, isUnhandledException);
			if (LoggerSettings.IsPowerShellWebService)
			{
				RequestDetailsLoggerBase<PswsLogger>.SafeAppendGenericError(RequestDetailsLoggerBase<PswsLogger>.Current, genericErrorKey, value);
				return;
			}
			RequestDetailsLoggerBase<RpsHttpLogger>.SafeAppendGenericError(RequestDetailsLoggerBase<RpsHttpLogger>.Current, genericErrorKey, value);
		}

		internal static void AsyncCommit(bool forceSync)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				if (RequestDetailsLoggerBase<PswsLogger>.Current != null)
				{
					RequestDetailsLoggerBase<PswsLogger>.Current.AsyncCommit(forceSync);
					return;
				}
			}
			else if (RequestDetailsLoggerBase<RpsHttpLogger>.Current != null)
			{
				RequestDetailsLoggerBase<RpsHttpLogger>.Current.AsyncCommit(forceSync);
			}
		}

		internal static void SafeAppendGenericInfo(string key, string value)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				RequestDetailsLoggerBase<PswsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<PswsLogger>.Current, key, value);
				return;
			}
			RequestDetailsLoggerBase<RpsHttpLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RpsHttpLogger>.Current, key, value);
		}

		internal static void SafeAppendColumn(Enum column, string key, string value)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				RequestDetailsLoggerBase<PswsLogger>.SafeAppendColumn(RequestDetailsLoggerBase<PswsLogger>.Current, column, key, value);
				return;
			}
			RequestDetailsLoggerBase<RpsHttpLogger>.SafeAppendColumn(RequestDetailsLoggerBase<RpsHttpLogger>.Current, column, key, value);
		}
	}
}
