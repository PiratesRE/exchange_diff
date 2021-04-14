using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Mserve;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.EdgeSync.Logging;
using Microsoft.Exchange.Net.Mserve;

namespace Microsoft.Exchange.EdgeSync.Mserve
{
	internal class MserveTargetConnection : DatacenterTargetConnection
	{
		public MserveTargetConnection(int localServerVersion, MserveTargetServerConfig config, EnhancedTimeSpan syncInterval, TestShutdownAndLeaseDelegate testShutdownAndLease, EdgeSyncLogSession logSession) : base(localServerVersion, config, syncInterval, logSession, ExTraceGlobals.TargetConnectionTracer)
		{
			this.testShutdownAndLease = testShutdownAndLease;
			this.config = config;
			this.InitializeTenantMEUSyncControlCache();
			this.InitializeClientToken();
			this.configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 313, ".ctor", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\Mserve\\MserveTargetConnection.cs");
			if (EdgeSyncSvc.EdgeSync != null && EdgeSyncSvc.EdgeSync.AppConfig != null)
			{
				this.duplicatedAddEntriesCacheSize = EdgeSyncSvc.EdgeSync.AppConfig.DuplicatedAddEntriesCacheSize;
				this.podSiteStartRange = EdgeSyncSvc.EdgeSync.AppConfig.PodSiteStartRange;
				this.podSiteEndRange = EdgeSyncSvc.EdgeSync.AppConfig.PodSiteEndRange;
				this.trackDuplicatedAddEntries = EdgeSyncSvc.EdgeSync.AppConfig.TrackDuplicatedAddEntries;
				if (!this.trackDuplicatedAddEntries)
				{
					this.duplicatedAddEntriesCacheSize = 0;
				}
			}
		}

		private void InitializeTenantMEUSyncControlCache()
		{
			if (MserveTargetConnection.tenantSyncControlCache == null)
			{
				if (EdgeSyncSvc.EdgeSync != null && EdgeSyncSvc.EdgeSync.AppConfig != null)
				{
					MserveTargetConnection.tenantSyncControlCache = new Cache<string, MserveTargetConnection.TenantSyncControl>(EdgeSyncSvc.EdgeSync.AppConfig.TenantSyncControlCacheSize, EdgeSyncSvc.EdgeSync.AppConfig.TenantSyncControlCacheExpiryInterval, EdgeSyncSvc.EdgeSync.AppConfig.TenantSyncControlCacheCleanupInterval, EdgeSyncSvc.EdgeSync.AppConfig.TenantSyncControlCachePurgeInterval, new MserveTargetConnection.TenantSyncControlCacheLogger<string>(EdgeSyncSvc.EdgeSync.EdgeSyncLogSession), null);
					return;
				}
				MserveTargetConnection.tenantSyncControlCache = new Cache<string, MserveTargetConnection.TenantSyncControl>(1048576L, TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(5.0), null, null);
			}
		}

		private void InitializeClientToken()
		{
			if (string.IsNullOrEmpty(MserveTargetConnection.clientToken))
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					MserveTargetConnection.clientToken = EdgeSyncMservConnector.GetMserveWebServiceClientTokenFromEndpointConfig(null);
					if (string.IsNullOrEmpty(MserveTargetConnection.clientToken))
					{
						throw new ExDirectoryException("Client token from Endpoint configuration is null or empty", null);
					}
				}, 3);
				if (!adoperationResult.Succeeded)
				{
					throw new ExDirectoryException("Unable to read client token from Endpoint configuration", adoperationResult.Exception);
				}
			}
		}

		protected override string LeaseFileName
		{
			get
			{
				return "mserv.lease";
			}
		}

		protected override IConfigurationSession ConfigSession
		{
			get
			{
				return this.configSession;
			}
		}

		protected IMserveService MserveService
		{
			get
			{
				return this.mserveService;
			}
			set
			{
				this.mserveService = value;
			}
		}

		public static ADObjectId GetCookieContainerId(IConfigurationSession notUsed)
		{
			return ADSession.GetConfigurationUnitsRootForLocalForest();
		}

		public override bool OnSynchronizing()
		{
			MserveEdgeSyncService mserveEdgeSyncService = null;
			try
			{
				mserveEdgeSyncService = new MserveEdgeSyncService(this.config.ProvisioningUrl, this.config.SettingsUrl, this.config.RemoteCertSubject, MserveTargetConnection.clientToken, base.LogSession, true, this.trackDuplicatedAddEntries);
				mserveEdgeSyncService.Initialize();
			}
			catch (MserveException ex)
			{
				ExTraceGlobals.TargetConnectionTracer.TraceError<string>((long)this.GetHashCode(), "Failed to initialize MserveWebService because of {0}", ex.Message);
				base.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, ex, "Failed to initialize MserveWebService");
				return false;
			}
			this.mserveService = mserveEdgeSyncService;
			return true;
		}

		public override void OnConnectedToSource(Connection sourceConnection)
		{
			ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>((long)this.GetHashCode(), "Connected to the source Domain Controller {0}", sourceConnection.Fqdn);
			this.sourceConnection = sourceConnection;
		}

		public override bool OnSynchronized()
		{
			List<RecipientSyncOperation> results = null;
			try
			{
				results = this.mserveService.Synchronize();
			}
			catch (MserveException ex)
			{
				ExTraceGlobals.TargetConnectionTracer.TraceError<string>((long)this.GetHashCode(), "MserveWebService failed to flush operations because of {0}", ex.Message);
				base.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, ex, "MserveWebService failed to flush");
				return false;
			}
			MserveTargetConnection.ProcessSyncResult processSyncResult = this.ProcessSyncResults(results);
			if (processSyncResult != MserveTargetConnection.ProcessSyncResult.Success)
			{
				base.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, new ExDirectoryException("ProcessSyncResults " + processSyncResult, null), "Failed OnSynchronized");
			}
			return processSyncResult == MserveTargetConnection.ProcessSyncResult.Success && this.ResolveDuplicateAddedEntries();
		}

		public override SyncResult OnAddEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			this.OnEntryChanged(entry);
			return SyncResult.Added;
		}

		public override SyncResult OnModifyEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes)
		{
			this.OnEntryChanged(entry);
			return SyncResult.Modified;
		}

		public override SyncResult OnDeleteEntry(ExSearchResultEntry entry)
		{
			this.OnEntryChanged(entry);
			return SyncResult.Deleted;
		}

		public override SyncResult OnRenameEntry(ExSearchResultEntry entry)
		{
			return SyncResult.Renamed;
		}

		public override void Dispose()
		{
			if (this.updateConnection != null)
			{
				this.updateConnection.Dispose();
				this.updateConnection = null;
			}
		}

		public void FilterSmtpProxyAddressesBasedOnTenantSetting(ExSearchResultEntry entry, RecipientTypeDetails recipientTypeDetail)
		{
			DirectoryAttribute directoryAttribute = null;
			if (!entry.Attributes.TryGetValue("msExchCU", out directoryAttribute))
			{
				throw new ExDirectoryException("TenantCU is missing for the user", null);
			}
			if (directoryAttribute == null || directoryAttribute.Count <= 0)
			{
				throw new ExDirectoryException("TenantCU has empty value for the user", null);
			}
			string text = directoryAttribute[0] as string;
			if (string.IsNullOrEmpty(text))
			{
				throw new ExDirectoryException("TenantCU attribute is not string value", null);
			}
			ADObjectId tenantCUId = null;
			try
			{
				tenantCUId = new ADObjectId(text);
			}
			catch (FormatException e)
			{
				throw new ExDirectoryException("TenantCU DN is of invalid format as " + text, e);
			}
			bool flag = MserveTargetConnection.IsEntryMailEnabledUser(entry, recipientTypeDetail);
			MserveTargetConnection.TenantSyncControl tenantSyncControlAndUpdateCache = this.GetTenantSyncControlAndUpdateCache(tenantCUId);
			if ((flag && !tenantSyncControlAndUpdateCache.SyncMEUSMTPToMServ) || (!flag && !tenantSyncControlAndUpdateCache.SyncMailboxSMTPToMserv))
			{
				entry.Attributes["proxyAddresses"] = new DirectoryAttribute("proxyAddresses", MserveSynchronizationProvider.EmptyList);
			}
		}

		protected override ADObjectId GetCookieContainerId()
		{
			if (MserveTargetConnection.configUnitsId == null)
			{
				MserveTargetConnection.configUnitsId = MserveTargetConnection.GetCookieContainerId(this.ConfigSession);
			}
			return MserveTargetConnection.configUnitsId;
		}

		protected List<RecipientSyncOperation> GetRecipientSyncOperation(ExSearchResultEntry entry)
		{
			List<RecipientSyncOperation> list = new List<RecipientSyncOperation>();
			ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "Try to GetRecipientSyncOperation for {0}", entry.DistinguishedName);
			RecipientSyncState recipientSyncState = null;
			if (entry.Attributes.ContainsKey("msExchExternalSyncState"))
			{
				byte[] bytes = Encoding.ASCII.GetBytes((string)entry.Attributes["msExchExternalSyncState"][0]);
				recipientSyncState = RecipientSyncState.DeserializeRecipientSyncState(bytes);
				ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "{0} has existing syncState", entry.DistinguishedName);
			}
			if (recipientSyncState == null)
			{
				if (entry.IsDeleted)
				{
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "{0} is deleted entry without syncState. Ignore the entry", entry.DistinguishedName);
					return list;
				}
				ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "{0} is a normal entry without syncState. Creating one", entry.DistinguishedName);
				recipientSyncState = new RecipientSyncState();
			}
			if (!entry.IsDeleted)
			{
				int partnerId = MserveSynchronizationProvider.PartnerId;
				if (partnerId == -1)
				{
					ExTraceGlobals.TargetConnectionTracer.TraceError<string>(0L, "Failed the sync because we could not get the partner Id for {0}", entry.DistinguishedName);
					throw new ExDirectoryException(new ArgumentException("Failed the sync because we could not get the partner Id for " + entry.DistinguishedName));
				}
				int num = (recipientSyncState.PartnerId != 0) ? recipientSyncState.PartnerId : partnerId;
				recipientSyncState.PartnerId = partnerId;
				if (num != partnerId)
				{
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<string, int, int>(0L, "{0}'s partnerId changed from {1} to {2}", entry.DistinguishedName, num, partnerId);
					base.LogSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, entry.DistinguishedName, "Warning: Changing partner ID from " + num.ToString() + " to " + partnerId.ToString());
				}
				else
				{
					RecipientSyncOperation recipientSyncOperation = new RecipientSyncOperation(entry.DistinguishedName, partnerId, recipientSyncState, false);
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<int>(0L, "Create new operation with partner Id {0}", partnerId);
					foreach (string key in MserveTargetConnection.ReplicationAddressAttributes)
					{
						if (entry.Attributes.ContainsKey(key))
						{
							MserveTargetConnection.OnAddressChange(entry.Attributes[key], recipientSyncOperation);
						}
					}
					if (recipientSyncOperation.RemovedEntries.Count != 0 || recipientSyncOperation.AddedEntries.Count != 0)
					{
						list.Add(recipientSyncOperation);
					}
				}
			}
			else
			{
				int partnerId2 = recipientSyncState.PartnerId;
				if (partnerId2 != 0)
				{
					RecipientSyncOperation recipientSyncOperation2 = new RecipientSyncOperation(entry.DistinguishedName, partnerId2, null, true);
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<int>(0L, "Create remove operation with partner Id {0}", partnerId2);
					foreach (string text in MserveTargetConnection.ReplicationAddressAttributes)
					{
						string recipientSyncStateAttribute = MserveTargetConnection.GetRecipientSyncStateAttribute(recipientSyncState, text);
						if (recipientSyncStateAttribute != null)
						{
							List<string> list2 = RecipientSyncState.AddressToList(recipientSyncStateAttribute);
							if (this.CanRemove(entry.DistinguishedName, text, list2))
							{
								foreach (string text2 in list2)
								{
									recipientSyncOperation2.RemovedEntries.Add(text2);
									ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "Add {0} to RemovedEntries", text2);
								}
							}
						}
					}
					if (recipientSyncOperation2.RemovedEntries.Count != 0)
					{
						list.Add(recipientSyncOperation2);
					}
				}
				else
				{
					ExTraceGlobals.TargetConnectionTracer.TraceDebug(0L, "No partner Id present on syncState. Skip the recipient");
				}
			}
			return list;
		}

		public void UpdateRecipientSyncStateValue(RecipientSyncOperation operation)
		{
			Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>(MserveTargetConnection.ReplicationAddressAttributes.Length, StringComparer.OrdinalIgnoreCase);
			foreach (string text in MserveTargetConnection.ReplicationAddressAttributes)
			{
				string recipientSyncStateAttribute = MserveTargetConnection.GetRecipientSyncStateAttribute(operation.RecipientSyncState, text);
				dictionary[text] = RecipientSyncState.AddressHashSetFromConcatStringValue(recipientSyncStateAttribute);
			}
			foreach (OperationType key in operation.PendingSyncStateCommitEntries.Keys)
			{
				foreach (string text2 in operation.PendingSyncStateCommitEntries[key])
				{
					string key2 = null;
					if (!operation.AddressTypeTable.TryGetValue(text2, out key2))
					{
						throw new InvalidOperationException(text2 + " is not in AddressTypeTable");
					}
					switch (key)
					{
					case OperationType.Add:
						if (!dictionary[key2].Contains(text2))
						{
							dictionary[key2].Add(text2);
						}
						break;
					case OperationType.Delete:
						if (dictionary[key2].Contains(text2))
						{
							dictionary[key2].Remove(text2);
						}
						break;
					}
				}
			}
			foreach (string text3 in MserveTargetConnection.ReplicationAddressAttributes)
			{
				MserveTargetConnection.SetRecipientSyncStateAttribute(operation.RecipientSyncState, text3, RecipientSyncState.AddressHashSetToConcatStringValue(dictionary[text3]));
			}
		}

		protected virtual void UpdateRecipientSyncStateValueInAD(RecipientSyncOperation operation)
		{
			if (this.updateConnection == null)
			{
				ADObjectId rootId = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					PooledLdapConnection readConnection = this.ConfigSession.GetReadConnection(this.sourceConnection.Fqdn, ref rootId);
					this.updateConnection = new Connection(readConnection, EdgeSyncSvc.EdgeSync.AppConfig);
				}, 3);
				if (!adoperationResult.Succeeded)
				{
					ExTraceGlobals.TargetConnectionTracer.TraceError<string>((long)this.GetHashCode(), "Failed to get AD connection to update SyncState because of {0}", adoperationResult.Exception.Message);
					throw new ExDirectoryException("Failed to get AD connection to update SyncState", adoperationResult.Exception);
				}
			}
			byte[] array = RecipientSyncState.SerializeRecipientSyncState(operation.RecipientSyncState);
			ModifyRequest request = new ModifyRequest(operation.DistinguishedName, DirectoryAttributeOperation.Replace, "msExchExternalSyncState", new object[]
			{
				array
			});
			this.updateConnection.SendRequest(request);
			ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>((long)this.GetHashCode(), "Successfully updated SyncState in AD for {0}", operation.DistinguishedName);
			base.LogSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, operation.DistinguishedName, "Successfully synced to MSERV and updated SyncState");
		}

		private static void SetRecipientSyncStateAttribute(RecipientSyncState syncState, string name, string value)
		{
			if (name.Equals("proxyAddresses", StringComparison.OrdinalIgnoreCase))
			{
				syncState.ProxyAddresses = value;
				return;
			}
			if (name.Equals("msExchSignupAddresses", StringComparison.OrdinalIgnoreCase))
			{
				syncState.SignupAddresses = value;
				return;
			}
			if (name.Equals("msExchUMAddresses", StringComparison.OrdinalIgnoreCase))
			{
				syncState.UMProxyAddresses = value;
				return;
			}
			if (name.Equals("ArchiveAddress", StringComparison.OrdinalIgnoreCase))
			{
				syncState.ArchiveAddress = value;
				return;
			}
			throw new ArgumentOutOfRangeException(name);
		}

		private static string GetRecipientSyncStateAttribute(RecipientSyncState syncState, string name)
		{
			if (name.Equals("proxyAddresses", StringComparison.OrdinalIgnoreCase))
			{
				return syncState.ProxyAddresses;
			}
			if (name.Equals("msExchSignupAddresses", StringComparison.OrdinalIgnoreCase))
			{
				return syncState.SignupAddresses;
			}
			if (name.Equals("msExchUMAddresses", StringComparison.OrdinalIgnoreCase))
			{
				return syncState.UMProxyAddresses;
			}
			if (name.Equals("ArchiveAddress", StringComparison.OrdinalIgnoreCase))
			{
				return syncState.ArchiveAddress;
			}
			throw new ArgumentOutOfRangeException(name);
		}

		private static void OnAddressChange(DirectoryAttribute attribute, RecipientSyncOperation operation)
		{
			HashSet<string> hashSet = RecipientSyncState.AddressHashSetFromConcatStringValue(MserveTargetConnection.GetRecipientSyncStateAttribute(operation.RecipientSyncState, attribute.Name));
			foreach (object obj in attribute)
			{
				string text = (string)obj;
				string text2;
				if (text.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
				{
					text2 = text.Substring(5);
				}
				else if (text.StartsWith("meum:", StringComparison.OrdinalIgnoreCase))
				{
					text2 = text.Substring(5);
				}
				else
				{
					text2 = text;
				}
				if (!hashSet.Contains(text2))
				{
					operation.AddedEntries.Add(text2);
					operation.AddressTypeTable[text2] = attribute.Name;
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "Add {0} to AddedEntries", text2);
				}
				else
				{
					hashSet.Remove(text2);
				}
			}
			foreach (string text3 in hashSet)
			{
				operation.RemovedEntries.Add(text3);
				operation.AddressTypeTable[text3] = attribute.Name;
				ExTraceGlobals.TargetConnectionTracer.TraceDebug<string>(0L, "Add {0} to RemovedEntries", text3);
			}
		}

		private static bool IsEntryMailEnabledUser(ExSearchResultEntry entry, RecipientTypeDetails recipientTypeDetail)
		{
			if (recipientTypeDetail != RecipientTypeDetails.None)
			{
				return recipientTypeDetail == RecipientTypeDetails.MailUser;
			}
			return entry.Attributes.ContainsKey("mailNickname") && !entry.Attributes.ContainsKey("msExchHomeServerName");
		}

		private MserveTargetConnection.TenantSyncControl GetTenantSyncControlSettingFromAD(ADObjectId tenantCUId, string key)
		{
			ExchangeConfigurationUnit tenantCU = null;
			int tryCount = 0;
			string savedDomainController = this.ConfigSession.DomainController;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				if (tryCount++ == 0)
				{
					this.ConfigSession.DomainController = this.sourceConnection.Fqdn;
				}
				else
				{
					this.ConfigSession.DomainController = savedDomainController;
				}
				tenantCU = this.ConfigSession.Read<ExchangeConfigurationUnit>(tenantCUId);
			}, 2);
			this.ConfigSession.DomainController = savedDomainController;
			if (!adoperationResult.Succeeded)
			{
				throw new ExDirectoryException(string.Format("Failed to read user's ExchangeConfigurationUnit {0}", tenantCUId.DistinguishedName), adoperationResult.Exception);
			}
			if (tenantCU == null)
			{
				throw new ExDirectoryException(string.Format("Failed to read user's ExchangeConfigurationUnit {0} because AD returns null", tenantCUId.DistinguishedName), null);
			}
			base.LogSession.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.TargetConnection, tenantCUId.DistinguishedName, "OrgStatus:" + tenantCU.OrganizationStatus);
			if (!tenantCU.IsOrganizationReadyForMservSync)
			{
				throw new ExDirectoryException(string.Format("Warning: ExchangeConfigurationUnit {0} with OrgStatus {1} is not ready for Mserv Sync yet.", tenantCUId.DistinguishedName, tenantCU.OrganizationStatus), null);
			}
			return new MserveTargetConnection.TenantSyncControl(tenantCU.SyncMEUSMTPToMServ, tenantCU.SyncMBXAndDLToMServ);
		}

		private MserveTargetConnection.TenantSyncControl GetTenantSyncControlAndUpdateCache(ADObjectId tenantCUId)
		{
			string unescapedName = tenantCUId.AncestorDN(1).Rdn.UnescapedName;
			MserveTargetConnection.TenantSyncControl tenantSyncControl = null;
			bool flag = false;
			if (!MserveTargetConnection.tenantSyncControlCache.TryGetValue(unescapedName, out tenantSyncControl, out flag) || flag)
			{
				tenantSyncControl = this.GetTenantSyncControlSettingFromAD(tenantCUId, unescapedName);
				MserveTargetConnection.tenantSyncControlCache.TryAdd(unescapedName, tenantSyncControl);
			}
			return tenantSyncControl;
		}

		private void OnEntryChanged(ExSearchResultEntry entry)
		{
			List<RecipientSyncOperation> recipientSyncOperation = this.GetRecipientSyncOperation(entry);
			List<RecipientSyncOperation> results = null;
			try
			{
				results = this.mserveService.Synchronize(recipientSyncOperation);
			}
			catch (MserveException e)
			{
				throw new ExDirectoryException("Mserve synchronization failed", e);
			}
			MserveTargetConnection.ProcessSyncResult processSyncResult = this.ProcessSyncResults(results);
			if (processSyncResult != MserveTargetConnection.ProcessSyncResult.Success)
			{
				throw new ExDirectoryException("OnEntryChanged failed with ProcessSyncResults " + processSyncResult, null);
			}
		}

		private MserveTargetConnection.ProcessSyncResult ProcessSyncResults(List<RecipientSyncOperation> results)
		{
			MserveTargetConnection.ProcessSyncResult processSyncResult = MserveTargetConnection.ProcessSyncResult.Success;
			foreach (RecipientSyncOperation recipientSyncOperation in results)
			{
				if (recipientSyncOperation.HasNonRetryableErrors)
				{
					this.LogFailedAddresses(recipientSyncOperation.NonRetryableEntries);
				}
				if (recipientSyncOperation.HasRetryableErrors)
				{
					this.LogFailedAddresses(recipientSyncOperation.RetryableEntries);
					processSyncResult |= MserveTargetConnection.ProcessSyncResult.FailedMServ;
				}
				if (recipientSyncOperation.DuplicatedAddEntries.Count > 0 && this.operationsWithDuplicatedAddEntries.Count <= this.duplicatedAddEntriesCacheSize)
				{
					this.operationsWithDuplicatedAddEntries.Add(recipientSyncOperation);
				}
				if (!recipientSyncOperation.SuppressSyncStateUpdate && recipientSyncOperation.TotalPendingSyncStateCommitEntries != 0)
				{
					this.UpdateRecipientSyncStateValue(recipientSyncOperation);
					try
					{
						this.UpdateRecipientSyncStateValueInAD(recipientSyncOperation);
						foreach (OperationType key in recipientSyncOperation.PendingSyncStateCommitEntries.Keys)
						{
							recipientSyncOperation.PendingSyncStateCommitEntries[key].Clear();
						}
					}
					catch (ExDirectoryException exception)
					{
						base.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, exception, "Failed to update SyncState for " + recipientSyncOperation.DistinguishedName);
						processSyncResult |= MserveTargetConnection.ProcessSyncResult.FailedUpdateSyncState;
					}
					if (this.testShutdownAndLease())
					{
						processSyncResult |= MserveTargetConnection.ProcessSyncResult.ShutdownOrLostLease;
						return processSyncResult;
					}
				}
			}
			return processSyncResult;
		}

		private bool ResolveDuplicateAddedEntries()
		{
			if (this.operationsWithDuplicatedAddEntries.Count == 0)
			{
				return true;
			}
			int num = 0;
			foreach (RecipientSyncOperation recipientSyncOperation in this.operationsWithDuplicatedAddEntries)
			{
				num += recipientSyncOperation.DuplicatedAddEntries.Count;
			}
			base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, "ResolveDuplicateAddedEntries: Count=" + this.operationsWithDuplicatedAddEntries.Count);
			bool result;
			try
			{
				this.mserveService.Reset();
				this.mserveService.TrackDuplicatedAddEntries = false;
				List<RecipientSyncOperation> list = new List<RecipientSyncOperation>();
				List<RecipientSyncOperation> list2 = null;
				foreach (RecipientSyncOperation recipientSyncOperation2 in this.operationsWithDuplicatedAddEntries)
				{
					foreach (string item in recipientSyncOperation2.DuplicatedAddEntries)
					{
						list.Add(new RecipientSyncOperation
						{
							ReadEntries = 
							{
								item
							}
						});
					}
				}
				base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, "ResolveDuplicateAddedEntries: Read duplicated entry's partnerId from MSERV");
				if (!this.SyncBatchOperations(list, out list2))
				{
					result = false;
				}
				else
				{
					List<RecipientSyncOperation> list3 = new List<RecipientSyncOperation>();
					List<RecipientSyncOperation> list4 = null;
					foreach (RecipientSyncOperation recipientSyncOperation3 in list2)
					{
						if (recipientSyncOperation3.PartnerId < this.podSiteStartRange || recipientSyncOperation3.PartnerId > this.podSiteEndRange)
						{
							RecipientSyncOperation recipientSyncOperation4 = new RecipientSyncOperation();
							recipientSyncOperation4.PartnerId = recipientSyncOperation3.PartnerId;
							recipientSyncOperation4.RemovedEntries.AddRange(recipientSyncOperation3.ReadEntries);
							list3.Add(recipientSyncOperation4);
						}
						else
						{
							base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, string.Concat(new object[]
							{
								"Warning: ",
								recipientSyncOperation3.ReadEntries[0],
								" has existing Exchange partnerID ",
								recipientSyncOperation3.PartnerId,
								". Skip fixing its partnerId as changing partnerID of Exchange forest is not supported."
							}));
						}
					}
					base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, "ResolveDuplicateAddedEntries: Delete duplicated entry's partnerId from MSERV");
					if (!this.SyncBatchOperations(list3, out list4))
					{
						result = false;
					}
					else
					{
						List<RecipientSyncOperation> list5 = null;
						foreach (RecipientSyncOperation recipientSyncOperation5 in this.operationsWithDuplicatedAddEntries)
						{
							recipientSyncOperation5.AddedEntries.Clear();
							recipientSyncOperation5.RemovedEntries.Clear();
							recipientSyncOperation5.ReadEntries.Clear();
							recipientSyncOperation5.AddedEntries.AddRange(recipientSyncOperation5.DuplicatedAddEntries);
							recipientSyncOperation5.DuplicatedAddEntries.Clear();
						}
						base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, "ResolveDuplicateAddedEntries: Add duplicated entry back to MSERV with Exchange partnerId");
						if (this.operationsWithDuplicatedAddEntries.Count > this.duplicatedAddEntriesCacheSize)
						{
							base.LogSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, null, "Failed: DuplicatedAddEntriesCacheSize limit reached");
							this.SyncBatchOperations(this.operationsWithDuplicatedAddEntries, out list5);
							result = false;
						}
						else
						{
							result = this.SyncBatchOperations(this.operationsWithDuplicatedAddEntries, out list5);
						}
					}
				}
			}
			finally
			{
				this.mserveService.TrackDuplicatedAddEntries = true;
			}
			return result;
		}

		private bool SyncBatchOperations(List<RecipientSyncOperation> operations, out List<RecipientSyncOperation> operationResults)
		{
			operationResults = null;
			bool result;
			try
			{
				operationResults = this.mserveService.Synchronize(operations);
				operationResults.AddRange(this.mserveService.Synchronize());
				result = (this.ProcessSyncResults(operationResults) == MserveTargetConnection.ProcessSyncResult.Success);
			}
			catch (MserveException exception)
			{
				base.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, exception, "SyncBatchOperations failed with MserveException");
				result = false;
			}
			return result;
		}

		private void LogFailedAddresses(Dictionary<OperationType, List<FailedAddress>> failedEntries)
		{
			foreach (KeyValuePair<OperationType, List<FailedAddress>> keyValuePair in failedEntries)
			{
				foreach (FailedAddress failedAddress in keyValuePair.Value)
				{
					if (!failedAddress.IsTransientError)
					{
						EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_MservEntrySyncFailure, null, new object[]
						{
							failedAddress.Name,
							failedAddress.ErrorCode
						});
					}
					base.LogSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, failedAddress.IsTransientError ? "Transient error" : "Non-transient error", string.Concat(new object[]
					{
						"Failed to synchronize the address ",
						failedAddress.Name,
						" to MSERV with errorCode ",
						failedAddress.ErrorCode
					}));
				}
			}
		}

		private bool CanRemove(string distinguishedName, string attributeName, List<string> addressList)
		{
			bool result = true;
			if (addressList.Count > 0)
			{
				string query = Schema.Query.BuildHostedRecipientAddressQuery(attributeName, addressList);
				foreach (ExSearchResultEntry exSearchResultEntry in this.sourceConnection.PagedScan(null, query, new string[]
				{
					attributeName
				}))
				{
					ExTraceGlobals.TargetConnectionTracer.TraceDebug<string, string, string>(0L, "Cannot remove {0} values from MSERVE for user {1} because it is still referenced by {2}", attributeName, distinguishedName, exSearchResultEntry.DistinguishedName);
					result = false;
				}
			}
			return result;
		}

		public const string MserveLeaseFileName = "mserv.lease";

		private static readonly string[] ReplicationAddressAttributes = new string[]
		{
			"proxyAddresses",
			"msExchSignupAddresses",
			"msExchUMAddresses",
			"ArchiveAddress"
		};

		private static Cache<string, MserveTargetConnection.TenantSyncControl> tenantSyncControlCache;

		private static string clientToken;

		private readonly MserveTargetServerConfig config;

		private readonly TestShutdownAndLeaseDelegate testShutdownAndLease;

		private readonly List<RecipientSyncOperation> operationsWithDuplicatedAddEntries = new List<RecipientSyncOperation>();

		private readonly bool trackDuplicatedAddEntries = true;

		private readonly int duplicatedAddEntriesCacheSize = 1500;

		private readonly int podSiteStartRange = 50000;

		private readonly int podSiteEndRange = 59999;

		private static ADObjectId configUnitsId;

		private IMserveService mserveService;

		private Connection updateConnection;

		private Connection sourceConnection;

		private IConfigurationSession configSession;

		private sealed class TenantSyncControl : CachableItem
		{
			public TenantSyncControl(bool syncMEUSMTPToMServ, bool syncMailboxSMTPToMserv)
			{
				this.SyncMEUSMTPToMServ = syncMEUSMTPToMServ;
				this.SyncMailboxSMTPToMserv = syncMailboxSMTPToMserv;
			}

			public override long ItemSize
			{
				get
				{
					return MserveTargetConnection.TenantSyncControl.Size;
				}
			}

			public static readonly long Size = 2L;

			public readonly bool SyncMEUSMTPToMServ;

			public readonly bool SyncMailboxSMTPToMserv;
		}

		private sealed class TenantSyncControlCacheLogger<K> : ICacheTracer<K>
		{
			public TenantSyncControlCacheLogger(EdgeSyncLogSession logSession)
			{
				this.logSession = logSession;
			}

			public void Accessed(K key, CachableItem value, AccessStatus accessStatus, DateTime timestamp)
			{
				MserveTargetConnection.TenantSyncControl tenantSyncControl = value as MserveTargetConnection.TenantSyncControl;
				this.logSession.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.TargetConnection, null, string.Format(CultureInfo.InvariantCulture, "TenantSyncControlCache-Accessed: AccessStatus {0}, key {1}, SyncMEUSMTPToMServ {2}, SyncMailboxSMTPToMServ {3}", new object[]
				{
					accessStatus,
					key,
					(tenantSyncControl != null) ? tenantSyncControl.SyncMEUSMTPToMServ.ToString() : "NULL",
					(tenantSyncControl != null) ? tenantSyncControl.SyncMailboxSMTPToMserv.ToString() : "NULL"
				}));
			}

			public void Flushed(long cacheSize, DateTime timestamp)
			{
				this.logSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, null, string.Format(CultureInfo.InvariantCulture, "TenantSyncControlCache-Flushed: Cached flushed. CacheSize was {0} bytes", new object[]
				{
					cacheSize
				}));
			}

			public void ItemAdded(K key, CachableItem value, DateTime timestamp)
			{
				MserveTargetConnection.TenantSyncControl tenantSyncControl = value as MserveTargetConnection.TenantSyncControl;
				this.logSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, null, string.Format(CultureInfo.InvariantCulture, "TenantSyncControlCache-ItemAdded: key {0}, SyncMEUSMTPToMServ {1}, SyncMailboxSMTPToMServ {2}", new object[]
				{
					key,
					(tenantSyncControl != null) ? tenantSyncControl.SyncMEUSMTPToMServ.ToString() : "NULL",
					(tenantSyncControl != null) ? tenantSyncControl.SyncMailboxSMTPToMserv.ToString() : "NULL"
				}));
			}

			public void ItemRemoved(K key, CachableItem value, CacheItemRemovedReason removeReason, DateTime timestamp)
			{
				MserveTargetConnection.TenantSyncControl tenantSyncControl = value as MserveTargetConnection.TenantSyncControl;
				this.logSession.LogEvent(EdgeSyncLoggingLevel.High, EdgeSyncEvent.TargetConnection, null, string.Format(CultureInfo.InvariantCulture, "TenantSyncControlCache-ItemRemoved: CacheItemRemovedReason {0}, key {1}, SyncMEUSMTPToMServ {2}, SyncMailboxSMTPToMserv {3}", new object[]
				{
					removeReason,
					key,
					(tenantSyncControl != null) ? tenantSyncControl.SyncMEUSMTPToMServ.ToString() : "NULL",
					(tenantSyncControl != null) ? tenantSyncControl.SyncMailboxSMTPToMserv.ToString() : "NULL"
				}));
			}

			public void TraceException(string details, Exception exception)
			{
				this.logSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, exception, details);
			}

			private readonly EdgeSyncLogSession logSession;
		}

		[Flags]
		private enum ProcessSyncResult : uint
		{
			Success = 0U,
			FailedMServ = 1U,
			FailedUpdateSyncState = 2U,
			ShutdownOrLostLease = 4U
		}
	}
}
