using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationCacheEntry : MigrationMessagePersistableBase
	{
		private MigrationCacheEntry(Guid mdbGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			this.MdbGuid = mdbGuid;
			this.organizationId = new Lazy<ADObjectId>(() => this.TenantPartitionHint.GetTenantScopedADSessionSettingsServiceOnly().CurrentOrganizationId.OrganizationalUnit, LazyThreadSafetyMode.PublicationOnly);
		}

		private MigrationCacheEntry(string migrationMailboxLegacyDN, Guid mdbGuid, TenantPartitionHint tenantPartitionHint)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(migrationMailboxLegacyDN, "migrationMailboxLegacyDN");
			MigrationUtil.ThrowOnNullArgument(tenantPartitionHint, "organizationId");
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			this.MigrationMailboxLegacyDN = migrationMailboxLegacyDN;
			this.MdbGuid = mdbGuid;
			this.TenantPartitionHint = tenantPartitionHint;
			this.LastUpdated = ExDateTime.UtcNow;
			this.NextProcessTime = null;
			this.organizationId = new Lazy<ADObjectId>(() => this.TenantPartitionHint.GetTenantScopedADSessionSettingsServiceOnly().CurrentOrganizationId.OrganizationalUnit, LazyThreadSafetyMode.PublicationOnly);
		}

		public string MigrationMailboxLegacyDN
		{
			get
			{
				return this.migrationMailboxLegacyDN;
			}
			private set
			{
				this.migrationMailboxLegacyDN = value;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
			private set
			{
				this.mdbGuid = value;
			}
		}

		public TenantPartitionHint TenantPartitionHint { get; private set; }

		public ExDateTime? NextProcessTime
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime?>("NextProcessTime", null);
			}
			private set
			{
				base.ExtendedProperties.Set<ExDateTime?>("NextProcessTime", value);
			}
		}

		public ExDateTime LastUpdated
		{
			get
			{
				return this.lastUpdated;
			}
			private set
			{
				this.lastUpdated = value;
			}
		}

		public override long MaximumSupportedVersion
		{
			get
			{
				return 1L;
			}
		}

		public override long MinimumSupportedVersion
		{
			get
			{
				return 1L;
			}
		}

		public override long CurrentSupportedVersion
		{
			get
			{
				return 1L;
			}
		}

		public ExDateTime LastChecked
		{
			get
			{
				return base.ExtendedProperties.Get<ExDateTime>("LastChecked", ExDateTime.MinValue);
			}
			private set
			{
				base.ExtendedProperties.Set<ExDateTime>("LastChecked", value);
			}
		}

		public MigrationProcessorResult LastProcessorResult
		{
			get
			{
				return base.ExtendedProperties.Get<MigrationProcessorResult>("LastProcessorResult", MigrationProcessorResult.Deleted);
			}
			private set
			{
				base.ExtendedProperties.Set<MigrationProcessorResult>("LastProcessorResult", value);
			}
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MigrationCacheEntry.MessagePropertyDefinition;
			}
		}

		public new ADObjectId OrganizationId
		{
			get
			{
				return this.organizationId.Value;
			}
		}

		public static MigrationCacheEntry Create(IMigrationDataProvider provider, string mailboxLegacyDN, Guid mdbGuid, TenantPartitionHint tenantPartitionHint)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(mailboxLegacyDN, "mailboxLegacyDN");
			MigrationUtil.ThrowOnNullArgument(tenantPartitionHint, "tenantPartitionHint");
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			MigrationCacheEntry.DeleteByLegacyDN(provider, mailboxLegacyDN);
			MigrationCacheEntry migrationCacheEntry = new MigrationCacheEntry(mailboxLegacyDN, mdbGuid, tenantPartitionHint);
			migrationCacheEntry.CreateInStore(provider, null);
			return migrationCacheEntry;
		}

		public static IEnumerable<MigrationCacheEntry> GetMigrationCacheEntries(IMigrationDataProvider provider, Guid mdbGuid)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnGuidEmptyArgument(mdbGuid, "mdbGuid");
			MigrationEqualityFilter primaryFilter = MigrationCacheEntry.MessageClassEqualityFilter;
			IEnumerable<StoreObjectId> messageIds = MigrationHelper.FindMessageIds(provider, primaryFilter, null, null, null);
			foreach (StoreObjectId messageId in messageIds)
			{
				MigrationCacheEntry entry = new MigrationCacheEntry(mdbGuid);
				if (!entry.TryLoad(provider, messageId))
				{
					MigrationLogger.Log(MigrationEventType.Warning, "skipping over cache entry with message id {0}", new object[]
					{
						messageId
					});
				}
				else
				{
					yield return entry;
				}
			}
			yield break;
		}

		public void Suspend()
		{
			this.UpdateFromLastRun(MigrationProcessorResult.Suspended, new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("CacheEntrySuspendedDuration")), true);
		}

		public void UpdateFromLastRun(MigrationProcessorResult result, TimeSpan? delayInterval, bool persist)
		{
			this.LastProcessorResult = result;
			this.LastChecked = ExDateTime.UtcNow;
			if (delayInterval != null)
			{
				MigrationLogger.Log(MigrationEventType.Information, "CacheEntry setting delay interval to {0} for {1} due to processing result {2}", new object[]
				{
					delayInterval.Value,
					this,
					result
				});
				this.NextProcessTime = new ExDateTime?(this.LastChecked + delayInterval.Value);
			}
			else
			{
				this.NextProcessTime = null;
			}
			if (!persist)
			{
				return;
			}
			using (IMigrationDataProvider migrationDataProvider = MigrationServiceFactory.Instance.CreateProviderForSystemMailbox(this.MdbGuid))
			{
				using (IMigrationMessageItem migrationMessageItem = base.FindMessageItem(migrationDataProvider, MigrationPersistableBase.MigrationBaseDefinitions))
				{
					migrationMessageItem.OpenAsReadWrite();
					this.WriteExtendedPropertiesToMessageItem(migrationMessageItem);
					migrationMessageItem.Save(SaveMode.NoConflictResolution);
				}
			}
		}

		public void Delete(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			if (base.StoreObjectId != null)
			{
				provider.RemoveMessage(base.StoreObjectId);
			}
		}

		public string GetDiagnosticComponentName()
		{
			return "MigrationCacheEntry";
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			if (this.TenantPartitionHint != null)
			{
				xelement.Add(new XElement("tenantPartitionHint", this.TenantPartitionHint));
			}
			xelement.Add(new object[]
			{
				new XElement("lastUpdated", this.LastUpdated),
				new XElement("mailboxLegacyDN", this.MigrationMailboxLegacyDN),
				new XElement("messageId", base.StoreObjectId),
				new XElement("databaseGuid", this.MdbGuid)
			});
			return base.GetDiagnosticInfo(dataProvider, argument, xelement);
		}

		public override string ToString()
		{
			return this.MigrationMailboxLegacyDN;
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			if (!base.ReadFromMessageItem(message))
			{
				return false;
			}
			this.MigrationMailboxLegacyDN = (string)message[MigrationBatchMessageSchema.MigrationCacheEntryMailboxLegacyDN];
			byte[] valueOrDefault = message.GetValueOrDefault<byte[]>(MigrationBatchMessageSchema.MigrationCacheEntryTenantPartitionHint, null);
			if (valueOrDefault == null || valueOrDefault.Length <= 0)
			{
				MigrationLogger.Log(MigrationEventType.Error, "Skipping cache entry {0} with leg dn {1} because no partition hint", new object[]
				{
					message.Id,
					this.MigrationMailboxLegacyDN
				});
				return false;
			}
			this.TenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(valueOrDefault);
			this.LastUpdated = MigrationHelper.GetExDateTimePropertyOrDefault(message, MigrationBatchMessageSchema.MigrationCacheEntryLastUpdated, message.CreationTime);
			return true;
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[StoreObjectSchema.ItemClass] = MigrationBatchMessageSchema.MigrationCacheEntryClass;
			message[MigrationBatchMessageSchema.MigrationCacheEntryMailboxLegacyDN] = this.MigrationMailboxLegacyDN;
			message[MigrationBatchMessageSchema.MigrationCacheEntryTenantPartitionHint] = this.TenantPartitionHint.GetPersistablePartitionHint();
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationCacheEntryLastUpdated, new ExDateTime?(this.LastUpdated));
			base.WriteToMessageItem(message, loaded);
		}

		private static void DeleteByLegacyDN(IMigrationDataProvider provider, string mailboxLegacyDN)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(mailboxLegacyDN, "mailboxLegacyDN");
			MigrationEqualityFilter primaryFilter = new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationCacheEntryMailboxLegacyDN, mailboxLegacyDN);
			IEnumerable<StoreObjectId> collection = MigrationHelper.FindMessageIds(provider, primaryFilter, new MigrationEqualityFilter[]
			{
				MigrationCacheEntry.MessageClassEqualityFilter
			}, null, null);
			List<StoreObjectId> list = new List<StoreObjectId>(collection);
			foreach (StoreObjectId messageId in list)
			{
				provider.RemoveMessage(messageId);
			}
		}

		private const string LastCheckedKey = "LastChecked";

		private const string NextProcessTimeKey = "NextProcessTime";

		private const string LastProcessorResultKey = "LastProcessorResult";

		private const long MigrationCacheEntryCurrentSupportedVersion = 1L;

		private static readonly PropertyDefinition[] MessagePropertyDefinition = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationCacheEntryMailboxLegacyDN,
				MigrationBatchMessageSchema.MigrationCacheEntryTenantPartitionHint,
				MigrationBatchMessageSchema.MigrationCacheEntryLastUpdated
			},
			MigrationPersistableBase.MigrationBaseDefinitions
		});

		private static readonly MigrationEqualityFilter MessageClassEqualityFilter = new MigrationEqualityFilter(StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationCacheEntryClass);

		private readonly Lazy<ADObjectId> organizationId;

		private string migrationMailboxLegacyDN;

		private Guid mdbGuid;

		private ExDateTime lastUpdated;
	}
}
