using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RedirectTenantWorkloads", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class RedirectTenantWorkloads : IExtensibleDataObject
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
		public string[] workloads
		{
			get
			{
				return this.workloadsField;
			}
			set
			{
				this.workloadsField = value;
			}
		}

		[DataMember(Order = 2)]
		public string newTargetWorkload
		{
			get
			{
				return this.newTargetWorkloadField;
			}
			set
			{
				this.newTargetWorkloadField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private string[] workloadsField;

		private string newTargetWorkloadField;
	}
}
