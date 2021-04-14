using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class TenantRelocationSyncPageToken : IFullSyncPageToken, ISyncCookie
	{
		public TenantRelocationSyncPageToken(Guid invocationId, ADObjectId tenantOuId, ADObjectId tenantCuId, TenantPartitionHint partitionHint, bool isTenantCuInConfigNC)
		{
			ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "New TenantRelocationSyncPageToken");
			this.Version = 1;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<int>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.Version = {0}", this.Version);
			this.InvocationId = invocationId;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.InvocationId = {0}", this.InvocationId);
			this.TenantOrganizationalUnitObjectGuid = tenantOuId.ObjectGuid;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.TenantOrganizationalUnitObjectGuid = {0}", this.TenantOrganizationalUnitObjectGuid);
			this.TenantConfigUnitObjectGuid = tenantCuId.ObjectGuid;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.TenantConfigUnitObjectGuid = {0}", this.TenantConfigUnitObjectGuid);
			this.PartitionHint = partitionHint;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.PartitionHint = {0}", this.PartitionHint.ToString());
			this.State = TenantRelocationSyncState.PreSyncAllObjects;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken this.State = {0}", this.State.ToString());
			this.ErrorObjectsAndFailureCounts = new Dictionary<string, int>();
		}

		public int Version { get; private set; }

		public Guid TenantOrganizationalUnitObjectGuid { get; private set; }

		public Guid TenantConfigUnitObjectGuid { get; private set; }

		public bool IsTenantConfigUnitInConfigNc { get; set; }

		public Guid InvocationId { get; internal set; }

		public TenantRelocationSyncState State { get; private set; }

		internal TenantPartitionHint PartitionHint { get; set; }

		internal long ConfigUnitObjectUSN { get; set; }

		internal long ConfigUnitTombstoneUSN { get; set; }

		internal long OrganizationalUnitObjectUSN { get; set; }

		internal long OrganizationalUnitTombstoneUSN { get; set; }

		internal long SpecialObjectsUSN { get; set; }

		internal long LinkPageStart { get; private set; }

		internal long LinkPageEnd { get; private set; }

		internal int LinkRangeStart { get; private set; }

		internal int ObjectsInLinkPage { get; private set; }

		internal WatermarkMap Watermarks { get; set; }

		internal WatermarkMap PendingWatermarks { get; set; }

		internal WatermarkMap ConfigNcWatermarks { get; set; }

		internal WatermarkMap PendingConfigNcWatermarks { get; set; }

		internal Guid WatermarksInvocationId { get; set; }

		internal Guid PendingWatermarksInvocationId { get; set; }

		public Dictionary<string, int> ErrorObjectsAndFailureCounts { get; private set; }

		public string AffinityDcFqdn { get; set; }

		public string AffinityTargetDcFqdn { get; set; }

		public byte[] PreSyncLdapPagingCookie { get; set; }

		public DateTime SequenceStartTimestamp
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Guid SequenceId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal static TenantRelocationSyncPageToken Parse(byte[] tokenBytes)
		{
			if (tokenBytes == null)
			{
				throw new ArgumentNullException("tokenBytes");
			}
			return new TenantRelocationSyncPageToken(tokenBytes);
		}

		public TenantRelocationSyncPageToken(byte[] tokenBytes)
		{
			Exception ex = null;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(tokenBytes, typeof(TenantRelocationSyncPageToken)))
				{
					this.Version = (int)backSyncCookieReader.GetNextAttributeValue();
					backSyncCookieReader.GetNextAttributeValue();
					this.Timestamp = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					this.LastReadFailureStartTime = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					this.InvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.TenantConfigUnitObjectGuid = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.TenantOrganizationalUnitObjectGuid = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.IsTenantConfigUnitInConfigNc = (bool)backSyncCookieReader.GetNextAttributeValue();
					byte[] array = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array != null)
					{
						this.PartitionHint = TenantPartitionHint.Deserialize(array);
					}
					this.State = (TenantRelocationSyncState)backSyncCookieReader.GetNextAttributeValue();
					this.ConfigUnitObjectUSN = (long)backSyncCookieReader.GetNextAttributeValue();
					this.ConfigUnitTombstoneUSN = (long)backSyncCookieReader.GetNextAttributeValue();
					this.OrganizationalUnitObjectUSN = (long)backSyncCookieReader.GetNextAttributeValue();
					this.OrganizationalUnitTombstoneUSN = (long)backSyncCookieReader.GetNextAttributeValue();
					this.SpecialObjectsUSN = (long)backSyncCookieReader.GetNextAttributeValue();
					byte[] array2 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array2 != null)
					{
						this.ConfigNcWatermarks = WatermarkMap.Parse(array2);
					}
					byte[] array3 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array3 != null)
					{
						this.PendingConfigNcWatermarks = WatermarkMap.Parse(array3);
					}
					byte[] array4 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array4 != null)
					{
						this.Watermarks = WatermarkMap.Parse(array4);
					}
					this.WatermarksInvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					this.PendingWatermarksInvocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					byte[] array5 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array5 != null)
					{
						this.PendingWatermarks = WatermarkMap.Parse(array5);
					}
					this.LinkPageStart = (long)backSyncCookieReader.GetNextAttributeValue();
					this.LinkPageEnd = (long)backSyncCookieReader.GetNextAttributeValue();
					this.LinkRangeStart = (int)backSyncCookieReader.GetNextAttributeValue();
					this.ObjectsInLinkPage = (int)backSyncCookieReader.GetNextAttributeValue();
					this.AffinityDcFqdn = (string)backSyncCookieReader.GetNextAttributeValue();
					this.AffinityTargetDcFqdn = (string)backSyncCookieReader.GetNextAttributeValue();
					this.PreSyncLdapPagingCookie = (byte[])backSyncCookieReader.GetNextAttributeValue();
				}
			}
			catch (ArgumentException ex2)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken ArgumentException {0}", ex2.ToString());
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken IOException {0}", ex3.ToString());
				ex = ex3;
			}
			catch (FormatException ex4)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken FormatException {0}", ex4.ToString());
				ex = ex4;
			}
			catch (InvalidCookieException ex5)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken InvalidCookieException {0}", ex5.ToString());
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError<string>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken throw InvalidCookieException {0}", ex.ToString());
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

		public bool IsPreSyncPhase
		{
			get
			{
				return this.State <= TenantRelocationSyncState.PreSyncAllObjects;
			}
		}

		public bool MoreData
		{
			get
			{
				return this.State != TenantRelocationSyncState.Complete;
			}
		}

		public DateTime Timestamp { get; set; }

		public DateTime LastReadFailureStartTime { get; set; }

		public virtual byte[] ToByteArray()
		{
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(TenantRelocationSyncPageToken)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(this.Version);
				backSyncCookieWriter.WriteNextAttributeValue("Exchange/SDF");
				backSyncCookieWriter.WriteNextAttributeValue(this.Timestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.LastReadFailureStartTime.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.InvocationId);
				backSyncCookieWriter.WriteNextAttributeValue(this.TenantConfigUnitObjectGuid);
				backSyncCookieWriter.WriteNextAttributeValue(this.TenantOrganizationalUnitObjectGuid);
				backSyncCookieWriter.WriteNextAttributeValue(this.IsTenantConfigUnitInConfigNc);
				if (this.PartitionHint == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				backSyncCookieWriter.WriteNextAttributeValue(TenantPartitionHint.Serialize(this.PartitionHint));
				backSyncCookieWriter.WriteNextAttributeValue((int)this.State);
				backSyncCookieWriter.WriteNextAttributeValue(this.ConfigUnitObjectUSN);
				backSyncCookieWriter.WriteNextAttributeValue(this.ConfigUnitTombstoneUSN);
				backSyncCookieWriter.WriteNextAttributeValue(this.OrganizationalUnitObjectUSN);
				backSyncCookieWriter.WriteNextAttributeValue(this.OrganizationalUnitTombstoneUSN);
				backSyncCookieWriter.WriteNextAttributeValue(this.SpecialObjectsUSN);
				if (this.ConfigNcWatermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue = this.ConfigNcWatermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue);
				}
				if (this.PendingConfigNcWatermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue2 = this.PendingConfigNcWatermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue2);
				}
				if (this.Watermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue3 = this.Watermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue3);
				}
				backSyncCookieWriter.WriteNextAttributeValue(this.WatermarksInvocationId);
				backSyncCookieWriter.WriteNextAttributeValue(this.PendingWatermarksInvocationId);
				if (this.PendingWatermarks == null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				else
				{
					byte[] attributeValue4 = this.PendingWatermarks.SerializeToBytes();
					backSyncCookieWriter.WriteNextAttributeValue(attributeValue4);
				}
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkPageStart);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkPageEnd);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkRangeStart);
				backSyncCookieWriter.WriteNextAttributeValue(this.ObjectsInLinkPage);
				backSyncCookieWriter.WriteNextAttributeValue(this.AffinityDcFqdn);
				backSyncCookieWriter.WriteNextAttributeValue(this.AffinityTargetDcFqdn);
				backSyncCookieWriter.WriteNextAttributeValue(this.PreSyncLdapPagingCookie);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		public void Reset()
		{
			if (this.State >= TenantRelocationSyncState.EnumerateConfigUnitLiveObjects)
			{
				this.State = TenantRelocationSyncState.EnumerateConfigUnitLiveObjects;
				return;
			}
			this.State = TenantRelocationSyncState.PreSyncAllObjects;
		}

		public void ResetPresyncCookie()
		{
			if (this.IsPreSyncPhase)
			{
				this.PreSyncLdapPagingCookie = null;
			}
		}

		public void PrepareForFailover()
		{
			this.InvocationId = Guid.Empty;
			this.AffinityDcFqdn = null;
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

		internal void SetInvocationId(Guid newInvocationId, string dcFqdn)
		{
			if (this.InvocationId == newInvocationId)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId: new invocationId is the same as the existing one, nothing to do");
				return;
			}
			this.InvocationId = newInvocationId;
			this.AffinityDcFqdn = dcFqdn;
			this.LastReadFailureStartTime = DateTime.MinValue;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId set new invocationId {0}", newInvocationId);
			long num = 0L;
			long num2 = 0L;
			if (this.Watermarks != null)
			{
				this.Watermarks.TryGetValue(this.InvocationId, out num);
			}
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId domain USN for the new invocationId is {0}", num);
			if (this.ConfigNcWatermarks != null)
			{
				this.ConfigNcWatermarks.TryGetValue(this.InvocationId, out num2);
			}
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId config NC USN for the new invocationId is {0}", num2);
			if (this.IsTenantConfigUnitInConfigNc)
			{
				this.ConfigUnitObjectUSN = num2 + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId ConfigUnitObjectUSN is set to {0}", this.ConfigUnitObjectUSN);
				this.ConfigUnitTombstoneUSN = num2 + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId ConfigUnitTombstoneUSN is set to {0}", this.ConfigUnitTombstoneUSN);
			}
			else
			{
				this.ConfigUnitObjectUSN = num + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId ConfigUnitObjectUSN is set to {0}", this.ConfigUnitObjectUSN);
				this.ConfigUnitTombstoneUSN = num + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId ConfigUnitTombstoneUSN is set to {0}", this.ConfigUnitTombstoneUSN);
			}
			this.OrganizationalUnitObjectUSN = num + 1L;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId OrganizationalUnitObjectUSN is set to {0}", this.OrganizationalUnitObjectUSN);
			this.OrganizationalUnitTombstoneUSN = num + 1L;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SetInvocationId OrganizationalUnitTombstoneUSN is set to {0}", this.OrganizationalUnitTombstoneUSN);
			this.SpecialObjectsUSN = num2 + 1L;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantFullSyncPageToken.SetInvocationId SpecialObjectsUSN is set to {0}", this.SpecialObjectsUSN);
			this.PendingWatermarks = null;
			this.PendingConfigNcWatermarks = null;
			this.PreSyncLdapPagingCookie = null;
			this.Reset();
		}

		public Exception SetErrorState(Exception e)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (this.LastReadFailureStartTime == DateTime.MinValue)
			{
				this.LastReadFailureStartTime = utcNow;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<DateTime, Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "SetErrorState: LastReadFailureStartTime set, time:{0}, tenant={1}", utcNow, this.TenantConfigUnitObjectGuid);
				return e;
			}
			if (utcNow.Subtract(this.LastReadFailureStartTime) < TenantRelocationSyncConfiguration.FailoverTimeout)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<DateTime, DateTime, Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "SetErrorState: subsequent error set, LastReadFailureStartTime:{0}, time:{1}, tenant={2}", this.LastReadFailureStartTime, DateTime.UtcNow, this.TenantConfigUnitObjectGuid);
				return e;
			}
			string text;
			LocalizedString localizedString;
			if (!string.IsNullOrEmpty(this.AffinityDcFqdn) && !SuitabilityVerifier.IsServerSuitableIgnoreExceptions(this.AffinityDcFqdn, false, null, out text, out localizedString))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<DateTime, DateTime, Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "SetErrorState: source failover triggered, LastReadFailureStartTime:{0}, time:{1}, tenant={2}", this.LastReadFailureStartTime, DateTime.UtcNow, this.TenantConfigUnitObjectGuid);
				this.PrepareForFailover();
			}
			if (!string.IsNullOrEmpty(this.AffinityTargetDcFqdn) && !SuitabilityVerifier.IsServerSuitableIgnoreExceptions(this.AffinityTargetDcFqdn, false, null, out text, out localizedString))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<DateTime, DateTime, Guid>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "SetErrorState: target failover triggered, LastReadFailureStartTime:{0}, time:{1}, tenant={2}", this.LastReadFailureStartTime, DateTime.UtcNow, this.TenantConfigUnitObjectGuid);
				this.AffinityTargetDcFqdn = null;
			}
			return e;
		}

		internal void SwitchToCompleteState()
		{
			this.CheckLinkPropertiesAreEmpty();
			this.State = TenantRelocationSyncState.Complete;
			this.Watermarks = this.PendingWatermarks;
			this.ConfigNcWatermarks = this.PendingConfigNcWatermarks;
			this.WatermarksInvocationId = this.PendingWatermarksInvocationId;
			long num;
			this.Watermarks.TryGetValue(this.InvocationId, out num);
			long num2;
			this.ConfigNcWatermarks.TryGetValue(this.InvocationId, out num2);
			if (this.OrganizationalUnitObjectUSN <= num)
			{
				this.OrganizationalUnitObjectUSN = num + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState OrganizationalUnitObjectUSN is set to {0}", this.OrganizationalUnitObjectUSN);
			}
			if (this.OrganizationalUnitTombstoneUSN <= num)
			{
				this.OrganizationalUnitTombstoneUSN = num + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState OrganizationalUnitTombstoneUSN is set to {0}", this.OrganizationalUnitTombstoneUSN);
			}
			if (this.IsTenantConfigUnitInConfigNc)
			{
				if (this.ConfigUnitObjectUSN <= num2)
				{
					this.ConfigUnitObjectUSN = num2 + 1L;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState ConfigUnitObjectUSN is set to {0}", this.ConfigUnitObjectUSN);
				}
				if (this.ConfigUnitTombstoneUSN <= num2)
				{
					this.ConfigUnitTombstoneUSN = num2 + 1L;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState ConfigUnitTombstoneUSN is set to {0}", this.ConfigUnitTombstoneUSN);
				}
			}
			else
			{
				if (this.ConfigUnitObjectUSN <= num)
				{
					this.ConfigUnitObjectUSN = num + 1L;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState ConfigUnitObjectUSN is set to {0}", this.ConfigUnitObjectUSN);
				}
				if (this.ConfigUnitTombstoneUSN <= num)
				{
					this.ConfigUnitTombstoneUSN = num + 1L;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState ConfigUnitTombstoneUSN is set to {0}", this.ConfigUnitTombstoneUSN);
				}
			}
			if (this.SpecialObjectsUSN <= num2)
			{
				this.SpecialObjectsUSN = num2 + 1L;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.SwitchToCompleteState SpecialObjectsUSN is set to {0}", this.SpecialObjectsUSN);
			}
		}

		internal void SwitchToNextState()
		{
			this.CheckLinkPropertiesAreEmpty();
			switch (this.State)
			{
			case TenantRelocationSyncState.PreSyncAllObjects:
				this.State = TenantRelocationSyncState.EnumerateSpecialObjects;
				return;
			case TenantRelocationSyncState.EnumerateConfigUnitLiveObjects:
			case TenantRelocationSyncState.EnumerateConfigUnitLinksInPage:
				this.State = TenantRelocationSyncState.EnumerateOrganizationalUnitLiveObjects;
				return;
			case TenantRelocationSyncState.EnumerateOrganizationalUnitLiveObjects:
			case TenantRelocationSyncState.EnumerateOrganizationalUnitLinksInPage:
				this.State = TenantRelocationSyncState.EnumerateConfigUnitDeletedObjects;
				return;
			case TenantRelocationSyncState.EnumerateConfigUnitDeletedObjects:
				this.State = TenantRelocationSyncState.EnumerateOrganizationalUnitDeletedObjects;
				return;
			case TenantRelocationSyncState.EnumerateOrganizationalUnitDeletedObjects:
				this.State = TenantRelocationSyncState.EnumerateSpecialObjects;
				return;
			case TenantRelocationSyncState.EnumerateSpecialObjects:
				this.SwitchToCompleteState();
				return;
			default:
				throw new InvalidOperationException("State transition");
			}
		}

		private void CheckLinkPropertiesAreEmpty()
		{
			ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "TenantRelocationSyncPageToken.CheckLinkPropertiesAreEmpty entering");
			if (this.PreSyncLdapPagingCookie != null)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceError((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "this.PreSyncLdapPagingCookie != null");
				throw new InvalidOperationException("PreSyncLdapPagingCookie");
			}
		}

		internal void SwitchToEnumerateLiveObjectsState()
		{
			if (this.State != TenantRelocationSyncState.EnumerateConfigUnitLinksInPage && this.State != TenantRelocationSyncState.EnumerateOrganizationalUnitLinksInPage)
			{
				throw new InvalidOperationException("State");
			}
			if (this.State == TenantRelocationSyncState.EnumerateConfigUnitLinksInPage)
			{
				this.State = TenantRelocationSyncState.EnumerateConfigUnitLiveObjects;
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid, Guid, long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "Starting enumeration of live objects for {0} on {1} from USN {2}", this.TenantConfigUnitObjectGuid, this.InvocationId, this.ConfigUnitObjectUSN);
				return;
			}
			this.State = TenantRelocationSyncState.EnumerateOrganizationalUnitLiveObjects;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid, Guid, long>((long)this.TenantConfigUnitObjectGuid.GetHashCode(), "Starting enumeration of live objects for {0} on {1} from USN {2}", this.TenantConfigUnitObjectGuid, this.InvocationId, this.OrganizationalUnitObjectUSN);
		}

		internal const int CurrentVersion = 1;

		internal static BackSyncCookieAttribute[] TenantRelocationSyncPageTokenAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
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
				Name = "TenantConfigUnitObjectGuid",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantOrganizationalUnitObjectGuid",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "IsTenantConfigUnitInConfigNc",
				DataType = typeof(bool),
				DefaultValue = true
			},
			new BackSyncCookieAttribute
			{
				Name = "PartitionHint",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "TenantRelocationSyncState",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "ConfigUnitObjectUSN",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "ConfigUnitTombstoneUSN",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "OrganizationalUnitObjectUSN",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "OrganizationalUnitTombstoneUSN",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "SpecialObjectsUSN",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "ConfigNcWaterMarks",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "PendingConfigNcWaterMarks",
				DataType = typeof(byte[]),
				DefaultValue = null
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
				Name = "PendingWatermarksInvocationId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "PendingWatermarks",
				DataType = typeof(byte[]),
				DefaultValue = null
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
			},
			new BackSyncCookieAttribute
			{
				Name = "AffinityDcFqdn",
				DataType = typeof(string),
				DefaultValue = string.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "AffinityTargetDcFqdn",
				DataType = typeof(string),
				DefaultValue = string.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "PreSyncLdapPagingCookie",
				DataType = typeof(byte[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[][] TenantRelocationSyncPageTokenAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			TenantRelocationSyncPageToken.TenantRelocationSyncPageTokenAttributeSchema_Version_1
		};
	}
}
