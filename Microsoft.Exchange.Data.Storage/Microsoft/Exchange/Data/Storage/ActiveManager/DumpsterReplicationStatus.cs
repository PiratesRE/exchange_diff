using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DumpsterReplicationStatus : IDumpsterReplicationStatus
	{
		public static void TestHookSetLagTime(TimeSpan delay)
		{
			DumpsterReplicationStatus.c_replayExtraLag = delay;
		}

		private TimeSpan CheckReplicationFlushedTimeSkew
		{
			get
			{
				int num = 5;
				Exception ex = null;
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", RegistryKeyPermissionCheck.ReadSubTree))
					{
						if (registryKey != null)
						{
							num = (int)registryKey.GetValue("CheckReplicationFlushedTimeSkewSeconds", 5);
						}
					}
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				catch (SecurityException ex3)
				{
					ex = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "Error reading {0}\\{1}: {2}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "CheckReplicationFlushedTimeSkewSeconds", ex);
				}
				return TimeSpan.FromSeconds((double)num);
			}
		}

		public Guid? GetDatabaseGuidFromDN(string distinguishedName)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, distinguishedName);
			MailboxDatabase[] mdbs = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				mdbs = this.m_adSession.Find<MailboxDatabase>(null, QueryScope.SubTree, queryFilter, null, 0);
			}, 1);
			if (adoperationResult != ADOperationResult.Success || mdbs == null || mdbs.Length != 1)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, ADOperationResult, int>((long)this.GetHashCode(), "GetDatabaseGuidFromDN unable to find database for DN {0}. AD result: {1} (number of results: {2}).", distinguishedName, adoperationResult, (mdbs == null) ? 0 : mdbs.Length);
				return null;
			}
			return new Guid?(mdbs[0].Guid);
		}

		public Dictionary<IADServer, CopyInfo> GetMailboxDatabaseCopyStatus(Guid databaseGuid)
		{
			MailboxDatabase mailboxDatabaseByGuid = this.GetMailboxDatabaseByGuid(databaseGuid);
			if (mailboxDatabaseByGuid != null)
			{
				return this.GetMailboxDatabaseCopyStatus(mailboxDatabaseByGuid);
			}
			throw new DataBaseNotFoundException(databaseGuid);
		}

		private MailboxDatabase GetMailboxDatabaseByGuid(Guid databaseGuid)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, databaseGuid);
			MailboxDatabase[] mdbs = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				mdbs = this.m_adSession.Find<MailboxDatabase>(null, QueryScope.SubTree, queryFilter, null, 0);
			}, 1);
			if (adoperationResult != ADOperationResult.Success || mdbs == null || mdbs.Length != 1)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<Guid, ADOperationResult, int>((long)this.GetHashCode(), "GetMailboxDatabaseByGuid unable to find database for guid {0}. AD result: {1} (number of results: {2}).", databaseGuid, adoperationResult, (mdbs == null) ? 0 : mdbs.Length);
				return null;
			}
			return mdbs[0];
		}

		private IADServer GetServerByName(string serverName)
		{
			KeyValuePair<DateTime, IADServer> keyValuePair;
			if (this.serverByNameHash.TryGetValue(serverName, out keyValuePair) && DateTime.UtcNow - keyValuePair.Key < DumpsterReplicationStatus.c_serverCacheExpiryTime)
			{
				return keyValuePair.Value;
			}
			IFindMiniServer findMiniServer = new SimpleMiniServerLookup(ADSessionFactory.CreateWrapper(this.m_adSession));
			Exception ex = null;
			IADServer iadserver = findMiniServer.FindMiniServerByShortNameEx(serverName, out ex);
			if (iadserver == null || ex != null)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string>((long)this.GetHashCode(), "GetServerByName unable to find server for name {0}. AD error: {1}", serverName, (ex != null) ? ex.Message : string.Empty);
				return null;
			}
			this.serverByNameHash.TryAdd(serverName, new KeyValuePair<DateTime, IADServer>(DateTime.UtcNow, iadserver));
			return iadserver;
		}

		private RpcDatabaseCopyStatus2[] ConvertLegacyCopyStatusArray(RpcDatabaseCopyStatus[] oldStatuses)
		{
			return (from status in oldStatuses
			let newStatus = new RpcDatabaseCopyStatus2(status)
			select newStatus).ToArray<RpcDatabaseCopyStatus2>();
		}

		private RpcDatabaseCopyStatus2 GetCopyStatus(IADServer server, Guid databaseGuid)
		{
			Guid[] array = new Guid[]
			{
				databaseGuid
			};
			RpcDatabaseCopyStatus2[] array2 = null;
			RpcErrorExceptionInfo rpcErrorExceptionInfo = null;
			if (AmRpcClientHelper.ServerIsPotentiallyInMaintenanceMode(server))
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError((long)this.GetHashCode(), "Skipping RPC RpccGetCopyStatusEx4 to server {0} version {1} for db guid {2} as it is possibly in maintenance mode or switched over. DatabaseCopyAutoActivationPolicy is '{3}', DatabaseCopyActivationDisabledAndMoveNow is {4}, ComponentStates is {5}.", new object[]
				{
					server.Fqdn,
					server.AdminDisplayVersion,
					databaseGuid,
					server.DatabaseCopyAutoActivationPolicy,
					server.DatabaseCopyActivationDisabledAndMoveNow,
					(server.ComponentStates != null) ? string.Join(",", server.ComponentStates.ToArray()) : string.Empty
				});
				return null;
			}
			RpcDatabaseCopyStatus2 result;
			try
			{
				using (ReplayRpcClient replayRpcClient = new ReplayRpcClient(server.Fqdn))
				{
					replayRpcClient.SetTimeOut(AmRpcClientHelper.RpcTimeoutShort);
					if (ReplayRpcVersionControl.IsGetCopyStatusEx4RpcSupported(server.AdminDisplayVersion))
					{
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string, ServerVersion, Guid>((long)this.GetHashCode(), "Making RPC call RpccGetCopyStatusEx4 to server {0} of version {1} for db guid {2}", server.Fqdn, server.AdminDisplayVersion, databaseGuid);
						rpcErrorExceptionInfo = replayRpcClient.RpccGetCopyStatusEx4(RpcGetDatabaseCopyStatusFlags2.None, array, ref array2);
					}
					else
					{
						RpcDatabaseCopyStatus[] array3 = null;
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<string, ServerVersion, Guid>((long)this.GetHashCode(), "Making RPC call GetCopyStatusEx2 to server {0} of version {1} for db guid {2}", server.Fqdn, server.AdminDisplayVersion, databaseGuid);
						rpcErrorExceptionInfo = replayRpcClient.GetCopyStatusEx2(RpcGetDatabaseCopyStatusFlags.UseServerSideCaching, array, ref array3);
						if (!rpcErrorExceptionInfo.IsFailed() && array3 != null)
						{
							array2 = this.ConvertLegacyCopyStatusArray(array3);
						}
					}
				}
				if (rpcErrorExceptionInfo != null && rpcErrorExceptionInfo.IsFailed())
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceError((long)this.GetHashCode(), "GetCopyStatus(server={0},dbGuid={1}) returned resultCode '{2}' at time {3}.", new object[]
					{
						server.Fqdn,
						databaseGuid,
						rpcErrorExceptionInfo,
						ExDateTime.UtcNow
					});
					result = null;
				}
				else if (array2 != null && array2.Length > 0)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "GetCopyStatus(server={0},dbGuid={1}) returned {2} items with resultCode '{3}' at time {4}.", new object[]
					{
						server.Fqdn,
						databaseGuid,
						array2.Length,
						rpcErrorExceptionInfo,
						ExDateTime.UtcNow
					});
					result = array2[0];
				}
				else
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceError((long)this.GetHashCode(), "GetCopyStatus(server={0},dbGuid={1}) returned no valid results, i.e. {2} items (-1 means null) with resultCode '{3}' at time {4}.", new object[]
					{
						server.Fqdn,
						databaseGuid,
						(array2 == null) ? -1 : array2.Length,
						rpcErrorExceptionInfo,
						ExDateTime.UtcNow
					});
					result = null;
				}
			}
			catch (RpcException arg)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, RpcException>((long)this.GetHashCode(), "RPC Exception when calling GetCopyStatus(server={0},dbGuid={1}): {2}", server.Fqdn, databaseGuid, arg);
				result = null;
			}
			return result;
		}

		private Dictionary<IADServer, CopyInfo> GetMailboxDatabaseCopyStatus(MailboxDatabase mdb)
		{
			Dictionary<IADServer, CopyInfo> dictionary = new Dictionary<IADServer, CopyInfo>(4);
			foreach (DatabaseCopy databaseCopy in mdb.DatabaseCopies)
			{
				IADServer serverByName = this.GetServerByName(databaseCopy.HostServerName);
				if (serverByName != null)
				{
					dictionary[serverByName] = new CopyInfo
					{
						Copy = databaseCopy,
						Status = this.GetCopyStatus(serverByName, mdb.Guid)
					};
				}
			}
			return dictionary;
		}

		public ConstraintCheckResultType CheckReplicationHealthConstraint(Guid databaseGuid, out TimeSpan waitTime, out LocalizedString failureReason, out ConstraintCheckAgent agent)
		{
			if (this.TestIntegrationEnabled(databaseGuid))
			{
				return this.TestIntegrationDatabaseConstraintResult(databaseGuid, out waitTime, out failureReason, out agent);
			}
			ConstraintCheckResultType constraintCheckResultType = ConstraintCheckResultType.Retry;
			waitTime = DumpsterReplicationStatus.c_defaultRetryWaitTime;
			failureReason = ServerStrings.DataMoveReplicationConstraintUnknown(databaseGuid);
			agent = ConstraintCheckAgent.None;
			DataMoveReplicationConstraintParameter dataMoveReplicationConstraintParameter = DataMoveReplicationConstraintParameter.None;
			MailboxDatabase mailboxDatabaseByGuid = this.GetMailboxDatabaseByGuid(databaseGuid);
			if (mailboxDatabaseByGuid != null)
			{
				Dictionary<IADServer, CopyInfo> mailboxDatabaseCopyStatus = this.GetMailboxDatabaseCopyStatus(mailboxDatabaseByGuid);
				constraintCheckResultType = this.GetCICurrentnessConstraint(mailboxDatabaseByGuid, mailboxDatabaseCopyStatus, out waitTime, out failureReason);
				if (constraintCheckResultType == ConstraintCheckResultType.NotSatisfied)
				{
					ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "CheckReplicationHealthConstraint GetCICurrentnessConstraint notsatisfied. databaseGuid={0}, result={1}, waitTime={2}, failureReason={3}, agent={4}.", new object[]
					{
						databaseGuid,
						constraintCheckResultType,
						waitTime,
						failureReason,
						agent
					});
					agent = ConstraintCheckAgent.ContentIndexing;
					return constraintCheckResultType;
				}
				agent = ConstraintCheckAgent.MailboxDatabaseReplication;
				if (mailboxDatabaseCopyStatus.Count > 1)
				{
					dataMoveReplicationConstraintParameter = (mailboxDatabaseByGuid.DataMoveReplicationConstraint & (DataMoveReplicationConstraintParameter)65535);
					int maximumNumberOfLogsInCopyQueue = DumpsterReplicationStatus.MaximumNumberOfLogsInCopyQueue;
					try
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", RegistryKeyPermissionCheck.ReadSubTree))
						{
							if (registryKey != null && !int.TryParse((string)registryKey.GetValue("MaximumCopyQueueLengthBeforeThrottle", DumpsterReplicationStatus.MaximumNumberOfLogsInCopyQueue.ToString()), out maximumNumberOfLogsInCopyQueue))
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string>((long)this.GetHashCode(), "Registry value of {0}\\{1} is not a number.", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "MaximumCopyQueueLengthBeforeThrottle");
							}
						}
					}
					catch (IOException arg)
					{
						ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string, IOException>((long)this.GetHashCode(), "Error reading {0}\\{1}: {2}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "MaximumCopyQueueLengthBeforeThrottle", arg);
					}
					catch (SecurityException arg2)
					{
						ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string, SecurityException>((long)this.GetHashCode(), "Error reading {0}\\{1}: {2}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "MaximumCopyQueueLengthBeforeThrottle", arg2);
					}
					catch (UnauthorizedAccessException arg3)
					{
						ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, string, UnauthorizedAccessException>((long)this.GetHashCode(), "Error reading {0}\\{1}: {2}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "MaximumCopyQueueLengthBeforeThrottle", arg3);
					}
					int num = 0;
					int num2 = 0;
					string text = null;
					Dictionary<string, TimeSpan> dictionary = new Dictionary<string, TimeSpan>(StringComparer.OrdinalIgnoreCase);
					switch (dataMoveReplicationConstraintParameter)
					{
					case DataMoveReplicationConstraintParameter.None:
						waitTime = TimeSpan.Zero;
						constraintCheckResultType = ConstraintCheckResultType.Satisfied;
						failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
						goto IL_79B;
					case DataMoveReplicationConstraintParameter.SecondCopy:
					case DataMoveReplicationConstraintParameter.AllCopies:
					{
						TimeSpan timeSpan = DumpsterReplicationStatus.c_maxWaitTime;
						TimeSpan timeSpan2 = TimeSpan.Zero;
						foreach (KeyValuePair<IADServer, CopyInfo> keyValuePair in mailboxDatabaseCopyStatus)
						{
							if (keyValuePair.Value.Status != null)
							{
								if (string.Equals(keyValuePair.Value.Status.ActiveDatabaseCopy, keyValuePair.Value.Status.MailboxServer, StringComparison.OrdinalIgnoreCase))
								{
									text = ((keyValuePair.Key.ServerSite != null) ? keyValuePair.Key.ServerSite.Name : string.Empty);
									num++;
								}
								else if (keyValuePair.Value.Status.CopyStatus == CopyStatusEnum.Healthy)
								{
									num2++;
									TimeSpan databaseHealthyStatusTimeEstimate = this.GetDatabaseHealthyStatusTimeEstimate(mailboxDatabaseByGuid, keyValuePair.Key, keyValuePair.Value, maximumNumberOfLogsInCopyQueue);
									if (databaseHealthyStatusTimeEstimate > timeSpan2)
									{
										timeSpan2 = databaseHealthyStatusTimeEstimate;
									}
									if (databaseHealthyStatusTimeEstimate < timeSpan)
									{
										timeSpan = databaseHealthyStatusTimeEstimate;
									}
								}
							}
							else
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "CheckReplicationHealthConstraint got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mailboxDatabaseByGuid.Name, databaseGuid, keyValuePair.Key.Name);
							}
						}
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<Guid, TimeSpan, TimeSpan>((long)this.GetHashCode(), "CheckReplicationHealthConstraint second or all copies. databaseGuid={0}, minBehindTime={1}, maxBehindTime={2}.", databaseGuid, timeSpan, timeSpan2);
						if (num2 == 0 || num == 0)
						{
							waitTime = DumpsterReplicationStatus.c_nothealthyWaitTime;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfiedNoHealthyCopies(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						if (dataMoveReplicationConstraintParameter == DataMoveReplicationConstraintParameter.SecondCopy)
						{
							if (timeSpan == TimeSpan.Zero)
							{
								waitTime = TimeSpan.Zero;
								constraintCheckResultType = ConstraintCheckResultType.Satisfied;
								failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								goto IL_79B;
							}
							waitTime = timeSpan;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						else
						{
							if (dataMoveReplicationConstraintParameter != DataMoveReplicationConstraintParameter.AllCopies)
							{
								goto IL_79B;
							}
							if (num != 1 || num2 < Math.Max(1, mailboxDatabaseCopyStatus.Count - 2))
							{
								if (timeSpan2 < DumpsterReplicationStatus.c_nothealthyWaitTime)
								{
									waitTime = DumpsterReplicationStatus.c_nothealthyWaitTime;
								}
								else
								{
									waitTime = timeSpan2;
								}
								constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
								failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								goto IL_79B;
							}
							if (timeSpan2 == TimeSpan.Zero)
							{
								waitTime = TimeSpan.Zero;
								constraintCheckResultType = ConstraintCheckResultType.Satisfied;
								failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								goto IL_79B;
							}
							waitTime = timeSpan2;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						break;
					}
					case DataMoveReplicationConstraintParameter.SecondDatacenter:
					case DataMoveReplicationConstraintParameter.AllDatacenters:
					{
						TimeSpan timeSpan;
						foreach (KeyValuePair<IADServer, CopyInfo> keyValuePair2 in mailboxDatabaseCopyStatus)
						{
							if (keyValuePair2.Value.Status != null)
							{
								if (string.Equals(keyValuePair2.Value.Status.ActiveDatabaseCopy, keyValuePair2.Value.Status.MailboxServer, StringComparison.OrdinalIgnoreCase))
								{
									text = ((keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty);
									num++;
								}
								else
								{
									if (keyValuePair2.Value.Status.CopyStatus == CopyStatusEnum.Healthy)
									{
										num2++;
										timeSpan = this.GetDatabaseHealthyStatusTimeEstimate(mailboxDatabaseByGuid, keyValuePair2.Key, keyValuePair2.Value, maximumNumberOfLogsInCopyQueue);
									}
									else
									{
										timeSpan = DumpsterReplicationStatus.c_nothealthyWaitTime;
									}
									TimeSpan databaseHealthyStatusTimeEstimate;
									if (dictionary.TryGetValue((keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty, out databaseHealthyStatusTimeEstimate))
									{
										if (databaseHealthyStatusTimeEstimate > TimeSpan.Zero && timeSpan < databaseHealthyStatusTimeEstimate)
										{
											dictionary[(keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty] = timeSpan;
										}
									}
									else
									{
										dictionary[(keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty] = timeSpan;
									}
								}
							}
							else
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "CheckReplicationHealthConstraint got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mailboxDatabaseByGuid.Name, databaseGuid, keyValuePair2.Key.Name);
							}
						}
						if (num2 == 0 || num == 0 || text == null)
						{
							waitTime = DumpsterReplicationStatus.c_nothealthyWaitTime;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfiedNoHealthyCopies(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						timeSpan = DumpsterReplicationStatus.c_maxWaitTime;
						TimeSpan timeSpan2 = TimeSpan.Zero;
						foreach (KeyValuePair<string, TimeSpan> keyValuePair3 in dictionary)
						{
							if (!string.Equals(keyValuePair3.Key, text, StringComparison.OrdinalIgnoreCase) && keyValuePair3.Value < timeSpan)
							{
								timeSpan = keyValuePair3.Value;
							}
							if (keyValuePair3.Value > timeSpan2)
							{
								timeSpan2 = keyValuePair3.Value;
							}
						}
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<Guid, TimeSpan, TimeSpan>((long)this.GetHashCode(), "CheckReplicationHealthConstraint second or all datacenter. databaseGuid={0}, minBehindTime={1}, maxBehindTime={2}.", databaseGuid, timeSpan, timeSpan2);
						if (dataMoveReplicationConstraintParameter == DataMoveReplicationConstraintParameter.SecondDatacenter)
						{
							if (timeSpan == TimeSpan.Zero)
							{
								waitTime = TimeSpan.Zero;
								constraintCheckResultType = ConstraintCheckResultType.Satisfied;
								failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								goto IL_79B;
							}
							waitTime = timeSpan;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						else
						{
							if (dataMoveReplicationConstraintParameter != DataMoveReplicationConstraintParameter.AllDatacenters)
							{
								goto IL_79B;
							}
							if (timeSpan2 == TimeSpan.Zero)
							{
								waitTime = TimeSpan.Zero;
								constraintCheckResultType = ConstraintCheckResultType.Satisfied;
								failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								goto IL_79B;
							}
							waitTime = timeSpan2;
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
							goto IL_79B;
						}
						break;
					}
					}
					waitTime = DumpsterReplicationStatus.c_nothealthyWaitTime;
					constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
					failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfiedInvalidConstraint(dataMoveReplicationConstraintParameter, databaseGuid);
					IL_79B:
					if (constraintCheckResultType != ConstraintCheckResultType.Satisfied)
					{
						StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCheckReplicationThrottling, databaseGuid.ToString(), new object[]
						{
							mailboxDatabaseByGuid.Name,
							mailboxDatabaseByGuid.DataMoveReplicationConstraint,
							mailboxDatabaseCopyStatus.Count,
							num2,
							num,
							text,
							waitTime,
							failureReason
						});
					}
				}
				else
				{
					waitTime = TimeSpan.Zero;
					constraintCheckResultType = ConstraintCheckResultType.Satisfied;
					failureReason = ServerStrings.DataMoveReplicationConstraintSatisfiedForNonReplicatedDatabase(dataMoveReplicationConstraintParameter, databaseGuid);
				}
			}
			else
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCheckReplicationThrottlingDatabaseNotFound, databaseGuid.ToString(), new object[]
				{
					databaseGuid
				});
			}
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "CheckReplicationHealthConstraint returned databaseGuid={0}, result={1}, waitTime={2}, failureReason={3}, agent={4}.", new object[]
			{
				databaseGuid,
				constraintCheckResultType,
				waitTime,
				failureReason,
				agent
			});
			return constraintCheckResultType;
		}

		public ConstraintCheckResultType CheckReplicationFlushed(Guid databaseGuid, DateTime utcCommitTime, out LocalizedString failureReason)
		{
			if (utcCommitTime.Kind != DateTimeKind.Utc)
			{
				throw new ArgumentException("utcCommitTime");
			}
			if (this.TestIntegrationEnabled(databaseGuid))
			{
				return this.TestHookCheckReplicationFlushed(databaseGuid, out failureReason);
			}
			utcCommitTime += this.CheckReplicationFlushedTimeSkew;
			ConstraintCheckResultType constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
			failureReason = ServerStrings.DataMoveReplicationConstraintUnknown(databaseGuid);
			MailboxDatabase mailboxDatabaseByGuid = this.GetMailboxDatabaseByGuid(databaseGuid);
			if (mailboxDatabaseByGuid != null)
			{
				DataMoveReplicationConstraintParameter dataMoveReplicationConstraintParameter = DataMoveReplicationConstraintParameter.None;
				int num = 0;
				int num2 = 0;
				string text = null;
				Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
				DateTime dateTime = DateTime.MaxValue;
				DateTime dateTime2 = DateTime.MinValue;
				Dictionary<IADServer, CopyInfo> mailboxDatabaseCopyStatus = this.GetMailboxDatabaseCopyStatus(mailboxDatabaseByGuid);
				if (mailboxDatabaseCopyStatus.Count > 1)
				{
					dataMoveReplicationConstraintParameter = (mailboxDatabaseByGuid.DataMoveReplicationConstraint & (DataMoveReplicationConstraintParameter)65535);
					switch (dataMoveReplicationConstraintParameter)
					{
					case DataMoveReplicationConstraintParameter.None:
						constraintCheckResultType = ConstraintCheckResultType.Satisfied;
						failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
						break;
					case DataMoveReplicationConstraintParameter.SecondCopy:
					case DataMoveReplicationConstraintParameter.AllCopies:
						foreach (KeyValuePair<IADServer, CopyInfo> keyValuePair in mailboxDatabaseCopyStatus)
						{
							CopyInfo value = keyValuePair.Value;
							if (value.Status != null)
							{
								if (!string.Equals(value.Status.ActiveDatabaseCopy, value.Status.MailboxServer, StringComparison.OrdinalIgnoreCase))
								{
									if (keyValuePair.Value.Status.CopyStatus == CopyStatusEnum.Healthy)
									{
										num2++;
										if (DateTime.Compare(value.Status.LastReplayedLogTime, dateTime) < 0)
										{
											dateTime = value.Status.LastReplayedLogTime;
										}
										if (DateTime.Compare(value.Status.LastReplayedLogTime, dateTime2) > 0)
										{
											dateTime2 = value.Status.LastReplayedLogTime;
										}
									}
								}
								else
								{
									text = ((keyValuePair.Key.ServerSite != null) ? keyValuePair.Key.ServerSite.Name : string.Empty);
									num++;
								}
							}
							else
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "CheckReplicationFlushed got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mailboxDatabaseByGuid.Name, databaseGuid, keyValuePair.Key.Name);
							}
						}
						ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "CheckReplicationFlushed: databaseGuid={0}, utcCommitTime={1}, minReplayTime={2}, maxReplayTime={3}.", new object[]
						{
							databaseGuid,
							utcCommitTime,
							dateTime,
							dateTime2
						});
						if (dateTime2 > utcCommitTime)
						{
							constraintCheckResultType = ConstraintCheckResultType.Satisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(DataMoveReplicationConstraintParameter.SecondCopy, databaseGuid);
						}
						else
						{
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintFlushNotSatisfied(DataMoveReplicationConstraintParameter.SecondCopy, databaseGuid, utcCommitTime, dateTime2);
						}
						break;
					case DataMoveReplicationConstraintParameter.SecondDatacenter:
					case DataMoveReplicationConstraintParameter.AllDatacenters:
						foreach (KeyValuePair<IADServer, CopyInfo> keyValuePair2 in mailboxDatabaseCopyStatus)
						{
							RpcDatabaseCopyStatus2 status = keyValuePair2.Value.Status;
							if (status != null)
							{
								if (string.Equals(status.ActiveDatabaseCopy, status.MailboxServer, StringComparison.OrdinalIgnoreCase))
								{
									text = ((keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty);
									num++;
								}
								else if (keyValuePair2.Value.Status.CopyStatus == CopyStatusEnum.Healthy)
								{
									num2++;
									DateTime t;
									if (dictionary.TryGetValue((keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty, out t))
									{
										if (t < keyValuePair2.Value.Status.LastReplayedLogTime)
										{
											dictionary[(keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty] = keyValuePair2.Value.Status.LastReplayedLogTime;
										}
									}
									else
									{
										dictionary[(keyValuePair2.Key.ServerSite != null) ? keyValuePair2.Key.ServerSite.Name : string.Empty] = keyValuePair2.Value.Status.LastReplayedLogTime;
									}
								}
							}
							else
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "CheckReplicationFlushed got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mailboxDatabaseByGuid.Name, databaseGuid, keyValuePair2.Key.Name);
							}
						}
						if (num2 == 0 || num == 0 || string.IsNullOrEmpty(text))
						{
							constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
							failureReason = ServerStrings.DataMoveReplicationConstraintFlushNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid, utcCommitTime, dateTime);
						}
						else
						{
							foreach (KeyValuePair<string, DateTime> keyValuePair3 in dictionary)
							{
								if (keyValuePair3.Value < dateTime)
								{
									dateTime = keyValuePair3.Value;
								}
								if (!string.Equals(keyValuePair3.Key, text, StringComparison.OrdinalIgnoreCase) && keyValuePair3.Value > dateTime2)
								{
									dateTime2 = keyValuePair3.Value;
								}
							}
							ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "CheckReplicationFlushed: databaseGuid={0}, utcCommitTime={1}, minReplayTime={2}, maxReplayTime={3}.", new object[]
							{
								databaseGuid,
								utcCommitTime,
								dateTime,
								dateTime2
							});
							if (dataMoveReplicationConstraintParameter == DataMoveReplicationConstraintParameter.SecondDatacenter)
							{
								if (dateTime2 > utcCommitTime)
								{
									constraintCheckResultType = ConstraintCheckResultType.Satisfied;
									failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								}
								else
								{
									constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
									failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								}
							}
							else if (dataMoveReplicationConstraintParameter == DataMoveReplicationConstraintParameter.AllDatacenters)
							{
								if (dateTime != DateTime.MaxValue && dateTime > utcCommitTime)
								{
									constraintCheckResultType = ConstraintCheckResultType.Satisfied;
									failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								}
								else
								{
									constraintCheckResultType = ConstraintCheckResultType.NotSatisfied;
									failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraintParameter, databaseGuid);
								}
							}
						}
						break;
					}
					if (constraintCheckResultType != ConstraintCheckResultType.Satisfied)
					{
						StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCheckReplicationFlushed, databaseGuid.ToString(), new object[]
						{
							mailboxDatabaseByGuid.Name,
							mailboxDatabaseByGuid.DataMoveReplicationConstraint,
							mailboxDatabaseCopyStatus.Count,
							num2,
							num,
							text,
							dateTime,
							dateTime2,
							utcCommitTime,
							failureReason
						});
					}
				}
				else
				{
					constraintCheckResultType = ConstraintCheckResultType.Satisfied;
					failureReason = ServerStrings.DataMoveReplicationConstraintSatisfiedForNonReplicatedDatabase(dataMoveReplicationConstraintParameter, databaseGuid);
				}
			}
			else
			{
				StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorCheckReplicationFlushedDatabaseNotFound, databaseGuid.ToString(), new object[]
				{
					databaseGuid
				});
			}
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "CheckReplicationFlushed returned databaseGuid={0}, result={1}, utcCommitTime={2}, m_lastActivityTime={3}.", new object[]
			{
				databaseGuid,
				constraintCheckResultType,
				utcCommitTime,
				ExDateTime.UtcNow
			});
			return constraintCheckResultType;
		}

		private TimeSpan GetDatabaseHealthyStatusTimeEstimate(MailboxDatabase mdb, IADServer server, CopyInfo dbCopy, int maximumNumberOfLogsInCopyQueue)
		{
			RpcDatabaseCopyStatus2 status = dbCopy.Status;
			long replayQueueLength = status.GetReplayQueueLength();
			TimeSpan timeSpan = TimeSpan.Zero;
			if (replayQueueLength > 0L && status.StatusRetrievedTime > status.CurrentReplayLogTime)
			{
				timeSpan = status.StatusRetrievedTime - status.CurrentReplayLogTime;
			}
			ExTraceGlobals.ActiveManagerClientTracer.TraceDebug((long)this.GetHashCode(), "GetDatabaseHealthyStatusTimeEstimate({0},{1}): Copy queue length = {2} ( <= maximum queue length = {3} ), ReplayQueueInTime = {4} ( <= c_replayExtraLag = {5} + replay lag time = {6}) ", new object[]
			{
				mdb.Guid,
				server.Name,
				replayQueueLength,
				maximumNumberOfLogsInCopyQueue,
				timeSpan,
				DumpsterReplicationStatus.c_replayExtraLag,
				dbCopy.Copy.ReplayLagTime
			});
			TimeSpan zero;
			if (dbCopy.Status.GetCopyQueueLength() <= (long)maximumNumberOfLogsInCopyQueue && timeSpan <= DumpsterReplicationStatus.c_replayExtraLag + dbCopy.Copy.ReplayLagTime)
			{
				zero = TimeSpan.Zero;
			}
			else
			{
				zero = DumpsterReplicationStatus.c_healthyWaitTime;
			}
			return zero;
		}

		private ConstraintCheckResultType GetCICurrentnessConstraint(MailboxDatabase mdb, Dictionary<IADServer, CopyInfo> copies, out TimeSpan waitTime, out LocalizedString failureReason)
		{
			ConstraintCheckResultType result = ConstraintCheckResultType.Satisfied;
			waitTime = TimeSpan.Zero;
			DataMoveReplicationConstraintParameter dataMoveReplicationConstraint = mdb.DataMoveReplicationConstraint;
			failureReason = ServerStrings.DataMoveReplicationConstraintSatisfied(dataMoveReplicationConstraint, mdb.Guid);
			if (copies == null || copies.Count == 0)
			{
				return result;
			}
			if ((dataMoveReplicationConstraint & DataMoveReplicationConstraintParameter.CINoReplication) == DataMoveReplicationConstraintParameter.None)
			{
				return result;
			}
			if (copies.Count > 1)
			{
				using (Dictionary<IADServer, CopyInfo>.Enumerator enumerator = copies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<IADServer, CopyInfo> keyValuePair = enumerator.Current;
						CopyInfo value = keyValuePair.Value;
						if (value.Status != null)
						{
							if (string.Equals(value.Status.ActiveDatabaseCopy, value.Status.MailboxServer, StringComparison.OrdinalIgnoreCase))
							{
								ExTraceGlobals.ActiveManagerClientTracer.TraceDebug<IADServer, Guid, ContentIndexCurrentness>((long)this.GetHashCode(), "GetCIHealthConstraint(): server {0}, databaseGuid {1}, CI Currentness = {1}", keyValuePair.Key, mdb.Guid, value.Status.CICurrentness);
								if (value.Status.CICurrentness == ContentIndexCurrentness.NotCurrent)
								{
									result = ConstraintCheckResultType.NotSatisfied;
									waitTime = DumpsterReplicationStatus.c_healthyWaitTime;
									failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfied(dataMoveReplicationConstraint, mdb.Guid);
								}
							}
						}
						else
						{
							ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "GetCICurrentnessConstraint got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mdb.Name, mdb.Guid, keyValuePair.Key.Name);
						}
					}
					return result;
				}
			}
			KeyValuePair<IADServer, CopyInfo> keyValuePair2 = copies.LastOrDefault<KeyValuePair<IADServer, CopyInfo>>();
			if (keyValuePair2.Value.Status != null)
			{
				if (keyValuePair2.Value.Status.CICurrentness != ContentIndexCurrentness.Current)
				{
					result = ConstraintCheckResultType.NotSatisfied;
					waitTime = DumpsterReplicationStatus.c_healthyWaitTime;
					failureReason = ServerStrings.DataMoveReplicationConstraintNotSatisfiedForNonReplicatedDatabase(dataMoveReplicationConstraint, mdb.Guid);
				}
			}
			else
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<string, Guid, string>((long)this.GetHashCode(), "GetCICurrentnessConstraint got no CopyStatus data for database {0} (Guid={1}) copy on server {2}", mdb.Name, mdb.Guid, keyValuePair2.Key.Name);
			}
			return result;
		}

		private bool TestIntegrationEnabled(Guid databaseGuid)
		{
			string name = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook\\{0}", databaseGuid.ToString());
			int num = 0;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (registryKey != null)
				{
					num = (int)registryKey.GetValue("TestHookEnabled", 0);
				}
			}
			return num != 0;
		}

		private ConstraintCheckResultType TestIntegrationDatabaseConstraintResult(Guid databaseGuid, out TimeSpan waitTime, out LocalizedString failureMessage, out ConstraintCheckAgent agent)
		{
			string name = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook\\{0}", databaseGuid.ToString());
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (registryKey != null)
				{
					string value = (string)registryKey.GetValue("ReturnValue", null);
					if (!string.IsNullOrEmpty(value))
					{
						waitTime = TimeSpan.FromSeconds((double)((int)registryKey.GetValue("WaitTime", 0)));
						failureMessage = new LocalizedString((string)registryKey.GetValue("FailureMessage", "Failure message from test hook"));
						agent = (ConstraintCheckAgent)((int)registryKey.GetValue("Agent", ConstraintCheckAgent.TestHook));
						try
						{
							return (ConstraintCheckResultType)Enum.Parse(typeof(ConstraintCheckResultType), value, true);
						}
						catch (ArgumentException)
						{
						}
					}
				}
			}
			waitTime = TimeSpan.MinValue;
			failureMessage = new LocalizedString(string.Empty);
			agent = ConstraintCheckAgent.None;
			return ConstraintCheckResultType.Satisfied;
		}

		private ConstraintCheckResultType TestHookCheckReplicationFlushed(Guid databaseGuid, out LocalizedString failureMessage)
		{
			string name = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook\\{0}", databaseGuid.ToString());
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (registryKey != null)
				{
					string value = (string)registryKey.GetValue("CheckReplicationFlushedReturnValue", null);
					if (!string.IsNullOrEmpty(value))
					{
						failureMessage = new LocalizedString((string)registryKey.GetValue("FailureMessage", "Failure message from CheckReplicationFlushed test hook"));
						try
						{
							return (ConstraintCheckResultType)Enum.Parse(typeof(ConstraintCheckResultType), value, true);
						}
						catch (ArgumentException)
						{
						}
					}
				}
			}
			failureMessage = new LocalizedString(string.Empty);
			return ConstraintCheckResultType.Satisfied;
		}

		private const string m_TestHookKeyBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\TestHook\\{0}";

		private const string m_TestHookReturnValue = "ReturnValue";

		private const string m_TestHookFailureMessage = "FailureMessage";

		private const string m_TestHookEnabled = "TestHookEnabled";

		private const string m_TestHookWaitTime = "WaitTime";

		private const string m_TestHookAgent = "Agent";

		private const string m_TestHookCheckReplicationFlushedReturnValue = "CheckReplicationFlushedReturnValue";

		public const string ReplayParametersKeyBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		public const string NumberOfLogsKey = "MaximumCopyQueueLengthBeforeThrottle";

		public const string CheckReplicationFlushedTimeSkewSecondsKey = "CheckReplicationFlushedTimeSkewSeconds";

		public const uint CIConstraintMask = 65536U;

		private readonly ITopologyConfigurationSession m_adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 187, "m_adSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ActiveManager\\DumpsterReplicationStatus.cs");

		private static TimeSpan c_defaultRetryWaitTime = TimeSpan.Parse("0:0:10");

		private static TimeSpan c_nothealthyWaitTime = TimeSpan.Parse("0:02:00");

		private static TimeSpan c_healthyWaitTime = TimeSpan.Parse("0:01:00");

		private static TimeSpan c_maxWaitTime = TimeSpan.Parse("0:10:00");

		private static TimeSpan c_replayExtraLag = TimeSpan.Parse("0:02:00");

		private static TimeSpan c_serverCacheExpiryTime = TimeSpan.FromSeconds(45.0);

		public static int MaximumNumberOfLogsInCopyQueue = 10;

		private ConcurrentDictionary<string, KeyValuePair<DateTime, IADServer>> serverByNameHash = new ConcurrentDictionary<string, KeyValuePair<DateTime, IADServer>>();
	}
}
