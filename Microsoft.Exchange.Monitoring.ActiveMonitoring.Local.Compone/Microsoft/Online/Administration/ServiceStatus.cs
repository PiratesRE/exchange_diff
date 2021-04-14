using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "ServiceStatus", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ServiceStatus : IExtensibleDataObject
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
		public ProvisioningStatus ProvisioningStatus
		{
			get
			{
				return this.ProvisioningStatusField;
			}
			set
			{
				this.ProvisioningStatusField = value;
			}
		}

		[DataMember]
		public ServicePlan ServicePlan
		{
			get
			{
				return this.ServicePlanField;
			}
			set
			{
				this.ServicePlanField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private ProvisioningStatus ProvisioningStatusField;

		private ServicePlan ServicePlanField;
	}
}
