using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "AddMsodsUserResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class AddMsodsUserResponse : IExtensibleDataObject
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
		public Guid AddMsodsUserResult
		{
			get
			{
				return this.AddMsodsUserResultField;
			}
			set
			{
				this.AddMsodsUserResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid AddMsodsUserResultField;
	}
}
