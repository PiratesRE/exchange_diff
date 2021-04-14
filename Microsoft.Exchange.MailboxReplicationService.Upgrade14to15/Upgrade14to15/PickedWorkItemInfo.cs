using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "PickedWorkItemInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.SymphonyHandlerService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class PickedWorkItemInfo : IExtensibleDataObject
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
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
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

		private PilotUserInfo PilotUserField;

		private DateTime ScheduledDateField;

		private Guid TenantIdField;

		private string WorkItemIdField;

		private WorkItemStatusInfo WorkItemStatusField;

		private string WorkItemTypeField;
	}
}
