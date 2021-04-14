using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailSubmission;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal static class SafetyNetRedelivery
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DumpsterTracer;
			}
		}

		public static void MarkRedeliveryRequired(ReplayConfiguration configuration, DateTime inspectorTime, long lastLogGenBeforeActivation, long numLogsLost)
		{
			DateTime utcNow = DateTime.UtcNow;
			SafetyNetRedelivery.Tracer.TraceDebug(0L, "Setting Dumpster re-delivery required for instance {0}. CreationTime {1}, StartTime {2}, LastLogGenBeforeActivation={3}, NumLogsLost={4}.", new object[]
			{
				configuration.Name,
				utcNow,
				inspectorTime,
				lastLogGenBeforeActivation,
				numLogsLost
			});
			DiagCore.AssertOrWatson(inspectorTime > ReplayState.ZeroFileTime, "inspectorTime is '{0}' which is unexpected!", new object[]
			{
				inspectorTime
			});
			DateTime utcNow2 = DateTime.UtcNow;
			DateTime startTimeUtc;
			DateTime endTimeUtc;
			SafetyNetRedelivery.GetConservativeLossTimes(inspectorTime, out startTimeUtc, out endTimeUtc);
			SafetyNetRedelivery.MarkRedeliveryRequired(configuration, utcNow2, startTimeUtc, endTimeUtc, lastLogGenBeforeActivation, numLogsLost);
		}

		public static void MarkRedeliveryRequired(ReplayConfiguration configuration, DateTime failoverTimeUtc, DateTime startTimeUtc, DateTime endTimeUtc, long lastLogGenBeforeActivation, long numLogsLost)
		{
			SafetyNetRedelivery.Tracer.TraceDebug(0L, "MarkRedeliveryRequired() for {0}({1}) called: failoverTimeUtc={2}, startTimeUtc={3}, endTimeUtc={4}, lastLogGenBeforeActivation={5}, numLogsLost={6}", new object[]
			{
				configuration.Name,
				configuration.Identity,
				failoverTimeUtc,
				startTimeUtc,
				endTimeUtc,
				lastLogGenBeforeActivation,
				numLogsLost
			});
			if (endTimeUtc < startTimeUtc)
			{
				SafetyNetRedelivery.Tracer.TraceError<string, string>(0L, "MarkRedeliveryRequired() for {0}({1}) failed because endTimeUtc should not be less than startTimeUtc", configuration.Name, configuration.Identity);
				DiagCore.AssertOrWatson(false, "endTimeUtc ({0}) should not be less than startTimeUtc ({1}) !", new object[]
				{
					endTimeUtc,
					startTimeUtc
				});
				throw new DumpsterInvalidResubmitRequestException(configuration.DatabaseName);
			}
			TimeSpan timeSpan = endTimeUtc.Subtract(startTimeUtc);
			TimeSpan timeSpan2 = TimeSpan.FromSeconds((double)RegistryParameters.DumpsterRedeliveryMaxTimeRangeInSecs);
			if (timeSpan > timeSpan2)
			{
				SafetyNetRedelivery.Tracer.TraceError(0L, "MarkRedeliveryRequired() for {0}({1}) failed because the request window ({2}) is larger than the maximum allowed of {3}", new object[]
				{
					configuration.Name,
					configuration.Identity,
					timeSpan,
					timeSpan2
				});
				throw new DumpsterInvalidResubmitRequestException(configuration.DatabaseName);
			}
			string text;
			ActiveManagerCore.GetDatabaseMountStatus(configuration.IdentityGuid, out text);
			SafetyNetInfo safetyNetInfo = new SafetyNetInfo(text, lastLogGenBeforeActivation, numLogsLost, failoverTimeUtc, startTimeUtc, endTimeUtc);
			SafetyNetInfoCache safetyNetTable = configuration.ReplayState.GetSafetyNetTable();
			safetyNetTable.Update(safetyNetInfo);
			ReplayEventLogConstants.Tuple_DatabaseDumpsterRedeliveryRequired.LogEvent(null, new object[]
			{
				configuration.DatabaseName,
				startTimeUtc,
				endTimeUtc
			});
			ReplayCrimsonEvents.DumpsterRedeliveryRequired.Log<string, Guid, string, long, long, DateTime, DateTime, DateTime, bool, bool>(configuration.DatabaseName, configuration.IdentityGuid, text, lastLogGenBeforeActivation, numLogsLost, failoverTimeUtc, startTimeUtc, endTimeUtc, false, true);
		}

		public static void DoRedeliveryIfRequired(object replayConfig)
		{
			ReplayConfiguration replayConfiguration = replayConfig as ReplayConfiguration;
			SafetyNetRedelivery.Tracer.TraceDebug<string, string>(0L, "DumpsterRedeliveryManager: DoRedeliveryIfRequired for {0}({1}).", replayConfiguration.Name, replayConfiguration.Identity);
			if (replayConfiguration == null)
			{
				SafetyNetRedelivery.Tracer.TraceError(0L, "DoRedeliveryIfRequired: Invalid ReplayConfiguration object passed in.");
				return;
			}
			Exception ex = null;
			bool flag = false;
			try
			{
				SafetyNetInfoCache safetyNetTable = replayConfiguration.ReplayState.GetSafetyNetTable();
				SafetyNetInfoHashTable safetyNetInfosReadThrough = safetyNetTable.GetSafetyNetInfosReadThrough();
				if (!safetyNetInfosReadThrough.RedeliveryRequired)
				{
					SafetyNetRedelivery.Tracer.TraceDebug<string, string>((long)replayConfiguration.GetHashCode(), "DoRedeliveryIfRequired: Skipping Redelivery for {0}({1}) because RedeliveryRequired is 'false'.", replayConfiguration.Name, replayConfiguration.Identity);
				}
				else
				{
					flag = SafetyNetRedelivery.TryEnterLock(replayConfiguration);
					if (!flag)
					{
						SafetyNetRedelivery.Tracer.TraceDebug<string, string>((long)replayConfiguration.GetHashCode(), "DoRedeliveryIfRequired: Another thread is doing Redelivery on {0}({1})", replayConfiguration.Name, replayConfiguration.Identity);
					}
					else
					{
						SafetyNetRedelivery.Tracer.TraceDebug<string, string>((long)replayConfiguration.GetHashCode(), "DoRedeliveryIfRequired: Processing dumpster re-delivery for instance {0}({1})", replayConfiguration.Name, replayConfiguration.Identity);
						SafetyNetRedelivery.DoRedeliveryIfRequiredLocked(replayConfiguration, safetyNetTable, safetyNetInfosReadThrough);
					}
				}
			}
			catch (DumpsterRedeliveryException ex2)
			{
				ex = ex2;
			}
			catch (MonitoringADConfigException ex3)
			{
				ex = ex3;
			}
			catch (ClusterException ex4)
			{
				ex = ex4;
			}
			catch (TransientException ex5)
			{
				ex = ex5;
			}
			catch (IOException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (ex != null)
				{
					SafetyNetRedelivery.Tracer.TraceError<string, string, Exception>((long)replayConfiguration.GetHashCode(), "DoRedeliveryIfRequired() for {0}({1}) failed with Exception: {2}", replayConfiguration.Name, replayConfiguration.Identity, ex);
					ReplayCrimsonEvents.DumpsterRedeliveryForDatabaseFailed.LogPeriodic<string, Guid, bool, string, Exception>(replayConfiguration.Identity, DiagCore.DefaultEventSuppressionInterval, replayConfiguration.DatabaseName, replayConfiguration.IdentityGuid, false, ex.Message, ex);
					ReplayEventLogConstants.Tuple_DumpsterRedeliveryFailed.LogEvent(replayConfiguration.Identity, new object[]
					{
						ex.Message,
						replayConfiguration.Name
					});
				}
				if (flag)
				{
					SafetyNetRedelivery.ExitLock(replayConfiguration);
				}
			}
		}

		private static void DoRedeliveryIfRequiredLocked(ReplayConfiguration config, SafetyNetInfoCache snCache, SafetyNetInfoHashTable safetyNetInfos)
		{
			foreach (KeyValuePair<SafetyNetRequestKey, SafetyNetInfo> request in safetyNetInfos)
			{
				SafetyNetRedelivery.ProcessSingleRequest(config, snCache, request);
			}
		}

		private static void ProcessSingleRequest(ReplayConfiguration config, SafetyNetInfoCache snCache, KeyValuePair<SafetyNetRequestKey, SafetyNetInfo> request)
		{
			SafetyNetRequestKey key = request.Key;
			SafetyNetInfo value = request.Value;
			string serverName = key.ServerName;
			DateTime requestCreationTimeUtc = key.RequestCreationTimeUtc;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.DumpsterRedeliveryExpirationDurationInSecs);
			if (requestCreationTimeUtc.Add(timeSpan) < DateTime.UtcNow)
			{
				SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Deleting Dumpster Redelivery request ({2}) for {0}({1}) because it is older than the maximum time for retries ({3}). CreationTimeUtc={4}", new object[]
				{
					config.Name,
					config.Identity,
					key,
					timeSpan,
					requestCreationTimeUtc
				});
				ReplayCrimsonEvents.DumpsterRedeliverySpecificRequestExpired.Log<string, Guid, bool, DateTime, SafetyNetRequestKey, string, TimeSpan, bool>(config.DatabaseName, config.IdentityGuid, false, requestCreationTimeUtc, key, value.GetSerializedForm(), timeSpan, value.RedeliveryRequired);
				snCache.Delete(value);
				return;
			}
			if (!value.IsVersionCompatible())
			{
				SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Skipping Dumpster Redelivery request ({2}) for {0}({1}) because it is of an incompatible version. ServerVersion={3}; RequestVersion={4}", new object[]
				{
					config.Name,
					config.Identity,
					key,
					SafetyNetInfo.VersionNumber,
					value.Version
				});
				ReplayCrimsonEvents.DumpsterRedeliveryRequestVersionIncompatible.LogPeriodic<string, Guid, bool, DateTime, SafetyNetRequestKey, string, bool, Version, Version>(config.Identity + key.ToString(), DateTimeHelper.OneDay, config.DatabaseName, config.IdentityGuid, false, requestCreationTimeUtc, key, value.GetSerializedForm(), value.RedeliveryRequired, SafetyNetInfo.VersionNumber, value.Version);
				return;
			}
			if (!value.RedeliveryRequired)
			{
				SafetyNetRedelivery.Tracer.TraceDebug<string, string, SafetyNetRequestKey>((long)config.GetHashCode(), "ProcessSingleRequest: Skipping and deleting request ({2}) for {0}({1}) because RedeliveryRequired is 'false'.", config.Name, config.Identity, key);
				snCache.Delete(value);
				return;
			}
			if (value.RequestNextDueTimeUtc > DateTime.UtcNow)
			{
				SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Skipped processing redelivery request ({2}) for {0}({1}) due to backoff logic. The request will next run at '{3}'", new object[]
				{
					config.Name,
					config.Identity,
					key,
					value.RequestNextDueTimeUtc
				});
				return;
			}
			SafetyNetRedelivery.Tracer.TraceDebug<string, string, SafetyNetRequestKey>((long)config.GetHashCode(), "ProcessSingleRequest: Processing redelivery request ({2}) for {0}({1})", config.Name, config.Identity, key);
			bool flag = true;
			DateTime startTimeUtc = value.StartTimeUtc;
			DateTime endTimeUtc = value.EndTimeUtc;
			List<string> list = null;
			List<string> list2 = null;
			TimeSpan value2 = TimeSpan.FromSeconds((double)RegistryParameters.DumpsterRedeliveryPrimaryRetryDurationInSecs);
			IMonitoringADConfig config2 = Dependencies.MonitoringADConfigProvider.GetConfig(true);
			if (value.HubServers == null || value.HubServers.Count == 0)
			{
				if (config2.ServerRole != MonitoringServerRole.DagMember)
				{
					SafetyNetRedelivery.LogSpecificRequestFailedEvent(config, key, value, "Local server is not a member of a DAG.", new object[0]);
					return;
				}
				List<IADServer> servers = config2.Servers;
				if (servers == null || servers.Count == 0)
				{
					SafetyNetRedelivery.LogSpecificRequestFailedEvent(config, key, value, "Could not find any BackEnd servers in local DAG '{0}'.", new object[]
					{
						config2.Dag.Name
					});
					return;
				}
				list = new List<string>(servers.Count<IADServer>());
				foreach (IADServer iadserver in servers)
				{
					list.Add(iadserver.Fqdn);
				}
				value.HubServers = list;
			}
			else if (requestCreationTimeUtc.Add(value2) >= DateTime.UtcNow)
			{
				list = value.PrimaryHubServers;
			}
			else
			{
				list = value.ShadowHubServers;
				flag = false;
				if (value.ShadowRequestCreateTimeUtc == DateTime.MinValue)
				{
					value.ShadowRequestCreateTimeUtc = DateTime.UtcNow;
				}
			}
			if (list == null || list.Count == 0)
			{
				list = value.HubServers;
			}
			if (SafetyNetRedelivery.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string text = string.Join(",", list);
				if (flag)
				{
					SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Processing redelivery request ({2}) for {0}({1}) : Primary phase. Servers={3}", new object[]
					{
						config.Name,
						config.Identity,
						key,
						text
					});
				}
				else
				{
					SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Processing redelivery request ({2}) for {0}({1}) : Shadow copy phase. Servers={3}", new object[]
					{
						config.Name,
						config.Identity,
						key,
						text
					});
				}
			}
			list2 = new List<string>(list.Count);
			value.RequestLastAttemptedTimeUtc = DateTime.UtcNow;
			foreach (string text2 in list)
			{
				Exception ex = SafetyNetRedelivery.DumpsterResubmitRpc(text2, config, key, value, config2, flag);
				if (ex != null)
				{
					list2.Add(text2);
				}
			}
			if (list2.Count == 0)
			{
				SafetyNetRedelivery.Tracer.TraceDebug<string, string, SafetyNetRequestKey>((long)config.GetHashCode(), "ProcessSingleRequest: Deleting dumpster request ({2}) for {0}({1}), since RPC has completed to all HUB servers", config.Name, config.Identity, key);
				value.RedeliveryRequired = false;
				value.RequestCompletedTimeUtc = DateTime.UtcNow;
				value.RequestNextDueTimeUtc = DateTime.MinValue;
			}
			else
			{
				value.RequestNextDueTimeUtc = SafetyNetRequestBackoff.GetNextDueTime(key, value, flag);
				SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "ProcessSingleRequest: Dumpster request ({2}) for {0}({1}): Setting next due time to {3}.", new object[]
				{
					config.Name,
					config.Identity,
					key,
					value.RequestNextDueTimeUtc
				});
				ReplayCrimsonEvents.DumpsterRedeliveryRequestNextDueAt.Log<string, Guid, DateTime, DateTime, DateTime, DateTime>(config.DatabaseName, config.IdentityGuid, key.RequestCreationTimeUtc, value.StartTimeUtc, value.EndTimeUtc, value.RequestNextDueTimeUtc);
			}
			if (flag)
			{
				value.PrimaryHubServers = list2;
			}
			else
			{
				value.ShadowHubServers = list2;
			}
			if (value.RedeliveryRequired)
			{
				snCache.Update(value);
				return;
			}
			snCache.Delete(value);
		}

		private static bool TryEnterLock(ReplayConfiguration config)
		{
			bool result;
			lock (SafetyNetRedelivery.s_inProgressRequests)
			{
				if (SafetyNetRedelivery.s_inProgressRequests.Contains(config.IdentityGuid))
				{
					result = false;
				}
				else
				{
					SafetyNetRedelivery.s_inProgressRequests.Add(config.IdentityGuid);
					result = true;
				}
			}
			return result;
		}

		private static void ExitLock(ReplayConfiguration config)
		{
			lock (SafetyNetRedelivery.s_inProgressRequests)
			{
				SafetyNetRedelivery.s_inProgressRequests.Remove(config.IdentityGuid);
			}
		}

		private static Exception DumpsterResubmitRpc(string hubServer, ReplayConfiguration config, SafetyNetRequestKey snKey, SafetyNetInfo snInfo, IMonitoringADConfig adConfig, bool inPrimaryPhase)
		{
			bool flag = false;
			Exception ex = null;
			SafetyNetRedelivery.Tracer.TraceDebug<string, string, string>((long)config.GetHashCode(), "DumpsterResubmitRpc() called for {0}({1}) to HUB server {2}.", config.Name, config.Identity, hubServer);
			if (SafetyNetRedelivery.IsSafetyNetRedeliveryRpcSupportedOnHubServer(hubServer, adConfig, out ex))
			{
				AddResubmitRequestStatus addResubmitRequestStatus;
				ex = SafetyNetRedelivery.DumpsterResubmitRpcInternal(true, hubServer, config, snKey, snInfo, inPrimaryPhase, out addResubmitRequestStatus);
				if (addResubmitRequestStatus == AddResubmitRequestStatus.Disabled)
				{
					flag = true;
					SafetyNetRedelivery.Tracer.TraceDebug<string, string, string>((long)config.GetHashCode(), "DumpsterResubmitRpc() requested from SafetyNet for {0}({1}) to HUB server {2}, but it is Disabled. Falling back to classic Dumpster...", config.Name, config.Identity, hubServer);
				}
				else if (addResubmitRequestStatus == AddResubmitRequestStatus.DuplicateRequest)
				{
					SafetyNetRedelivery.Tracer.TraceDebug<string, string, string>((long)config.GetHashCode(), "DumpsterResubmitRpc() requested from SafetyNet for {0}({1}) to HUB server {2}, but it is DuplicateRequest. Treating this duplicate RPC as successful.", config.Name, config.Identity, hubServer);
				}
			}
			else if (ex != null)
			{
				SafetyNetRedelivery.LogResubmitToServerException(true, hubServer, config, snKey, snInfo, inPrimaryPhase, ex, string.Empty);
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				AddResubmitRequestStatus addResubmitRequestStatus;
				ex = SafetyNetRedelivery.DumpsterResubmitRpcInternal(false, hubServer, config, snKey, snInfo, inPrimaryPhase, out addResubmitRequestStatus);
			}
			return ex;
		}

		private static Exception DumpsterResubmitRpcInternal(bool isSafetyNetRpc, string hubServer, ReplayConfiguration config, SafetyNetRequestKey snKey, SafetyNetInfo snInfo, bool inPrimaryPhase, out AddResubmitRequestStatus snStatus)
		{
			Exception ex = null;
			SubmissionStatus dpStatus = SubmissionStatus.Error;
			string unresponsivePrimaryServersStr = string.Empty;
			snStatus = AddResubmitRequestStatus.Error;
			AddResubmitRequestStatus tempSnStatus = AddResubmitRequestStatus.Error;
			SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "DumpsterResubmitRpcInternal() called for {0}({1}) to HUB server {2}. isSafetyNetRpc={3}. inPrimaryPhase={4}", new object[]
			{
				config.Name,
				config.Identity,
				hubServer,
				isSafetyNetRpc,
				inPrimaryPhase
			});
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					using (MailSubmissionServiceRpcClient mailSubmissionServiceRpcClient = new MailSubmissionServiceRpcClient(hubServer))
					{
						mailSubmissionServiceRpcClient.SetTimeOut(RegistryParameters.DumpsterRpcTimeoutInMSecs);
						if (isSafetyNetRpc)
						{
							string[] array = null;
							if (!inPrimaryPhase)
							{
								List<string> list = snInfo.PrimaryHubServers;
								if (list == null || list.Count == 0)
								{
									list = snInfo.HubServers;
									SafetyNetRedelivery.Tracer.TraceDebug<string, string, string>((long)config.GetHashCode(), "DumpsterResubmitRpcInternal() for {0}({1}) to HUB server {2} : PrimaryHubServers list was empty, so assuming all HubServers failed to respond to primary and will issue shadow requests for all of them.", config.Name, config.Identity, hubServer);
								}
								if (list != null && list.Count > 0)
								{
									array = list.ToArray();
									unresponsivePrimaryServersStr = string.Join(",", array);
									if (SafetyNetRedelivery.Tracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "DumpsterResubmitRpcInternal() for {0}({1}) to HUB server {2} : unresponsivePrimaryServers = {3}", new object[]
										{
											config.Name,
											config.Identity,
											hubServer,
											unresponsivePrimaryServersStr
										});
									}
								}
							}
							tempSnStatus = mailSubmissionServiceRpcClient.AddMdbResubmitRequest(Guid.Empty, config.IdentityGuid, snInfo.StartTimeUtc.Ticks, snInfo.EndTimeUtc.Ticks, array, null);
						}
						else
						{
							dpStatus = mailSubmissionServiceRpcClient.SubmitDumpsterMessages(config.DatabaseDn, snInfo.StartTimeUtc.Ticks, snInfo.EndTimeUtc.Ticks);
						}
					}
				}, TimeSpan.FromMilliseconds((double)RegistryParameters.DumpsterRpcTimeoutInMSecs));
				if (isSafetyNetRpc)
				{
					snStatus = tempSnStatus;
					if (snStatus == AddResubmitRequestStatus.DuplicateRequest)
					{
						ReplayCrimsonEvents.DumpsterRedeliverySpecificRequestDuplicate.Log<string, Guid, DateTime, DateTime, DateTime, string>(config.DatabaseName, config.IdentityGuid, snKey.RequestCreationTimeUtc, snInfo.StartTimeUtc, snInfo.EndTimeUtc, hubServer);
					}
					else if (snStatus != AddResubmitRequestStatus.Success && snStatus != AddResubmitRequestStatus.Disabled)
					{
						ex = new DumpsterSafetyNetRpcFailedException(hubServer, tempSnStatus.ToString());
						return ex;
					}
				}
				TimeSpan timeSpan = DateTime.UtcNow.Subtract(snKey.RequestCreationTimeUtc);
				SafetyNetRedelivery.Tracer.TraceDebug((long)config.GetHashCode(), "DumpsterResubmitRpcInternal() succeeded for {0}({1}) to HUB server {2} after {3}. isSafetyNetRpc={4}. snStatus returned = {5}.", new object[]
				{
					config.Name,
					config.Identity,
					hubServer,
					timeSpan,
					isSafetyNetRpc,
					snStatus
				});
				ReplayCrimsonEvents.DumpsterRedeliverySpecificRequestRPCSucceeded.Log<string, Guid, bool, DateTime, DateTime, DateTime, string, SafetyNetRequestKey, string, TimeSpan, bool, bool, string>(config.DatabaseName, config.IdentityGuid, false, snKey.RequestCreationTimeUtc, snInfo.StartTimeUtc, snInfo.EndTimeUtc, hubServer, snKey, snInfo.GetSerializedForm(), timeSpan, isSafetyNetRpc, inPrimaryPhase, unresponsivePrimaryServersStr);
				ReplayEventLogConstants.Tuple_DatabaseSubmitDumpsterMessages.LogEvent(config.IdentityGuid + hubServer, new object[]
				{
					hubServer,
					snInfo.StartTimeUtc,
					snInfo.EndTimeUtc,
					config.Name
				});
			}
			catch (RpcException ex2)
			{
				ex = ex2;
			}
			catch (DumpsterCouldNotFindHubServerException ex3)
			{
				ex = ex3;
			}
			catch (TimeoutException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					SafetyNetRedelivery.LogResubmitToServerException(isSafetyNetRpc, hubServer, config, snKey, snInfo, inPrimaryPhase, ex, unresponsivePrimaryServersStr);
				}
			}
			return ex;
		}

		private static void LogResubmitToServerException(bool isSafetyNetRpc, string hubServer, ReplayConfiguration config, SafetyNetRequestKey snKey, SafetyNetInfo snInfo, bool inPrimaryPhase, Exception exception, string unresponsivePrimaryServersStr)
		{
			string text = string.Format("{0}{1}{2}{3}", new object[]
			{
				config.Identity,
				snKey.ToString(),
				hubServer,
				isSafetyNetRpc.ToString(CultureInfo.InvariantCulture)
			});
			SafetyNetRedelivery.Tracer.TraceError((long)config.GetHashCode(), "DumpsterResubmitRpc() made for {0}({1}) to HUB server {2} FAILED! isSafetyNetRpc={3}. Exception: {4}", new object[]
			{
				config.Name,
				config.Identity,
				hubServer,
				isSafetyNetRpc,
				exception
			});
			ReplayCrimsonEvents.DumpsterRedeliverySpecificRequestRPCFailed.LogPeriodic<string, Guid, bool, DateTime, DateTime, DateTime, string, SafetyNetRequestKey, string, string, bool, bool, string>(text, DiagCore.DefaultEventSuppressionInterval, config.DatabaseName, config.IdentityGuid, false, snKey.RequestCreationTimeUtc, snInfo.StartTimeUtc, snInfo.EndTimeUtc, hubServer, snKey, snInfo.GetSerializedForm(), exception.Message, isSafetyNetRpc, inPrimaryPhase, unresponsivePrimaryServersStr);
			ReplayEventLogConstants.Tuple_SubmitDumpsterMessagesFailed.LogEvent(text, new object[]
			{
				hubServer,
				snInfo.StartTimeUtc,
				snInfo.EndTimeUtc,
				config.Name,
				exception.Message
			});
		}

		private static void LogSpecificRequestFailedEvent(ReplayConfiguration config, SafetyNetRequestKey snKey, SafetyNetInfo snInfo, string errorMessageFormatStr, params object[] messageParams)
		{
			if (SafetyNetRedelivery.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				SafetyNetRedelivery.Tracer.TraceError((long)config.GetHashCode(), "ProcessSingleRequest: Redelivery request ({2}) for {0}({1}) failed. Error: {3}", new object[]
				{
					config.Name,
					config.Identity,
					snKey,
					string.Format(errorMessageFormatStr, messageParams)
				});
			}
			ReplayCrimsonEvents.DumpsterRedeliverySpecificRequestFailed.LogPeriodic<string, Guid, bool, DateTime, SafetyNetRequestKey, string, string>(config.Identity + snKey.ToString(), DiagCore.DefaultEventSuppressionInterval, config.DatabaseName, config.IdentityGuid, false, snKey.RequestCreationTimeUtc, snKey, snInfo.GetSerializedForm(), string.Format(errorMessageFormatStr, messageParams));
		}

		private static bool IsSafetyNetRedeliveryRpcSupportedOnHubServer(string hubServerName, IMonitoringADConfig adConfig, out Exception exception)
		{
			exception = null;
			IADServer iadserver = adConfig.LookupMiniServerByName(new AmServerName(hubServerName));
			if (iadserver == null)
			{
				exception = new DumpsterCouldNotFindHubServerException(hubServerName);
				return false;
			}
			return SafetyNetVersionChecker.IsSafetyNetRedeliveryRpcSupportedOnHubServer(iadserver.AdminDisplayVersion);
		}

		private static void GetConservativeLossTimes(DateTime inspectorTime, out DateTime lossStartTime, out DateTime lossEndTime)
		{
			lossStartTime = inspectorTime.Subtract(TimeSpan.FromSeconds((double)RegistryParameters.DumpsterRedeliveryStartBufferSeconds));
			lossEndTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds((double)RegistryParameters.DumpsterRedeliveryEndBufferSeconds));
		}

		private static HashSet<Guid> s_inProgressRequests = new HashSet<Guid>();
	}
}
