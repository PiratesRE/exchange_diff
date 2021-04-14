using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "UpdateTenantReadiness", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class UpdateTenantReadiness : IExtensibleDataObject
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
		public TenantReadiness[] tenantReadiness
		{
			get
			{
				return this.tenantReadinessField;
			}
			set
			{
				this.tenantReadinessField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private TenantReadiness[] tenantReadinessField;
	}
}
