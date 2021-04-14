using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "SetServiceInstanceInfo", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class SetServiceInstanceInfoRequest
	{
		public SetServiceInstanceInfoRequest()
		{
		}

		public SetServiceInstanceInfoRequest(string serviceInstance, ServiceInstanceInfoValue serviceInstanceInfo)
		{
			this.serviceInstance = serviceInstance;
			this.serviceInstanceInfo = serviceInstanceInfo;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		public string serviceInstance;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 1)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceInfoValue serviceInstanceInfo;
	}
}
