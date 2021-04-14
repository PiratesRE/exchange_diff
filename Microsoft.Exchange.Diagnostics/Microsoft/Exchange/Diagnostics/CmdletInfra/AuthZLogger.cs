using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class AuthZLogger
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
				RpsAuthZLogger rpsAuthZLogger = RequestDetailsLoggerBase<RpsAuthZLogger>.Current;
				return rpsAuthZLogger != null && !rpsAuthZLogger.IsDisposed;
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
					if (RequestDetailsLoggerBase<RpsAuthZLogger>.Current != null)
					{
						return RequestDetailsLoggerBase<RpsAuthZLogger>.Current.ActivityScope;
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
			if (LoggerSettings.IsRemotePS)
			{
				IActivityScope activityScope = null;
				if (!ActivityContext.IsStarted)
				{
					ActivityContext.ClearThreadScope();
					activityScope = ActivityContext.Start(null);
				}
				RequestDetailsLoggerBase<RpsAuthZLogger>.InitializeRequestLogger(activityScope ?? ActivityContext.GetCurrentActivityScope());
			}
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
			RequestDetailsLoggerBase<RpsAuthZLogger>.SafeSetLogger(RequestDetailsLoggerBase<RpsAuthZLogger>.Current, key, value);
		}

		internal static void SafeAppendGenericError(string key, Exception ex, Func<Exception, bool> funcToVerifyException)
		{
			AuthZLogger.SafeAppendGenericError(key, (ex == null) ? null : ex.ToString(), funcToVerifyException(ex));
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
			RequestDetailsLoggerBase<RpsAuthZLogger>.SafeAppendGenericError(RequestDetailsLoggerBase<RpsAuthZLogger>.Current, genericErrorKey, value);
		}

		internal static void AsyncCommit(bool forceSync)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsRemotePS && RequestDetailsLoggerBase<RpsAuthZLogger>.Current != null)
			{
				RequestDetailsLoggerBase<RpsAuthZLogger>.Current.AsyncCommit(forceSync);
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
			RequestDetailsLoggerBase<RpsAuthZLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RpsAuthZLogger>.Current, key, value);
		}

		internal static void UpdateLatency(Enum latencyMetadata, double latencyInMilliseconds)
		{
			if (!LoggerSettings.LogEnabled)
			{
				return;
			}
			if (LoggerSettings.IsPowerShellWebService)
			{
				if (RequestDetailsLoggerBase<PswsLogger>.Current != null)
				{
					RequestDetailsLoggerBase<PswsLogger>.Current.UpdateLatency(latencyMetadata, latencyInMilliseconds);
					return;
				}
			}
			else if (RequestDetailsLoggerBase<RpsAuthZLogger>.Current != null)
			{
				RequestDetailsLoggerBase<RpsAuthZLogger>.Current.UpdateLatency(latencyMetadata, latencyInMilliseconds);
			}
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
			RequestDetailsLoggerBase<RpsAuthZLogger>.SafeAppendColumn(RequestDetailsLoggerBase<RpsAuthZLogger>.Current, column, key, value);
		}
	}
}
