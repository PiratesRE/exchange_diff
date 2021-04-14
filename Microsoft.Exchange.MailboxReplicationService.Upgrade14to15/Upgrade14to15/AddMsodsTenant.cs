using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AddMsodsTenant", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class AddMsodsTenant : IExtensibleDataObject
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
		public string tenantDomainPrefix
		{
			get
			{
				return this.tenantDomainPrefixField;
			}
			set
			{
				this.tenantDomainPrefixField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string tenantDomainPrefixField;
	}
}
