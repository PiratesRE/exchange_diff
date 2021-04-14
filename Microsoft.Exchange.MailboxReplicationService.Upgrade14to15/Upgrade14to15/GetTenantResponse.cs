using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "GetTenantResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetTenantResponse : IExtensibleDataObject
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
		public Tenant GetTenantResult
		{
			get
			{
				return this.GetTenantResultField;
			}
			set
			{
				this.GetTenantResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Tenant GetTenantResultField;
	}
}
