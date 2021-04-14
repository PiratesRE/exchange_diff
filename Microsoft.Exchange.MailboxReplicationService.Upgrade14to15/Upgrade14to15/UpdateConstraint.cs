using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "UpdateConstraint", Namespace = "http://tempuri.org/")]
	public class UpdateConstraint : IExtensibleDataObject
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
		public Constraint[] constraint
		{
			get
			{
				return this.constraintField;
			}
			set
			{
				this.constraintField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Constraint[] constraintField;
	}
}
