using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBestCopySelector
	{
		public AmBestCopySelector(AmBcsContext bcsContext)
		{
			this.m_bcsContext = bcsContext;
		}

		public Exception LastException
		{
			get
			{
				if (this.m_lastException == null)
				{
					return this.Context.ErrorLogger.GetLastException();
				}
				return this.m_lastException;
			}
		}

		internal AmBcsContext Context
		{
			get
			{
				return this.m_bcsContext;
			}
		}

		public AmServerName FindNextBestCopy()
		{
			AmServerName amServerName = null;
			AmBcsChecks amBcsChecks = AmBcsChecks.None;
			try
			{
				this.Initialize();
				List<AmBcsChecks> list = new List<AmBcsChecks>
				{
					AmBcsChecks.ManagedAvailabilityAllHealthy,
					AmBcsChecks.ManagedAvailabilityUptoNormalHealthy,
					AmBcsChecks.ManagedAvailabilityAllBetterThanSource,
					AmBcsChecks.ManagedAvailabilitySameAsSource
				};
				if (!this.m_bcsContext.ActionCode.IsAutomaticManagedAvailabilityFailover)
				{
					list.Add(AmBcsChecks.None);
				}
				else
				{
					amBcsChecks = AmBcsChecks.ManagedAvailabilityInitiatorBetterThanSource;
				}
				if (this.m_bcsContext.DatabaseNeverMounted)
				{
					AmTrace.Debug("FindNextBestCopy: Database '{0}' has never been mounted. Running non-status related checks.", new object[]
					{
						this.m_bcsContext.GetDatabaseNameOrGuid()
					});
					if (amServerName == null)
					{
						amServerName = this.RunPhaseN(AmBcsChecks.IsPassiveCopy);
					}
				}
				else
				{
					if (this.m_bcsContext.ActionCode.IsAdminMoveOperation)
					{
						AmBcsChecks amBcsChecks2 = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;
						AmBcsChecks amBcsChecks3 = AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit;
						using (List<AmBcsChecks>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								AmBcsChecks amBcsChecks4 = enumerator.Current;
								AmBcsChecks amBcsChecks5 = amBcsChecks4 | amBcsChecks;
								if (amServerName == null)
								{
									AmBcsChecks amBcsChecks6 = amBcsChecks2 | amBcsChecks5;
									AmTrace.Debug("FindNextBestCopy: IsAdminMoveOperation='true', so first attempting '{0}' checks.", new object[]
									{
										amBcsChecks6
									});
									amServerName = this.RunPhaseN(amBcsChecks6);
								}
								if (amServerName == null)
								{
									AmBcsChecks amBcsChecks7 = amBcsChecks3 | amBcsChecks5;
									AmTrace.Debug("FindNextBestCopy: IsAdminMoveOperation='true', so second attempting '{0}' checks.", new object[]
									{
										amBcsChecks7
									});
									amServerName = this.RunPhaseN(amBcsChecks7);
								}
								if (amServerName != null)
								{
									break;
								}
							}
							goto IL_2FA;
						}
					}
					if (this.m_bcsContext.ActionCode.Reason == AmDbActionReason.CatalogFailureItem)
					{
						AmTrace.Debug("FindNextBestCopy: Catalog induced failover, so a healthy catalog must be found", new object[0]);
						using (List<AmBcsChecks>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AmBcsChecks amBcsChecks8 = enumerator2.Current;
								if (amServerName == null)
								{
									amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks8);
								}
								if (amServerName == null)
								{
									amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks8);
								}
								if (amServerName == null)
								{
									amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks8);
								}
								if (amServerName == null)
								{
									amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks8);
								}
								if (amServerName != null)
								{
									break;
								}
							}
							goto IL_2FA;
						}
					}
					foreach (AmBcsChecks amBcsChecks9 in list)
					{
						AmBcsChecks amBcsChecks10 = amBcsChecks9 | amBcsChecks;
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderPreferredLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.CopyQueueLength | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName == null)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusCrawling | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.ActivationEnabled | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName == null && amBcsChecks10 == AmBcsChecks.None)
						{
							amServerName = this.RunPhaseN(AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsPassiveCopy | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.MaxActivesUnderHighestLimit | amBcsChecks10);
						}
						if (amServerName != null)
						{
							break;
						}
					}
				}
				IL_2FA:;
			}
			finally
			{
				if (!AmServerName.IsNullOrEmpty(amServerName))
				{
					this.AddAttemptedServer(amServerName);
				}
			}
			if (this.m_attemptedServers == null || this.m_attemptedServers.Count == 0)
			{
				AmTrace.Error("FindNextBestCopy: Database '{0}' has no possible copies for activation.", new object[]
				{
					this.m_bcsContext.GetDatabaseNameOrGuid()
				});
				if (this.Context.ErrorLogger.GetLastException() == null)
				{
					Exception ex = new AmDbNotMountedNoViableServersException(this.m_bcsContext.GetDatabaseNameOrGuid());
					this.m_lastException = new AmBcsSelectionException(ex.Message, ex);
				}
			}
			return amServerName;
		}

		protected virtual Dictionary<AmServerName, int> BuildActivationPreferenceCache(Guid dbGuid, ref IADDatabase database)
		{
			IADDatabaseCopy[] databaseCopies = AmBestCopySelectionHelper.GetDatabaseCopies(dbGuid, ref database);
			Dictionary<AmServerName, int> dictionary = new Dictionary<AmServerName, int>(databaseCopies.Length);
			foreach (IADDatabaseCopy iaddatabaseCopy in databaseCopies)
			{
				AmServerName key = new AmServerName(iaddatabaseCopy.HostServerName);
				dictionary[key] = iaddatabaseCopy.ActivationPreference;
			}
			return dictionary;
		}

		protected virtual IOrderedEnumerable<KeyValuePair<AmServerName, RpcDatabaseCopyStatus2>> SortByCopyQueueAndActivationPreference(Dictionary<AmServerName, RpcDatabaseCopyStatus2> statusTable)
		{
			bool flag = false;
			if (this.m_bcsContext.SortCopiesByActivationPreference)
			{
				flag = true;
				AmTrace.Debug("SortByCopyQueueAndActivationPreference: Sorting only by ActivationPreference since DB {0} is undergoing a lossless move/failover", new object[]
				{
					this.m_bcsContext.DatabaseGuid
				});
			}
			else if (this.m_bcsContext.DatabaseNeverMounted)
			{
				flag = true;
				AmTrace.Debug("SortByCopyQueueAndActivationPreference: Sorting only by ActivationPreference since DB {0} has never been mounted", new object[]
				{
					this.m_bcsContext.DatabaseGuid
				});
			}
			IOrderedEnumerable<KeyValuePair<AmServerName, RpcDatabaseCopyStatus2>> result;
			if (flag)
			{
				AmTrace.Debug("SortByCopyQueueAndActivationPreference: Sorting by ActivationPreference ONLY for DB {0}", new object[]
				{
					this.m_bcsContext.DatabaseGuid
				});
				result = from kvp in statusTable
				orderby this.IsServerActive(kvp.Key) descending, this.GetActivationPreference(kvp.Key)
				select kvp;
			}
			else
			{
				AmTrace.Debug("SortByCopyQueueAndActivationPreference: Sorting by LastLogInspected and ActivationPreference for DB {0}", new object[]
				{
					this.m_bcsContext.DatabaseGuid
				});
				result = from kvp in statusTable
				orderby this.IsServerActive(kvp.Key) descending, kvp.Value.LastLogInspected descending, this.GetActivationPreference(kvp.Key)
				select kvp;
			}
			return result;
		}

		private void Initialize()
		{
			if (this.m_activationPreferenceCache == null)
			{
				IADDatabase database = this.m_bcsContext.Database;
				this.m_activationPreferenceCache = this.BuildActivationPreferenceCache(this.m_bcsContext.DatabaseGuid, ref database);
				if (database != null)
				{
					this.m_bcsContext.Database = database;
				}
			}
			if (this.m_sortedStatusCollection == null)
			{
				this.m_sortedStatusCollection = this.SortByCopyQueueAndActivationPreference(this.m_bcsContext.StatusTable);
			}
			if (this.m_attemptedServers == null)
			{
				this.m_attemptedServers = new HashSet<AmServerName>();
			}
		}

		private void AddAttemptedServer(AmServerName attemptedServer)
		{
			this.m_attemptedServers.Add(attemptedServer);
		}

		private int IsServerActive(AmServerName server)
		{
			if (!AmServerName.IsEqual(server, this.m_bcsContext.SourceServerName))
			{
				return 0;
			}
			return 1;
		}

		private int GetActivationPreference(AmServerName server)
		{
			AmTrace.Debug("AmBestCopySelector.GetActivationPreference: Server ({0})", new object[]
			{
				server
			});
			if (this.m_activationPreferenceCache.ContainsKey(server))
			{
				return this.m_activationPreferenceCache[server];
			}
			return int.MaxValue;
		}

		private AmServerName RunPhaseN(AmBcsChecks checks)
		{
			bool flag = false;
			AmServerName result = null;
			AmServerName amServerName = null;
			if (this.m_bcsContext.ActionCode.Reason == AmDbActionReason.ActivationDisabled)
			{
				checks |= AmBcsChecks.ActivationEnabled;
			}
			if (this.m_bcsContext.ActionCode.IsMountOrRemountOperation || this.m_bcsContext.ActionCode.IsAdminMoveOperation)
			{
				amServerName = this.m_bcsContext.SourceServerName;
				if (this.m_bcsContext.IsSourceServerAllowedForMount)
				{
					flag = this.RunChecksWrapper(checks, out result, amServerName, null);
				}
				else
				{
					AmTrace.Debug("BCS: RunPhaseN: Ignoring source server {0} since determine servers did not allow it.", new object[]
					{
						amServerName
					});
				}
			}
			if (!flag)
			{
				foreach (KeyValuePair<AmServerName, RpcDatabaseCopyStatus2> keyValuePair in this.m_sortedStatusCollection)
				{
					if (!AmServerName.IsEqual(amServerName, keyValuePair.Key))
					{
						flag = this.RunChecksWrapper(checks, out result, keyValuePair.Key, keyValuePair.Value);
						if (flag)
						{
							break;
						}
					}
				}
			}
			return result;
		}

		private bool RunChecksWrapper(AmBcsChecks checks, out AmServerName selectedCopy, AmServerName targetServer, RpcDatabaseCopyStatus2 copyStatus)
		{
			LocalizedString empty = LocalizedString.Empty;
			selectedCopy = null;
			AmBcsChecks amBcsChecks;
			bool flag = this.RunChecks(this.m_bcsContext.SourceServerName, targetServer, copyStatus, checks, out amBcsChecks, ref empty);
			if (flag)
			{
				AmTrace.Info("BCS: DatabaseCopy '{0}' on server '{1}' passed checks: {2}", new object[]
				{
					this.m_bcsContext.GetDatabaseNameOrGuid(),
					targetServer,
					checks
				});
				selectedCopy = targetServer;
				ReplayCrimsonEvents.BcsDbNodeChecksPassed.Log<string, Guid, AmServerName, AmBcsChecks>(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_bcsContext.DatabaseGuid, targetServer, amBcsChecks);
			}
			else
			{
				AmTrace.Error("BCS: DatabaseCopy '{0}' on server '{1}' failed checks: {2}", new object[]
				{
					this.m_bcsContext.GetDatabaseNameOrGuid(),
					targetServer,
					checks
				});
			}
			return flag;
		}

		private bool RunChecks(AmServerName sourceServer, AmServerName targetServer, RpcDatabaseCopyStatus2 status, AmBcsChecks amBcsChecks, out AmBcsChecks completedChecks, ref LocalizedString error)
		{
			bool flag = this.HasNotBeenTried(targetServer, ref error);
			completedChecks = AmBcsChecks.None;
			if (flag)
			{
				if (this.m_bcsContext.ActionCode.IsMountOrRemountOperation && AmServerName.IsEqual(sourceServer, targetServer))
				{
					AmTrace.Debug("BCS: Target server '{0}' is skipping validation checks to allow mount to take place for database '{1}'.", new object[]
					{
						targetServer.NetbiosName,
						this.m_bcsContext.GetDatabaseNameOrGuid()
					});
					flag = true;
				}
				else
				{
					AmBcsCopyValidation amBcsCopyValidation = new AmBcsCopyValidation(this.m_bcsContext.DatabaseGuid, this.m_bcsContext.GetDatabaseNameOrGuid(), amBcsChecks, sourceServer, targetServer, status, this.m_bcsContext.ErrorLogger, this.m_bcsContext.SkipValidationChecks, this.m_bcsContext.ComponentStateWrapper);
					flag = amBcsCopyValidation.RunChecks(ref error);
					completedChecks = amBcsCopyValidation.CompletedChecks;
				}
			}
			else
			{
				AmTrace.Debug("BCS: Target server '{0}' has already been tried for database '{1}'.", new object[]
				{
					targetServer.NetbiosName,
					this.m_bcsContext.GetDatabaseNameOrGuid()
				});
			}
			return flag;
		}

		private bool HasNotBeenTried(AmServerName server, ref LocalizedString error)
		{
			bool flag = this.m_attemptedServers.Contains(server);
			if (flag)
			{
				error = ReplayStrings.AmBcsDatabaseCopyAlreadyTried(this.m_bcsContext.GetDatabaseNameOrGuid(), server.Fqdn);
				this.m_bcsContext.ErrorLogger.ReportServerFailure(server, "CopyHasBeenTriedCheck", error, false);
			}
			return !flag;
		}

		private readonly AmBcsContext m_bcsContext;

		private Dictionary<AmServerName, int> m_activationPreferenceCache;

		private HashSet<AmServerName> m_attemptedServers;

		private IOrderedEnumerable<KeyValuePair<AmServerName, RpcDatabaseCopyStatus2>> m_sortedStatusCollection;

		private Exception m_lastException;
	}
}
