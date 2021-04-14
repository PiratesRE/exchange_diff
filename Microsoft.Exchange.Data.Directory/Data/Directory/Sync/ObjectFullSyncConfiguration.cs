using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class ObjectFullSyncConfiguration : FullSyncConfiguration
	{
		private ObjectFullSyncPageToken PageToken
		{
			get
			{
				return (ObjectFullSyncPageToken)base.FullSyncPageToken;
			}
		}

		public ObjectFullSyncConfiguration(ObjectFullSyncPageToken pageToken, Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter) : base(pageToken, invocationId, writeResult, eventLogger, excludedObjectReporter, ObjectFullSyncPageToken.Parse(pageToken.ToByteArray()))
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New ObjectFullSyncConfiguration");
		}

		public override IEnumerable<ADRawEntry> GetDataPage()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage entering");
			while (this.ShouldReadMoreData())
			{
				if (this.ShouldContinueLinkIteration())
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage continue link iteration ...");
					ADRawEntry linkEntry = this.ReadNextLinkPage();
					if (linkEntry != null)
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage linkEntry {0}", linkEntry.Id);
						yield return linkEntry;
					}
				}
				else
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage read objects...");
					foreach (ADRawEntry entry in this.ReadObjects())
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage entry {0}", entry.Id);
						yield return entry;
					}
				}
			}
			this.PageToken.LastReadFailureStartTime = DateTime.MinValue;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage set this.PageToken.LastReadFailureStartTime to {0}", this.PageToken.LastReadFailureStartTime);
			this.PageToken.Timestamp = DateTime.UtcNow;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.GetDataPage set this.PageToken.Timestamp to {0}", this.PageToken.Timestamp);
			yield break;
		}

		protected virtual Result<ADRawEntry>[] FindByExternalDirectoryObjectIds(string[] externalObjectIds, bool includeDeletedObjects, PropertyDefinition[] properties)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.FindByExternalDirectoryObjectIds entering");
			return base.RecipientSession.FindByExternalDirectoryObjectIds(externalObjectIds, includeDeletedObjects, properties);
		}

		private IEnumerable<ADRawEntry> ReadObjects()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects entering");
			bool includeLinks = SyncConfiguration.InlcudeLinks(this.PageToken.SyncOptions);
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects includeLinks = {0}", includeLinks);
			PropertyDefinition[] properties = FullSyncConfiguration.GetPropertyDefinitions(includeLinks);
			int objectCount = Math.Min(this.PageToken.ObjectIds.Count, FullSyncConfiguration.ObjectsPerPageLimit);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects objectCount = {0}", objectCount);
			string[] externalObjectIds = new string[objectCount];
			IList<SyncObjectId> objectIds = this.PageToken.ObjectIds.ToList<SyncObjectId>();
			for (int k = 0; k < objectCount; k++)
			{
				externalObjectIds[k] = objectIds[k].ObjectId;
				ExTraceGlobals.BackSyncTracer.TraceDebug<int, string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects externalObjectIds[{0}] = \"{1}\"", k, externalObjectIds[k]);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects FindByExternalDirectoryObjectIds ...");
			Result<ADRawEntry>[] results = this.FindByExternalDirectoryObjectIds(externalObjectIds, true, properties);
			List<ADRawEntry> completeEntries = new List<ADRawEntry>();
			List<ADRawEntry> deletedEntries = new List<ADRawEntry>();
			List<ADRawEntry> pagingRequiredEntries = new List<ADRawEntry>();
			for (int l = 0; l < externalObjectIds.Length; l++)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<int, string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects externalObjectIds[{0}] = \"{1}\"", l, externalObjectIds[l]);
				if (results[l].Data != null)
				{
					ADRawEntry data = results[l].Data;
					ExTraceGlobals.BackSyncTracer.TraceDebug<int, ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects found entry[{0}] = \"{1}\"", l, data.Id);
					data.propertyBag.SetField(SyncObjectSchema.SyncObjectId, objectIds[l]);
					if ((bool)data[SyncObjectSchema.Deleted])
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects add entry to deletedEntries");
						deletedEntries.Add(data);
					}
					else
					{
						MultiValuedProperty<LinkMetadata> multiValuedProperty = (MultiValuedProperty<LinkMetadata>)data[FullSyncConfiguration.InitialLinkMetadataRangedProperty];
						data.propertyBag.SetField(ADRecipientSchema.LinkMetadata, multiValuedProperty);
						if (FullSyncConfiguration.NotAllLinksRetrieved(multiValuedProperty))
						{
							ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects add entry to pagingRequiredEntries");
							pagingRequiredEntries.Add(data);
						}
						else
						{
							ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects add entry to completeEntries");
							completeEntries.Add(data);
						}
					}
				}
				else if (results[l].Error == ProviderError.NotFound)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects not found entry[{0}]", l);
					SyncObjectId syncObjectId = objectIds[l];
					base.ExcludedObjectReporter.ReportExcludedObject(syncObjectId, DirectoryObjectErrorCode.ObjectNotFound, ProcessingStage.ObjectFullSyncConfiguration);
					this.PageToken.ObjectIds.Remove(syncObjectId);
				}
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects process deletedEntries ...");
			int i = 0;
			while (i < deletedEntries.Count && this.ShouldReadMoreData())
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects remove {0} from this.PageToken.ObjectIds", deletedEntries[i].Id);
				this.PageToken.ObjectIds.Remove((SyncObjectId)deletedEntries[i][SyncObjectSchema.SyncObjectId]);
				base.ReturnedObjectCount++;
				yield return deletedEntries[i];
				i++;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects process completeEntries ...");
			int j = 0;
			while (j < completeEntries.Count && this.ShouldReadMoreData())
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects remove {0} from this.PageToken.ObjectIds", completeEntries[j].Id);
				this.PageToken.ObjectIds.Remove((SyncObjectId)completeEntries[j][SyncObjectSchema.SyncObjectId]);
				base.ReturnedObjectCount++;
				base.ReturnedLinkCount += ((MultiValuedProperty<LinkMetadata>)completeEntries[j][FullSyncConfiguration.InitialLinkMetadataRangedProperty]).Count;
				yield return completeEntries[j];
				j++;
			}
			if (this.ShouldReadMoreData() && pagingRequiredEntries.Count > 0)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects read more data ...");
				ADRawEntry linkEntry = pagingRequiredEntries[0];
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadObjects linkEntry {0}", linkEntry.Id);
				base.ReturnedObjectCount++;
				MultiValuedProperty<LinkMetadata> metadata = (MultiValuedProperty<LinkMetadata>)linkEntry[FullSyncConfiguration.InitialLinkMetadataRangedProperty];
				LinkMetadata overlapLink = metadata[0];
				base.ReturnedLinkCount += metadata.Count;
				int nextPageLinkRangeStart = metadata.ValueRange.UpperBound - 10 + 1;
				this.PageToken.ObjectCookie = new FullSyncObjectCookie((SyncObjectId)linkEntry[SyncObjectSchema.SyncObjectId], overlapLink, nextPageLinkRangeStart, (long)linkEntry[ADRecipientSchema.UsnChanged], this.PageToken.ServiceInstanceId);
				yield return linkEntry;
			}
			yield break;
		}

		private ADRawEntry ReadNextLinkPage()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage entering");
			SyncObjectId objectId = this.PageToken.ObjectCookie.ObjectId;
			if (this.PageToken.ObjectCookie.ReadRestartsCount > 10)
			{
				if (base.EventLogger != null)
				{
					base.EventLogger.LogTooManyObjectReadRestartsEvent(objectId.ToString(), 10);
				}
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage TooManyObjectReadRestarts {0}", objectId.ToString());
				base.ExcludedObjectReporter.ReportExcludedObject(objectId, DirectoryObjectErrorCode.Busy, ProcessingStage.ObjectFullSyncConfiguration);
				this.PageToken.RemoveObjectFromList(objectId, true);
				return null;
			}
			int num = Math.Max(0, FullSyncConfiguration.LinksPerPageLimit - base.ReturnedLinkCount);
			if (num <= 10)
			{
				return null;
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			int linkRangeStart = this.PageToken.ObjectCookie.LinkRangeStart;
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage this.PageToken.ObjectCookie.LinkRangeStart = {0}", this.PageToken.ObjectCookie.LinkRangeStart);
			ADPropertyDefinition adpropertyDefinition = RangedPropertyHelper.CreateRangedProperty(ADRecipientSchema.LinkMetadata, new IntRange(linkRangeStart, linkRangeStart + num - 1));
			list.Add(adpropertyDefinition);
			list.Add(ADRecipientSchema.UsnChanged);
			bool flag = !this.PageToken.ObjectCookie.EnumerateLinks;
			if (flag)
			{
				list.AddRange(SyncSchema.Instance.AllBackSyncBaseProperties.Cast<PropertyDefinition>());
				list.AddRange(SyncObject.BackSyncProperties.Cast<PropertyDefinition>());
			}
			else
			{
				list.AddRange(SyncObject.FullSyncLinkPageBackSyncProperties.Cast<PropertyDefinition>());
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage objectId.ObjectId = {0}", objectId.ObjectId);
			string[] externalObjectIds = new string[]
			{
				objectId.ObjectId
			};
			Result<ADRawEntry>[] array = this.FindByExternalDirectoryObjectIds(externalObjectIds, true, list.ToArray());
			Result<ADRawEntry> result = array[0];
			if (result.Data != null)
			{
				ADRawEntry data = result.Data;
				ExTraceGlobals.BackSyncTracer.TraceDebug<ADObjectId>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage entry = {0}", data.Id);
				long num2 = (long)data[ADRecipientSchema.UsnChanged];
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage usnChanged = {0}", num2);
				MultiValuedProperty<LinkMetadata> multiValuedProperty = (MultiValuedProperty<LinkMetadata>)data[adpropertyDefinition];
				if (!flag)
				{
					if (num2 != this.PageToken.ObjectCookie.UsnChanged)
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage RestartObjectReadAfterObjectChange usnChanged = {0}", num2);
						this.PageToken.ObjectCookie.RestartObjectReadAfterObjectChange(num2);
						return null;
					}
					if (this.PageToken.ObjectCookie.LinkRangeStart > 0 && !this.CheckLinkOverlap(multiValuedProperty))
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage RestartObjectReadAfterTargetLinksChange");
						this.PageToken.ObjectCookie.RestartObjectReadAfterTargetLinksChange();
						return null;
					}
				}
				result.Data.propertyBag.SetField(ADRecipientSchema.LinkMetadata, multiValuedProperty);
				LinkMetadata overlapLink = multiValuedProperty[0];
				base.ReturnedLinkCount += multiValuedProperty.Count;
				if (multiValuedProperty.ValueRange != null)
				{
					if (multiValuedProperty.ValueRange.UpperBound == 2147483647)
					{
						this.PageToken.RemoveObjectFromList(objectId, true);
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage last link page read");
					}
					else
					{
						int num3 = multiValuedProperty.ValueRange.UpperBound - 10 + 1;
						ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage set this.PageToken.nextLinkPageRangeStart to {0}", num3);
						this.PageToken.ObjectCookie.SetNextPageData(objectId, overlapLink, num3, (long)result.Data[ADRecipientSchema.UsnChanged]);
					}
				}
				else
				{
					this.PageToken.RemoveObjectFromList(objectId, true);
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage this.PageToken.RemoveObjectFromList objectId = {0}", objectId.ObjectId);
				}
				return result.Data;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.ReadNextLinkPage FindByExternalDirectoryObjectIds returned no data.");
			this.PageToken.ObjectCookie.RestartObjectRead(false);
			return null;
		}

		public override DirectoryObjectError[] GetReportedErrors()
		{
			return base.ExcludedObjectReporter.GetDirectoryObjectErrors();
		}

		private bool CheckLinkOverlap(MultiValuedProperty<LinkMetadata> metadata)
		{
			int num = Math.Min(10, metadata.Count);
			for (int i = metadata.Count - num; i < metadata.Count; i++)
			{
				LinkMetadata metadata2 = metadata[i];
				if (this.PageToken.ObjectCookie.ContainsOverlapLink(metadata2))
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.CheckLinkOverlap return true");
					return true;
				}
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration.CheckLinkOverlap return false");
			return false;
		}

		private bool ShouldContinueLinkIteration()
		{
			bool flag = this.PageToken.ObjectCookie != null;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration shouldContinueLinkIteration = {0}", flag);
			return flag;
		}

		protected override bool ShouldReadMoreData()
		{
			bool flag = base.ShouldReadMoreData() && this.PageToken.ObjectIds.Count > 0 && base.ReturnedLinkCount < FullSyncConfiguration.LinksPerPageLimit - 10;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "ObjectFullSyncConfiguration shouldReadMoreData = {0}", flag);
			return flag;
		}

		private const int PagedLinkReadOverlapSize = 10;

		private const int PagedLinkReadRestartsLimit = 10;
	}
}
