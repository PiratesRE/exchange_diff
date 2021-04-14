using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "QueryTenantReadiness", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class QueryTenantReadiness : IExtensibleDataObject
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
		public Guid[] tenantIds
		{
			get
			{
				return this.tenantIdsField;
			}
			set
			{
				this.tenantIdsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid[] tenantIdsField;
	}
}
