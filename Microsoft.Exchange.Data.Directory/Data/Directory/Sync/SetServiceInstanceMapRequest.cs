using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "SetServiceInstanceMap", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class SetServiceInstanceMapRequest
	{
		public SetServiceInstanceMapRequest()
		{
		}

		public SetServiceInstanceMapRequest(string serviceType, ServiceInstanceMapValue serviceInstanceMap)
		{
			this.serviceType = serviceType;
			this.serviceInstanceMap = serviceInstanceMap;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public string serviceType;

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 1)]
		public ServiceInstanceMapValue serviceInstanceMap;
	}
}
