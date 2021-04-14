using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Database)]
	[Serializable]
	public sealed class MailboxDatabase : Database, IProvisioningCacheInvalidation
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MailboxDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MailboxDatabase.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				DatabaseSchema.IssueWarningQuota,
				MailboxDatabaseSchema.ProhibitSendQuota,
				MailboxDatabaseSchema.ProhibitSendReceiveQuota
			}, this.Identity));
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				MailboxDatabaseSchema.RecoverableItemsWarningQuota,
				MailboxDatabaseSchema.RecoverableItemsQuota
			}, this.Identity));
			errors.AddRange(Database.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				MailboxDatabaseSchema.CalendarLoggingQuota,
				MailboxDatabaseSchema.RecoverableItemsQuota
			}, this.Identity));
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(MailboxDatabaseSchema.ProhibitSendReceiveQuota))
			{
				this.ProhibitSendReceiveQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromMB(2355UL));
			}
			if (!base.IsModified(MailboxDatabaseSchema.ProhibitSendQuota))
			{
				this.ProhibitSendQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(2UL));
			}
			if (!base.IsModified(MailboxDatabaseSchema.RecoverableItemsWarningQuota))
			{
				this.RecoverableItemsWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(20UL));
			}
			if (!base.IsModified(MailboxDatabaseSchema.RecoverableItemsQuota))
			{
				this.RecoverableItemsQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(30UL));
			}
			if (!base.IsModified(MailboxDatabaseSchema.CalendarLoggingQuota))
			{
				this.CalendarLoggingQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(6UL));
			}
			this.ReservedFlag = true;
			base.StampPersistableDefaultValues();
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.ObjectState == ObjectState.New || base.ObjectState == ObjectState.Deleted)
			{
				keys = new Guid[]
				{
					CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnLocalSite,
					CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites,
					CannedProvisioningCacheKeys.MailboxDatabaseForDefaultRetentionValuesCacheKey
				};
			}
			else if (base.ObjectState == ObjectState.Changed)
			{
				List<Guid> list = new List<Guid>();
				if (base.IsChanged(MailboxDatabaseSchema.IsExcludedFromProvisioning) || base.IsChanged(MailboxDatabaseSchema.IsSuspendedFromProvisioning) || base.IsChanged(DatabaseSchema.Recovery))
				{
					list.Add(CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnLocalSite);
					list.Add(CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingGlobalData, CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites.ToString(), new object[]
					{
						CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites.ToString()
					});
				}
				else if (base.IsChanged(MailboxDatabaseSchema.IsExcludedFromInitialProvisioning))
				{
					list.Add(CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites);
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCResettingGlobalData, CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites.ToString(), new object[]
					{
						CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnAllSites.ToString()
					});
				}
				if (base.IsChanged(DatabaseSchema.DeletedItemRetention) || base.IsChanged(DatabaseSchema.RetainDeletedItemsUntilBackup))
				{
					list.Add(CannedProvisioningCacheKeys.MailboxDatabaseForDefaultRetentionValuesCacheKey);
				}
				if (list.Count > 0)
				{
					keys = list.ToArray();
				}
			}
			return keys != null;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		public ADObjectId JournalRecipient
		{
			get
			{
				return (ADObjectId)this[MailboxDatabaseSchema.JournalRecipient];
			}
			set
			{
				this[MailboxDatabaseSchema.JournalRecipient] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxRetention
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxDatabaseSchema.MailboxRetention];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxRetention] = value;
			}
		}

		public ADObjectId OfflineAddressBook
		{
			get
			{
				return (ADObjectId)this[MailboxDatabaseSchema.OfflineAddressBook];
			}
			set
			{
				this[MailboxDatabaseSchema.OfflineAddressBook] = value;
			}
		}

		public ADObjectId OriginalDatabase
		{
			get
			{
				return (ADObjectId)this[MailboxDatabaseSchema.OriginalDatabase];
			}
			internal set
			{
				this[MailboxDatabaseSchema.OriginalDatabase] = value;
			}
		}

		public ADObjectId PublicFolderDatabase
		{
			get
			{
				return (ADObjectId)this[MailboxDatabaseSchema.PublicFolderDatabase];
			}
			set
			{
				this[MailboxDatabaseSchema.PublicFolderDatabase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxDatabaseSchema.ProhibitSendReceiveQuota];
			}
			set
			{
				this[MailboxDatabaseSchema.ProhibitSendReceiveQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitSendQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxDatabaseSchema.ProhibitSendQuota];
			}
			set
			{
				this[MailboxDatabaseSchema.ProhibitSendQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxDatabaseSchema.RecoverableItemsQuota];
			}
			set
			{
				this[MailboxDatabaseSchema.RecoverableItemsQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxDatabaseSchema.RecoverableItemsWarningQuota];
			}
			set
			{
				this[MailboxDatabaseSchema.RecoverableItemsWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> CalendarLoggingQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxDatabaseSchema.CalendarLoggingQuota];
			}
			set
			{
				this[MailboxDatabaseSchema.CalendarLoggingQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IndexEnabled
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IndexEnabled];
			}
			set
			{
				this[MailboxDatabaseSchema.IndexEnabled] = value;
			}
		}

		internal bool ReservedFlag
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.ReservedFlag];
			}
			set
			{
				this[MailboxDatabaseSchema.ReservedFlag] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromProvisioning
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IsExcludedFromProvisioning];
			}
			set
			{
				this[MailboxDatabaseSchema.IsExcludedFromProvisioning] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromProvisioningBySchemaVersionMonitoring
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IsExcludedFromProvisioningBySchemaVersionMonitoring];
			}
			set
			{
				this[MailboxDatabaseSchema.IsExcludedFromProvisioningBySchemaVersionMonitoring] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromInitialProvisioning
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IsExcludedFromInitialProvisioning];
			}
			set
			{
				this[MailboxDatabaseSchema.IsExcludedFromInitialProvisioning] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsSuspendedFromProvisioning
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IsSuspendedFromProvisioning];
			}
			set
			{
				this[MailboxDatabaseSchema.IsSuspendedFromProvisioning] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromProvisioningBySpaceMonitoring
		{
			get
			{
				return (bool)this[MailboxDatabaseSchema.IsExcludedFromProvisioningBySpaceMonitoring];
			}
			set
			{
				this[MailboxDatabaseSchema.IsExcludedFromProvisioningBySpaceMonitoring] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
		{
			get
			{
				return (ByteQuantifiedSize?)this[MailboxDatabaseSchema.MailboxLoadBalanceMaximumEdbFileSize];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxLoadBalanceMaximumEdbFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceRelativeLoadCapacity
		{
			get
			{
				return (int?)this[MailboxDatabaseSchema.MailboxLoadBalanceRelativeLoadCapacity];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxLoadBalanceRelativeLoadCapacity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceOverloadedThreshold
		{
			get
			{
				return (int?)this[MailboxDatabaseSchema.MailboxLoadBalanceOverloadedThreshold];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxLoadBalanceOverloadedThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceUnderloadedThreshold
		{
			get
			{
				return (int?)this[MailboxDatabaseSchema.MailboxLoadBalanceUnderloadedThreshold];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxLoadBalanceUnderloadedThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? MailboxLoadBalanceEnabled
		{
			get
			{
				return (bool?)this[MailboxDatabaseSchema.MailboxLoadBalanceEnabled];
			}
			set
			{
				this[MailboxDatabaseSchema.MailboxLoadBalanceEnabled] = value;
			}
		}

		internal static QueryFilter IsExcludedFromProvisioningFilterBuilder(SinglePropertyFilter filter)
		{
			return SharedPropertyDefinitions.ProvisioningFlagsFilterBuilder(DatabaseProvisioningFlags.IsExcludedFromProvisioning, filter);
		}

		internal static QueryFilter IsSuspendedFromProvisioningFilterBuilder(SinglePropertyFilter filter)
		{
			return SharedPropertyDefinitions.ProvisioningFlagsFilterBuilder(DatabaseProvisioningFlags.IsSuspendedFromProvisioning, filter);
		}

		public DumpsterStatisticsEntry[] DumpsterStatistics
		{
			get
			{
				return this.m_DumpsterStatistics;
			}
		}

		public string[] DumpsterServersNotAvailable
		{
			get
			{
				return this.m_DumpsterServersNotAvailable;
			}
		}

		private static MailboxDatabaseSchema schema = ObjectSchema.GetInstance<MailboxDatabaseSchema>();

		internal static readonly string MostDerivedClass = "msExchPrivateMDB";

		internal DumpsterStatisticsEntry[] m_DumpsterStatistics;

		internal string[] m_DumpsterServersNotAvailable;
	}
}
