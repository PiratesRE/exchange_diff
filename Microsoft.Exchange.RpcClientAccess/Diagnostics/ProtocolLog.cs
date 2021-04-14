using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal sealed class ProtocolLog
	{
		private ProtocolLog()
		{
			ProtocolLogConfiguration protocolLogConfiguration = Configuration.ProtocolLogConfiguration;
			this.schema = new LogSchema(protocolLogConfiguration.SoftwareName, protocolLogConfiguration.SoftwareVersion, protocolLogConfiguration.LogTypeName, ProtocolLog.GetColumnArray());
			this.log = new Log(protocolLogConfiguration.LogFilePrefix, new LogHeaderFormatter(this.schema), protocolLogConfiguration.LogComponent);
			this.Refresh();
		}

		public static void Initialize()
		{
			ProtocolLog.instance = new ProtocolLog();
		}

		public static void Shutdown()
		{
			if (ProtocolLog.instance == null)
			{
				return;
			}
			ProtocolLog.instance.Close();
		}

		public static void Referesh()
		{
			if (ProtocolLog.instance == null)
			{
				return;
			}
			ProtocolLog.instance.Refresh();
		}

		internal static ProtocolLogSession CreateNewSession()
		{
			return ProtocolLog.CreateNewSession(false);
		}

		internal static ProtocolLogSession CreateNewSession(bool lazyInitializeRpcTimers)
		{
			if (ProtocolLog.instance == null)
			{
				return null;
			}
			return ProtocolLog.instance.InternalCreateNewSession(lazyInitializeRpcTimers);
		}

		internal LogRowFormatter CreateRowFormatter()
		{
			return new LogRowFormatter(this.schema);
		}

		internal void Append(LogRowFormatter row)
		{
			if (!this.isEnabled)
			{
				return;
			}
			this.log.Append(row, 0);
		}

		public static void SetConnectionParameters(int connectionId, string user, MapiVersion version, IPAddress clientIpAddress, IPAddress serverIpAddress, string protocolSequence)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetConnectionParameters(connectionId, user, version, clientIpAddress, serverIpAddress, protocolSequence);
		}

		public static void SetHttpParameters(IList<string> sessionCookies, IList<string> requestIds)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetHttpParameters(sessionCookies, requestIds);
		}

		public static void SetApplicationParameters(ClientMode clientMode, string clientProcessName)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetApplicationParameters(clientMode, clientProcessName);
		}

		public static void SetOrganizationInfo(string organizationInfo)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetOrganizationInfo(organizationInfo);
		}

		public static void SetClientConnectionInfo(string connectionInfo)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetClientConnectionInfo(connectionInfo);
		}

		public static void SetClientIpAddress(IPAddress clientIpAddress)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetClientIpAddress(clientIpAddress);
		}

		public static void SetActivityData()
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalSetActivityData();
		}

		public static void LogConnect(SecurityIdentifier sid, ConnectionFlags connectionFlags)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogConnect(sid, connectionFlags);
		}

		public static void LogDisconnect(DisconnectReason disconnectReason)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogDisconnect(disconnectReason);
		}

		public static void LogLogonPending(ProtocolLogLogonType logonType, ExchangePrincipal targetMailbox, string applicationId)
		{
			bool flag = ProtocolLog.IsLogEnabled || ExTraceGlobals.LogonTracer.IsTraceEnabled(TraceType.InfoTrace);
			string text = null;
			if (flag)
			{
				text = ProtocolLogSession.GenerateLogonOperationSpecificData(logonType, targetMailbox);
			}
			ExTraceGlobals.LogonTracer.TraceInformation(0, Activity.TraceId, text);
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogLogon(logonType, text, applicationId);
		}

		public static void LogLogonSuccess(int logonId)
		{
			ExTraceGlobals.LogonTracer.TraceInformation<int>(0, Activity.TraceId, "Logon successful. LogonId = {0}", logonId);
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogLogonSuccess(logonId);
		}

		public static void LogLogonRedirect(string reason, string suggestedNewServer)
		{
			ExTraceGlobals.LogonTracer.TraceWarning<string, string>(0, Activity.TraceId, "Redirecting a client: {0}, suggested new server: {1}", reason, suggestedNewServer);
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogLogonRedirect(reason, suggestedNewServer);
		}

		public static void LogLogoff(ProtocolLogLogonType logonType, int logonId)
		{
			ExTraceGlobals.LogonTracer.TraceInformation<int>(0, Activity.TraceId, "Logon has been released. LogonId = {0}", logonId);
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogLogoff(logonType, logonId);
		}

		public static void LogNewCall()
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogNewCall();
		}

		public static void LogInputRops(IEnumerable<RopId> rops)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogInputRops(rops);
		}

		public static void LogOutputRop(RopId ropId, ErrorCode errorCode)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogOutputRop(ropId, errorCode);
		}

		public static void LogRopFailure(RopId ropId, bool isWarning, bool shouldLog, ErrorCode errorCode, Exception exception)
		{
			if (isWarning)
			{
				ExTraceGlobals.FailedRopTracer.TraceWarning<RopId, ErrorCode, string>(0, Activity.TraceId, "{0} | {1} | {2}", ropId, errorCode, (exception != null) ? exception.Message : null);
				if (shouldLog && ProtocolLog.IsLogEnabled)
				{
					ProtocolLog.LogWarning("{0} succeeded with warning ec={1} \"{2}\"", new object[]
					{
						ropId,
						errorCode,
						exception
					});
				}
			}
			else
			{
				ExTraceGlobals.FailedRopTracer.TraceError<RopId, ErrorCode, string>(0, Activity.TraceId, "{0} | {1} | {2}", ropId, errorCode, (exception != null) ? exception.Message : null);
				if (shouldLog && ProtocolLog.IsLogEnabled)
				{
					ProtocolLog.instance.InternalLogFailure(ProtocolLogFailureLevel.RopHandler, string.Format("{0} (rop::{1})", (int)errorCode, errorCode), ropId, exception);
				}
			}
			if (exception != null)
			{
				ExTraceGlobals.FailedRopTracer.TraceDebug<Exception>(0, Activity.TraceId, "{0}", exception);
			}
		}

		public static void LogRpcFailure(bool shouldLog, RpcErrorCode errorCode, Exception exception)
		{
			ExTraceGlobals.FailedRpcTracer.TraceError<RpcErrorCode, string>(0, Activity.TraceId, "{0} | {1}", errorCode, (exception != null) ? exception.Message : "(no exception)");
			if (exception != null)
			{
				ExTraceGlobals.FailedRpcTracer.TraceDebug<Exception>(0, Activity.TraceId, "RPC Error: {0}", exception);
			}
			if (shouldLog && ProtocolLog.IsLogEnabled)
			{
				ProtocolLog.instance.InternalLogFailure(ProtocolLogFailureLevel.RpcDispatch, string.Format("{0} (rpc::{1})", (uint)errorCode, errorCode), RopId.None, exception);
			}
		}

		public static void LogRpcException(RpcServiceException exception)
		{
			ExTraceGlobals.FailedRpcTracer.TraceError<string, int>(0L, "Raising an RPC exception: {0}. Status = {1:X4}", exception.Message, exception.RpcStatus);
			if (exception != null)
			{
				ExTraceGlobals.FailedRpcTracer.TraceDebug<RpcServiceException>(0, Activity.TraceId, "RPC Exception: {0}", exception);
			}
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogFailure(ProtocolLogFailureLevel.RpcEndPoint, string.Format("0x{0:X} (rpc::Exception)", exception.RpcStatus), RopId.None, exception);
		}

		public static void LogWebServiceFailure(string status, string message, Exception exception, string emailAddress, string organizationInfo, string protocolSequence, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			ProtocolLog.LogProtocolFailure(ProtocolLogFailureLevel.WebServiceEndPoint, null, null, status, message, exception, emailAddress, organizationInfo, protocolSequence, null, trace);
		}

		public static void LogMapiHttpProtocolFailure(IList<string> requestIds, IList<string> cookies, string status, string message, Exception exception, string emailAddress, string organizationInfo, string protocolSequence, string clientAddress, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			ProtocolLog.LogProtocolFailure(ProtocolLogFailureLevel.MapiHttpEndPoint, requestIds, cookies, status, message, exception, emailAddress, organizationInfo, protocolSequence, clientAddress, trace);
		}

		public static void LogWatsonFailure(bool isFatal, Exception exception)
		{
			ExTraceGlobals.UnhandledExceptionTracer.TraceError<bool, Exception>(0, Activity.TraceId, "Unhandled exception. Create watson report. Recycle process = {0}.\r\n{1}", isFatal, exception);
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogFailure(ProtocolLogFailureLevel.Watson, "fault", RopId.None, exception);
		}

		public static void LogWarning(string message, params object[] args)
		{
			if (!ProtocolLog.IsLogEnabled || (ProtocolLog.instance.EnabledTags & ProtocolLoggingTag.Warnings) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogWarning(string.Format(message, args));
		}

		public static void LogThrottlingStatistics(float lowestBudgetBalance, uint throttleCount)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogThrottlingStatistics(lowestBudgetBalance, throttleCount);
		}

		public static void LogThrottlingSnapshot(IBudget budgetToLog)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogThrottlingSnapshot(budgetToLog);
		}

		public static void LogMicroDelay(DelayEnforcementResults delayinfo)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogMicroDelay(delayinfo);
		}

		public static void LogCriticalResourceHealth(ResourceUnhealthyException unhealthyException)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogCriticalResourceHealth(unhealthyException.ResourceKey.ToString());
		}

		public static void LogThrottlingOverBudget(string policy, int backoffTime)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogThrottlingOverBudget(policy, backoffTime);
		}

		public static void LogThrottlingConnectionLimitHit()
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogThrottlingConnectionLimitHit();
		}

		public static void LogConnectionRpcProcessingTime()
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogConnectionRpcProcessingTime();
		}

		public static void UpdateClientRpcLatency(TimeSpan clientLatency)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalUpdateClientRpcLatency(clientLatency);
		}

		public static void UpdateClientRpcFailureData(ExDateTime timeStamp, FailureCounterData failureCounterData)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalUpdateClientRpcFailureData(timeStamp, failureCounterData);
		}

		public static void UpdateClientRpcAttemptsData(ExDateTime timeStamp, IRpcCounterData attemptedCounterData)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalUpdateClientRpcAttemptsData(timeStamp, attemptedCounterData);
		}

		public static void UpdateMailboxServerRpcProcessingTime(TimeSpan serverLatency)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalUpdateMailboxServerRpcProcessingTime(serverLatency);
		}

		public static void LogData(bool flushRow, string value)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogData(flushRow, value);
		}

		public static void LogData<TArg0>(bool flushRow, string format, TArg0 arg0)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogData<TArg0>(flushRow, format, arg0);
		}

		public static void LogData<TArg0, TArg1>(bool flushRow, string format, TArg0 arg0, TArg1 arg1)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogData<TArg0, TArg1>(flushRow, format, arg0, arg1);
		}

		public static void LogData<TArg0, TArg1, TArg2>(bool flushRow, string format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			if (!ProtocolLog.IsLogEnabled)
			{
				return;
			}
			ProtocolLog.instance.InternalLogData<TArg0, TArg1, TArg2>(flushRow, format, arg0, arg1, arg2);
		}

		[Conditional("DEBUG")]
		private static void ValidateFieldDefinition()
		{
			for (int i = 0; i < ProtocolLog.Fields.Length; i++)
			{
				ProtocolLog.Field field = ProtocolLog.Fields[i].Field;
			}
		}

		private static string[] GetColumnArray()
		{
			string[] array = new string[ProtocolLog.Fields.Length];
			for (int i = 0; i < ProtocolLog.Fields.Length; i++)
			{
				array[i] = ProtocolLog.Fields[i].ColumnName;
			}
			return array;
		}

		private static bool IsLogEnabled
		{
			get
			{
				return ProtocolLog.instance != null && ProtocolLog.instance.isEnabled && Activity.Current != null;
			}
		}

		private static bool IsRepeatingBackoffException(Exception ex)
		{
			while (ex != null)
			{
				ClientBackoffException ex2 = ex as ClientBackoffException;
				if (ex2 != null && ex2.IsRepeatingBackoff)
				{
					return true;
				}
				ex = ex.InnerException;
			}
			return false;
		}

		private static void LogProtocolFailure(ProtocolLogFailureLevel protocolFailureLevel, IList<string> requestIds, IList<string> cookies, string status, string message, Exception exception, string emailAddress, string organizationInfo, string protocolSequence, string clientAddress, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			trace.TraceError<string, string, string>(0L, "[{0}] {1} | {2}", status, message ?? "(no message)", (exception != null) ? exception.Message : "(no exception)");
			if (ProtocolLog.instance == null || !ProtocolLog.instance.isEnabled)
			{
				return;
			}
			if ((ProtocolLog.instance.EnabledTags & ProtocolLoggingTag.Failures) == ProtocolLoggingTag.Failures)
			{
				ProtocolLogSession protocolLogSession = ProtocolLog.CreateNewSession(true);
				if (protocolLogSession != null)
				{
					try
					{
						IPAddress clientIpAddress = null;
						if (!string.IsNullOrEmpty(clientAddress) && !IPAddress.TryParse(clientAddress, out clientIpAddress))
						{
							clientIpAddress = null;
						}
						protocolLogSession.OnActivityResume();
						protocolLogSession.SetConnectionParameters(emailAddress, clientIpAddress, protocolSequence);
						protocolLogSession.SetOrganizationInfo(organizationInfo);
						protocolLogSession.SetHttpParameters(cookies, requestIds);
						protocolLogSession.LogFailure(protocolFailureLevel, status, RopId.None, exception, message);
					}
					finally
					{
						protocolLogSession.OnActivityPause();
					}
				}
			}
		}

		private ProtocolLoggingTag EnabledTags
		{
			get
			{
				return Configuration.ProtocolLogConfiguration.EnabledTags;
			}
		}

		private ProtocolLogSession InternalCreateNewSession(bool lazyInitializeRpcTimers)
		{
			return new ProtocolLogSession(this, lazyInitializeRpcTimers);
		}

		private void InternalSetConnectionParameters(int connectionId, string user, MapiVersion version, IPAddress clientIpAddress, IPAddress serverIpAddress, string protocolSequence)
		{
			if (this.EnabledTags == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetConnectionParameters(connectionId, user, version, clientIpAddress, serverIpAddress, protocolSequence);
		}

		private void InternalSetHttpParameters(IList<string> sessionCookies, IList<string> requestIds)
		{
			if (this.EnabledTags == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetHttpParameters(sessionCookies, requestIds);
		}

		private void InternalSetApplicationParameters(ClientMode clientMode, string clientProcessName)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.ApplicationData) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetApplicationParameters(clientMode, clientProcessName);
		}

		private void InternalSetOrganizationInfo(string organizationInfo)
		{
			if (this.EnabledTags == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetOrganizationInfo(organizationInfo);
		}

		private void InternalSetClientConnectionInfo(string connectionInfo)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.ConnectDisconnect) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetClientConnectionInfo(connectionInfo);
		}

		private void InternalSetClientIpAddress(IPAddress clientIpAddress)
		{
			if (this.EnabledTags == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetClientIpAddress(clientIpAddress);
		}

		private void InternalSetActivityData()
		{
			if ((this.EnabledTags & ProtocolLoggingTag.ConnectDisconnect) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.SetActivityData();
		}

		private void InternalLogConnect(SecurityIdentifier sid, ConnectionFlags connectionFlags)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.ConnectDisconnect) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogConnect(sid, connectionFlags);
		}

		private void InternalLogDisconnect(DisconnectReason disconnectReason)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.ConnectDisconnect) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogDisconnect(disconnectReason);
		}

		private void InternalLogLogon(ProtocolLogLogonType logonType, string operationSpecificData, string applicationId)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Logon) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogLogonPending(logonType, applicationId);
			Activity.Current.ProtocolLogSession.LogOperationSpecificData(false, operationSpecificData);
		}

		private void InternalLogLogonSuccess(int logonId)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Logon) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogLogonSuccess(logonId);
		}

		private void InternalLogLogonRedirect(string reason, string suggestedNewServer)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Logon) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogLogonRedirect(reason, suggestedNewServer);
		}

		private void InternalLogLogoff(ProtocolLogLogonType logonType, int logonId)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Logon) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogLogoff(logonType, logonId);
		}

		private void InternalLogNewCall()
		{
			Activity.Current.ProtocolLogSession.LogNewCall();
		}

		private void InternalLogInputRops(IEnumerable<RopId> rops)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Rops) == ProtocolLoggingTag.None)
			{
				return;
			}
			foreach (RopId ropId in rops)
			{
				Activity.Current.ProtocolLogSession.LogInputRop(ropId);
			}
		}

		private void InternalLogOutputRop(RopId ropId, ErrorCode errorCode)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Rops) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogOutputRop(ropId, errorCode);
		}

		private void InternalLogFailure(ProtocolLogFailureLevel failureLevel, string status, RopId ropId, Exception exception)
		{
			if (ProtocolLog.IsRepeatingBackoffException(exception))
			{
				Activity.Current.ProtocolLogSession.IgnorePendingData();
				return;
			}
			if ((this.EnabledTags & ProtocolLoggingTag.Failures) == ProtocolLoggingTag.Failures)
			{
				Activity.Current.ProtocolLogSession.LogFailure(failureLevel, status, ropId, exception);
			}
		}

		private void InternalLogThrottlingStatistics(float lowestBudgetBalance, uint throttleCount)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None)
			{
				return;
			}
			string text = string.Format("Lowest Budget Balance: {0},Session Throttled Count = {1}", lowestBudgetBalance, throttleCount);
			Activity.Current.ProtocolLogSession.LogThrottling(text);
		}

		private void InternalLogThrottlingOverBudget(string policyPartViolated, int backoffTime)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None)
			{
				return;
			}
			string text = string.Format("Over budget for: {0}  Backoff time: {1}", policyPartViolated, backoffTime);
			Activity.Current.ProtocolLogSession.LogThrottling(text);
		}

		private void InternalLogCriticalResourceHealth(string resourceIdentity)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None)
			{
				return;
			}
			string text = string.Format("Critical resource={0}", resourceIdentity);
			Activity.Current.ProtocolLogSession.LogThrottling(text);
		}

		private void InternalLogThrottlingSnapshot(IBudget budgetToLog)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None || budgetToLog == null)
			{
				return;
			}
			string text = string.Format("BS={0}", budgetToLog);
			Activity.Current.ProtocolLogSession.LogThrottling(text);
		}

		private void InternalLogMicroDelay(DelayEnforcementResults delayInfo)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None || delayInfo.DelayInfo == DelayInfo.NoDelay)
			{
				return;
			}
			string text;
			if (!string.IsNullOrEmpty(delayInfo.NotEnforcedReason))
			{
				text = string.Format("NotEnforcedMicroDelay:{0}, NotEnforcedReason:{1}", delayInfo.DelayInfo.Delay, delayInfo.NotEnforcedReason);
			}
			else
			{
				text = string.Format("MicroDelay:{0}", delayInfo.DelayedAmount);
			}
			Activity.Current.ProtocolLogSession.LogThrottling(text);
		}

		private void InternalLogThrottlingConnectionLimitHit()
		{
			if ((this.EnabledTags & ProtocolLoggingTag.Throttling) == ProtocolLoggingTag.None)
			{
				return;
			}
			Activity.Current.ProtocolLogSession.LogThrottling("Connection Limit Exceeded");
		}

		private void InternalLogConnectionRpcProcessingTime()
		{
			Activity.Current.ProtocolLogSession.LogConnectionRpcProcessingTime();
		}

		private void InternalUpdateClientRpcLatency(TimeSpan clientLatency)
		{
			Activity.Current.ProtocolLogSession.UpdateClientRpcLatency(clientLatency);
		}

		private void InternalUpdateClientRpcFailureData(ExDateTime timeStamp, FailureCounterData failureCounterData)
		{
			Activity.Current.ProtocolLogSession.UpdateClientRpcFailureData(timeStamp, failureCounterData);
		}

		private void InternalUpdateClientRpcAttemptsData(ExDateTime timeStamp, IRpcCounterData attemptedCounterData)
		{
			Activity.Current.ProtocolLogSession.UpdateClientRpcAttemptsData(timeStamp, attemptedCounterData);
		}

		private void InternalUpdateMailboxServerRpcProcessingTime(TimeSpan serverLatency)
		{
			Activity.Current.ProtocolLogSession.UpdateMailboxServerRpcProcessingTime(serverLatency);
		}

		private void InternalLogData(bool flushRow, string format)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.OperationSpecific) == ProtocolLoggingTag.None)
			{
				return;
			}
			this.LogOperationSpecificData(flushRow, format);
		}

		private void InternalLogData<TArg0>(bool flushRow, string format, TArg0 arg0)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.OperationSpecific) == ProtocolLoggingTag.None)
			{
				return;
			}
			this.LogOperationSpecificData(flushRow, string.Format(format, arg0));
		}

		private void InternalLogData<TArg0, TArg1>(bool flushRow, string format, TArg0 arg0, TArg1 arg1)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.OperationSpecific) == ProtocolLoggingTag.None)
			{
				return;
			}
			this.LogOperationSpecificData(flushRow, string.Format(format, arg0, arg1));
		}

		private void InternalLogData<TArg0, TArg1, TArg2>(bool flushRow, string format, TArg0 arg0, TArg1 arg1, TArg2 arg2)
		{
			if ((this.EnabledTags & ProtocolLoggingTag.OperationSpecific) == ProtocolLoggingTag.None)
			{
				return;
			}
			this.LogOperationSpecificData(flushRow, string.Format(format, arg0, arg1, arg2));
		}

		private void LogOperationSpecificData(bool flushRow, string data)
		{
			Activity.Current.ProtocolLogSession.LogOperationSpecificData(flushRow, data);
		}

		private void Refresh()
		{
			ProtocolLogConfiguration protocolLogConfiguration = Configuration.ProtocolLogConfiguration;
			this.isEnabled = protocolLogConfiguration.IsEnabled;
			this.log.Configure(protocolLogConfiguration.LogFilePath, protocolLogConfiguration.AgeQuota, protocolLogConfiguration.DirectorySizeQuota, protocolLogConfiguration.PerFileSizeQuota, protocolLogConfiguration.ApplyHourPrecision);
		}

		private void Close()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
			this.isEnabled = false;
		}

		private readonly Log log;

		private readonly LogSchema schema;

		internal static readonly ProtocolLog.FieldInfo[] Fields = new ProtocolLog.FieldInfo[]
		{
			new ProtocolLog.FieldInfo(ProtocolLog.Field.DateTime, "date-time", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.SessionId, "session-id", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.SequenceNumber, "seq-number", ProtocolLog.FieldScope.Call),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientName, "client-name", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.OrganizationInfo, "organization-info", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientSoftware, "client-software", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientSoftwareVersion, "client-software-version", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientMode, "client-mode", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientIpAddress, "client-ip", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ClientConnectionInfo, "client-connection-info", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ServerIpAddress, "server-ip", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Protocol, "protocol", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ApplicationId, "application-id", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.RequestIds, "request-ids", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.SessionCookie, "session-cookie", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Operation, "operation", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Status, "rpc-status", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ProcessingTime, "processing-time", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.OperationSpecificData, "operation-specific", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Failures, "failures", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.PerformanceData, "performance-data", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.ActivityContextData, "activity-context-data", ProtocolLog.FieldScope.Record),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.UserEmail, "user-email", ProtocolLog.FieldScope.Session),
			new ProtocolLog.FieldInfo(ProtocolLog.Field.Puid, "passport-unique-id", ProtocolLog.FieldScope.Session)
		};

		private static ProtocolLog instance;

		private bool isEnabled;

		internal struct FieldInfo
		{
			public FieldInfo(ProtocolLog.Field field, string columnName, ProtocolLog.FieldScope scope)
			{
				this.Field = field;
				this.ColumnName = columnName;
				this.Scope = scope;
			}

			public readonly ProtocolLog.Field Field;

			public readonly string ColumnName;

			public readonly ProtocolLog.FieldScope Scope;
		}

		internal enum Field
		{
			DateTime,
			SessionId,
			SequenceNumber,
			ClientName,
			OrganizationInfo,
			ClientSoftware,
			ClientSoftwareVersion,
			ClientMode,
			ClientIpAddress,
			ClientConnectionInfo,
			ServerIpAddress,
			Protocol,
			ApplicationId,
			RequestIds,
			SessionCookie,
			Operation,
			Status,
			ProcessingTime,
			OperationSpecificData,
			Failures,
			PerformanceData,
			ActivityContextData,
			UserEmail,
			Puid,
			NumberOfFields
		}

		internal enum FieldScope
		{
			Session,
			Call,
			Record
		}
	}
}
