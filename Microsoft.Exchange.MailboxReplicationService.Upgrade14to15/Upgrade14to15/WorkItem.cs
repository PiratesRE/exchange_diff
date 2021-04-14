using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "WorkItem", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.ManagementService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class WorkItem : IExtensibleDataObject
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
		public string Comment
		{
			get
			{
				return this.CommentField;
			}
			set
			{
				this.CommentField = value;
			}
		}

		[DataMember]
		public int CompletedCount
		{
			get
			{
				return this.CompletedCountField;
			}
			set
			{
				this.CompletedCountField = value;
			}
		}

		[DataMember]
		public string HandlerState
		{
			get
			{
				return this.HandlerStateField;
			}
			set
			{
				this.HandlerStateField = value;
			}
		}

		[DataMember]
		public PilotUser PilotUser
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
		public Status Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		[DataMember]
		public Uri StatusDetails
		{
			get
			{
				return this.StatusDetailsField;
			}
			set
			{
				this.StatusDetailsField = value;
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
		public int TotalCount
		{
			get
			{
				return this.TotalCountField;
			}
			set
			{
				this.TotalCountField = value;
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

		[DataMember]
		public string WorkloadName
		{
			get
			{
				return this.WorkloadNameField;
			}
			set
			{
				this.WorkloadNameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string CommentField;

		private int CompletedCountField;

		private string HandlerStateField;

		private PilotUser PilotUserField;

		private DateTime ScheduledDateField;

		private Status StatusField;

		private Uri StatusDetailsField;

		private Guid TenantIdField;

		private int TotalCountField;

		private string WorkItemIdField;

		private string WorkItemTypeField;

		private string WorkloadNameField;
	}
}
