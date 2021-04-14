using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.AnchorService.Storage;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BandSettingsStorage : DisposeTrackableBase, IBandSettingsProvider, IDisposable
	{
		public BandSettingsStorage(IAnchorDataProvider anchorDataProvider, LoadBalanceAnchorContext anchorContext)
		{
			this.anchorDataProvider = anchorDataProvider;
			this.logger = anchorContext.Logger;
			this.loadBalanceAnchorContext = anchorContext;
		}

		public IEnumerable<Band> GetBandSettings()
		{
			base.CheckDisposed();
			IList<PersistedBandDefinition> list = this.GetPersistedBands().ToList<PersistedBandDefinition>();
			if (list.Count == 0)
			{
				return BandSettingsStorage.DefaultBandSettings;
			}
			return from persistedBand in list
			where persistedBand.IsEnabled
			select persistedBand.ToBand();
		}

		public PersistedBandDefinition PersistBand(Band band, bool enabled)
		{
			base.CheckDisposed();
			PersistedBandDefinition persistedBandDefinition = new PersistedBandDefinition(band, enabled);
			List<PersistedBandDefinition> source = this.GetPersistedBands().ToList<PersistedBandDefinition>();
			if (enabled)
			{
				if (source.Any((PersistedBandDefinition pbd) => pbd.IsEnabled))
				{
					this.CheckNewBandDoesntConflict(persistedBandDefinition, from pb in source
					where pb.IsEnabled
					select pb);
				}
			}
			new AnchorXmlSerializableObject<PersistedBandDefinition>(this.loadBalanceAnchorContext)
			{
				PersistedObject = persistedBandDefinition
			}.CreateInStore(this.anchorDataProvider, null);
			return persistedBandDefinition;
		}

		public IEnumerable<PersistedBandDefinition> GetPersistedBands()
		{
			base.CheckDisposed();
			return from persistedBandDefinition in this.GetBandDefinitionXmlSerializable()
			select persistedBandDefinition.PersistedObject;
		}

		public PersistedBandDefinition DisableBand(Band band)
		{
			base.CheckDisposed();
			AnchorXmlSerializableObject<PersistedBandDefinition> bandPersistedMessage = this.GetBandPersistedMessage(band);
			bandPersistedMessage.PersistedObject.IsEnabled = false;
			this.UpdatePersistedObject(bandPersistedMessage);
			return bandPersistedMessage.PersistedObject;
		}

		public PersistedBandDefinition EnableBand(Band band)
		{
			base.CheckDisposed();
			AnchorXmlSerializableObject<PersistedBandDefinition> bandPersistedMessage = this.GetBandPersistedMessage(band);
			this.CheckNewBandDoesntConflict(bandPersistedMessage.PersistedObject, from pb in this.GetPersistedBands()
			where pb.IsEnabled
			select pb);
			bandPersistedMessage.PersistedObject.IsEnabled = true;
			this.UpdatePersistedObject(bandPersistedMessage);
			return bandPersistedMessage.PersistedObject;
		}

		public PersistedBandDefinition RemoveBand(Band band)
		{
			base.CheckDisposed();
			AnchorXmlSerializableObject<PersistedBandDefinition> bandPersistedMessage = this.GetBandPersistedMessage(band);
			this.anchorDataProvider.RemoveMessage(bandPersistedMessage.StoreObjectId);
			return bandPersistedMessage.PersistedObject;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BandSettingsStorage>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.anchorDataProvider != null)
			{
				this.anchorDataProvider.Dispose();
			}
		}

		private AnchorXmlSerializableObject<PersistedBandDefinition> GetBandPersistedMessage(Band band)
		{
			AnchorXmlSerializableObject<PersistedBandDefinition> anchorXmlSerializableObject = this.GetBandDefinitionXmlSerializable().FirstOrDefault((AnchorXmlSerializableObject<PersistedBandDefinition> persisted) => persisted.PersistedObject.Matches(band));
			if (anchorXmlSerializableObject == null)
			{
				throw new BandDefinitionNotFoundException(band.ToString());
			}
			return anchorXmlSerializableObject;
		}

		private void CheckNewBandDoesntConflict(PersistedBandDefinition incomingBand, IEnumerable<PersistedBandDefinition> existingBands)
		{
			Band incoming = incomingBand.ToBand();
			Band band2 = (from persistedBand in existingBands
			select persistedBand.ToBand()).FirstOrDefault((Band band) => band.IsOverlap(incoming));
			if (band2 != null)
			{
				this.logger.LogError(null, "Band {0} conflicts with defined band {1}.", new object[]
				{
					incoming,
					band2
				});
				throw new OverlappingBandDefinitionException(incoming.ToString(), band2.ToString());
			}
		}

		private IEnumerable<AnchorXmlSerializableObject<PersistedBandDefinition>> GetBandDefinitionXmlSerializable()
		{
			QueryFilter messageType = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemClass, AnchorXmlSerializableObject<PersistedBandDefinition>.GetItemClass());
			IEnumerable<StoreObjectId> bandDefinitionMessageIds = this.anchorDataProvider.FindMessageIds(messageType, BandSettingsStorage.ItemClassPropertyDefinitions, BandSettingsStorage.SortByItemClassAscending, new AnchorRowSelector(AnchorXmlSerializableObject<PersistedBandDefinition>.SelectByItemClassAndStopProcessing), null);
			foreach (StoreObjectId bandDefinitionMessageId in bandDefinitionMessageIds)
			{
				AnchorXmlSerializableObject<PersistedBandDefinition> xmlSerializable = new AnchorXmlSerializableObject<PersistedBandDefinition>(this.loadBalanceAnchorContext);
				if (!xmlSerializable.TryLoad(this.anchorDataProvider, bandDefinitionMessageId))
				{
					this.logger.LogWarning("Could not load band definition from message {0}.", new object[]
					{
						bandDefinitionMessageId.ToBase64String()
					});
				}
				else
				{
					yield return xmlSerializable;
				}
			}
			yield break;
		}

		private void UpdatePersistedObject(AnchorXmlSerializableObject<PersistedBandDefinition> persisted)
		{
			using (IAnchorStoreObject anchorStoreObject = persisted.FindStoreObject(this.anchorDataProvider))
			{
				anchorStoreObject.OpenAsReadWrite();
				persisted.WriteToMessageItem(anchorStoreObject, true);
				anchorStoreObject.Save(SaveMode.NoConflictResolution);
			}
		}

		private static readonly Band[] DefaultBandSettings = new Band[]
		{
			new Band(Band.BandProfile.SizeBased, ByteQuantifiedSize.FromMB(100UL), ByteQuantifiedSize.FromGB(512UL), 30.0, true, null, null)
		};

		private static readonly PropertyDefinition[] ItemClassPropertyDefinitions = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		};

		private static readonly SortBy[] SortByItemClassAscending = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private readonly IAnchorDataProvider anchorDataProvider;

		private readonly ILogger logger;

		private readonly LoadBalanceAnchorContext loadBalanceAnchorContext;
	}
}
