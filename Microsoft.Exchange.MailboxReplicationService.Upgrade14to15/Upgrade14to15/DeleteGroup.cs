using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "DeleteGroup", Namespace = "http://tempuri.org/")]
	public class DeleteGroup : IExtensibleDataObject
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
		public string workloadName
		{
			get
			{
				return this.workloadNameField;
			}
			set
			{
				this.workloadNameField = value;
			}
		}

		[DataMember(Order = 1)]
		public string[] groupName
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

		private ExtensionDataObject extensionDataField;

		private string workloadNameField;

		private string[] groupNameField;
	}
}
