using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class AuthZLogHelper
	{
		public static IScopedPerformanceMonitor[] AuthZPerfMonitors
		{
			get
			{
				return new IScopedPerformanceMonitor[]
				{
					new LatencyMonitor(AuthZLogHelper.latencyTracker)
				};
			}
		}

		public static LatencyTracker LantencyTracker
		{
			get
			{
				return AuthZLogHelper.latencyTracker;
			}
		}

		internal static void StartAndEndLoging(string funcName, Action action)
		{
			AuthZLogHelper.StartAndEndLoging<bool>(funcName, delegate()
			{
				action();
				return true;
			});
		}

		internal static T StartAndEndLoging<T>(string funcName, Func<T> func)
		{
			bool flag = false;
			bool flag2 = false;
			T result;
			try
			{
				flag2 = AuthZLogHelper.StartLogging(funcName, out flag);
				result = func();
			}
			finally
			{
				if (flag2)
				{
					AuthZLogHelper.EndLogging(true);
				}
				else if (flag)
				{
					AuthZLogHelper.EndLogging(false);
				}
			}
			return result;
		}

		internal static void ExecuteWSManPluginAPI(string funcName, bool throwException, bool trackLatency, Action action)
		{
			AuthZLogHelper.ExecuteWSManPluginAPI<bool>(funcName, throwException, trackLatency, false, delegate()
			{
				action();
				return false;
			});
		}

		internal static T ExecuteWSManPluginAPI<T>(string funcName, bool throwException, bool trackLatency, T defaultReturnValue, Func<T> func)
		{
			ExWatson.IsExceptionInteresting isExceptionInteresting = null;
			T result;
			try
			{
				AuthZLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, funcName, DateTime.UtcNow.ToString());
				string funcName2 = funcName;
				bool throwException2 = throwException;
				LatencyTracker latencyTracker = trackLatency ? AuthZLogHelper.latencyTracker : null;
				ExEventLog rbacEventLogger = AuthZLogHelper.RbacEventLogger;
				ExEventLog.EventTuple tuple_RemotePSPublicAPIFailed = Microsoft.Exchange.Configuration.ObjectModel.EventLog.TaskEventLogConstants.Tuple_RemotePSPublicAPIFailed;
				Trace publicPluginAPITracer = Microsoft.Exchange.Diagnostics.Components.Authorization.ExTraceGlobals.PublicPluginAPITracer;
				if (isExceptionInteresting == null)
				{
					isExceptionInteresting = ((object ex) => AuthZPluginHelper.IsFatalException(ex as Exception));
				}
				result = Diagnostics.ExecuteAndLog<T>(funcName2, throwException2, latencyTracker, rbacEventLogger, tuple_RemotePSPublicAPIFailed, publicPluginAPITracer, isExceptionInteresting, delegate(Exception ex)
				{
					AuthZLogHelper.LogException(ex, funcName, throwException);
				}, defaultReturnValue, () => AuthZLogHelper.HandleExceptionAndRetry<T>(funcName, func, throwException, defaultReturnValue));
			}
			catch (Exception ex)
			{
				string arg = (AuthZLogger.ActivityScope != null) ? AuthZLogger.ActivityScope.ActivityId.ToString() : null;
				AuthZLogHelper.EndLogging(true);
				Exception ex3;
				string str = string.Format("[FailureCategory={0}] ", FailureCategory.AuthZ + "-" + ex3.GetType().Name);
				string str2 = string.Format("[AuthZRequestId={0}]", arg);
				LocalizedString message = new LocalizedString(str2 + str + ex3.Message);
				AuthorizationException ex2 = new AuthorizationException(message, ex3);
				throw ex2;
			}
			return result;
		}

		internal static bool StartLogging(string funcName)
		{
			bool flag = false;
			return AuthZLogHelper.StartLogging(funcName, out flag);
		}

		internal static bool StartLogging(string funcName, out bool latencyTrackerStartedByMe)
		{
			AuthZLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, funcName, DateTime.UtcNow.ToString());
			latencyTrackerStartedByMe = false;
			if (AuthZLogHelper.latencyTracker == null)
			{
				Diagnostics.ExecuteAndLog("AuthZLogHelper.StartLatencyTracker", false, null, Constants.CoreEventLogger, Microsoft.Exchange.Configuration.Core.EventLog.TaskEventLogConstants.Tuple_NonCrashingException, Microsoft.Exchange.Diagnostics.Components.Configuration.Core.ExTraceGlobals.InstrumentationTracer, null, delegate(Exception ex)
				{
					AuthZLogHelper.LogException(ex, "AuthZLogHelper.StartLatencyTracker", false);
				}, delegate()
				{
					AuthZLogHelper.StartLatencyTracker(funcName);
				});
				latencyTrackerStartedByMe = (AuthZLogHelper.latencyTracker != null);
			}
			if (AuthZLogger.LoggerNotDisposed)
			{
				return false;
			}
			InitializeLoggerSettingsHelper.InitLoggerSettings();
			AuthZLogger.InitializeRequestLogger();
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.Function, funcName);
			return true;
		}

		internal static void EndLogging()
		{
			AuthZLogHelper.EndLogging(true);
		}

		internal static void EndLogging(bool shouldCommit)
		{
			Microsoft.Exchange.Diagnostics.Components.Configuration.Core.ExTraceGlobals.InstrumentationTracer.TraceDebug(0L, "[AuthZLogHelper.EndLogging] End logging.");
			try
			{
				if (AuthZLogHelper.latencyTracker != null)
				{
					long num = Diagnostics.ExecuteAndLog<long>("AuthZLogHelper.StopLatencyTracker", false, null, Constants.CoreEventLogger, Microsoft.Exchange.Configuration.Core.EventLog.TaskEventLogConstants.Tuple_NonCrashingException, Microsoft.Exchange.Diagnostics.Components.Configuration.Core.ExTraceGlobals.InstrumentationTracer, null, delegate(Exception ex)
					{
						AuthZLogHelper.LogException(ex, "AuthZLogHelper.StopLatencyTracker", false);
					}, -1L, new Func<long>(AuthZLogHelper.StopLatencyTracker));
					AuthZLogger.SafeSetLogger(ConfigurationCoreMetadata.TotalTime, num);
					AuthZLogHelper.latencyTracker.PushLatencyDetailsToLog(AuthZLogHelper.funcNameToLogMetadataDic, new Action<Enum, double>(AuthZLogger.UpdateLatency), delegate(string funcName, string totalLatency)
					{
						AuthZLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, funcName, totalLatency);
					});
				}
				else
				{
					AuthZLogger.SafeAppendColumn(RpsCommonMetadata.GenericLatency, "LatencyMissed", "AuthZLogHelper.latencyTracker is null");
				}
			}
			finally
			{
				try
				{
					if (shouldCommit)
					{
						AuthZLogger.AsyncCommit(true);
					}
				}
				finally
				{
					AuthZLogHelper.latencyTracker = null;
				}
			}
		}

		internal static void StartLatencyTracker(string funcName)
		{
			if (AuthZLogHelper.latencyTracker == null)
			{
				AuthZLogHelper.latencyTracker = new LatencyTracker(funcName, () => AuthZLogger.ActivityScope);
				AuthZLogHelper.latencyTracker.Start();
			}
		}

		internal static bool StartInternalTracking(string groupName, string funcName)
		{
			return AuthZLogHelper.latencyTracker != null && AuthZLogHelper.latencyTracker.StartInternalTracking(groupName, funcName, false);
		}

		internal static bool StartInternalTracking(string funcName)
		{
			return AuthZLogHelper.latencyTracker != null && AuthZLogHelper.latencyTracker.StartInternalTracking(funcName);
		}

		internal static void EndInternalTracking(string groupName, string funcName)
		{
			if (AuthZLogHelper.latencyTracker == null)
			{
				return;
			}
			AuthZLogHelper.latencyTracker.EndInternalTracking(groupName, funcName);
		}

		internal static void EndInternalTracking(string funcName)
		{
			if (AuthZLogHelper.latencyTracker == null)
			{
				return;
			}
			AuthZLogHelper.latencyTracker.EndInternalTracking(funcName);
		}

		internal static void LogAuthZUserToken(AuthZPluginUserToken userToken)
		{
			if (userToken == null)
			{
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.IsAuthorized, false);
				return;
			}
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.IsAuthorized, true);
			AuthZLogger.SafeSetLogger(ServiceCommonMetadata.AuthenticatedUser, userToken.UserNameForLogging);
			AuthZLogger.SafeSetLogger(ActivityStandardMetadata.AuthenticationType, userToken.AuthenticationType);
			AuthZLogger.SafeSetLogger(ActivityStandardMetadata.TenantId, userToken.OrgIdInString);
			if (userToken.DelegatedPrincipal != null)
			{
				AuthZLogger.SafeSetLogger(ConfigurationCoreMetadata.ManagedOrganization, "Delegate:" + userToken.DelegatedPrincipal.DelegatedOrganization);
			}
		}

		private static void LogException(Exception ex, string funcName, bool throwException)
		{
			if (throwException)
			{
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.IsAuthorized, false);
			}
			AuthZLogger.SafeAppendGenericError(funcName, ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
		}

		private static long StopLatencyTracker()
		{
			long result = -1L;
			if (AuthZLogHelper.latencyTracker != null)
			{
				result = AuthZLogHelper.latencyTracker.Stop();
			}
			return result;
		}

		private static T HandleExceptionAndRetry<T>(string methodName, Func<T> func, bool throwException, T defaultReturnValue)
		{
			for (int i = 0; i < 2; i++)
			{
				try
				{
					Microsoft.Exchange.Diagnostics.Components.Authorization.ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, int>(0L, "Retry function {0} the {1} times.", methodName, i);
					return func();
				}
				catch (Exception ex)
				{
					bool flag = ex is TransientException;
					bool flag2 = AuthZPluginHelper.IsFatalException(ex);
					bool flag3 = flag2 || AuthZLogHelper.ExceptionNoNeedToRetry(ex);
					Microsoft.Exchange.Diagnostics.Components.Authorization.ExTraceGlobals.PublicPluginAPITracer.TraceDebug(0L, "{0} caught Exception {1}. IsTransientException = {2}. IsFatalException = {3}. NoNeedToRetry = {4}.", new object[]
					{
						methodName,
						ex,
						flag,
						flag2,
						flag3
					});
					ExEventLog.EventTuple eventInfo = Microsoft.Exchange.Configuration.ObjectModel.EventLog.TaskEventLogConstants.Tuple_RBACUnavailable_UnknownError;
					if (flag)
					{
						eventInfo = Microsoft.Exchange.Configuration.ObjectModel.EventLog.TaskEventLogConstants.Tuple_RBACUnavailable_TransientError;
					}
					else if (flag2)
					{
						eventInfo = Microsoft.Exchange.Configuration.ObjectModel.EventLog.TaskEventLogConstants.Tuple_RBACUnavailable_FatalError;
					}
					TaskLogger.LogRbacEvent(eventInfo, null, new object[]
					{
						methodName,
						ex
					});
					if (flag3 || i == 1)
					{
						if (!(ex is ADTransientException) && (flag2 || throwException))
						{
							throw;
						}
						AuthZLogHelper.LogException(ex, methodName, false);
						break;
					}
					else
					{
						AuthZLogger.SafeAppendGenericInfo(methodName + "-" + ex.GetType().Name + "-Retried", ex.Message);
					}
				}
			}
			Microsoft.Exchange.Diagnostics.Components.Authorization.ExTraceGlobals.PublicPluginAPITracer.TraceError<string, T>(0L, "{0} returns default value {1}.", methodName, defaultReturnValue);
			return defaultReturnValue;
		}

		private static bool ExceptionNoNeedToRetry(Exception ex)
		{
			return ex is AuthorizationException;
		}

		private const int MaxRetryForADTransient = 2;

		public const string ExchangeRunspaceConfigurationGroupName = "ExchangeRunspaceConfiguration";

		public const string InitialSessionStateBuilderGroupName = "InitialSessionStateBuilder";

		[ThreadStatic]
		private static LatencyTracker latencyTracker;

		private static readonly ExEventLog RbacEventLogger = new ExEventLog(Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.LogTracer.Category, "MSExchange RBAC");

		private static readonly Dictionary<string, Enum> funcNameToLogMetadataDic = new Dictionary<string, Enum>
		{
			{
				"AuthorizeUser",
				RpsAuthZMetadata.AuthorizeUser
			},
			{
				"AuthorizeOperation",
				RpsAuthZMetadata.AuthorizeOperation
			},
			{
				"GetQuota",
				RpsAuthZMetadata.GetQuota
			},
			{
				"WSManOperationComplete",
				RpsAuthZMetadata.WSManOperationComplete
			},
			{
				"WSManUserComplete",
				RpsAuthZMetadata.WSManUserComplete
			},
			{
				"WSManQuotaComplete",
				RpsAuthZMetadata.WSManQuotaComplete
			},
			{
				"ValidateConnectionLimit",
				RpsAuthZMetadata.ValidateConnectionLimit
			},
			{
				"GetApplicationPrivateData",
				RpsAuthZMetadata.GetApplicationPrivateData
			},
			{
				"GetInitialSessionState",
				RpsAuthZMetadata.GetInitialSessionState
			}
		};
	}
}
