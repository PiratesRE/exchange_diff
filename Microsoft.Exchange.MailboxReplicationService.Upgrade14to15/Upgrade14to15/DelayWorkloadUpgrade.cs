using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DelayWorkloadUpgrade", Namespace = "http://tempuri.org/")]
	public class DelayWorkloadUpgrade : IExtensibleDataObject
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
		public Guid tenantId
		{
			get
			{
				return this.tenantIdField;
			}
			set
			{
				this.tenantIdField = value;
			}
		}

		[DataMember]
		public string[] workloadsToDelay
		{
			get
			{
				return this.workloadsToDelayField;
			}
			set
			{
				this.workloadsToDelayField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private string[] workloadsToDelayField;
	}
}
