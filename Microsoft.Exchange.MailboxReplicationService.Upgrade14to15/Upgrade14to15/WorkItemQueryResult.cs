using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "WorkItemQueryResult", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	[DebuggerStepThrough]
	public class WorkItemQueryResult : IExtensibleDataObject
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
		public byte[] Bookmark
		{
			get
			{
				return this.BookmarkField;
			}
			set
			{
				this.BookmarkField = value;
			}
		}

		[DataMember]
		public bool HasMoreResults
		{
			get
			{
				return this.HasMoreResultsField;
			}
			set
			{
				this.HasMoreResultsField = value;
			}
		}

		[DataMember]
		public WorkItemInfo[] WorkItems
		{
			get
			{
				return this.WorkItemsField;
			}
			set
			{
				this.WorkItemsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private byte[] BookmarkField;

		private bool HasMoreResultsField;

		private WorkItemInfo[] WorkItemsField;
	}
}
