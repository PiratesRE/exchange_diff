using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class TenantFullSyncPageToken : IFullSyncPageToken, ISyncCookie
	{
		public TenantFullSyncPageToken(Guid invocationId, Guid tenantExternalDirectoryId, ADObjectId tenantOuId, ServiceInstanceId serviceInstanceId, bool useDirSyncBasedTfs = false)
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "New TenantFullSyncPageToken");
			this.Version = 3;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<int>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.Version = {0}", this.Version);
			this.TenantExternalDirectoryId = tenantExternalDirectoryId;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.TenantExternalDirectoryId = {0}", this.TenantExternalDirectoryId);
			this.TenantObjectGuid = tenantOuId.ObjectGuid;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.TenantObjectGuid = {0}", this.TenantObjectGuid);
			this.State = TenantFullSyncState.EnumerateLiveObjects;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.State = {0}", this.State.ToString());
			this.ServiceInstanceId = serviceInstanceId;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<ServiceInstanceId>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.ServiceInstanceId = {0}", this.ServiceInstanceId);
			this.ErrorObjectsAndFailureCounts = new Dictionary<string, int>();
			this.SequenceId = Guid.NewGuid();
			this.SequenceStartTimestamp = DateTime.UtcNow;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, DateTime>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken Starting a new sequence this.SequenceId = {0} this.SequenceStartTimestamp = {1} ", this.SequenceId, this.SequenceStartTimestamp);
			this.TenantScopedBackSyncCookie = (useDirSyncBasedTfs ? new BackSyncCookie(this.ServiceInstanceId) : null);
			this.InvocationId = (useDirSyncBasedTfs ? this.TenantScopedBackSyncCookie.InvocationId : invocationId);
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken this.InvocationId = {0}", this.InvocationId);
			this.UseContainerizedUsnChangedIndex = false;
			if (SyncConfiguration.EnableContainerizedUsnChangedOptimization())
			{
				Guid preferredDCWithContainerizedUsnChanged = SyncConfiguration.GetPreferredDCWithContainerizedUsnChanged(this.ServiceInstanceId.InstanceId);
				if (preferredDCWithContainerizedUsnChanged != Guid.Empty)
				{
					this.InvocationId = preferredDCWithContainerizedUsnChanged;
					this.UseContainerizedUsnChangedIndex = true;
					ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken overwriting this.InvocationId = {0} and this.UseContainerizedUsnChangedIndex = true", this.InvocationId);
				}
				else
				{
					ExTraceGlobals.TenantFullSyncTracer.TraceDebug<ServiceInstanceId>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken: Could not find preferred DC for service instance {0}. Containerized USN index will NOT be used.", this.ServiceInstanceId);
				}
			}
			if (this.UseContainerizedUsnChangedIndex && useDirSyncBasedTfs)
			{
				throw new InvalidOperationException("Invalid configuration - cannot use Containerized UsnChanged Index and Dirsync based TFS simultaneously.");
			}
			if (this.UseContainerizedUsnChangedIndex)
			{
				this.PreviousState = this.State;
			}
		}

		public int Version { get; private set; }

		public Guid TenantExternalDirectoryId { get; private set; }

		public Guid TenantObjectGuid { get; private set; }

		public Guid InvocationId { get; internal set; }

		public bool ReadyToMerge
		{
			get
			{
				return this.State == TenantFullSyncState.Complete;
			}
		}

		public TenantFullSyncState State { get; private set; }

		public Dictionary<string, int> ErrorObjectsAndFailureCounts { get; protected set; }

		internal long ObjectUpdateSequenceNumber { get; set; }

		internal long TombstoneUpdateSequenceNumber { get; set; }

		internal long LinkPageStart { get; private set; }

		internal long LinkPageEnd { get; private set; }

		internal int LinkRangeStart { get; private set; }

		internal int ObjectsInLinkPage { get; private set; }

		internal WatermarkMap Watermarks { get; set; }

		internal WatermarkMap PendingWatermarks { get; set; }

		internal Guid WatermarksInvocationId { get; set; }

		internal Guid PendingWatermarksInvocationId { get; set; }

		internal ServiceInstanceId ServiceInstanceId { get; private set; }

		public DateTime SequenceStartTimestamp { get; private set; }

		public Guid SequenceId { get; private set; }

		public BackSyncCookie TenantScopedBackSyncCookie { get; set; }

		internal bool UseContainerizedUsnChangedIndex { get; set; }

		internal long SoftDeletedObjectUpdateSequenceNumber { get; set; }

		public TenantFullSyncState PreviousState { get; private set; }

		internal static TenantFullSyncPageToken Parse(byte[] tokenBytes)
		{
			if (tokenBytes == null)
			{
				throw new ArgumentNullException("tokenBytes");
			}
			return new TenantFullSyncPageToken(tokenBytes);
		}

		protected TenantFullSyncPageToken(byte[] tokenBytes)
		{
			Exception ex = null;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(tokenBytes, typeof(TenantFullSyncPageToken)))
				{
					this.Version = (int)backSyncCookieReader.GetNextAttributeValue();
					this.ServiceInstanceId = new ServiceInstanceId((string)backSyncCookieReader.GetNextAttributeValue());
					this.Timestamp = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					this.LastReadFailureStartTime = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					this.InvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.TenantExternalDirectoryId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.TenantObjectGuid = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.State = (TenantFullSyncState)backSyncCookieReader.GetNextAttributeValue();
					this.ObjectUpdateSequenceNumber = (long)backSyncCookieReader.GetNextAttributeValue();
					this.TombstoneUpdateSequenceNumber = (long)backSyncCookieReader.GetNextAttributeValue();
					byte[] array = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array != null)
					{
						this.PendingWatermarks = WatermarkMap.Parse(array);
					}
					this.PendingWatermarksInvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					byte[] array2 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array2 != null)
					{
						this.Watermarks = WatermarkMap.Parse(array2);
					}
					this.WatermarksInvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.LinkPageStart = (long)backSyncCookieReader.GetNextAttributeValue();
					this.LinkPageEnd = (long)backSyncCookieReader.GetNextAttributeValue();
					this.LinkRangeStart = (int)backSyncCookieReader.GetNextAttributeValue();
					this.ObjectsInLinkPage = (int)backSyncCookieReader.GetNextAttributeValue();
					string[] array3 = (string[])backSyncCookieReader.GetNextAttributeValue();
					this.ErrorObjectsAndFailureCounts = ((array3 != null) ? BackSyncCookie.ParseErrorObjectsAndFailureCounts(array3) : new Dictionary<string, int>());
					this.SequenceStartTimestamp = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					this.SequenceId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					byte[] array4 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array4 != null)
					{
						this.TenantScopedBackSyncCookie = BackSyncCookie.Parse(array4);
						this.InvocationId = this.TenantScopedBackSyncCookie.InvocationId;
					}
					this.UseContainerizedUsnChangedIndex = (bool)backSyncCookieReader.GetNextAttributeValue();
					this.SoftDeletedObjectUpdateSequenceNumber = (long)backSyncCookieReader.GetNextAttributeValue();
					this.PreviousState = (TenantFullSyncState)backSyncCookieReader.GetNextAttributeValue();
				}
			}
			catch (ArgumentException ex2)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken ArgumentException {0}", ex2.ToString());
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken IOException {0}", ex3.ToString());
				ex = ex3;
			}
			catch (FormatException ex4)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken FormatException {0}", ex4.ToString());
				ex = ex4;
			}
			catch (InvalidCookieException ex5)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken InvalidCookieException {0}", ex5.ToString());
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken throw InvalidCookieException {0}", ex.ToString());
				throw new InvalidCookieException(ex);
			}
		}

		public BackSyncOptions SyncOptions
		{
			get
			{
				return BackSyncOptions.IncludeLinks;
			}
		}

		public bool MoreData
		{
			get
			{
				return this.State != TenantFullSyncState.Complete;
			}
		}

		public DateTime Timestamp { get; set; }

		public DateTime LastReadFailureStartTime { get; set; }

		public virtual byte[] ToByteArray()
		{
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(TenantFullSyncPageToken)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(this.Version);
				backSyncCookieWriter.WriteNextAttributeValue(this.ServiceInstanceId.InstanceId);
				backSyncCookieWriter.WriteNextAttributeValue(this.Timestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.LastReadFailureStartTime.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.InvocationId);
				backSyncCookieWriter.WriteNextAttributeValue(this.TenantExternalDirectoryId);
				backSyncCookieWriter.WriteNextAttributeValue(this.TenantObjectGuid);
				backSyncCookieWriter.WriteNextAttributeValue((int)this.State);
				backSyncCookieWriter.WriteNextAttributeValue(this.ObjectUpdateSequenceNumber);
				backSyncCookieWriter.WriteNextAttributeValue(this.TombstoneUpdateSequenceNumber);
				if (this.PendingWatermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue = this.PendingWatermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue);
				}
				backSyncCookieWriter.WriteNextAttributeValue(this.PendingWatermarksInvocationId);
				if (this.Watermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue2 = this.Watermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue2);
				}
				backSyncCookieWriter.WriteNextAttributeValue(this.WatermarksInvocationId);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkPageStart);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkPageEnd);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkRangeStart);
				backSyncCookieWriter.WriteNextAttributeValue(this.ObjectsInLinkPage);
				string[] attributeValue3 = BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray(this.ErrorObjectsAndFailureCounts);
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue3);
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceStartTimestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceId);
				if (this.TenantScopedBackSyncCookie == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue4 = this.TenantScopedBackSyncCookie.ToByteArray();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue4);
				}
				backSyncCookieWriter.WriteNextAttributeValue(this.UseContainerizedUsnChangedIndex);
				backSyncCookieWriter.WriteNextAttributeValue(this.SoftDeletedObjectUpdateSequenceNumber);
				backSyncCookieWriter.WriteNextAttributeValue((int)this.PreviousState);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		public void PrepareForFailover()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.PrepareForFailover entering");
			long num = 0L;
			if (this.Watermarks != null)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "Get USN from DSA invocation id {0}", this.InvocationId);
				this.Watermarks.TryGetValue(this.InvocationId, out num);
				ExTraceGlobals.TenantFullSyncTracer.TraceDebug<long>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.PrepareForFailover usnFromCurrentDc = {0}", num);
			}
			if (this.ObjectUpdateSequenceNumber <= num + 1L && this.TombstoneUpdateSequenceNumber <= num + 1L)
			{
				this.InvocationId = Guid.Empty;
				ExTraceGlobals.ActiveDirectoryTracer.TraceWarning<string, Guid, Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "Allowing failover for {0} (Tenant={1}) from {2}. New DC will be picked on the next request.", base.GetType().Name, this.TenantExternalDirectoryId, this.InvocationId);
				return;
			}
			ExTraceGlobals.ActiveDirectoryTracer.TraceWarning<string, Guid, Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "NOT allowing failover for {0} (Tenant={1}) from {2} because some data have already been read.", base.GetType().Name, this.TenantExternalDirectoryId, this.InvocationId);
		}

		internal static string GetCurrentServerFromSession(IDirectorySession session)
		{
			string text = session.ServerSettings.PreferredGlobalCatalog(session.SessionSettings.GetAccountOrResourceForestFqdn());
			if (string.IsNullOrEmpty(text))
			{
				ADObjectId adobjectId = null;
				PooledLdapConnection pooledLdapConnection = null;
				try
				{
					pooledLdapConnection = session.GetReadConnection(null, ref adobjectId);
					text = pooledLdapConnection.ServerName;
				}
				finally
				{
					if (pooledLdapConnection != null)
					{
						pooledLdapConnection.ReturnToPool();
					}
				}
			}
			return text;
		}

		internal virtual Guid SelectDomainController(PartitionId partitionId)
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SelectDomainController entering");
			if (this.InvocationId != Guid.Empty)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<Guid>((long)this.TenantExternalDirectoryId.GetHashCode(), "InvocationId {0} already set", this.InvocationId);
				throw new InvalidOperationException("InvocationId");
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 795, "SelectDomainController", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\TenantFullSyncPageToken.cs");
			string currentServerFromSession = TenantFullSyncPageToken.GetCurrentServerFromSession(topologyConfigurationSession);
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SelectDomainController dcName {0}", currentServerFromSession);
			this.InvocationId = topologyConfigurationSession.GetInvocationIdByFqdn(currentServerFromSession);
			ExTraceGlobals.ActiveDirectoryTracer.TraceInformation<Guid, string, Guid>(10429, (long)this.TenantExternalDirectoryId.GetHashCode(), "Randomly picked DC {0} for {1} (Tenant={2}).", this.InvocationId, base.GetType().Name, this.TenantExternalDirectoryId);
			return this.InvocationId;
		}

		internal void SwitchToEnumerateDeletedObjectsState()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SwitchToEnumerateDeletedObjectsState entering");
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<TenantFullSyncState>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SwitchToEnumerateDeletedObjectsState this.State = {0}", this.State);
			if (this.State == TenantFullSyncState.EnumerateDeletedObjects)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "Invalid state {0} to SwitchToEnumerateDeletedObjectsState", this.State.ToString());
				throw new InvalidOperationException("State");
			}
			this.CheckLinkPropertiesAreEmpty();
			this.State = TenantFullSyncState.EnumerateDeletedObjects;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, Guid, long>((long)this.TenantExternalDirectoryId.GetHashCode(), "Starting enumeration of deleted objects for {0} on {1} from USN {2}", this.TenantExternalDirectoryId, this.InvocationId, this.TombstoneUpdateSequenceNumber);
		}

		internal void SwitchToEnumerateLiveObjectsState()
		{
			if (this.ObjectsInLinkPage == 0)
			{
				throw new InvalidOperationException("ObjectsInLinkPage");
			}
			if (this.State != TenantFullSyncState.EnumerateLinksInPage && this.State != TenantFullSyncState.Complete)
			{
				throw new InvalidOperationException("State");
			}
			this.State = TenantFullSyncState.EnumerateLiveObjects;
			this.LinkPageStart = 0L;
			this.LinkPageEnd = 0L;
			this.LinkRangeStart = 0;
			this.ObjectsInLinkPage = 0;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, Guid, long>((long)this.TenantExternalDirectoryId.GetHashCode(), "Starting enumeration of live objects for {0} on {1} from USN {2}", this.TenantExternalDirectoryId, this.InvocationId, this.ObjectUpdateSequenceNumber);
		}

		internal void SwitchToEnumerateSoftDeletedObjectsState()
		{
			if (this.State != TenantFullSyncState.EnumerateLinksInPage && this.State != TenantFullSyncState.EnumerateLiveObjects)
			{
				throw new InvalidOperationException("State");
			}
			this.PreviousState = this.State;
			this.State = TenantFullSyncState.EnumerateSoftDeletedObjects;
			this.LinkPageStart = 0L;
			this.LinkPageEnd = 0L;
			this.LinkRangeStart = 0;
			this.ObjectsInLinkPage = 0;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, Guid, long>((long)this.TenantExternalDirectoryId.GetHashCode(), "Starting enumeration of soft-deleted objects for {0} on {1} from USN {2}", this.TenantExternalDirectoryId, this.InvocationId, this.SoftDeletedObjectUpdateSequenceNumber);
		}

		internal void SwitchToEnumerateLinksState(long linkPageStart, long linkPageEnd, int objectsInLinkPage)
		{
			if (this.State != TenantFullSyncState.EnumerateLiveObjects && this.State != TenantFullSyncState.EnumerateSoftDeletedObjects)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.State != EnumerateLiveObjects and EnumerateSoftDeletedObjects");
				throw new InvalidOperationException("State");
			}
			this.PreviousState = this.State;
			this.CheckLinkPropertiesAreEmpty();
			this.SetEnumerateLinksParams(linkPageStart, linkPageEnd, FullSyncConfiguration.InitialLinkReadSize, objectsInLinkPage, this.State);
			this.State = TenantFullSyncState.EnumerateLinksInPage;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "Starting enumeration of links for {0} on {1} in USN range {2}-{3} (total count {4})", new object[]
			{
				this.TenantExternalDirectoryId,
				this.InvocationId,
				this.LinkPageStart,
				this.LinkPageEnd,
				this.ObjectsInLinkPage
			});
		}

		internal void UpdateEnumerateLinksState(long linkPageStart, long linkPageEnd, int linkRangeStart, int objectsInLinkPage)
		{
			this.SetEnumerateLinksParams(linkPageStart, linkPageEnd, linkRangeStart, objectsInLinkPage, TenantFullSyncState.EnumerateLinksInPage);
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "Setting link range for {0} on {1} to USN range {2}-{3} (total count {4}), values start at {5}", new object[]
			{
				this.TenantExternalDirectoryId,
				this.InvocationId,
				this.LinkPageStart,
				this.LinkPageEnd,
				this.ObjectsInLinkPage,
				this.LinkRangeStart
			});
		}

		private void CheckLinkPropertiesAreEmpty()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.CheckLinkPropertiesAreEmpty entering");
			if (this.ObjectsInLinkPage != 0)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.ObjectsInLinkPage != 0");
				throw new InvalidOperationException("ObjectsInLinkPage");
			}
			if (this.LinkPageStart != 0L)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.LinkPageStart != 0");
				throw new InvalidOperationException("LinkPageStart");
			}
			if (this.LinkPageEnd != 0L)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.LinkPageEnd != 0");
				throw new InvalidOperationException("LinkPageEnd");
			}
			if (this.LinkRangeStart != 0)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.LinkRangeStart != 0");
				throw new InvalidOperationException("LinkRangeStart");
			}
		}

		private void SetEnumerateLinksParams(long linkPageStart, long linkPageEnd, int linkRangeStart, int objectsInLinkPage, TenantFullSyncState expectedState)
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SetEnumerateLinksParams entering");
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<int>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SetEnumerateLinksParams objectsInLinkPage = {0}", objectsInLinkPage);
			if (objectsInLinkPage == 0)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "objectsInLinkPage != 0");
				throw new ArgumentOutOfRangeException("objectsInLinkPage");
			}
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<string, string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SetEnumerateLinksParams this.State = {0}, expectedState = {1}", this.State.ToString(), expectedState.ToString());
			if (this.State != expectedState)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.State != expectedState");
				throw new InvalidOperationException("State");
			}
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<long, long>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.SetEnumerateLinksParams linkPageStart = {0}, linkPageEnd = {1}", linkPageStart, linkPageEnd);
			if (linkPageStart > linkPageEnd)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "linkPageStart > linkPageEnd");
				throw new ArgumentOutOfRangeException("linkPageStart");
			}
			if ((long)objectsInLinkPage > linkPageEnd - linkPageStart + 1L)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "objectsInLinkPage > (linkPageEnd - linkPageStart + 1)");
				throw new ArgumentOutOfRangeException("linkPageEnd");
			}
			this.LinkPageStart = linkPageStart;
			this.LinkPageEnd = linkPageEnd;
			this.LinkRangeStart = linkRangeStart;
			this.ObjectsInLinkPage = objectsInLinkPage;
		}

		internal void FinishFullSync()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.FinishFullSync entering");
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.FinishFullSync this.State = {0}", this.State.ToString());
			if (this.TenantScopedBackSyncCookie == null && this.State != TenantFullSyncState.EnumerateDeletedObjects)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<TenantFullSyncState>((long)this.TenantExternalDirectoryId.GetHashCode(), "this.State != TenantFullSyncState.EnumerateDeletedObjects. State: {0}", this.State);
				throw new InvalidOperationException("State");
			}
			if (this.TenantScopedBackSyncCookie != null && this.State != TenantFullSyncState.EnumerateLiveObjects && this.State != TenantFullSyncState.EnumerateDeletedObjects)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError<TenantFullSyncState>((long)this.TenantExternalDirectoryId.GetHashCode(), "this.State != TenantFullSyncState.EnumerateLiveObjects and EnumerateDeletedObjects. State: {0}", this.State);
				throw new InvalidOperationException("State");
			}
			this.CheckLinkPropertiesAreEmpty();
			if (this.TenantScopedBackSyncCookie == null)
			{
				if (this.PendingWatermarks == null)
				{
					ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.PendingWatermarks == null");
					throw new InvalidOperationException("PendingWatermarks");
				}
				if (this.PendingWatermarksInvocationId == Guid.Empty)
				{
					ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.PendingWatermarksInvocationId == Guid.Empty");
					throw new InvalidOperationException("PendingWatermarksInvocationId");
				}
				this.WatermarksInvocationId = this.PendingWatermarksInvocationId;
				this.PendingWatermarksInvocationId = Guid.Empty;
				this.Watermarks = this.PendingWatermarks;
				this.PendingWatermarks = null;
			}
			else
			{
				if (this.TenantScopedBackSyncCookie.MoreDirSyncData)
				{
					ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.TenantScopedBackSyncCookie.MoreDirSyncData == true");
					throw new InvalidOperationException("TenantScopedBackSyncCookie.MoreDirSyncData");
				}
				ADDirSyncCookie addirSyncCookie = ADDirSyncCookie.Parse(this.TenantScopedBackSyncCookie.DirSyncCookie);
				if (addirSyncCookie.Cursors == null || addirSyncCookie.Cursors.Count == 0)
				{
					ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "latestDirSyncCookie.Cursors is null or empty");
					throw new InvalidOperationException("latestDirSyncCookie.Cursors");
				}
				this.WatermarksInvocationId = this.TenantScopedBackSyncCookie.InvocationId;
				this.Watermarks = new WatermarkMap();
				foreach (ReplicationCursor replicationCursor in addirSyncCookie.Cursors)
				{
					this.Watermarks[replicationCursor.SourceInvocationId] = replicationCursor.UpToDatenessUsn;
				}
			}
			this.State = TenantFullSyncState.Complete;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "Tenant full sync for {0} on {1}({2}) is complete. Watermarks stored: {3}", new object[]
			{
				this.TenantExternalDirectoryId,
				this.InvocationId,
				this.InvocationId,
				this.Watermarks.SerializeToString()
			});
		}

		internal void StartNewSyncSequence()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, DateTime>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken Starting a new sequence. Old sequence info: this.SequenceId = {0} this.SequenceStartTimestamp = {1} ", this.SequenceId, this.SequenceStartTimestamp);
			this.SequenceId = Guid.NewGuid();
			this.SequenceStartTimestamp = DateTime.UtcNow;
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, DateTime>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken Starting a new sequence. New sequence info: this.SequenceId = {0} this.SequenceStartTimestamp = {1} ", this.SequenceId, this.SequenceStartTimestamp);
		}

		protected void StartMerge()
		{
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.StartMerge entering");
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<string>((long)this.TenantExternalDirectoryId.GetHashCode(), "TenantFullSyncPageToken.StartMerge this.State = {0}", this.State.ToString());
			if (this.State != TenantFullSyncState.Complete)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.State != TenantFullSyncState.Complete");
				throw new InvalidOperationException("State");
			}
			if (this.Watermarks == null)
			{
				ExTraceGlobals.TenantFullSyncTracer.TraceError((long)this.TenantExternalDirectoryId.GetHashCode(), "this.Watermarks == null");
				throw new InvalidOperationException("Watermarks");
			}
			this.CheckLinkPropertiesAreEmpty();
			this.State = TenantFullSyncState.EnumerateLiveObjects;
			this.StartNewSyncSequence();
			ExTraceGlobals.TenantFullSyncTracer.TraceDebug<Guid, Guid, long>((long)this.TenantExternalDirectoryId.GetHashCode(), "[Merge] Starting enumeration of live objects for {0} on {1} from USN {2}", this.TenantExternalDirectoryId, this.InvocationId, this.ObjectUpdateSequenceNumber);
		}

		internal const int CurrentVersion = 3;

		internal static BackSyncCookieAttribute[] TenantFullSyncPageTokenAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "TimeStampRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "LastReadFailureStartTimeRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "InvocationId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantExternalDirectoryId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantObjectGuid",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantFullSyncState",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "ObjectUpdateSequenceNumber",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "TombstoneUpdateSequenceNumber",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "PendingWatermarks",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "PendingWatermarksInvocationId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "Watermarks",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "WatermarksInvocationId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkPageStart",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkPageEnd",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkRangeStart",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "ObjectsInLinkPage",
				DataType = typeof(int),
				DefaultValue = 0
			}
		};

		internal static BackSyncCookieAttribute[] TenantFullSyncPageTokenAttributeSchema_Version_2 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "ErrorObjectsAndFailureCounts",
				DataType = typeof(string[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] TenantFullSyncPageTokenAttributeSchema_Version_3 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "SequenceStartTimeRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "SequenceId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantScopedBackSyncCookie",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "UseContainerizedUsnChangedIndex",
				DataType = typeof(bool),
				DefaultValue = false
			},
			new BackSyncCookieAttribute
			{
				Name = "SoftDeletedObjectUpdateSequenceNumber",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantFullSyncPreviousState",
				DataType = typeof(int),
				DefaultValue = 0
			}
		};

		internal static BackSyncCookieAttribute[][] TenantFullSyncPageTokenAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			TenantFullSyncPageToken.TenantFullSyncPageTokenAttributeSchema_Version_1,
			TenantFullSyncPageToken.TenantFullSyncPageTokenAttributeSchema_Version_2,
			TenantFullSyncPageToken.TenantFullSyncPageTokenAttributeSchema_Version_3
		};
	}
}
