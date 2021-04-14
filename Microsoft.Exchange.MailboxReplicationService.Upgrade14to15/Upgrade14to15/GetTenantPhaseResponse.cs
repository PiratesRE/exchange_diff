using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "GetTenantPhaseResponse", Namespace = "http://tempuri.org/")]
	public class GetTenantPhaseResponse : IExtensibleDataObject
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
		public string GetTenantPhaseResult
		{
			get
			{
				return this.GetTenantPhaseResultField;
			}
			set
			{
				this.GetTenantPhaseResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string GetTenantPhaseResultField;
	}
}
