using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "QueryTenantReadinessResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class QueryTenantReadinessResponse : IExtensibleDataObject
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
		public TenantReadiness[] QueryTenantReadinessResult
		{
			get
			{
				return this.QueryTenantReadinessResultField;
			}
			set
			{
				this.QueryTenantReadinessResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private TenantReadiness[] QueryTenantReadinessResultField;
	}
}
