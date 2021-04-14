using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class LegacyPublicFolderDatabase : LegacyDatabase
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyPublicFolderDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LegacyPublicFolderDatabase.MostDerivedClass;
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
			errors.AddRange(LegacyDatabase.ValidateAscendingQuotas(this.propertyBag, new ProviderPropertyDefinition[]
			{
				LegacyDatabaseSchema.IssueWarningQuota,
				LegacyPublicFolderDatabaseSchema.ProhibitPostQuota
			}, this.Identity));
			if (!this.UseCustomReferralServerList && this.CustomReferralServerList.Count != 0)
			{
				this.CustomReferralServerList.Clear();
			}
			foreach (ServerCostPair serverCostPair in this.CustomReferralServerList)
			{
				if (string.IsNullOrEmpty(serverCostPair.ServerName))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.PublicFolderReferralServerNotExisting(serverCostPair.ServerGuid.ToString()), this.Identity, string.Empty));
				}
			}
			if (this.CustomReferralServerList.Count > 1)
			{
				for (int i = 0; i < this.CustomReferralServerList.Count - 1; i++)
				{
					for (int j = i + 1; j < this.CustomReferralServerList.Count; j++)
					{
						if (this.CustomReferralServerList[i].ServerGuid == this.CustomReferralServerList[j].ServerGuid && this.CustomReferralServerList[i].Cost != this.CustomReferralServerList[j].Cost)
						{
							errors.Add(new ObjectValidationError(DirectoryStrings.ErrorPublicFolderReferralConflict(this.CustomReferralServerList[i].ToString(), this.CustomReferralServerList[j].ToString()), this.Identity, string.Empty));
							break;
						}
					}
				}
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[LegacyPublicFolderDatabaseSchema.Alias];
			}
			internal set
			{
				this[LegacyPublicFolderDatabaseSchema.Alias] = value;
			}
		}

		public bool FirstInstance
		{
			get
			{
				return (bool)this[LegacyPublicFolderDatabaseSchema.FirstInstance];
			}
			internal set
			{
				this[LegacyPublicFolderDatabaseSchema.FirstInstance] = value;
			}
		}

		internal ADObjectId HomeMta
		{
			get
			{
				return (ADObjectId)this[LegacyPublicFolderDatabaseSchema.HomeMta];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.HomeMta] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[LegacyPublicFolderDatabaseSchema.MaxItemSize];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.MaxItemSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> ItemRetentionPeriod
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[LegacyPublicFolderDatabaseSchema.ItemRetentionPeriod];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.ItemRetentionPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint ReplicationPeriod
		{
			get
			{
				return (uint)this[LegacyPublicFolderDatabaseSchema.ReplicationPeriod];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.ReplicationPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[LegacyPublicFolderDatabaseSchema.ProhibitPostQuota];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.ProhibitPostQuota] = value;
			}
		}

		public ADObjectId PublicFolderHierarchy
		{
			get
			{
				return (ADObjectId)this[LegacyPublicFolderDatabaseSchema.PublicFolderHierarchy];
			}
			internal set
			{
				this[LegacyPublicFolderDatabaseSchema.PublicFolderHierarchy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ReplicationMessageSize
		{
			get
			{
				return (ByteQuantifiedSize)this[LegacyPublicFolderDatabaseSchema.ReplicationMessageSize];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.ReplicationMessageSize] = value;
			}
		}

		internal ScheduleMode ReplicationMode
		{
			get
			{
				return (ScheduleMode)this[LegacyPublicFolderDatabaseSchema.ReplicationMode];
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule ReplicationSchedule
		{
			get
			{
				return (Schedule)this[LegacyPublicFolderDatabaseSchema.ReplicationSchedule];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.ReplicationSchedule] = value;
			}
		}

		public bool UseCustomReferralServerList
		{
			get
			{
				return (bool)this[LegacyPublicFolderDatabaseSchema.UseCustomReferralServerList];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.UseCustomReferralServerList] = value;
			}
		}

		public MultiValuedProperty<ServerCostPair> CustomReferralServerList
		{
			get
			{
				return (MultiValuedProperty<ServerCostPair>)this[LegacyPublicFolderDatabaseSchema.CustomReferralServerList];
			}
			set
			{
				this[LegacyPublicFolderDatabaseSchema.CustomReferralServerList] = value;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(LegacyPublicFolderDatabaseSchema.MaxItemSize))
			{
				this.MaxItemSize = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(10240UL));
			}
			if (!base.IsModified(LegacyPublicFolderDatabaseSchema.ProhibitPostQuota))
			{
				this.ProhibitPostQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(2UL));
			}
			base.StampPersistableDefaultValues();
		}

		internal static object ReplicationScheduleGetter(IPropertyBag propertyBag)
		{
			switch ((ScheduleMode)propertyBag[LegacyPublicFolderDatabaseSchema.ReplicationMode])
			{
			case ScheduleMode.Never:
				return Schedule.Never;
			case ScheduleMode.Always:
				return Schedule.Always;
			}
			return propertyBag[LegacyPublicFolderDatabaseSchema.ReplicationScheduleBitmaps];
		}

		internal static void ReplicationScheduleSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				value = Schedule.Never;
			}
			propertyBag[LegacyPublicFolderDatabaseSchema.ReplicationMode] = ((Schedule)value).Mode;
			propertyBag[LegacyPublicFolderDatabaseSchema.ReplicationScheduleBitmaps] = value;
		}

		private static LegacyPublicFolderDatabaseSchema schema = ObjectSchema.GetInstance<LegacyPublicFolderDatabaseSchema>();

		internal static readonly string MostDerivedClass = "msExchPublicMDB";
	}
}
