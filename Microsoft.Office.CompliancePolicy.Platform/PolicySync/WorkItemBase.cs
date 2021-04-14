using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public abstract class WorkItemBase
	{
		public WorkItemBase(string externalIdentity, bool processNow, TenantContext tenantContext, bool hasPersistentBackup = false) : this(externalIdentity, default(DateTime), processNow, tenantContext, hasPersistentBackup)
		{
		}

		internal WorkItemBase(string externalIdentity, DateTime executeTimeUTC, bool processNow, TenantContext tenantContext, bool hasPersistentBackup = false)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("externalIdentity", externalIdentity);
			ArgumentValidator.ThrowIfNull("tenantContext", tenantContext);
			this.ExternalIdentity = externalIdentity;
			this.ExecuteTimeUTC = executeTimeUTC;
			this.ProcessNow = processNow;
			this.tenantContext = tenantContext;
			this.HasPersistentBackUp = hasPersistentBackup;
			this.Status = WorkItemStatus.NotStarted;
			this.Errors = new List<SyncAgentExceptionBase>();
			this.SerializableErrors = new List<SerializableException>();
		}

		public DateTime ExecuteTimeUTC { get; set; }

		public bool ProcessNow { get; set; }

		public string WorkItemId
		{
			get
			{
				return this.workItemId;
			}
			set
			{
				ArgumentValidator.ThrowIfNullOrEmpty("WorkItemId", value);
				this.workItemId = value;
			}
		}

		public string ExternalIdentity
		{
			get
			{
				return this.externalIdentity;
			}
			set
			{
				ArgumentValidator.ThrowIfNullOrEmpty("ExternalIdentity", value);
				this.externalIdentity = value;
			}
		}

		public TenantContext TenantContext
		{
			get
			{
				return this.tenantContext;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("TenantContext", value);
				this.tenantContext = value;
			}
		}

		public bool HasPersistentBackUp { get; set; }

		public WorkItemStatus Status { get; set; }

		public List<SyncAgentExceptionBase> Errors
		{
			get
			{
				return this.errors;
			}
			set
			{
				this.errors = ((value != null) ? value : new List<SyncAgentExceptionBase>());
			}
		}

		public int TryCount { get; set; }

		internal List<SerializableException> SerializableErrors { get; private set; }

		public static WorkItemBase Deserialize(byte[] binaryData)
		{
			if (binaryData == null || binaryData.Length == 0)
			{
				return null;
			}
			WorkItemBase workItemBase = (WorkItemBase)CommonUtility.BytesToObject(binaryData);
			if (workItemBase != null)
			{
				if (workItemBase.Errors == null)
				{
					workItemBase.Errors = new List<SyncAgentExceptionBase>();
				}
				if (workItemBase.SerializableErrors == null)
				{
					workItemBase.SerializableErrors = new List<SerializableException>();
				}
			}
			return workItemBase;
		}

		public abstract bool Merge(WorkItemBase newWorkItem);

		public abstract bool IsEqual(WorkItemBase newWorkItem);

		public abstract Guid GetPrimaryKey();

		public byte[] Serialize()
		{
			if (this.Errors.Any<SyncAgentExceptionBase>())
			{
				foreach (SyncAgentExceptionBase ex in this.Errors)
				{
					this.SerializableErrors.Add(new SerializableException(ex));
				}
			}
			return CommonUtility.ObjectToBytes(this);
		}

		internal void ResetStatus()
		{
			this.Status = WorkItemStatus.NotStarted;
			this.Errors = new List<SyncAgentExceptionBase>();
			this.SerializableErrors = new List<SerializableException>();
		}

		internal virtual WorkItemBase Split()
		{
			return null;
		}

		internal virtual bool RoughCompare(object other)
		{
			WorkItemBase workItemBase = other as WorkItemBase;
			return workItemBase != null && (this.ProcessNow == workItemBase.ProcessNow && this.ExternalIdentity.Equals(workItemBase.ExternalIdentity, StringComparison.OrdinalIgnoreCase) && this.Status == workItemBase.Status && this.TenantContext.TenantId == workItemBase.TenantContext.TenantId) && this.TryCount == workItemBase.TryCount;
		}

		private string workItemId;

		private string externalIdentity;

		private TenantContext tenantContext;

		[NonSerialized]
		private List<SyncAgentExceptionBase> errors;
	}
}
