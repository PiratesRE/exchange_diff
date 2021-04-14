using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "GetServiceInstanceInfoResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class GetServiceInstanceInfoResponse
	{
		public GetServiceInstanceInfoResponse()
		{
		}

		public GetServiceInstanceInfoResponse(ServiceInstanceInfoValue GetServiceInstanceInfoResult)
		{
			this.GetServiceInstanceInfoResult = GetServiceInstanceInfoResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceInfoValue GetServiceInstanceInfoResult;
	}
}
