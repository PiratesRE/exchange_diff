using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class UnifiedPolicyNotificationBase : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return UnifiedPolicyNotificationBase.schema;
			}
		}

		public UnifiedPolicyNotificationBase()
		{
		}

		internal UnifiedPolicyNotificationBase(WorkItemBase workItem, ADObjectId mailboxOwnerId)
		{
			if (workItem == null)
			{
				throw new ArgumentNullException("workItem");
			}
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.InternalIdentity = new UnifiedPolicySyncNotificationId(workItem.ExternalIdentity);
			this.workItem = workItem;
			base.MailboxOwnerId = mailboxOwnerId;
			if (!string.IsNullOrEmpty(workItem.WorkItemId))
			{
				this.StoreObjectId = VersionedId.Deserialize(workItem.WorkItemId);
			}
		}

		internal UnifiedPolicySyncNotificationId InternalIdentity
		{
			get
			{
				return (UnifiedPolicySyncNotificationId)this[UnifiedPolicyNotificationBaseSchema.Identity];
			}
			set
			{
				this[UnifiedPolicyNotificationBaseSchema.Identity] = value;
			}
		}

		internal Guid Version
		{
			get
			{
				return (Guid)this[UnifiedPolicyNotificationBaseSchema.Version];
			}
			set
			{
				this[UnifiedPolicyNotificationBaseSchema.Version] = value;
			}
		}

		[Parameter]
		public override ObjectId Identity
		{
			get
			{
				return this.InternalIdentity;
			}
		}

		[Parameter]
		public ExDateTime? ExecuteTime
		{
			get
			{
				return new ExDateTime?((ExDateTime)this.workItem.ExecuteTimeUTC);
			}
		}

		[Parameter]
		public WorkItemStatus Status
		{
			get
			{
				return this.workItem.Status;
			}
		}

		[Parameter]
		public TenantContext TenantContext
		{
			get
			{
				return this.workItem.TenantContext;
			}
		}

		[Parameter]
		public List<SyncAgentExceptionBase> Erros
		{
			get
			{
				return this.workItem.Errors;
			}
		}

		[Parameter]
		public int RetryCount
		{
			get
			{
				return this.workItem.TryCount;
			}
		}

		internal StoreId StoreObjectId { get; set; }

		internal WorkItemBase GetWorkItem()
		{
			return this.workItem;
		}

		private static XsoMailboxConfigurationObjectSchema schema = ObjectSchema.GetInstance<UnifiedPolicyNotificationBaseSchema>();

		protected WorkItemBase workItem;
	}
}
