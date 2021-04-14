using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DeleteConstraint", Namespace = "http://tempuri.org/")]
	public class DeleteConstraint : IExtensibleDataObject
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
		public string[] constraintName
		{
			get
			{
				return this.constraintNameField;
			}
			set
			{
				this.constraintNameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string workloadNameField;

		private string[] constraintNameField;
	}
}
