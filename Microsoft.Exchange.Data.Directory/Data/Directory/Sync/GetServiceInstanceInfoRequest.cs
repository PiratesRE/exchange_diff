using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "GetServiceInstanceInfo", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	public class GetServiceInstanceInfoRequest
	{
		public GetServiceInstanceInfoRequest()
		{
		}

		public GetServiceInstanceInfoRequest(string serviceInstance)
		{
			this.serviceInstance = serviceInstance;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public string serviceInstance;
	}
}
