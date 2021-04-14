using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "QueryWorkItemsResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class QueryWorkItemsResponse : IExtensibleDataObject
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
		public WorkItemQueryResult QueryWorkItemsResult
		{
			get
			{
				return this.QueryWorkItemsResultField;
			}
			set
			{
				this.QueryWorkItemsResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private WorkItemQueryResult QueryWorkItemsResultField;
	}
}
