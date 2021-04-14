using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class UpgradeWorkItem : ConfigurableObject
	{
		public UpgradeWorkItem() : base(new SimplePropertyBag(UpgradeWorkItem.UpgradeWorkItemSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			base.ResetChangeTracking();
		}

		public UpgradeWorkItem(WorkItemInfo workItemInfo) : this()
		{
			this.Identity = workItemInfo.WorkItemId.ToString();
			this.Created = new DateTime?(workItemInfo.Created);
			this.Modified = new DateTime?(workItemInfo.Modified);
			this.Scheduled = new DateTime?(workItemInfo.ScheduledDate);
			this.TenantID = new Guid?(workItemInfo.Tenant.TenantId);
			this.TenantPrimaryDomain = workItemInfo.Tenant.PrimaryDomain;
			this.TenantTier = workItemInfo.Tenant.Tier;
			this.StatusComment = workItemInfo.WorkItemStatus.Comment;
			this.StatusCompletedCount = new int?(workItemInfo.WorkItemStatus.CompletedCount);
			this.StatusDetails = workItemInfo.WorkItemStatus.StatusDetails;
			this.StatusHandlerState = workItemInfo.WorkItemStatus.HandlerState;
			this.StatusTotalCount = new int?(workItemInfo.WorkItemStatus.TotalCount);
			this.Status = workItemInfo.WorkItemStatus.Status.ToString();
			this.Type = workItemInfo.WorkItemType;
			this.TenantInitialDomain = workItemInfo.Tenant.InitialDomain;
			this.TenantScheduledUpgradeDate = workItemInfo.Tenant.ScheduledUpgradeDate;
			if (workItemInfo.PilotUser != null)
			{
				this.PilotUpn = workItemInfo.PilotUser.Upn;
				this.PilotUserID = new Guid?(workItemInfo.PilotUser.PilotUserId);
				return;
			}
			this.PilotUpn = null;
			this.PilotUserID = null;
		}

		public new string Identity
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.Identity];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Identity] = value;
			}
		}

		public DateTime? Created
		{
			get
			{
				return (DateTime?)this[UpgradeWorkItem.UpgradeWorkItemSchema.Created];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Created] = value;
			}
		}

		public DateTime? Modified
		{
			get
			{
				return (DateTime?)this[UpgradeWorkItem.UpgradeWorkItemSchema.Modified];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Modified] = value;
			}
		}

		public DateTime? Scheduled
		{
			get
			{
				return (DateTime?)this[UpgradeWorkItem.UpgradeWorkItemSchema.Scheduled];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Scheduled] = value;
			}
		}

		public Guid? TenantID
		{
			get
			{
				return (Guid?)this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantID];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantID] = value;
			}
		}

		public string TenantPrimaryDomain
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantPrimaryDomain];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantPrimaryDomain] = value;
			}
		}

		public string TenantInitialDomain
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantInitialDomain];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantInitialDomain] = value;
			}
		}

		public DateTime? TenantScheduledUpgradeDate
		{
			get
			{
				return (DateTime?)this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantScheduledUpgradeDate];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantScheduledUpgradeDate] = value;
			}
		}

		public string TenantTier
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantTier];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.TenantTier] = value;
			}
		}

		public string StatusComment
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusComment];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusComment] = value;
			}
		}

		public int? StatusCompletedCount
		{
			get
			{
				return (int?)this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusCompletedCount];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusCompletedCount] = value;
			}
		}

		public Uri StatusDetails
		{
			get
			{
				return (Uri)this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusDetails];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusDetails] = value;
			}
		}

		public string StatusHandlerState
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusHandlerState];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusHandlerState] = value;
			}
		}

		public int? StatusTotalCount
		{
			get
			{
				return (int?)this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusTotalCount];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.StatusTotalCount] = value;
			}
		}

		public string Status
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.Status];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Status] = value;
			}
		}

		public string Type
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.Type];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.Type] = value;
			}
		}

		public Guid? PilotUserID
		{
			get
			{
				return (Guid?)this[UpgradeWorkItem.UpgradeWorkItemSchema.PilotUserID];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.PilotUserID] = value;
			}
		}

		public string PilotUpn
		{
			get
			{
				return (string)this[UpgradeWorkItem.UpgradeWorkItemSchema.PilotUpnField];
			}
			internal set
			{
				this[UpgradeWorkItem.UpgradeWorkItemSchema.PilotUpnField] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UpgradeWorkItem.schema;
			}
		}

		public override bool Equals(object obj)
		{
			UpgradeWorkItem upgradeWorkItem = obj as UpgradeWorkItem;
			return upgradeWorkItem != null && string.Equals(this.Identity.ToString(), upgradeWorkItem.Identity.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static UpgradeWorkItem.UpgradeWorkItemSchema schema = ObjectSchema.GetInstance<UpgradeWorkItem.UpgradeWorkItemSchema>();

		internal class UpgradeWorkItemSchema : SimpleProviderObjectSchema
		{
			public new static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Created = new SimpleProviderPropertyDefinition("Created", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Modified = new SimpleProviderPropertyDefinition("Modified", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Scheduled = new SimpleProviderPropertyDefinition("Scheduled", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition TenantID = new SimpleProviderPropertyDefinition("TenantID", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition TenantPrimaryDomain = new SimpleProviderPropertyDefinition("TenantPrimaryDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition TenantTier = new SimpleProviderPropertyDefinition("TenantTier", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StatusComment = new SimpleProviderPropertyDefinition("StatusComment", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StatusCompletedCount = new SimpleProviderPropertyDefinition("StatusCompletedCount", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StatusHandlerState = new SimpleProviderPropertyDefinition("StatusHandlerState", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StatusDetails = new SimpleProviderPropertyDefinition("StatusDetails", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StatusTotalCount = new SimpleProviderPropertyDefinition("StatusTotalCount", ExchangeObjectVersion.Exchange2010, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition Type = new SimpleProviderPropertyDefinition("Type", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition PilotUserID = new SimpleProviderPropertyDefinition("PilotUserID", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition PilotUpnField = new SimpleProviderPropertyDefinition("PilotUpnField", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition TenantInitialDomain = new SimpleProviderPropertyDefinition("TenantInitialDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition TenantScheduledUpgradeDate = new SimpleProviderPropertyDefinition("TenantScheduledUpgradeDate", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
