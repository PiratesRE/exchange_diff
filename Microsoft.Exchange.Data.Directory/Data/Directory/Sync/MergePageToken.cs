using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal sealed class MergePageToken : TenantFullSyncPageToken
	{
		public MergeState MergeState { get; private set; }

		public MergePageToken(byte[] tenantFullSyncPageTokenBytes, byte[] backSyncCookieBytes) : this(MergeState.Start, tenantFullSyncPageTokenBytes, backSyncCookieBytes, null)
		{
			if (!base.ReadyToMerge)
			{
				ExTraceGlobals.MergeTracer.TraceError<bool>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken this.ReadyToMerge = {0}", base.ReadyToMerge);
				throw new ArgumentException("tenantFullSyncPageTokenBytes");
			}
		}

		public void UpdateMergeState(PartitionId partitionId)
		{
			ExTraceGlobals.MergeTracer.TraceDebug<string>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.UpdateMergeState this.MergeState = {0}", this.MergeState.ToString());
			if (!this.IsMergeNeeded(partitionId))
			{
				this.MergeState = MergeState.Complete;
				ExTraceGlobals.MergeTracer.TraceDebug<Guid, Guid>((long)base.TenantExternalDirectoryId.GetHashCode(), "Merge operation is complete {0} on {1}.", base.TenantExternalDirectoryId, base.InvocationId);
				return;
			}
			if (this.MergeState == MergeState.Start)
			{
				this.MergeState = MergeState.InProgress;
				base.StartMerge();
				ExTraceGlobals.MergeTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "Starting Merge operation for {0} on {1}. \r\nFull sync watermarks: \r\n{2}\r\nIncremental sync cursors: \r\n{3}", new object[]
				{
					base.TenantExternalDirectoryId,
					base.InvocationId,
					base.Watermarks.SerializeToString(),
					this.dirSyncCookie.Cursors.ToString()
				});
			}
		}

		private MergePageToken(MergeState mergeState, byte[] tenantFullSyncPageTokenBytes, byte[] incrementalSyncCookieBytes, Dictionary<string, int> errorObjectsAndFailureCounts) : base(tenantFullSyncPageTokenBytes)
		{
			ExTraceGlobals.MergeTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "New MergePageToken");
			this.MergeState = mergeState;
			ExTraceGlobals.MergeTracer.TraceDebug<string>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken this.MergeState = {0}", this.MergeState.ToString());
			this.BackSyncCookie = BackSyncCookie.Parse(incrementalSyncCookieBytes);
			if (!this.BackSyncCookie.ServiceInstanceId.Equals(base.ServiceInstanceId))
			{
				ExTraceGlobals.MergeTracer.TraceError((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken serviceInstanceId != tenantFullSyncPageTokenBytes.ServiceInstanceId");
				throw new InvalidCookieException(new ArgumentException("serviceInstanceId"));
			}
			this.MergeVersion = 2;
			ExTraceGlobals.MergeTracer.TraceDebug<int>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken this.MergeVersion = {0}", this.MergeVersion);
			this.dirSyncCookie = ADDirSyncCookie.Parse(this.BackSyncCookie.DirSyncCookie);
			base.ErrorObjectsAndFailureCounts = (errorObjectsAndFailureCounts ?? new Dictionary<string, int>());
			if (!this.BackSyncCookie.ReadyToMerge || this.dirSyncCookie.MoreData)
			{
				ExTraceGlobals.MergeTracer.TraceError((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken !this.BackSyncCookie.ReadyToMerge || dirSyncCookie.MoreData");
				throw new InvalidCookieException(new ArgumentException("incrementalSyncCookieBytes"));
			}
		}

		public int MergeVersion { get; private set; }

		public bool IsMergeComplete
		{
			get
			{
				return this.MergeState == MergeState.Complete;
			}
		}

		public BackSyncCookie BackSyncCookie { get; internal set; }

		internal new static MergePageToken Parse(byte[] tokenBytes)
		{
			if (tokenBytes == null)
			{
				ExTraceGlobals.MergeTracer.TraceError((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse tokenBytes is NULL");
				throw new ArgumentNullException("tokenBytes");
			}
			ExTraceGlobals.MergeTracer.TraceDebug((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse entering");
			Exception ex2;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(tokenBytes, typeof(MergePageToken)))
				{
					int num = (int)backSyncCookieReader.GetNextAttributeValue();
					backSyncCookieReader.GetNextAttributeValue();
					MergeState mergeState = (MergeState)backSyncCookieReader.GetNextAttributeValue();
					if (mergeState == MergeState.Complete)
					{
						throw new InvalidCookieException();
					}
					byte[] tenantFullSyncPageTokenBytes = (byte[])backSyncCookieReader.GetNextAttributeValue();
					byte[] incrementalSyncCookieBytes = (byte[])backSyncCookieReader.GetNextAttributeValue();
					string[] errorObjects = (string[])backSyncCookieReader.GetNextAttributeValue();
					Dictionary<string, int> errorObjectsAndFailureCounts = BackSyncCookie.ParseErrorObjectsAndFailureCounts(errorObjects);
					return new MergePageToken(mergeState, tenantFullSyncPageTokenBytes, incrementalSyncCookieBytes, errorObjectsAndFailureCounts);
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.MergeTracer.TraceError<string>((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse ArgumentException {0}", ex.ToString());
				ex2 = ex;
			}
			catch (IOException ex3)
			{
				ExTraceGlobals.MergeTracer.TraceError<string>((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse IOException {0}", ex3.ToString());
				ex2 = ex3;
			}
			catch (FormatException ex4)
			{
				ExTraceGlobals.MergeTracer.TraceError<string>((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse FormatException {0}", ex4.ToString());
				ex2 = ex4;
			}
			catch (InvalidCookieException ex5)
			{
				ExTraceGlobals.MergeTracer.TraceError<string>((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse InvalidCookieException {0}", ex5.ToString());
				ex2 = ex5;
			}
			ExTraceGlobals.MergeTracer.TraceError<string>((long)typeof(MergePageToken).GetHashCode(), "MergePageToken.Parse throw InvalidCookieException {0}", ex2.ToString());
			throw new InvalidCookieException(ex2);
		}

		public override byte[] ToByteArray()
		{
			if (this.MergeState == MergeState.Complete)
			{
				ExTraceGlobals.MergeTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.ToByteArray this.MergeState is MergeState.Complete.");
				return this.BackSyncCookie.ToByteArray();
			}
			ExTraceGlobals.MergeTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.ToByteArray create new token ...");
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(MergePageToken)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(base.Version);
				backSyncCookieWriter.WriteNextAttributeValue(base.ServiceInstanceId.InstanceId);
				backSyncCookieWriter.WriteNextAttributeValue((int)this.MergeState);
				byte[] attributeValue = base.ToByteArray();
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue);
				byte[] attributeValue2 = this.BackSyncCookie.ToByteArray();
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue2);
				string[] attributeValue3 = BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray(base.ErrorObjectsAndFailureCounts);
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue3);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		private bool IsMergeNeeded(PartitionId partitionId)
		{
			ADReplicationCursorCollection cursors = this.dirSyncCookie.Cursors;
			ExTraceGlobals.MergeTracer.TraceDebug<ADReplicationCursorCollection>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.IsMergeNeeded original cursors from dirsync cookie = {0}", cursors);
			WatermarkMap watermarkMap = this.FilterOutNotExistingDCs(cursors, partitionId);
			ExTraceGlobals.MergeTracer.TraceDebug<string>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.IsMergeNeeded cursors from dirsync cookie filtered only for live DCs = {0}", watermarkMap.SerializeToString());
			ExTraceGlobals.MergeTracer.TraceDebug<string>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.IsMergeNeeded watermarks from token = {0}", base.Watermarks.SerializeToString());
			bool flag = watermarkMap.Any((KeyValuePair<Guid, long> dcw) => !base.Watermarks.ContainsKey(dcw.Key) || base.Watermarks[dcw.Key] < dcw.Value);
			ExTraceGlobals.MergeTracer.TraceDebug<bool>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.IsMergeNeeded isMergeNeeded = {0}", flag);
			return flag;
		}

		private WatermarkMap FilterOutNotExistingDCs(IEnumerable<ReplicationCursor> adReplicationCursorCollection, PartitionId partitionId)
		{
			WatermarkMap watermarkMap = new WatermarkMap();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 336, "FilterOutNotExistingDCs", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\MergePageToken.cs");
			WatermarkMap watermarkMap2 = null;
			ADServer adserver = topologyConfigurationSession.FindDCByInvocationId(base.WatermarksInvocationId);
			if (adserver != null)
			{
				topologyConfigurationSession.DomainController = adserver.DnsHostName;
				watermarkMap2 = SyncConfiguration.GetReplicationCursors(topologyConfigurationSession);
			}
			foreach (ReplicationCursor replicationCursor in adReplicationCursorCollection)
			{
				if (watermarkMap2 == null || watermarkMap2.ContainsKey(replicationCursor.SourceInvocationId))
				{
					watermarkMap[replicationCursor.SourceInvocationId] = replicationCursor.UpToDatenessUsn;
				}
			}
			return watermarkMap;
		}

		internal override Guid SelectDomainController(PartitionId partitionId)
		{
			ExTraceGlobals.ActiveDirectoryTracer.TraceDebug<Guid>((long)base.TenantExternalDirectoryId.GetHashCode(), "Selecting a DC for Merge operation of {0}. Will examine all DCs in the local site AND domain", base.TenantExternalDirectoryId);
			if (base.InvocationId != Guid.Empty)
			{
				ExTraceGlobals.ActiveDirectoryTracer.TraceError<Guid>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.SelectDomainController this.InvocationId {0} is not Guid.Empty", base.InvocationId);
				throw new InvalidOperationException("InvocationId");
			}
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 390, "SelectDomainController", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\MergePageToken.cs");
			IList<ADServerInfo> serversForRole = TopologyProvider.GetInstance().GetServersForRole(partitionId.ForestFQDN, new List<string>(0), ADServerRole.DomainController, int.MaxValue, false);
			ExTraceGlobals.ActiveDirectoryTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.SelectDomainController searching dcs in preferred site");
			foreach (ADServerInfo adserverInfo in serversForRole)
			{
				Guid result;
				if (this.TrySelectDomainController(session, adserverInfo.Fqdn, partitionId, false, out result))
				{
					return result;
				}
			}
			ReadOnlyCollection<ADServer> readOnlyCollection = ADForest.GetForest(partitionId).FindRootDomain().FindAllDomainControllers();
			ExTraceGlobals.ActiveDirectoryTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.SelectDomainController searching dcs in other sites");
			foreach (ADServer adserver in readOnlyCollection)
			{
				Guid result2;
				if (!ConnectionPoolManager.IsServerInPreferredSite(partitionId.ForestFQDN, adserver) && this.TrySelectDomainController(session, adserver.DnsHostName, partitionId, true, out result2))
				{
					return result2;
				}
			}
			ExTraceGlobals.ActiveDirectoryTracer.TraceError<Guid, string>((long)base.TenantExternalDirectoryId.GetHashCode(), "Could not find any DC that has all changes reported by the Tenant Full Sync Watermarks for {0}. \r\nFull sync watermarks: \r\n{1}", base.TenantExternalDirectoryId, base.Watermarks.SerializeToString());
			throw new BackSyncDataSourceUnavailableException();
		}

		private bool TrySelectDomainController(ITopologyConfigurationSession session, string domainControllerFqdn, PartitionId partitionId, bool checkSuitability, out Guid resultInvocationId)
		{
			ExTraceGlobals.ActiveDirectoryTracer.TraceDebug<string>((long)base.TenantExternalDirectoryId.GetHashCode(), "MergePageToken.TrySelectDomainController dc {0}", domainControllerFqdn);
			resultInvocationId = Guid.Empty;
			string text;
			LocalizedString localizedString;
			if (checkSuitability && !SuitabilityVerifier.IsServerSuitableIgnoreExceptions(domainControllerFqdn, false, null, out text, out localizedString))
			{
				ExTraceGlobals.ActiveDirectoryTracer.TraceWarning<string, string>((long)base.TenantExternalDirectoryId.GetHashCode(), "DC {0} is not available for Tenant Full Backsync, error {1}", domainControllerFqdn, localizedString.ToString());
				return false;
			}
			ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainControllerFqdn, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 461, "TrySelectDomainController", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\MergePageToken.cs");
			WatermarkMap replicationCursors = SyncConfiguration.GetReplicationCursors(configSession, false, true);
			if (replicationCursors.ContainsAllChanges(base.Watermarks))
			{
				Guid invocationIdByFqdn = session.GetInvocationIdByFqdn(domainControllerFqdn);
				long num;
				if (base.Watermarks.TryGetValue(invocationIdByFqdn, out num))
				{
					base.ObjectUpdateSequenceNumber = num + 1L;
					base.TombstoneUpdateSequenceNumber = num + 1L;
					base.InvocationId = invocationIdByFqdn;
					if (base.UseContainerizedUsnChangedIndex)
					{
						base.SoftDeletedObjectUpdateSequenceNumber = num + 1L;
					}
					ExTraceGlobals.ActiveDirectoryTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "DC {0}({1})has all changes reported by the Tenant Full Sync Watermarks for {2} and CAN be used. \r\nFull sync watermarks: \r\n{3}\r\nDC replication cursors: \r\n{4}", new object[]
					{
						domainControllerFqdn,
						base.InvocationId,
						base.TenantExternalDirectoryId,
						base.Watermarks.SerializeToString(),
						replicationCursors.SerializeToString()
					});
					resultInvocationId = invocationIdByFqdn;
					return true;
				}
				ExTraceGlobals.ActiveDirectoryTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "DC {0}({1})has all changes reported by the Tenant Full Sync Watermarks for {2} but cannot be used since its invocationId is not part of the TFS watermarks. \r\nFull sync watermarks: \r\n{3}\r\n", new object[]
				{
					domainControllerFqdn,
					base.InvocationId,
					base.TenantExternalDirectoryId,
					base.Watermarks.SerializeToString()
				});
			}
			else
			{
				ExTraceGlobals.ActiveDirectoryTracer.TraceDebug((long)base.TenantExternalDirectoryId.GetHashCode(), "DC {0} does not have all changes reported by the Tenant Full Sync Watermarks for {1} and cannot be used. \r\nFull sync watermarks: \r\n{2}\r\nDC replication cursors: \r\n{3}", new object[]
				{
					domainControllerFqdn,
					base.TenantExternalDirectoryId,
					base.Watermarks.SerializeToString(),
					replicationCursors.SerializeToString()
				});
			}
			return false;
		}

		internal new const int CurrentVersion = 2;

		internal static BackSyncCookieAttribute[] MergePageTokenAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "MergeState",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantFullSyncPageToken",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "IncrementalSyncCookie",
				DataType = typeof(byte[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] MergePageTokenAttributeSchema_Version_2 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "ErrorObjectsAndFailureCounts",
				DataType = typeof(string[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[][] MergePageTokenAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			MergePageToken.MergePageTokenAttributeSchema_Version_1,
			MergePageToken.MergePageTokenAttributeSchema_Version_2
		};

		private readonly ADDirSyncCookie dirSyncCookie;
	}
}
