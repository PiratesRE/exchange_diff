using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "ServicePlan", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ServicePlan : IExtensibleDataObject
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
		public string ServiceName
		{
			get
			{
				return this.ServiceNameField;
			}
			set
			{
				this.ServiceNameField = value;
			}
		}

		[DataMember]
		public Guid? ServicePlanId
		{
			get
			{
				return this.ServicePlanIdField;
			}
			set
			{
				this.ServicePlanIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string ServiceNameField;

		private Guid? ServicePlanIdField;
	}
}
