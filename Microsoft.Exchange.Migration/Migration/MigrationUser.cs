using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	[Serializable]
	public class MigrationUser : ConfigurableObject
	{
		static MigrationUser()
		{
			foreach (KeyValuePair<MigrationUserStatusSummary, MigrationUserStatus[]> keyValuePair in MigrationUser.MapFromSummaryToStatus)
			{
				foreach (MigrationUserStatus key in keyValuePair.Value)
				{
					MigrationUser.mapFromStatusToSummary.Add(key, keyValuePair.Key);
				}
			}
		}

		public MigrationUser() : base(new SimplePropertyBag(MigrationUserSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public new MigrationUserId Identity
		{
			get
			{
				return (MigrationUserId)this[MigrationUserSchema.Identity];
			}
			internal set
			{
				this[MigrationUserSchema.Identity] = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.Identity.JobItemGuid;
			}
		}

		public string Identifier
		{
			get
			{
				return this.Identity.Id;
			}
		}

		public MigrationBatchId BatchId
		{
			get
			{
				return (MigrationBatchId)this[MigrationUserSchema.BatchId];
			}
			internal set
			{
				this[MigrationUserSchema.BatchId] = value;
			}
		}

		public SmtpAddress EmailAddress
		{
			get
			{
				return (SmtpAddress)this[MigrationUserSchema.EmailAddress];
			}
			internal set
			{
				this[MigrationUserSchema.EmailAddress] = value;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return (MigrationUserRecipientType)this[MigrationUserSchema.RecipientType];
			}
			internal set
			{
				this[MigrationUserSchema.RecipientType] = value;
			}
		}

		public long SkippedItemCount
		{
			get
			{
				return (long)this[MigrationUserSchema.SkippedItemCount];
			}
			internal set
			{
				this[MigrationUserSchema.SkippedItemCount] = value;
			}
		}

		public long SyncedItemCount
		{
			get
			{
				return (long)this[MigrationUserSchema.SyncedItemCount];
			}
			internal set
			{
				this[MigrationUserSchema.SyncedItemCount] = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[MigrationUserSchema.MailboxGuid];
			}
			internal set
			{
				this[MigrationUserSchema.MailboxGuid] = value;
			}
		}

		public string MailboxLegacyDN
		{
			get
			{
				return (string)this[MigrationUserSchema.MailboxLegacyDN];
			}
			internal set
			{
				this[MigrationUserSchema.MailboxLegacyDN] = value;
			}
		}

		public Guid RequestGuid
		{
			get
			{
				return (Guid)this[MigrationUserSchema.MRSId];
			}
			internal set
			{
				this[MigrationUserSchema.MRSId] = value;
			}
		}

		public DateTime? LastSuccessfulSyncTime
		{
			get
			{
				return (DateTime?)this[MigrationUserSchema.LastSuccessfulSyncTime];
			}
			internal set
			{
				this[MigrationUserSchema.LastSuccessfulSyncTime] = value;
			}
		}

		public MigrationUserStatus Status
		{
			get
			{
				return (MigrationUserStatus)this[MigrationUserSchema.Status];
			}
			internal set
			{
				this[MigrationUserSchema.Status] = value;
				if (MigrationUser.mapFromStatusToSummary.ContainsKey(value))
				{
					this.StatusSummary = MigrationUser.mapFromStatusToSummary[value];
					return;
				}
				this.StatusSummary = (MigrationUserStatusSummary)value;
			}
		}

		public MigrationUserStatusSummary StatusSummary
		{
			get
			{
				return (MigrationUserStatusSummary)this[MigrationUserSchema.StatusSummary];
			}
			private set
			{
				this[MigrationUserSchema.StatusSummary] = value;
			}
		}

		public DateTime? LastSubscriptionCheckTime
		{
			get
			{
				return (DateTime?)this[MigrationUserSchema.SubscriptionLastChecked];
			}
			internal set
			{
				this[MigrationUserSchema.SubscriptionLastChecked] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MigrationUser.schema;
			}
		}

		public override bool Equals(object obj)
		{
			MigrationUser migrationUser = obj as MigrationUser;
			return migrationUser != null && string.Equals(this.Identity.ToString(), migrationUser.Identity.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			if (this.Identity == null)
			{
				return 0;
			}
			return this.Identity.GetHashCode();
		}

		public override string ToString()
		{
			return this.Guid.ToString();
		}

		internal static readonly Dictionary<MigrationUserStatusSummary, MigrationUserStatus[]> MapFromSummaryToStatus = new Dictionary<MigrationUserStatusSummary, MigrationUserStatus[]>
		{
			{
				MigrationUserStatusSummary.Active,
				new MigrationUserStatus[]
				{
					MigrationUserStatus.Validating,
					MigrationUserStatus.Provisioning,
					MigrationUserStatus.ProvisionUpdating,
					MigrationUserStatus.Queued,
					MigrationUserStatus.Syncing,
					MigrationUserStatus.Completing,
					MigrationUserStatus.Starting,
					MigrationUserStatus.Stopping,
					MigrationUserStatus.Removing
				}
			},
			{
				MigrationUserStatusSummary.Synced,
				new MigrationUserStatus[]
				{
					MigrationUserStatus.Synced,
					MigrationUserStatus.IncrementalSyncing,
					MigrationUserStatus.CompletionSynced
				}
			},
			{
				MigrationUserStatusSummary.Completed,
				new MigrationUserStatus[]
				{
					MigrationUserStatus.Completed
				}
			},
			{
				MigrationUserStatusSummary.Failed,
				new MigrationUserStatus[]
				{
					MigrationUserStatus.Failed,
					MigrationUserStatus.IncrementalFailed,
					MigrationUserStatus.CompletionFailed,
					MigrationUserStatus.CompletedWithWarnings,
					MigrationUserStatus.Corrupted
				}
			},
			{
				MigrationUserStatusSummary.Stopped,
				new MigrationUserStatus[]
				{
					MigrationUserStatus.Stopped,
					MigrationUserStatus.IncrementalStopped
				}
			}
		};

		internal static readonly PropertyDefinition IdPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemId;

		internal static readonly PropertyDefinition IdentifierPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemIdentifier;

		internal static readonly PropertyDefinition LocalMailboxIdentifierPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemLocalMailboxIdentifier;

		internal static readonly PropertyDefinition StatusPropertyDefinition = MigrationBatchMessageSchema.MigrationUserStatus;

		internal static readonly PropertyDefinition MailboxGuidPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemMailboxId;

		internal static readonly PropertyDefinition BatchIdPropertyDefinition = MigrationBatchMessageSchema.MigrationJobId;

		internal static readonly PropertyDefinition BatchNamePropertyDefinition = MigrationBatchMessageSchema.MigrationJobName;

		internal static readonly PropertyDefinition RecipientTypePropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemRecipientType;

		internal static readonly PropertyDefinition ItemsSkippedPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemItemsSkipped;

		internal static readonly PropertyDefinition ItemsSyncedPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemItemsSynced;

		internal static readonly PropertyDefinition MailboxLegacyDNPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN;

		internal static readonly PropertyDefinition MRSIdPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemMRSId;

		internal static readonly PropertyDefinition LastSuccessfulSyncTimePropertyDefinition = MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime;

		internal static readonly PropertyDefinition SubscriptionLastCheckedPropertyDefinition = MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked;

		internal static readonly PropertyDefinition VersionPropertyDefinition = MigrationBatchMessageSchema.MigrationVersion;

		internal static readonly PropertyDefinition[] PropertyDefinitions = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MigrationBatchMessageSchema.MigrationJobId,
			MigrationBatchMessageSchema.MigrationJobItemId,
			MigrationBatchMessageSchema.MigrationJobItemIdentifier,
			MigrationBatchMessageSchema.MigrationJobItemLocalMailboxIdentifier,
			MigrationBatchMessageSchema.MigrationJobItemRecipientType,
			MigrationBatchMessageSchema.MigrationJobItemItemsSkipped,
			MigrationBatchMessageSchema.MigrationJobItemItemsSynced,
			MigrationBatchMessageSchema.MigrationJobItemMailboxId,
			MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN,
			MigrationBatchMessageSchema.MigrationJobItemMRSId,
			MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime,
			MigrationBatchMessageSchema.MigrationUserStatus,
			MigrationBatchMessageSchema.MigrationJobItemSubscriptionLastChecked,
			MigrationBatchMessageSchema.MigrationVersion
		};

		private static readonly Dictionary<MigrationUserStatus, MigrationUserStatusSummary> mapFromStatusToSummary = new Dictionary<MigrationUserStatus, MigrationUserStatusSummary>();

		private static MigrationUserSchema schema = ObjectSchema.GetInstance<MigrationUserSchema>();
	}
}
