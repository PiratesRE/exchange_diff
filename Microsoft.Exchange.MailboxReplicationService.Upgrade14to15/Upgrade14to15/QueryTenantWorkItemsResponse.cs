using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "QueryTenantWorkItemsResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class QueryTenantWorkItemsResponse : IExtensibleDataObject
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
		public WorkItemInfo[] QueryTenantWorkItemsResult
		{
			get
			{
				return this.QueryTenantWorkItemsResultField;
			}
			set
			{
				this.QueryTenantWorkItemsResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private WorkItemInfo[] QueryTenantWorkItemsResultField;
	}
}
