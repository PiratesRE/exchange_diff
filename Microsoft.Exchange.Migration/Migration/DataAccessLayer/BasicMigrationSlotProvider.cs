using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal class BasicMigrationSlotProvider
	{
		private BasicMigrationSlotProvider(Unlimited<int> maximumConcurrentMigrations, Unlimited<int> maximumConcurrentIncrementalSyncs, Guid associatedGuid)
		{
			this.MaximumConcurrentMigrations = maximumConcurrentMigrations;
			this.MaximumConcurrentIncrementalSyncs = maximumConcurrentIncrementalSyncs;
			this.SlotProviderGuid = associatedGuid;
			this.cachedItemCounts = new MigrationCountCache();
		}

		public static BasicMigrationSlotProvider Unlimited
		{
			get
			{
				return BasicMigrationSlotProvider.UnlimitedSlotProvider;
			}
		}

		public Unlimited<int> MaximumConcurrentMigrations { get; private set; }

		public Unlimited<int> MaximumConcurrentIncrementalSyncs { get; private set; }

		public Unlimited<int> AvailableInitialSeedingSlots
		{
			get
			{
				return this.MaximumConcurrentMigrations - this.ActiveMigrationCount;
			}
		}

		public Unlimited<int> AvailableIncrementalSyncSlots
		{
			get
			{
				return this.MaximumConcurrentIncrementalSyncs - this.ActiveIncrementalSyncCount;
			}
		}

		public int ActiveMigrationCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedOtherCount("InitialSeeding");
			}
		}

		public int ActiveIncrementalSyncCount
		{
			get
			{
				return this.cachedItemCounts.GetCachedOtherCount("IncrementaSync");
			}
		}

		public Guid SlotProviderGuid { get; private set; }

		public static BasicMigrationSlotProvider Get(Guid ownerId, Unlimited<int> maximumConcurrentMigrations, Unlimited<int> maximumConcurrentIncrementalSyncs)
		{
			BasicMigrationSlotProvider basicMigrationSlotProvider = new BasicMigrationSlotProvider(maximumConcurrentMigrations, maximumConcurrentIncrementalSyncs, ownerId);
			MigrationLogger.Log(MigrationEventType.Verbose, "Getting new slot provider for owner {0}, {1}/{2} initial seeding slots, {3}/{4} incremental sync slots.", new object[]
			{
				ownerId,
				basicMigrationSlotProvider.AvailableInitialSeedingSlots,
				basicMigrationSlotProvider.MaximumConcurrentMigrations,
				basicMigrationSlotProvider.AvailableIncrementalSyncSlots,
				basicMigrationSlotProvider.MaximumConcurrentIncrementalSyncs
			});
			return basicMigrationSlotProvider;
		}

		public static BasicMigrationSlotProvider FromMessageItem(Guid ownerId, IMigrationStoreObject message)
		{
			BasicMigrationSlotProvider basicMigrationSlotProvider = new BasicMigrationSlotProvider(0, 0, ownerId);
			basicMigrationSlotProvider.ReadFromMessageItem(message);
			return basicMigrationSlotProvider;
		}

		public BasicMigrationSlotProvider.SlotAcquisitionGuard AcquireSlot(MigrationJobItem jobItem, MigrationSlotType slotType, IMigrationDataProvider dataProvider)
		{
			if (jobItem.ConsumedSlotType == slotType)
			{
				return new BasicMigrationSlotProvider.SlotAcquisitionGuard();
			}
			this.UpdateAllocationCounts(dataProvider);
			MigrationLogger.Log(MigrationEventType.Verbose, "Attempting to acquire a slot of type {0} from {1}. Job item is currently using a slot of type {2} from {3}.", new object[]
			{
				slotType,
				this.SlotProviderGuid,
				jobItem.ConsumedSlotType,
				jobItem.MigrationSlotProviderGuid
			});
			Unlimited<int> maximumCapacity;
			string slotCountKey;
			switch (slotType)
			{
			case MigrationSlotType.InitialSeeding:
				maximumCapacity = this.MaximumConcurrentMigrations;
				slotCountKey = "InitialSeeding";
				break;
			case MigrationSlotType.IncrementalSync:
				maximumCapacity = this.MaximumConcurrentIncrementalSyncs;
				slotCountKey = "IncrementaSync";
				break;
			default:
				return new BasicMigrationSlotProvider.SlotAcquisitionGuard();
			}
			this.AcquireSlotCapacity(slotCountKey, maximumCapacity, 1);
			return new BasicMigrationSlotProvider.SlotAcquisitionGuard(this, dataProvider, jobItem, new BasicMigrationSlotProvider.SlotInformation(this.SlotProviderGuid, slotType));
		}

		public void ReleaseSlot(MigrationJobItem jobItem, IMigrationDataProvider dataProvider)
		{
			this.UpdateAllocationCounts(dataProvider);
			MigrationSlotType consumedSlotType = jobItem.ConsumedSlotType;
			jobItem.UpdateConsumedSlot(Guid.Empty, MigrationSlotType.None, dataProvider);
			this.ReleaseSlot(consumedSlotType);
		}

		public void ReleaseSlot(MigrationSlotType slotType)
		{
			switch (slotType)
			{
			case MigrationSlotType.None:
				return;
			case MigrationSlotType.InitialSeeding:
				this.ReleaseInitialSeedingSlot();
				break;
			case MigrationSlotType.IncrementalSync:
				this.ReleaseIncrementalSyncSlot();
				break;
			default:
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Released {0} slot consumed by job item.", new object[]
			{
				slotType
			});
		}

		public void WriteToMessageItem(IMigrationStoreObject message)
		{
			message[MigrationBatchMessageSchema.MigrationSlotMaximumInitialSeedings] = this.MaximumConcurrentMigrations.ToString();
			message[MigrationBatchMessageSchema.MigrationSlotMaximumIncrementalSeedings] = this.MaximumConcurrentIncrementalSyncs.ToString();
			this.WriteCachedCountsToMessageItem(message);
		}

		public void WriteCachedCountsToMessageItem(IMigrationStoreObject message)
		{
			message[MigrationBatchMessageSchema.MigrationJobCountCache] = this.cachedItemCounts.Serialize();
		}

		public void ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.MaximumConcurrentMigrations = (MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationSlotMaximumInitialSeedings) ?? Unlimited<int>.UnlimitedValue);
			this.MaximumConcurrentIncrementalSyncs = (MigrationHelper.ReadUnlimitedProperty(message, MigrationBatchMessageSchema.MigrationSlotMaximumIncrementalSeedings) ?? Unlimited<int>.UnlimitedValue);
			string text = (string)message[MigrationBatchMessageSchema.MigrationJobCountCache];
			if (!string.IsNullOrEmpty(text))
			{
				this.cachedItemCounts = MigrationCountCache.Deserialize(text);
			}
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("SlotProvider");
			xelement.Add(new XElement("MaximumConcurrentMigrations", this.MaximumConcurrentMigrations));
			xelement.Add(new XElement("MaximumConcurrentIncrementalSyncs", this.MaximumConcurrentIncrementalSyncs));
			xelement.Add(this.cachedItemCounts.GetDiagnosticInfo(dataProvider, argument));
			return xelement;
		}

		internal void UpdateAllocationCounts(IMigrationDataProvider dataProvider)
		{
			TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("EndpointCountsRefreshThreshold");
			ExDateTime exDateTime = this.cachedItemCounts.GetCachedTimestamp("AllocationsLastUpdatedOn") ?? ExDateTime.MinValue;
			if (exDateTime + config > ExDateTime.UtcNow)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Allocation counts for {0} are only {1} old, not refreshing since our threshold is {2}.", new object[]
				{
					this.SlotProviderGuid,
					ExDateTime.UtcNow - exDateTime,
					config
				});
				return;
			}
			MigrationUtil.RunTimedOperation(delegate()
			{
				int count = MigrationJobItem.GetCount(dataProvider, this.SlotProviderGuid, MigrationSlotType.IncrementalSync);
				int count2 = MigrationJobItem.GetCount(dataProvider, this.SlotProviderGuid, MigrationSlotType.InitialSeeding);
				MigrationLogger.Log(MigrationEventType.Verbose, "Updated allocation counts for {0}, {1} items in initial seeding and {2} items in incremental syncs.", new object[]
				{
					this.SlotProviderGuid,
					count2,
					count
				});
				this.cachedItemCounts.SetCachedOtherCount("InitialSeeding", count2);
				this.cachedItemCounts.SetCachedOtherCount("IncrementaSync", count);
				this.cachedItemCounts.SetCachedTimestamp("AllocationsLastUpdatedOn", new ExDateTime?(ExDateTime.UtcNow));
			}, string.Format("EndpointId={0}", this.SlotProviderGuid));
		}

		private void ReleaseInitialSeedingSlot()
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "Releasing initial seeding slot from provider {0}. Current usage: {1}/{2}", new object[]
			{
				this.SlotProviderGuid,
				this.ActiveMigrationCount,
				this.MaximumConcurrentMigrations
			});
			this.AcquireSlotCapacity("InitialSeeding", this.MaximumConcurrentMigrations, -1);
			if (this.AvailableInitialSeedingSlots > this.MaximumConcurrentMigrations)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Released too many initial seeding slots {0}/{1}.", new object[]
				{
					this.AvailableInitialSeedingSlots,
					this.MaximumConcurrentMigrations
				});
			}
		}

		private void ReleaseIncrementalSyncSlot()
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "Releasing incremental sync slot from provider {0}. Current usage: {1}/{2}", new object[]
			{
				this.SlotProviderGuid,
				this.ActiveIncrementalSyncCount,
				this.MaximumConcurrentIncrementalSyncs
			});
			this.AcquireSlotCapacity("IncrementaSync", this.MaximumConcurrentIncrementalSyncs, -1);
			if (this.AvailableIncrementalSyncSlots > this.MaximumConcurrentIncrementalSyncs)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Released too many incremental sync slots {0}/{1}.", new object[]
				{
					this.AvailableIncrementalSyncSlots,
					this.MaximumConcurrentIncrementalSyncs
				});
			}
		}

		private void AcquireSlotCapacity(string slotCountKey, Unlimited<int> maximumCapacity, int requestedCount)
		{
			int num = this.cachedItemCounts.GetCachedOtherCount(slotCountKey);
			if (requestedCount > 0 && !maximumCapacity.IsUnlimited && num + requestedCount > maximumCapacity)
			{
				throw new MigrationSlotCapacityExceededException(maximumCapacity - num, requestedCount);
			}
			num += requestedCount;
			this.cachedItemCounts.SetCachedOtherCount(slotCountKey, Math.Max(0, num));
		}

		public const string CacheUpdateKey = "AllocationsLastUpdatedOn";

		public const string IncrementalSyncCountKey = "IncrementaSync";

		public const string InitialSeedingCountKey = "InitialSeeding";

		public static readonly PropertyDefinition[] ConcurrencyPropertyDefinition = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationSlotMaximumInitialSeedings,
			MigrationBatchMessageSchema.MigrationSlotMaximumIncrementalSeedings
		};

		public static readonly StorePropertyDefinition[] CachedCountsPropertyDefinition = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobCountCache
		};

		public static readonly PropertyDefinition[] PropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			BasicMigrationSlotProvider.ConcurrencyPropertyDefinition,
			BasicMigrationSlotProvider.CachedCountsPropertyDefinition
		});

		private static readonly Guid UnlimitedSlotProviderId = Guid.Parse("e1489fce-b9f5-43c9-9981-a36cdde44843");

		private static readonly BasicMigrationSlotProvider UnlimitedSlotProvider = new BasicMigrationSlotProvider(Unlimited<int>.UnlimitedValue, Unlimited<int>.UnlimitedValue, BasicMigrationSlotProvider.UnlimitedSlotProviderId);

		private MigrationCountCache cachedItemCounts;

		internal class SlotAcquisitionGuard : DisposeTrackableBase
		{
			internal SlotAcquisitionGuard(BasicMigrationSlotProvider slotProvider, IMigrationDataProvider dataProvider, MigrationJobItem jobItem, BasicMigrationSlotProvider.SlotInformation slotInformation)
			{
				MigrationUtil.ThrowOnNullArgument(slotProvider, "slotProvider");
				MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
				MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
				this.slotProvider = slotProvider;
				this.jobItem = jobItem;
				this.dataProvider = dataProvider;
				this.slotInformation = slotInformation;
				this.success = false;
			}

			public BasicMigrationSlotProvider.SlotInformation SlotInformation
			{
				get
				{
					return this.slotInformation;
				}
			}

			internal SlotAcquisitionGuard()
			{
				this.success = true;
			}

			public void Success()
			{
				if (!this.success)
				{
					this.jobItem.UpdateConsumedSlot(this.slotInformation.SlotProviderGuid, this.slotInformation.SlotType, this.dataProvider);
				}
				this.success = true;
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<BasicMigrationSlotProvider.SlotAcquisitionGuard>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (!disposing || this.success || this.dataProvider == null)
				{
					return;
				}
				MigrationLogger.Log(MigrationEventType.Warning, "Did not received a success signal while protecting the slot acquisition for job item {0} (type={1}), releasing slot.", new object[]
				{
					this.jobItem,
					this.jobItem.ConsumedSlotType
				});
				this.slotProvider.ReleaseSlot(this.jobItem, this.dataProvider);
			}

			private readonly BasicMigrationSlotProvider slotProvider;

			private readonly MigrationJobItem jobItem;

			private readonly IMigrationDataProvider dataProvider;

			private readonly BasicMigrationSlotProvider.SlotInformation slotInformation;

			private bool success;
		}

		internal class SlotInformation
		{
			public SlotInformation(Guid slotProviderGuid, MigrationSlotType slotType)
			{
				this.SlotProviderGuid = slotProviderGuid;
				this.SlotType = slotType;
			}

			public Guid SlotProviderGuid { get; private set; }

			public MigrationSlotType SlotType { get; private set; }
		}
	}
}
