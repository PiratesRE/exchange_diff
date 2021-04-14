using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncSubscribedPlan : SyncObject
	{
		public SyncSubscribedPlan(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncSubscribedPlan.schema;
			}
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.SubscribedPlan;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new SubscribedPlan();
		}

		internal SyncProperty<string> AccountId
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.AccountId];
			}
			set
			{
				base[SyncSubscribedPlanSchema.AccountId] = value;
			}
		}

		internal SyncProperty<string> Capability
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.Capability];
			}
			set
			{
				base[SyncSubscribedPlanSchema.Capability] = value;
			}
		}

		internal SyncProperty<string> ServiceType
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.ServiceType];
			}
			set
			{
				base[SyncSubscribedPlanSchema.ServiceType] = value;
			}
		}

		internal SyncProperty<string> MaximumOverageUnitsDetail
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.MaximumOverageUnitsDetail];
			}
			set
			{
				base[SyncSubscribedPlanSchema.MaximumOverageUnitsDetail] = value;
			}
		}

		internal SyncProperty<string> PrepaidUnitsDetail
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.PrepaidUnitsDetail];
			}
			set
			{
				base[SyncSubscribedPlanSchema.PrepaidUnitsDetail] = value;
			}
		}

		internal SyncProperty<string> TotalTrialUnitsDetail
		{
			get
			{
				return (SyncProperty<string>)base[SyncSubscribedPlanSchema.TotalTrialUnitsDetail];
			}
			set
			{
				base[SyncSubscribedPlanSchema.TotalTrialUnitsDetail] = value;
			}
		}

		private static readonly SyncSubscribedPlanSchema schema = ObjectSchema.GetInstance<SyncSubscribedPlanSchema>();
	}
}
