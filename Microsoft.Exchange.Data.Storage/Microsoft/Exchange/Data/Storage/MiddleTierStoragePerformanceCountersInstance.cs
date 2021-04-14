using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MiddleTierStoragePerformanceCountersInstance : PerformanceCounterInstance
	{
		internal MiddleTierStoragePerformanceCountersInstance(string instanceName, MiddleTierStoragePerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Middle-Tier Storage")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NamedPropertyCacheEntries = new ExPerformanceCounter(base.CategoryName, "Named Property cache entries.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheEntries);
				this.NamedPropertyCacheMisses = new ExPerformanceCounter(base.CategoryName, "Named Property cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheMisses);
				this.NamedPropertyCacheMisses_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for Named Property cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheMisses_Base);
				this.DumpsterSessionsActive = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterSessionsActive);
				this.DumpsterDelegateSessionsActive = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Delegate Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterDelegateSessionsActive);
				this.DumpsterADSettingCacheSize = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings Cache Entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheSize);
				this.DumpsterADSettingRefreshRate = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings Refresh/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingRefreshRate);
				this.DumpsterMoveItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Moved to Dumpster/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterMoveItemsRate);
				this.DumpsterCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Copied to Dumpster/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCopyItemsRate);
				this.DumpsterCalendarLogsRate = new ExPerformanceCounter(base.CategoryName, "Dumpster Calendar Log Entries/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCalendarLogsRate);
				this.DumpsterDeletionsItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Deletions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterDeletionsItemsRate);
				this.DumpsterPurgesItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Purges/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterPurgesItemsRate);
				this.DumpsterVersionsItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Versions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterVersionsItemsRate);
				this.DumpsterFolderEnumRate = new ExPerformanceCounter(base.CategoryName, "Folder Enumerations for Dumpster/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterFolderEnumRate);
				this.DumpsterForceCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Forced Copied into Dumpster/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterForceCopyItemsRate);
				this.DumpsterMoveNoKeepItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Moved in Dumpster not Kept/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterMoveNoKeepItemsRate);
				this.DumpsterCopyNoKeepItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Copy in Dumpster not Kept/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCopyNoKeepItemsRate);
				this.AuditFolderBindRate = new ExPerformanceCounter(base.CategoryName, "Audit records for folder bind/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditFolderBindRate);
				this.AuditGroupChangeRate = new ExPerformanceCounter(base.CategoryName, "Audit records for group change/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditGroupChangeRate);
				this.AuditItemChangeRate = new ExPerformanceCounter(base.CategoryName, "Audit records for item change/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditItemChangeRate);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Audits saved/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Average time for saving audits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalAuditSaveTime = new ExPerformanceCounter(base.CategoryName, "Total time for saving audits", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalAuditSaveTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Base of average time for saving audits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalAuditSave = new ExPerformanceCounter(base.CategoryName, "Total audits saved", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter3
				});
				list.Add(this.TotalAuditSave);
				this.DiscoveryCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items copied to discovery mailbox/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryCopyItemsRate);
				this.DiscoveryMailboxSearchesQueued = new ExPerformanceCounter(base.CategoryName, "Number of mailbox searches that are queued", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchesQueued);
				this.DiscoveryMailboxSearchesActive = new ExPerformanceCounter(base.CategoryName, "Number of mailbox searches that are active", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchesActive);
				this.DiscoveryMailboxSearchSourceMailboxesActive = new ExPerformanceCounter(base.CategoryName, "Number of mailboxes being searched", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchSourceMailboxesActive);
				this.DumpsterVersionRollback = new ExPerformanceCounter(base.CategoryName, "Dumpster versions reverted on failure.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterVersionRollback);
				this.DumpsterADSettingCacheMisses = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheMisses);
				this.DumpsterADSettingCacheMisses_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for Dumpster Active Directory Settings cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheMisses_Base);
				this.ActivityLogsActivityCount = new ExPerformanceCounter(base.CategoryName, "Activity Logger Activity Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsActivityCount);
				this.ActivityLogsSelectedForStore = new ExPerformanceCounter(base.CategoryName, "Activity Logger Selected Activities For Store", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsSelectedForStore);
				this.ActivityLogsStoreWriteExceptions = new ExPerformanceCounter(base.CategoryName, "Activity Logger Exception Count on Store Submit", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsStoreWriteExceptions);
				this.ActivityLogsFileWriteExceptions = new ExPerformanceCounter(base.CategoryName, "Activity Logger Exception Count on File Log", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsFileWriteExceptions);
				this.ActivityLogsFileWriteCount = new ExPerformanceCounter(base.CategoryName, "Activity Logger Activity Count written to File Log", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsFileWriteCount);
				long num = this.NamedPropertyCacheEntries.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MiddleTierStoragePerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Middle-Tier Storage")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NamedPropertyCacheEntries = new ExPerformanceCounter(base.CategoryName, "Named Property cache entries.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheEntries);
				this.NamedPropertyCacheMisses = new ExPerformanceCounter(base.CategoryName, "Named Property cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheMisses);
				this.NamedPropertyCacheMisses_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for Named Property cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NamedPropertyCacheMisses_Base);
				this.DumpsterSessionsActive = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterSessionsActive);
				this.DumpsterDelegateSessionsActive = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Delegate Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterDelegateSessionsActive);
				this.DumpsterADSettingCacheSize = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings Cache Entries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheSize);
				this.DumpsterADSettingRefreshRate = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings Refresh/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingRefreshRate);
				this.DumpsterMoveItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Moved to Dumpster/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterMoveItemsRate);
				this.DumpsterCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Copied to Dumpster/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCopyItemsRate);
				this.DumpsterCalendarLogsRate = new ExPerformanceCounter(base.CategoryName, "Dumpster Calendar Log Entries/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCalendarLogsRate);
				this.DumpsterDeletionsItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Deletions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterDeletionsItemsRate);
				this.DumpsterPurgesItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Purges/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterPurgesItemsRate);
				this.DumpsterVersionsItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Dumpster Versions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterVersionsItemsRate);
				this.DumpsterFolderEnumRate = new ExPerformanceCounter(base.CategoryName, "Folder Enumerations for Dumpster/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterFolderEnumRate);
				this.DumpsterForceCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Forced Copied into Dumpster/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterForceCopyItemsRate);
				this.DumpsterMoveNoKeepItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Moved in Dumpster not Kept/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterMoveNoKeepItemsRate);
				this.DumpsterCopyNoKeepItemsRate = new ExPerformanceCounter(base.CategoryName, "Items Copy in Dumpster not Kept/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterCopyNoKeepItemsRate);
				this.AuditFolderBindRate = new ExPerformanceCounter(base.CategoryName, "Audit records for folder bind/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditFolderBindRate);
				this.AuditGroupChangeRate = new ExPerformanceCounter(base.CategoryName, "Audit records for group change/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditGroupChangeRate);
				this.AuditItemChangeRate = new ExPerformanceCounter(base.CategoryName, "Audit records for item change/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AuditItemChangeRate);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Audits saved/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Average time for saving audits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalAuditSaveTime = new ExPerformanceCounter(base.CategoryName, "Total time for saving audits", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalAuditSaveTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Base of average time for saving audits", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalAuditSave = new ExPerformanceCounter(base.CategoryName, "Total audits saved", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter3
				});
				list.Add(this.TotalAuditSave);
				this.DiscoveryCopyItemsRate = new ExPerformanceCounter(base.CategoryName, "Items copied to discovery mailbox/sec.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryCopyItemsRate);
				this.DiscoveryMailboxSearchesQueued = new ExPerformanceCounter(base.CategoryName, "Number of mailbox searches that are queued", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchesQueued);
				this.DiscoveryMailboxSearchesActive = new ExPerformanceCounter(base.CategoryName, "Number of mailbox searches that are active", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchesActive);
				this.DiscoveryMailboxSearchSourceMailboxesActive = new ExPerformanceCounter(base.CategoryName, "Number of mailboxes being searched", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DiscoveryMailboxSearchSourceMailboxesActive);
				this.DumpsterVersionRollback = new ExPerformanceCounter(base.CategoryName, "Dumpster versions reverted on failure.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterVersionRollback);
				this.DumpsterADSettingCacheMisses = new ExPerformanceCounter(base.CategoryName, "Dumpster Active Directory Settings cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheMisses);
				this.DumpsterADSettingCacheMisses_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for Dumpster Active Directory Settings cache misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DumpsterADSettingCacheMisses_Base);
				this.ActivityLogsActivityCount = new ExPerformanceCounter(base.CategoryName, "Activity Logger Activity Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsActivityCount);
				this.ActivityLogsSelectedForStore = new ExPerformanceCounter(base.CategoryName, "Activity Logger Selected Activities For Store", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsSelectedForStore);
				this.ActivityLogsStoreWriteExceptions = new ExPerformanceCounter(base.CategoryName, "Activity Logger Exception Count on Store Submit", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsStoreWriteExceptions);
				this.ActivityLogsFileWriteExceptions = new ExPerformanceCounter(base.CategoryName, "Activity Logger Exception Count on File Log", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsFileWriteExceptions);
				this.ActivityLogsFileWriteCount = new ExPerformanceCounter(base.CategoryName, "Activity Logger Activity Count written to File Log", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActivityLogsFileWriteCount);
				long num = this.NamedPropertyCacheEntries.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter NamedPropertyCacheEntries;

		public readonly ExPerformanceCounter NamedPropertyCacheMisses;

		public readonly ExPerformanceCounter NamedPropertyCacheMisses_Base;

		public readonly ExPerformanceCounter DumpsterSessionsActive;

		public readonly ExPerformanceCounter DumpsterDelegateSessionsActive;

		public readonly ExPerformanceCounter DumpsterADSettingCacheSize;

		public readonly ExPerformanceCounter DumpsterADSettingRefreshRate;

		public readonly ExPerformanceCounter DumpsterMoveItemsRate;

		public readonly ExPerformanceCounter DumpsterCopyItemsRate;

		public readonly ExPerformanceCounter DumpsterCalendarLogsRate;

		public readonly ExPerformanceCounter DumpsterDeletionsItemsRate;

		public readonly ExPerformanceCounter DumpsterPurgesItemsRate;

		public readonly ExPerformanceCounter DumpsterVersionsItemsRate;

		public readonly ExPerformanceCounter DumpsterFolderEnumRate;

		public readonly ExPerformanceCounter DumpsterForceCopyItemsRate;

		public readonly ExPerformanceCounter DumpsterMoveNoKeepItemsRate;

		public readonly ExPerformanceCounter DumpsterCopyNoKeepItemsRate;

		public readonly ExPerformanceCounter AuditFolderBindRate;

		public readonly ExPerformanceCounter AuditGroupChangeRate;

		public readonly ExPerformanceCounter AuditItemChangeRate;

		public readonly ExPerformanceCounter TotalAuditSave;

		public readonly ExPerformanceCounter TotalAuditSaveTime;

		public readonly ExPerformanceCounter DiscoveryCopyItemsRate;

		public readonly ExPerformanceCounter DiscoveryMailboxSearchesQueued;

		public readonly ExPerformanceCounter DiscoveryMailboxSearchesActive;

		public readonly ExPerformanceCounter DiscoveryMailboxSearchSourceMailboxesActive;

		public readonly ExPerformanceCounter DumpsterVersionRollback;

		public readonly ExPerformanceCounter DumpsterADSettingCacheMisses;

		public readonly ExPerformanceCounter DumpsterADSettingCacheMisses_Base;

		public readonly ExPerformanceCounter ActivityLogsActivityCount;

		public readonly ExPerformanceCounter ActivityLogsSelectedForStore;

		public readonly ExPerformanceCounter ActivityLogsStoreWriteExceptions;

		public readonly ExPerformanceCounter ActivityLogsFileWriteExceptions;

		public readonly ExPerformanceCounter ActivityLogsFileWriteCount;
	}
}
