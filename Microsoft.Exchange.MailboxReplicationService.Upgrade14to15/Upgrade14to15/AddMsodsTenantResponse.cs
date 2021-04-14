using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AddMsodsTenantResponse", Namespace = "http://tempuri.org/")]
	public class AddMsodsTenantResponse : IExtensibleDataObject
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
		public Guid AddMsodsTenantResult
		{
			get
			{
				return this.AddMsodsTenantResultField;
			}
			set
			{
				this.AddMsodsTenantResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid AddMsodsTenantResultField;
	}
}
