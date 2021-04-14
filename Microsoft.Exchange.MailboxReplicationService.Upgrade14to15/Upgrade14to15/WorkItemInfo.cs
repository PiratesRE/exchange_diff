using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "WorkItemInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	public class WorkItemInfo : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public DateTime Created
		{
			get
			{
				return this.CreatedField;
			}
			set
			{
				this.CreatedField = value;
			}
		}

		[DataMember]
		public DateTime Modified
		{
			get
			{
				return this.ModifiedField;
			}
			set
			{
				this.ModifiedField = value;
			}
		}

		[DataMember]
		public PilotUserInfo PilotUser
		{
			get
			{
				return this.PilotUserField;
			}
			set
			{
				this.PilotUserField = value;
			}
		}

		[DataMember]
		public DateTime ScheduledDate
		{
			get
			{
				return this.ScheduledDateField;
			}
			set
			{
				this.ScheduledDateField = value;
			}
		}

		[DataMember]
		public TenantInfo Tenant
		{
			get
			{
				return this.TenantField;
			}
			set
			{
				this.TenantField = value;
			}
		}

		[DataMember]
		public string WorkItemId
		{
			get
			{
				return this.WorkItemIdField;
			}
			set
			{
				this.WorkItemIdField = value;
			}
		}

		[DataMember]
		public WorkItemStatusInfo WorkItemStatus
		{
			get
			{
				return this.WorkItemStatusField;
			}
			set
			{
				this.WorkItemStatusField = value;
			}
		}

		[DataMember]
		public string WorkItemType
		{
			get
			{
				return this.WorkItemTypeField;
			}
			set
			{
				this.WorkItemTypeField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime CreatedField;

		private DateTime ModifiedField;

		private PilotUserInfo PilotUserField;

		private DateTime ScheduledDateField;

		private TenantInfo TenantField;

		private string WorkItemIdField;

		private WorkItemStatusInfo WorkItemStatusField;

		private string WorkItemTypeField;
	}
}
