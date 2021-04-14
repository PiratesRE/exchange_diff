using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	[MessageContract(WrapperName = "GetServiceInstanceMapResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	public class GetServiceInstanceMapResponse
	{
		public GetServiceInstanceMapResponse()
		{
		}

		public GetServiceInstanceMapResponse(ServiceInstanceMapValue GetServiceInstanceMapResult)
		{
			this.GetServiceInstanceMapResult = GetServiceInstanceMapResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMapValue GetServiceInstanceMapResult;
	}
}
