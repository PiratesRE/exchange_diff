using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ServiceInstanceInformation", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	public class ServiceInstanceInformation : IExtensibleDataObject
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
		public string ServiceInstance
		{
			get
			{
				return this.ServiceInstanceField;
			}
			set
			{
				this.ServiceInstanceField = value;
			}
		}

		[DataMember]
		public ServiceEndpoint[] ServiceInstanceEndpoints
		{
			get
			{
				return this.ServiceInstanceEndpointsField;
			}
			set
			{
				this.ServiceInstanceEndpointsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string ServiceInstanceField;

		private ServiceEndpoint[] ServiceInstanceEndpointsField;
	}
}
