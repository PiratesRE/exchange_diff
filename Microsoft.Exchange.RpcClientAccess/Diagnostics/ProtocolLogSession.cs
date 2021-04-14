using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Messages;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Mapi;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal class ProtocolLogSession
	{
		internal ProtocolLogSession(ProtocolLog protocolLog) : this(protocolLog, false)
		{
		}

		internal ProtocolLogSession(ProtocolLog protocolLog, bool lazyInitializeRpcTimers)
		{
			this.protocolLog = protocolLog;
			this.contextValues = new string[24];
			if (!lazyInitializeRpcTimers)
			{
				this.connectionProcessingTime = new ProtocolLogSession.RpcProcessingTime();
				this.failureData = new ProtocolLogSession.RpcFailureData();
			}
		}

		internal static BatchingUploader<MoMTRawData> StreamInsightUploader
		{
			get
			{
				if (ProtocolLogSession.uploader == null)
				{
					DataContractSerializerEncoder<MoMTRawData> dataContractSerializerEncoder = new DataContractSerializerEncoder<MoMTRawData>();
					ProtocolLogSession.uploader = new BatchingUploader<MoMTRawData>(dataContractSerializerEncoder, ServiceConfiguration.StreamInsightEngineURI, ServiceConfiguration.StreamInsightUploaderQueueSize, TimeSpan.FromSeconds(30.0), 1000, 1, 3, true, "", false, null, null, null, null, false);
				}
				return ProtocolLogSession.uploader;
			}
		}

		public static string GenerateActivityScopeReport(bool statisticsOnly = false)
		{
			ReferencedActivityScope referencedActivityScope = ReferencedActivityScope.Current;
			if (referencedActivityScope == null)
			{
				return string.Empty;
			}
			return LogRowFormatter.FormatCollection(statisticsOnly ? referencedActivityScope.ActivityScope.GetFormattableStatistics() : referencedActivityScope.ActivityScope.GetFormattableMetadata().Concat(referencedActivityScope.ActivityScope.GetFormattableStatistics()));
		}

		internal static string GenerateLogonOperationSpecificData(ProtocolLogLogonType logonType, ExchangePrincipal targetMailbox)
		{
			object obj = (targetMailbox.MailboxInfo.MailboxDatabase != null) ? targetMailbox.MailboxInfo.MailboxDatabase.Name : null;
			string legacyDn = targetMailbox.LegacyDn;
			IMailboxLocation location = targetMailbox.MailboxInfo.Location;
			if (location != null)
			{
				return string.Format("Logon: {0}, {1} in database {2} last mounted on {3}", new object[]
				{
					logonType,
					legacyDn,
					obj,
					location.ServerFqdn
				});
			}
			return string.Format("Logon: {0}{1}, {2} in database {3}", new object[]
			{
				logonType,
				targetMailbox.MailboxInfo.IsRemote ? "-Remote" : string.Empty,
				legacyDn,
				obj
			});
		}

		internal void IgnorePendingData()
		{
			ProtocolLogSession.tlsData.ClearPerRecordData();
		}

		internal void OnClientActivityResume()
		{
			this.OnActivityResume();
		}

		internal void OnClientActivityPause()
		{
			this.ConnectionProcessingTime.UpdateClientAccessServerRpcProcessingTime(ProtocolLogSession.tlsData.ElapsedTime);
			this.OnActivityPause();
		}

		internal void OnActivityResume()
		{
			if (ProtocolLogSession.tlsData == null)
			{
				ProtocolLogSession.tlsData = new ProtocolLogSession.ThreadLocalData(this.protocolLog);
			}
			ProtocolLogSession.tlsData.ClearPendingData();
			ProtocolLogSession.tlsData.CurrentCallStartTickCount = new int?(Environment.TickCount);
		}

		internal void OnActivityPause()
		{
			this.FlushRow();
			this.ClearPersistentData();
			ProtocolLogSession.tlsData.ClearPendingData();
		}

		internal void SetConnectionParameters(int connectionId, string user, MapiVersion version, IPAddress clientIpAddress, IPAddress serverIpAddress, string protocolSequence)
		{
			this.SetColumn(ProtocolLog.Field.SessionId, connectionId.ToString());
			this.SetColumn(ProtocolLog.Field.ClientName, user);
			this.SetColumn(ProtocolLog.Field.UserEmail, this.GetUserEmail());
			this.SetColumn(ProtocolLog.Field.Puid, this.GetPuid());
			this.SetColumn(ProtocolLog.Field.ClientSoftwareVersion, version.ToString());
			this.SetColumn(ProtocolLog.Field.ClientIpAddress, clientIpAddress.ToString());
			this.SetColumn(ProtocolLog.Field.ServerIpAddress, serverIpAddress.ToString());
			this.SetColumn(ProtocolLog.Field.Protocol, protocolSequence);
		}

		internal void SetHttpParameters(IList<string> sessionCookies, IList<string> requestIds)
		{
			if (sessionCookies != null)
			{
				string value = string.Join("|", sessionCookies);
				ProtocolLogSession.tlsData.PersistentData[ProtocolLog.Field.SessionCookie] = value;
			}
			if (requestIds != null)
			{
				string value2 = string.Join("|", requestIds);
				ProtocolLogSession.tlsData.PersistentData[ProtocolLog.Field.RequestIds] = value2;
			}
		}

		internal void SetConnectionParameters(string user, IPAddress clientIpAddress, string protocolSequence)
		{
			this.SetColumn(ProtocolLog.Field.ClientName, user);
			this.SetColumn(ProtocolLog.Field.Protocol, protocolSequence);
			if (clientIpAddress != null)
			{
				this.SetColumn(ProtocolLog.Field.ClientIpAddress, clientIpAddress.ToString());
			}
		}

		internal void SetClientIpAddress(IPAddress clientIpAddress)
		{
			if (clientIpAddress != null)
			{
				this.SetColumn(ProtocolLog.Field.ClientIpAddress, clientIpAddress.ToString(), false);
			}
		}

		internal void SetApplicationParameters(ClientMode clientMode, string clientProcessName)
		{
			this.SetColumn(ProtocolLog.Field.ClientMode, ProtocolLogSession.clientModeFormatter.Format(clientMode));
			this.SetColumn(ProtocolLog.Field.ClientSoftware, clientProcessName);
		}

		internal void SetOrganizationInfo(string organizationInfo)
		{
			this.organizationInfo = organizationInfo;
			this.SetColumn(ProtocolLog.Field.OrganizationInfo, organizationInfo);
		}

		internal void SetClientConnectionInfo(string connectionInfo)
		{
			this.SetColumn(ProtocolLog.Field.ClientConnectionInfo, connectionInfo);
		}

		internal void SetActivityData()
		{
			this.SetColumn(ProtocolLog.Field.ActivityContextData, ProtocolLogSession.GenerateActivityScopeReport(false));
		}

		internal void LogConnect(SecurityIdentifier sid, ConnectionFlags connectionFlags)
		{
			this.LogOperationPending(ProtocolLogSession.OperationConnectEx);
			this.LogOperationSpecificData(false, string.Format("SID={0}, Flags={1}", sid, connectionFlags));
			this.startTickCount = Environment.TickCount;
		}

		internal void LogDisconnect(DisconnectReason disconnectReason)
		{
			switch (disconnectReason)
			{
			case DisconnectReason.ClientDisconnect:
				this.LogOperationPending(ProtocolLogSession.OperationDisconnect);
				break;
			case DisconnectReason.ServerDropped:
				this.LogOperationSpecificData(false, ProtocolLogSession.operationDataSessionDropped);
				break;
			case DisconnectReason.NetworkRundown:
				this.LogOperationPending(ProtocolLogSession.OperationNetworkRundown);
				break;
			default:
				throw new InvalidOperationException(string.Format("Invalid DisconnectReason; disconnectReason={0}", disconnectReason));
			}
			this.SetColumn(ProtocolLog.Field.PerformanceData, string.Format("{0};{1}", this.ConnectionProcessingTime.DetachRpcProcessingTimeLogLine(), this.FailureData.DetachClientRpcStatisticsLogLine()));
			this.SetColumn(ProtocolLog.Field.ActivityContextData, ProtocolLogSession.GenerateActivityScopeReport(false));
			this.SetColumn(ProtocolLog.Field.OrganizationInfo, this.organizationInfo);
			ProtocolLogSession.tlsData.CurrentCallStartTickCount = new int?(this.startTickCount);
		}

		internal void LogConnectionRpcProcessingTime()
		{
			if (ProtocolLogSession.tlsData == null)
			{
				ProtocolLogSession.tlsData = new ProtocolLogSession.ThreadLocalData(this.protocolLog);
			}
			this.SetColumn(ProtocolLog.Field.Operation, ProtocolLogSession.OperationSystemLogging);
			this.SetColumn(ProtocolLog.Field.PerformanceData, string.Format("{0};{1}", this.ConnectionProcessingTime.DetachRpcProcessingTimeLogLine(), this.FailureData.DetachClientRpcStatisticsLogLine()));
			this.SetColumn(ProtocolLog.Field.ActivityContextData, ProtocolLogSession.GenerateActivityScopeReport(false));
			this.SetColumn(ProtocolLog.Field.OrganizationInfo, this.organizationInfo);
			this.FlushRow();
		}

		internal void UpdateClientRpcLatency(TimeSpan clientLatency)
		{
			this.ConnectionProcessingTime.UpdateClientRpcLatency(clientLatency);
		}

		internal void UpdateClientRpcFailureData(ExDateTime timeStamp, FailureCounterData failureCounterData)
		{
			this.FailureData.UpdateClientRpcFailureData(timeStamp, failureCounterData);
		}

		internal void UpdateClientRpcAttemptsData(ExDateTime timeStamp, IRpcCounterData attemptedCounterData)
		{
			this.FailureData.UpdateClientRpcAttemptsData(timeStamp, attemptedCounterData);
		}

		internal void UpdateMailboxServerRpcProcessingTime(TimeSpan serverLatency)
		{
			this.connectionProcessingTime.UpdateMailboxServerRpcProcessingTime(serverLatency);
		}

		internal void LogLogonPending(ProtocolLogLogonType logonType, string applicationId)
		{
			this.LogOperationPending(logonType.ToString() + ProtocolLogSession.OperationSuffixLogon);
			this.SetColumn(ProtocolLog.Field.ApplicationId, applicationId);
		}

		internal void LogLogonSuccess(int logonId)
		{
			this.LogLogonId(logonId);
			this.FlushRow();
			this.ClearPersistentData();
		}

		internal void LogLogonRedirect(string reason, string suggestedNewServer)
		{
			this.LogOperationSpecificData(false, string.Format("Redirected: {0}, suggested new server: {1}", reason, suggestedNewServer));
		}

		internal void LogLogoff(ProtocolLogLogonType logonType, int logonId)
		{
			this.LogOperationPending(logonType.ToString() + ProtocolLogSession.OperationSuffixLogoff);
			this.LogLogonId(logonId);
			this.FlushRow();
		}

		internal void LogNewCall()
		{
			this.SetColumn(ProtocolLog.Field.Status, "0", false);
			this.SetColumn(ProtocolLog.Field.SequenceNumber, (Interlocked.Increment(ref this.rpcRequestCount) - 1).ToString(), false);
		}

		internal void LogInputRop(RopId ropId)
		{
			this.LogOperationSpecificData(false, ProtocolLogSession.inputRopFormatter.Format(ropId));
		}

		internal void LogOutputRop(RopId ropId, ErrorCode errorCode)
		{
			if (errorCode == ErrorCode.None)
			{
				this.LogOperationSpecificData(false, ProtocolLogSession.outputRopFormatter.Format(ropId));
				return;
			}
			this.LogOperationSpecificData(false, string.Format(ProtocolLogSession.outputRopWithErrorFormatter.Format(ropId), errorCode));
		}

		internal void LogFailure(ProtocolLogFailureLevel failureLevel, string status, RopId ropId, Exception exception)
		{
			this.LogFailure(failureLevel, status, ropId, exception, null);
		}

		internal void LogFailure(ProtocolLogFailureLevel failureLevel, string status, RopId ropId, Exception exception, string message)
		{
			this.SetColumn(ProtocolLog.Field.Status, status);
			StringBuilder stringBuilder = new StringBuilder(ProtocolLogSession.logFailureLevelFormatter.Format(failureLevel));
			stringBuilder.Append(": ");
			if (!string.IsNullOrEmpty(message))
			{
				stringBuilder.Append("[");
				stringBuilder.Append(message);
				stringBuilder.Append("] ");
			}
			if (failureLevel == ProtocolLogFailureLevel.RopHandler)
			{
				stringBuilder.Append(ProtocolLogSession.genericRopFormatter.Format(ropId));
				stringBuilder.Append(": ");
			}
			string text = null;
			bool flag = false;
			Exception ex = exception;
			while (exception != null)
			{
				if (ex != exception)
				{
					stringBuilder.Append(" -> ");
				}
				string text2 = exception.Message;
				string text3 = null;
				MapiRetryableException ex2 = exception as MapiRetryableException;
				MapiPermanentException ex3 = exception as MapiPermanentException;
				if (text2 != null && (ex2 != null || ex3 != null))
				{
					string text4 = exception.GetType().Name + ": ";
					int num = text2.StartsWith(text4) ? text4.Length : 0;
					int num2 = text2.IndexOf(DiagnosticContext.MessageHeader);
					if (num != 0 || num2 != -1)
					{
						if (num2 == -1)
						{
							text2 = text2.Substring(num);
						}
						else
						{
							text2 = text2.Substring(num, num2 - num);
						}
					}
					if (ex2 != null && ex2.DiagCtx != null)
					{
						text3 = Convert.ToBase64String(ex2.DiagCtx.ToByteArray());
					}
					else if (ex3 != null && ex3.DiagCtx != null)
					{
						text3 = Convert.ToBase64String(ex3.DiagCtx.ToByteArray());
					}
				}
				stringBuilder.AppendFormat("[{0}] {1}", exception.GetType().Name, text2);
				if (text3 != null)
				{
					stringBuilder.AppendFormat(" [diag::{0}]", text3);
				}
				if (!flag)
				{
					flag = true;
					if (!ProtocolLogSession.IsCommonlyKnownException(exception))
					{
						string stackTrace = exception.StackTrace;
						if (!string.IsNullOrEmpty(stackTrace))
						{
							text = stackTrace;
						}
					}
				}
				exception = exception.InnerException;
			}
			stringBuilder.Replace('\r', ' ').Replace('\n', ' ');
			if (!string.IsNullOrEmpty(text))
			{
				text = ProtocolLogSession.AbbreviateStackTrace(text);
				int num3 = 1500 - stringBuilder.Length - 1;
				if (text.Length < num3)
				{
					stringBuilder.Append(text);
				}
				else if (num3 > 0)
				{
					stringBuilder.Append(text.Substring(0, num3));
				}
			}
			this.SetColumn(ProtocolLog.Field.Failures, stringBuilder.ToString());
			this.FlushRow();
			this.ClearPersistentData();
		}

		private static string AbbreviateStackTrace(string fullStackTrace)
		{
			string text = fullStackTrace;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Replace('\r', ' ').Replace('\n', ' ');
				text = text.Replace("\"", "'");
				text = text.Replace("Microsoft.", "M.");
				text = text.Replace(".Exchange.", ".E.");
				text = text.Replace(".RpcClientAccess.", ".R.");
				text = text.Replace(".Data.", ".D.");
				text = text.Replace(".Directory.", ".Dir.");
				text = text.Replace(".Storage.", ".S.");
				text = text.Replace(".Handler.", ".H.");
				text = text.Replace(".Parser.", ".P.");
				text = text.Replace(".Common.", ".C.");
			}
			return text;
		}

		private static bool IsCommonlyKnownException(Exception exception)
		{
			while (exception != null)
			{
				if (ProtocolLogSession.KnownExceptions.Contains(exception.GetType()))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(exception.Message))
				{
					if (ProtocolLogSession.KnownErrors.Any((string str) => exception.Message.Contains(str)))
					{
						return true;
					}
				}
				exception = exception.InnerException;
			}
			return false;
		}

		internal void LogThrottling(string text)
		{
			this.LogOperationSpecificData(false, text);
		}

		internal void LogWarning(string text)
		{
			this.LogOperationSpecificData(false, text);
		}

		internal void LogOperationSpecificData(bool flushRow, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				ProtocolLogSession.tlsData.HasInterestingDataToFlush = true;
				ProtocolLogSession.tlsData.OperationSpecificData.Append(value);
			}
			if (flushRow)
			{
				this.FlushRow();
			}
		}

		private void SetColumn(ProtocolLog.Field field, string value)
		{
			this.SetColumn(field, value, true);
		}

		private void SetColumn(ProtocolLog.Field field, string value, bool markRowAsDirty)
		{
			if (markRowAsDirty)
			{
				ProtocolLogSession.tlsData.HasInterestingDataToFlush = true;
			}
			if (ProtocolLog.Fields[(int)field].Scope == ProtocolLog.FieldScope.Session)
			{
				lock (this.contextValues)
				{
					this.contextValues[(int)field] = value;
					return;
				}
			}
			ProtocolLogSession.tlsData.Row[(int)field] = value;
		}

		private void LogLogonId(int logonId)
		{
			this.LogOperationSpecificData(false, string.Format("LogonId: {0}", logonId));
		}

		private void LogOperationPending(string operation)
		{
			this.FlushRow();
			this.SetColumn(ProtocolLog.Field.Operation, operation);
			this.SetColumn(ProtocolLog.Field.Status, "0", false);
		}

		private void FlushRow()
		{
			if (ProtocolLogSession.tlsData.HasInterestingDataToFlush)
			{
				foreach (KeyValuePair<ProtocolLog.Field, string> keyValuePair in ProtocolLogSession.tlsData.PersistentData)
				{
					this.SetColumn(keyValuePair.Key, keyValuePair.Value, false);
				}
				this.SetColumn(ProtocolLog.Field.OperationSpecificData, ProtocolLogSession.tlsData.OperationSpecificData.Detach());
				this.SetColumn(ProtocolLog.Field.ProcessingTime, ProtocolLogSession.tlsData.ElapsedTime.ToString());
				lock (this.contextValues)
				{
					for (int i = 0; i < this.contextValues.Length; i++)
					{
						if (this.contextValues[i] != null)
						{
							ProtocolLogSession.tlsData.Row[i] = this.contextValues[i];
						}
					}
				}
				this.protocolLog.Append(ProtocolLogSession.tlsData.Row);
			}
			if (ServiceConfiguration.StreamInsightUploaderEnabled)
			{
				object obj2 = ProtocolLogSession.tlsData.Row[15];
				object obj3 = ProtocolLogSession.tlsData.Row[5];
				if (obj2 != null && obj3 != null && ((string)obj2).Equals("OwnerLogon", StringComparison.OrdinalIgnoreCase) && !((string)obj3).Equals("Microsoft.Exchange.RpcClientAccess.Monitoring.dll", StringComparison.OrdinalIgnoreCase))
				{
					this.UploadToStreamInsight();
				}
			}
			ProtocolLogSession.tlsData.ClearPerRecordData();
		}

		private void ClearPersistentData()
		{
			ProtocolLogSession.tlsData.PersistentData.Clear();
		}

		private void PopulateStreamInsightDataObject(out MoMTRawData momtRawData)
		{
			momtRawData = new MoMTRawData();
			momtRawData.DateTimeUtc = ((DateTime)ProtocolLogSession.tlsData.Row[0]).ToString("o");
			momtRawData.ClientName = this.contextValues[3];
			momtRawData.OrganizationInfo = this.organizationInfo;
			momtRawData.Failures = (string)ProtocolLogSession.tlsData.Row[19];
		}

		private void UploadToStreamInsight()
		{
			try
			{
				MoMTRawData moMTRawData;
				this.PopulateStreamInsightDataObject(out moMTRawData);
				if (!ProtocolLogSession.StreamInsightUploader.TryEnqueueItem(moMTRawData))
				{
					this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_StreamInsightsDataUploadFailed, "RpcClientAccess_StreamInsightsDataUploadFailed", new object[]
					{
						ServiceConfiguration.Component,
						ServiceConfiguration.StreamInsightEngineURI
					});
				}
			}
			catch (Exception ex)
			{
				this.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_StreamInsightsDataUploadExceptionThrown, "RpcClientAccess_StreamInsightsDataUploadExceptionThrown", new object[]
				{
					ServiceConfiguration.Component,
					ex.ToString()
				});
			}
		}

		private ProtocolLogSession.RpcProcessingTime ConnectionProcessingTime
		{
			get
			{
				if (this.connectionProcessingTime == null)
				{
					this.connectionProcessingTime = new ProtocolLogSession.RpcProcessingTime();
				}
				return this.connectionProcessingTime;
			}
		}

		private string GetUserEmail()
		{
			ReferencedActivityScope referencedActivityScope = ReferencedActivityScope.Current;
			if (referencedActivityScope == null)
			{
				return string.Empty;
			}
			return referencedActivityScope.UserEmail;
		}

		private string GetPuid()
		{
			ReferencedActivityScope referencedActivityScope = ReferencedActivityScope.Current;
			if (referencedActivityScope == null || referencedActivityScope.Puid == null)
			{
				return string.Empty;
			}
			return referencedActivityScope.Puid;
		}

		private ProtocolLogSession.RpcFailureData FailureData
		{
			get
			{
				if (this.failureData == null)
				{
					this.failureData = new ProtocolLogSession.RpcFailureData();
				}
				return this.failureData;
			}
		}

		private const int LogFailureMaxLength = 1500;

		internal static readonly string OperationConnectEx = "Connect";

		internal static readonly string OperationDisconnect = "Disconnect";

		internal static readonly string OperationSuffixLogon = "Logon";

		internal static readonly string OperationSuffixLogoff = "Logoff";

		internal static readonly string OperationSystemLogging = "SystemLogging";

		internal static readonly string OperationNetworkRundown = "NetworkRundown";

		private static readonly string operationDataSessionDropped = "SessionDropped";

		private static readonly EnumFormatter<RopId> genericRopFormatter = new EnumFormatter<RopId>("{0}", (RopId v) => (int)v);

		private static readonly EnumFormatter<RopId> inputRopFormatter = new EnumFormatter<RopId>(">{0}", (RopId v) => (int)v);

		private static readonly EnumFormatter<RopId> outputRopFormatter = new EnumFormatter<RopId>("<{0}", (RopId v) => (int)v);

		private static readonly EnumFormatter<RopId> outputRopWithErrorFormatter = new EnumFormatter<RopId>("<{0}({{0}})", (RopId v) => (int)v);

		private static readonly EnumFormatter<ClientMode> clientModeFormatter = new EnumFormatter<ClientMode>("{0}", (ClientMode v) => (int)v);

		private static readonly EnumFormatter<ProtocolLogFailureLevel> logFailureLevelFormatter = new EnumFormatter<ProtocolLogFailureLevel>("{0}", (ProtocolLogFailureLevel v) => (int)v);

		private static readonly EnumFormatter<DatabaseLocationInfoResult> databaseLocationInfoFormatter = new EnumFormatter<DatabaseLocationInfoResult>("{0}", (DatabaseLocationInfoResult v) => (int)v).OverrideFormat(DatabaseLocationInfoResult.Success, "Mounted");

		private static BatchingUploader<MoMTRawData> uploader;

		[ThreadStatic]
		private static ProtocolLogSession.ThreadLocalData tlsData;

		private readonly ProtocolLog protocolLog;

		private readonly string[] contextValues;

		private int rpcRequestCount;

		private int startTickCount;

		private ProtocolLogSession.RpcProcessingTime connectionProcessingTime;

		private ProtocolLogSession.RpcFailureData failureData;

		private string organizationInfo = string.Empty;

		private ExEventLog eventLog = new ExEventLog(ServiceConfiguration.ComponentGuid, "MSExchangeRPC");

		private static readonly string[] KnownErrors = new string[]
		{
			ErrorCode.MdbOffline.ToString()
		};

		private static readonly Type[] KnownExceptions = new Type[]
		{
			typeof(MapiExceptionIllegalCrossServerConnection),
			typeof(WrongServerException),
			typeof(SessionDeadException)
		};

		internal class RpcFailureData
		{
			public RpcFailureData()
			{
				this.rpcFailureDataLock = new object();
				this.timeBucketedFailureCounters = new RpcTimeIntervalCounterGroups<RpcFailureCounters>();
				this.timeBucketedRpcAttemptedCounters = new RpcTimeIntervalCounterGroups<RpcAttemptedCounters>();
			}

			public void UpdateClientRpcFailureData(ExDateTime timeStamp, FailureCounterData failureCounterData)
			{
				lock (this.rpcFailureDataLock)
				{
					this.timeBucketedFailureCounters.IncrementCounter(timeStamp, failureCounterData);
				}
			}

			public void UpdateClientRpcAttemptsData(ExDateTime timeStamp, IRpcCounterData attemptedCounterData)
			{
				lock (this.rpcFailureDataLock)
				{
					this.timeBucketedRpcAttemptedCounters.IncrementCounter(timeStamp, attemptedCounterData);
				}
			}

			internal string DetachClientRpcStatisticsLogLine()
			{
				string result;
				lock (this.rpcFailureDataLock)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ExDateTime exDateTime in this.timeBucketedFailureCounters.GetTimeIntervals())
					{
						stringBuilder.AppendFormat("[{0}|{1};{2}]", exDateTime, this.timeBucketedRpcAttemptedCounters.GetFormattedCounterDataForInterval(exDateTime), this.timeBucketedFailureCounters.GetFormattedCounterDataForInterval(exDateTime));
					}
					this.Reset();
					result = string.Format("ClientRpcFailureData:{0}", stringBuilder);
				}
				return result;
			}

			private void Reset()
			{
				lock (this.rpcFailureDataLock)
				{
					this.timeBucketedFailureCounters.ResetCounters();
					this.timeBucketedRpcAttemptedCounters.ResetCounters();
				}
			}

			private const string LogLineFormat = "ClientRpcFailureData:{0}";

			private readonly RpcTimeIntervalCounterGroups<RpcFailureCounters> timeBucketedFailureCounters;

			private readonly RpcTimeIntervalCounterGroups<RpcAttemptedCounters> timeBucketedRpcAttemptedCounters;

			private readonly object rpcFailureDataLock;
		}

		internal class RpcProcessingTime
		{
			internal string DetachRpcProcessingTimeLogLine()
			{
				string result;
				lock (this.rpcProcessingTimeLock)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("ClientRPCCount={0};AvgClientLatency={1};ServerRPCCount={2};AvgCasRPCProcessingTime={3};AvgMbxProcessingTime={4};MaxCasRPCProcessingTime={5}", new object[]
					{
						this.clientRpcCount,
						(this.clientRpcCount > 1) ? TimeSpan.FromTicks(this.totalClientRPCLatency.Ticks / (long)this.clientRpcCount) : this.totalClientRPCLatency,
						this.serverRpcCount,
						(this.serverRpcCount > 1) ? TimeSpan.FromTicks(this.totalCasRPCProcessingTime.Ticks / (long)this.serverRpcCount) : this.totalCasRPCProcessingTime,
						(this.serverRpcCount > 1) ? TimeSpan.FromTicks(this.totalMbxRPCProcessingTime.Ticks / (long)this.serverRpcCount) : this.totalMbxRPCProcessingTime,
						this.maxRpcProcessingTime
					});
					StringBuilder stringBuilder2 = new StringBuilder();
					StringBuilder stringBuilder3 = new StringBuilder();
					StringBuilder stringBuilder4 = new StringBuilder();
					for (int i = 0; i < ProtocolLogSession.RpcProcessingTime.Percentiles.Length; i++)
					{
						stringBuilder2.AppendFormat(";{0}%ClientPercentile={1}", ProtocolLogSession.RpcProcessingTime.Percentiles[i], this.clientPercentileCounter.PercentileQuery(ProtocolLogSession.RpcProcessingTime.Percentiles[i]) + 10L);
						stringBuilder3.AppendFormat(";{0}%CasPercentile={1}", ProtocolLogSession.RpcProcessingTime.Percentiles[i], this.casPercentileCounter.PercentileQuery(ProtocolLogSession.RpcProcessingTime.Percentiles[i]) + 10L);
						stringBuilder4.AppendFormat(";{0}%MbxPercentile={1}", ProtocolLogSession.RpcProcessingTime.Percentiles[i], this.mbxPercentileCounter.PercentileQuery(ProtocolLogSession.RpcProcessingTime.Percentiles[i]) + 10L);
					}
					stringBuilder.AppendFormat("{0}{1}{2}", stringBuilder2.ToString(), stringBuilder3.ToString(), stringBuilder4.ToString());
					this.Reset();
					result = stringBuilder.ToString();
				}
				return result;
			}

			internal void UpdateClientAccessServerRpcProcessingTime(TimeSpan requestProcessingTime)
			{
				lock (this.rpcProcessingTimeLock)
				{
					if (requestProcessingTime > this.maxRpcProcessingTime)
					{
						this.maxRpcProcessingTime = requestProcessingTime;
					}
					this.totalCasRPCProcessingTime += requestProcessingTime;
					this.serverRpcCount++;
					this.casPercentileCounter.AddValue((long)requestProcessingTime.Milliseconds);
				}
			}

			internal void UpdateMailboxServerRpcProcessingTime(TimeSpan requestProcessingTime)
			{
				lock (this.rpcProcessingTimeLock)
				{
					this.totalMbxRPCProcessingTime += requestProcessingTime;
					this.mbxPercentileCounter.AddValue((long)requestProcessingTime.Milliseconds);
				}
			}

			internal void UpdateClientRpcLatency(TimeSpan clientLatency)
			{
				lock (this.rpcProcessingTimeLock)
				{
					this.totalClientRPCLatency += clientLatency;
					this.clientRpcCount++;
					this.clientPercentileCounter.AddValue((long)clientLatency.Milliseconds);
				}
			}

			private void Reset()
			{
				this.serverRpcCount = 0;
				this.clientRpcCount = 0;
				this.maxRpcProcessingTime = TimeSpan.Zero;
				this.totalClientRPCLatency = TimeSpan.Zero;
				this.totalCasRPCProcessingTime = TimeSpan.Zero;
				this.totalMbxRPCProcessingTime = TimeSpan.Zero;
				this.clientPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);
				this.casPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);
				this.mbxPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);
			}

			internal const long MaxValue = 2000L;

			internal const long BinSize = 10L;

			private const string LogLineFormat = "ClientRPCCount={0};AvgClientLatency={1};ServerRPCCount={2};AvgCasRPCProcessingTime={3};AvgMbxProcessingTime={4};MaxCasRPCProcessingTime={5}";

			private static readonly double[] Percentiles = new double[]
			{
				20.0,
				40.0,
				60.0,
				80.0,
				90.0,
				95.0,
				99.0,
				100.0
			};

			private readonly object rpcProcessingTimeLock = new object();

			private int serverRpcCount;

			private int clientRpcCount;

			private TimeSpan maxRpcProcessingTime = TimeSpan.Zero;

			private TimeSpan totalClientRPCLatency = TimeSpan.Zero;

			private TimeSpan totalCasRPCProcessingTime = TimeSpan.Zero;

			private TimeSpan totalMbxRPCProcessingTime = TimeSpan.Zero;

			private PercentileCounter clientPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);

			private PercentileCounter casPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);

			private PercentileCounter mbxPercentileCounter = new PercentileCounter(TimeSpan.MaxValue, TimeSpan.MaxValue, 10L, 2000L);
		}

		private class ThreadLocalData
		{
			internal ThreadLocalData(ProtocolLog protocolLog)
			{
				this.Row = protocolLog.CreateRowFormatter();
				this.PersistentData = new Dictionary<ProtocolLog.Field, string>();
			}

			public TimeSpan ElapsedTime
			{
				get
				{
					if (this.CurrentCallStartTickCount == null)
					{
						throw new InvalidOperationException("CurrentCallStartTickCount is not set");
					}
					return TimeSpan.FromMilliseconds(Environment.TickCount - this.CurrentCallStartTickCount.Value);
				}
			}

			public void ClearPendingData()
			{
				this.ClearPerRecordData();
				for (int i = 0; i < 24; i++)
				{
					this.Row[i] = null;
				}
				this.CurrentCallStartTickCount = null;
			}

			public void ClearPerRecordData()
			{
				for (int i = 0; i < 24; i++)
				{
					if (ProtocolLog.Fields[i].Scope == ProtocolLog.FieldScope.Record)
					{
						this.Row[i] = null;
					}
				}
				this.OperationSpecificData.Clear();
				this.HasInterestingDataToFlush = false;
			}

			internal readonly LogRowFormatter Row;

			internal readonly Dictionary<ProtocolLog.Field, string> PersistentData;

			internal int? CurrentCallStartTickCount = null;

			internal ProtocolLogSession.ThreadLocalData.DataAccumulator OperationSpecificData;

			internal bool HasInterestingDataToFlush;

			internal struct DataAccumulator
			{
				internal void Append(string value)
				{
					if (this.data == null)
					{
						this.data = new StringBuilder();
					}
					if (this.data.Length != 0)
					{
						this.data.Append("; ");
					}
					this.data.Append(value);
				}

				internal string Detach()
				{
					string result = null;
					if (this.data != null && this.data.Length != 0)
					{
						result = this.data.ToString();
					}
					this.Clear();
					return result;
				}

				internal void Clear()
				{
					if (this.data != null)
					{
						this.data.Length = 0;
					}
				}

				private const string DataDelimiter = "; ";

				private StringBuilder data;
			}
		}
	}
}
