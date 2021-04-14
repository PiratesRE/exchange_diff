using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "WorkItemStatusInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class WorkItemStatusInfo : IExtensibleDataObject
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
		public WorkItemStatus Status
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

		private ExtensionDataObject extensionDataField;

		private string CommentField;

		private int CompletedCountField;

		private string HandlerStateField;

		private WorkItemStatus StatusField;

		private Uri StatusDetailsField;

		private int TotalCountField;
	}
}
