using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "QueryWorkItems", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class QueryWorkItems : IExtensibleDataObject
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
		public string groupName
		{
			get
			{
				return this.groupNameField;
			}
			set
			{
				this.groupNameField = value;
			}
		}

		[DataMember]
		public string tenantTier
		{
			get
			{
				return this.tenantTierField;
			}
			set
			{
				this.tenantTierField = value;
			}
		}

		[DataMember]
		public string workItemType
		{
			get
			{
				return this.workItemTypeField;
			}
			set
			{
				this.workItemTypeField = value;
			}
		}

		[DataMember(Order = 3)]
		public WorkItemStatus status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		[DataMember(Order = 4)]
		public int pageSize
		{
			get
			{
				return this.pageSizeField;
			}
			set
			{
				this.pageSizeField = value;
			}
		}

		[DataMember(Order = 5)]
		public byte[] bookmark
		{
			get
			{
				return this.bookmarkField;
			}
			set
			{
				this.bookmarkField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string groupNameField;

		private string tenantTierField;

		private string workItemTypeField;

		private WorkItemStatus statusField;

		private int pageSizeField;

		private byte[] bookmarkField;
	}
}
