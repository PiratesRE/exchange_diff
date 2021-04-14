using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetTenantApplicationData", Namespace = "http://tempuri.org/")]
	public class GetTenantApplicationData : IExtensibleDataObject
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
		public Guid tenantGuid
		{
			get
			{
				return this.tenantGuidField;
			}
			set
			{
				this.tenantGuidField = value;
			}
		}

		[DataMember(Order = 1)]
		public string applicationDataKey
		{
			get
			{
				return this.applicationDataKeyField;
			}
			set
			{
				this.applicationDataKeyField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantGuidField;

		private string applicationDataKeyField;
	}
}
