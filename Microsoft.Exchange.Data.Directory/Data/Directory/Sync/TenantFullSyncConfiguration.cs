using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class TenantFullSyncConfiguration : FullSyncConfiguration
	{
		private TenantFullSyncPageToken PageToken
		{
			get
			{
				return (TenantFullSyncPageToken)base.FullSyncPageToken;
			}
		}

		public TenantFullSyncConfiguration(TenantFullSyncPageToken pageToken, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter) : this(pageToken, invocationId, writeResult, eventLogger, excludedObjectReporter, TenantFullSyncPageToken.Parse(pageToken.ToByteArray()))
		{
		}

		public TenantFullSyncConfiguration(TenantFullSyncPageToken pageToken, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter, TenantFullSyncPageToken originalToken) : base(pageToken, invocationId, writeResult, eventLogger, excludedObjectReporter, originalToken)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New TenantFullSyncConfiguration");
		}

		public override IEnumerable<ADRawEntry> GetDataPage()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration GetDataPage ...");
			WatermarkMap watermarks = SyncConfiguration.GetReplicationCursors(base.RootOrgConfigurationSession);
			string dcName = base.RootOrgConfigurationSession.DomainController;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration dcName {0}", dcName);
			Guid watermarksInvocationId = base.RootOrgConfigurationSession.GetInvocationIdByFqdn(dcName);
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration watermarksInvocationId {0}", watermarksInvocationId);
			long maxUsn = watermarks[watermarksInvocationId];
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration maxUsn {0}", maxUsn);
			if (this.PageToken.InvocationId == Guid.Empty)
			{
				this.PageToken.InvocationId = watermarksInvocationId;
				ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.InvocationId to {0}", this.PageToken.InvocationId);
			}
			while (this.ShouldReadMoreData())
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration this.PageToken.State = {0}", this.PageToken.State.ToString());
				switch (this.PageToken.State)
				{
				case TenantFullSyncState.EnumerateLiveObjects:
					foreach (ADRawEntry entry in this.ReadLiveObjects())
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration EnumerateLiveObjects yield {0}", entry.Id);
						yield return entry;
					}
					this.PageToken.PendingWatermarksInvocationId = watermarksInvocationId;
					ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration EnumerateLiveObjects set this.PageToken.PendingWatermarksInvocationId to {0}", this.PageToken.PendingWatermarksInvocationId);
					this.PageToken.PendingWatermarks = watermarks;
					continue;
				case TenantFullSyncState.EnumerateLinksInPage:
					foreach (ADRawEntry entry2 in this.ReadLinks())
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration EnumerateLinksInPage yield {0}", entry2.Id);
						yield return entry2;
					}
					continue;
				case TenantFullSyncState.EnumerateDeletedObjects:
					foreach (ADRawEntry entry3 in this.ReadDeletedObjects())
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration EnumerateDeletedObjects yield {0}", entry3.Id);
						yield return entry3;
					}
					continue;
				case TenantFullSyncState.EnumerateSoftDeletedObjects:
					foreach (ADRawEntry entry4 in this.ReadSoftDeletedObjects())
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration EnumerateSoftDeletedObjects yield {0}", entry4.Id);
						yield return entry4;
					}
					continue;
				}
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Invalid PageToken stat {0}", this.PageToken.State.ToString());
				throw new ArgumentException(this.PageToken.State.ToString());
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration this.PageToken.ObjectUpdateSequenceNumber = {0}", this.PageToken.ObjectUpdateSequenceNumber);
			if (this.PageToken.ObjectUpdateSequenceNumber > maxUsn + 1L)
			{
				this.PageToken.ObjectUpdateSequenceNumber = maxUsn + 1L;
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.ObjectUpdateSequenceNumber to {0}", this.PageToken.ObjectUpdateSequenceNumber);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration this.PageToken.TombstoneUpdateSequenceNumber = {0}", this.PageToken.TombstoneUpdateSequenceNumber);
			if (this.PageToken.TombstoneUpdateSequenceNumber > maxUsn + 1L)
			{
				this.PageToken.TombstoneUpdateSequenceNumber = maxUsn + 1L;
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.TombstoneUpdateSequenceNumber to {0}", this.PageToken.TombstoneUpdateSequenceNumber);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration this.PageToken.SoftDeletedObjectUpdateSequenceNumber = {0}", this.PageToken.SoftDeletedObjectUpdateSequenceNumber);
			if (this.PageToken.SoftDeletedObjectUpdateSequenceNumber > maxUsn + 1L)
			{
				this.PageToken.SoftDeletedObjectUpdateSequenceNumber = maxUsn + 1L;
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.SoftDeletedObjectUpdateSequenceNumber to {0}", this.PageToken.SoftDeletedObjectUpdateSequenceNumber);
			}
			this.PageToken.LastReadFailureStartTime = DateTime.MinValue;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.LastReadFailureStartTime to {0}", this.PageToken.LastReadFailureStartTime);
			this.PageToken.Timestamp = DateTime.UtcNow;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration set this.PageToken.Timestamp to {0}", this.PageToken.Timestamp);
			yield break;
		}

		protected override bool ShouldReadMoreData()
		{
			bool flag = base.ShouldReadMoreData() && this.PageToken.MoreData && this.PageToken.ObjectsInLinkPage <= FullSyncConfiguration.LinksPerPageLimit - base.ReturnedLinkCount;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration ShouldReadMoreData = {0}", flag);
			return flag;
		}

		private IEnumerable<ADRawEntry> ReadLiveObjects()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration ReadLiveObjects ...");
			PropertyDefinition[] properties = FullSyncConfiguration.GetPropertyDefinitions(true);
			int sizeLimit = Math.Max(1, FullSyncConfiguration.ObjectsPerPageLimit - base.ReturnedObjectCount);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects sizeLimit = {0}", sizeLimit);
			ADObjectId root = base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit;
			ADRawEntry[] results = base.RecipientSession.FindADRawEntryByUsnRange(root, this.PageToken.ObjectUpdateSequenceNumber, long.MaxValue, sizeLimit, properties, this.PageToken.UseContainerizedUsnChangedIndex ? QueryScope.OneLevel : QueryScope.SubTree, this.PageToken.UseContainerizedUsnChangedIndex ? SyncRecipient.SyncRecipientObjectTypeFilterOptDisabled : SyncRecipient.SyncRecipientObjectTypeFilter);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects results count = {0}", (results != null) ? results.Length : 0);
			long startLinkPageUsn = 0L;
			long endLinkPageUsn = 0L;
			int linkPageObjectCount = 0;
			long maxUsnReturned = long.MaxValue;
			bool returnedAllResults = true;
			foreach (ADRawEntry entry in results)
			{
				if (!this.ShouldReadMoreData())
				{
					returnedAllResults = false;
					break;
				}
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects entry {0}", entry.Id);
				MultiValuedProperty<LinkMetadata> linkMetadata = (MultiValuedProperty<LinkMetadata>)entry[FullSyncConfiguration.InitialLinkMetadataRangedProperty];
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects linkMetadata count = {0}", (linkMetadata != null) ? linkMetadata.Count : 0);
				entry.propertyBag.SetField(ADRecipientSchema.LinkMetadata, linkMetadata);
				long entryUsn = (long)entry[ADRecipientSchema.UsnChanged];
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects entryUsn = {0}", entryUsn);
				if (FullSyncConfiguration.NotAllLinksRetrieved(linkMetadata))
				{
					linkPageObjectCount++;
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects linkPageObjectCount = {0}", linkPageObjectCount);
					if (startLinkPageUsn == 0L)
					{
						startLinkPageUsn = entryUsn;
						ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects set startLinkPageUsn to {0}", startLinkPageUsn);
					}
					endLinkPageUsn = entryUsn;
					ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects set endLinkPageUsn to {0}", endLinkPageUsn);
				}
				base.ReturnedObjectCount++;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects this.ReturnedObjectCount = {0}", base.ReturnedObjectCount);
				base.ReturnedLinkCount += linkMetadata.Count;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects this.ReturnedLinkCount = {0}", base.ReturnedLinkCount);
				maxUsnReturned = entryUsn;
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects maxUsnReturned = {0}", maxUsnReturned);
				this.StampTenantContext(entry);
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects yield {0}", entry.Id);
				yield return entry;
			}
			this.PageToken.ObjectUpdateSequenceNumber = ((maxUsnReturned == long.MaxValue) ? long.MaxValue : (maxUsnReturned + 1L));
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects set this.PageToken.ObjectUpdateSequenceNumber to {0}", this.PageToken.ObjectUpdateSequenceNumber);
			if (linkPageObjectCount == 0)
			{
				if (results.Length < sizeLimit && returnedAllResults)
				{
					if (this.PageToken.UseContainerizedUsnChangedIndex)
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects switch to 'soft deleted objects' state");
						this.PageToken.SwitchToEnumerateSoftDeletedObjectsState();
					}
					else
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects switch to 'deleted objects' state");
						this.PageToken.SwitchToEnumerateDeletedObjectsState();
					}
				}
				else
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects do nothing, page token is already up to date");
				}
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLiveObjects switch to enumerate links state");
				this.PageToken.SwitchToEnumerateLinksState(startLinkPageUsn, endLinkPageUsn, linkPageObjectCount);
			}
			yield break;
		}

		private IEnumerable<ADRawEntry> ReadSoftDeletedObjects()
		{
			if (!this.PageToken.UseContainerizedUsnChangedIndex)
			{
				throw new InvalidOperationException("this.PageToken.UseContainerizedUsnChangedIndex is false");
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration ReadSoftDeletedObjects ...");
			PropertyDefinition[] properties = FullSyncConfiguration.GetPropertyDefinitions(true);
			int sizeLimit = Math.Max(1, FullSyncConfiguration.ObjectsPerPageLimit - base.ReturnedObjectCount);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects sizeLimit = {0}", sizeLimit);
			ADObjectId root = new ADObjectId("OU=Soft Deleted Objects," + base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit.DistinguishedName);
			ADRawEntry[] results;
			if (this.PageToken.SoftDeletedObjectUpdateSequenceNumber == 0L && !this.IsExistsSoftDeletedObjectsOU())
			{
				results = new ADRawEntry[0];
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects - Could not find OU=Soft Deleted Objects container. Returning empty page result");
			}
			else
			{
				results = base.RecipientSession.FindADRawEntryByUsnRange(root, this.PageToken.SoftDeletedObjectUpdateSequenceNumber, long.MaxValue, sizeLimit, properties, QueryScope.OneLevel, SyncRecipient.SyncRecipientObjectTypeFilterOptDisabled);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects results count = {0}", (results != null) ? results.Length : 0);
			long startLinkPageUsn = 0L;
			long endLinkPageUsn = 0L;
			int linkPageObjectCount = 0;
			long maxUsnReturned = long.MaxValue;
			bool returnedAllResults = true;
			foreach (ADRawEntry entry in results)
			{
				if (!this.ShouldReadMoreData())
				{
					returnedAllResults = false;
					break;
				}
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects entry {0}", entry.Id);
				MultiValuedProperty<LinkMetadata> linkMetadata = (MultiValuedProperty<LinkMetadata>)entry[FullSyncConfiguration.InitialLinkMetadataRangedProperty];
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects linkMetadata count = {0}", (linkMetadata != null) ? linkMetadata.Count : 0);
				entry.propertyBag.SetField(ADRecipientSchema.LinkMetadata, linkMetadata);
				long entryUsn = (long)entry[ADRecipientSchema.UsnChanged];
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects entryUsn = {0}", entryUsn);
				if (FullSyncConfiguration.NotAllLinksRetrieved(linkMetadata))
				{
					linkPageObjectCount++;
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects linkPageObjectCount = {0}", linkPageObjectCount);
					if (startLinkPageUsn == 0L)
					{
						startLinkPageUsn = entryUsn;
						ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects set startLinkPageUsn to {0}", startLinkPageUsn);
					}
					endLinkPageUsn = entryUsn;
					ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects set endLinkPageUsn to {0}", endLinkPageUsn);
				}
				base.ReturnedObjectCount++;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects this.ReturnedObjectCount = {0}", base.ReturnedObjectCount);
				base.ReturnedLinkCount += linkMetadata.Count;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects this.ReturnedLinkCount = {0}", base.ReturnedLinkCount);
				maxUsnReturned = entryUsn;
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects maxUsnReturned = {0}", maxUsnReturned);
				this.StampTenantContext(entry);
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects yield {0}", entry.Id);
				yield return entry;
			}
			this.PageToken.SoftDeletedObjectUpdateSequenceNumber = ((maxUsnReturned == long.MaxValue) ? long.MaxValue : (maxUsnReturned + 1L));
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects set this.PageToken.SoftDeletedObjectUpdateSequenceNumber to {0}", this.PageToken.SoftDeletedObjectUpdateSequenceNumber);
			if (linkPageObjectCount == 0)
			{
				if (results.Length < sizeLimit && returnedAllResults)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects switch to 'deleted objects' state");
					this.PageToken.SwitchToEnumerateDeletedObjectsState();
				}
				else
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects do nothing, page token is already up to date");
				}
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadSoftDeletedObjects switch to enumerate links state");
				this.PageToken.SwitchToEnumerateLinksState(startLinkPageUsn, endLinkPageUsn, linkPageObjectCount);
			}
			yield break;
		}

		private bool IsExistsSoftDeletedObjectsOU()
		{
			bool useConfigNC = base.TenantConfigurationSession.UseConfigNC;
			bool result;
			try
			{
				base.TenantConfigurationSession.UseConfigNC = false;
				ADObjectId adobjectId = new ADObjectId("OU=Soft Deleted Objects," + base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit.DistinguishedName);
				bool flag = base.TenantConfigurationSession.ReadADRawEntry(adobjectId, new PropertyDefinition[]
				{
					ADObjectSchema.Id
				}) != null;
				ExTraceGlobals.BackSyncTracer.TraceDebug<bool, string, bool>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.IsExistsSoftDeletedObjectsOU = {0}. DN used: {1} useConfigNCOldValue = {2}", flag, adobjectId.DistinguishedName, useConfigNC);
				result = flag;
			}
			finally
			{
				base.TenantConfigurationSession.UseConfigNC = useConfigNC;
			}
			return result;
		}

		private IEnumerable<ADRawEntry> ReadDeletedObjects()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects ...");
			FullSyncConfiguration.GetPropertyDefinitions(true);
			int sizeLimit = Math.Max(1, FullSyncConfiguration.ObjectsPerPageLimit - base.ReturnedObjectCount);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects sizeLimit = {0}", sizeLimit);
			ADRawEntry[] results = base.RecipientSession.FindDeletedADRawEntryByUsnRange(base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit, this.PageToken.TombstoneUpdateSequenceNumber, sizeLimit, TenantFullSyncConfiguration.PropertyDefinitionsForDeletedObjects, SyncRecipient.SyncRecipientObjectTypeFilter);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects results count = {0}", (results != null) ? results.Length : 0);
			long maxUsnReturned = long.MaxValue;
			foreach (ADRawEntry entry in results)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects entry {0}", entry.Id);
				base.ReturnedObjectCount++;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects this.ReturnedObjectCount = {0}", base.ReturnedObjectCount);
				maxUsnReturned = (long)entry[SyncObjectSchema.UsnChanged];
				this.StampTenantContext(entry);
				yield return entry;
			}
			this.PageToken.TombstoneUpdateSequenceNumber = ((maxUsnReturned == long.MaxValue) ? long.MaxValue : (maxUsnReturned + 1L));
			ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects set this.PageToken.TombstoneUpdateSequenceNumber to {0}", this.PageToken.TombstoneUpdateSequenceNumber);
			if (results.Length < sizeLimit)
			{
				this.FinishFullSync();
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects no more deleted objects, we are done");
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadDeletedObjects do nothing, page token is already up to date");
			}
			yield break;
		}

		protected virtual void FinishFullSync()
		{
			this.PageToken.FinishFullSync();
		}

		private IEnumerable<ADRawEntry> ReadLinks()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks ...");
			int numberOfLinksToRead = Math.Max(0, FullSyncConfiguration.LinksPerPageLimit - base.ReturnedLinkCount) / this.PageToken.ObjectsInLinkPage;
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks numberOfLinksToRead = {0}", numberOfLinksToRead);
			if (numberOfLinksToRead > 0)
			{
				List<PropertyDefinition> properties = new List<PropertyDefinition>();
				int rangeStart = this.PageToken.LinkRangeStart;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks rangeStart = {0}", rangeStart);
				int rangeEnd = rangeStart + numberOfLinksToRead - 1;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks rangeEnd = {0}", rangeEnd);
				ADPropertyDefinition linkMetadataRangedProperty = RangedPropertyHelper.CreateRangedProperty(ADRecipientSchema.LinkMetadata, new IntRange(rangeStart, rangeEnd));
				properties.Add(linkMetadataRangedProperty);
				properties.Add(ADRecipientSchema.UsnChanged);
				properties.AddRange(SyncObject.FullSyncLinkPageBackSyncProperties.Cast<PropertyDefinition>());
				ADObjectId root;
				if (this.PageToken.UseContainerizedUsnChangedIndex && this.PageToken.PreviousState == TenantFullSyncState.EnumerateSoftDeletedObjects)
				{
					root = new ADObjectId("OU=Soft Deleted Objects," + base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit.DistinguishedName);
				}
				else
				{
					root = base.RecipientSession.SessionSettings.CurrentOrganizationId.OrganizationalUnit;
				}
				ADRawEntry[] results = base.RecipientSession.FindADRawEntryByUsnRange(root, this.PageToken.LinkPageStart, this.PageToken.LinkPageEnd, 0, properties, this.PageToken.UseContainerizedUsnChangedIndex ? QueryScope.OneLevel : QueryScope.SubTree, this.PageToken.UseContainerizedUsnChangedIndex ? SyncRecipient.SyncRecipientObjectTypeFilterOptDisabled : SyncRecipient.SyncRecipientObjectTypeFilter);
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks results count = {0}", results.Length);
				List<ADRawEntry> entriesWithMoreLinks = new List<ADRawEntry>(results.Length);
				foreach (ADRawEntry entry in results)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks entry {0}", entry.Id);
					MultiValuedProperty<LinkMetadata> metadata = (MultiValuedProperty<LinkMetadata>)entry[linkMetadataRangedProperty];
					entry.propertyBag.SetField(ADRecipientSchema.LinkMetadata, metadata);
					if (metadata.ValueRange == null)
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks entry has no links. Skip it.");
					}
					else
					{
						if (metadata.ValueRange.UpperBound != 2147483647)
						{
							ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks add entry to entriesWithMoreLinks (count = {0})", entriesWithMoreLinks.Count);
							entriesWithMoreLinks.Add(entry);
						}
						base.ReturnedLinkCount += metadata.Count;
						ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks this.ReturnedLinkCount = {0}", base.ReturnedLinkCount);
						this.StampTenantContext(entry);
						yield return entry;
					}
				}
				if (entriesWithMoreLinks.Count == 0)
				{
					if (this.PageToken.UseContainerizedUsnChangedIndex && this.PageToken.PreviousState == TenantFullSyncState.EnumerateSoftDeletedObjects)
					{
						this.PageToken.SwitchToEnumerateSoftDeletedObjectsState();
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks done enumerating links on this page, switch back to enumerate soft-deleted objects state");
					}
					else
					{
						this.PageToken.SwitchToEnumerateLiveObjectsState();
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "TenantFullSyncConfiguration.ReadLinks done enumerating links on this page, switch back to live objects state");
					}
				}
				else
				{
					long linkPageStart = (long)entriesWithMoreLinks[0][ADRecipientSchema.UsnChanged];
					long linkPageEnd = (long)entriesWithMoreLinks[entriesWithMoreLinks.Count - 1][ADRecipientSchema.UsnChanged];
					this.PageToken.UpdateEnumerateLinksState(linkPageStart, linkPageEnd, rangeEnd + 1, entriesWithMoreLinks.Count);
				}
			}
			yield break;
		}

		private void StampTenantContext(ADRawEntry entry)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string, ADObjectId>((long)SyncConfiguration.TraceId, "Set context id {0} on entry {1}", this.PageToken.TenantExternalDirectoryId.ToString("D"), entry.Id);
			entry.propertyBag.SetField(SyncObjectSchema.ContextId, this.PageToken.TenantExternalDirectoryId.ToString("D"));
		}

		private static readonly PropertyDefinition[] PropertyDefinitionsForDeletedObjects = new PropertyDefinition[]
		{
			ADObjectSchema.ObjectClass,
			SyncObjectSchema.UsnChanged,
			SyncObjectSchema.ObjectId,
			SyncObjectSchema.LastKnownParent,
			SyncObjectSchema.Deleted
		};
	}
}
