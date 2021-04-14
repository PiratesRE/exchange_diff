using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "GetServiceInstanceMoveTaskStatusResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class GetServiceInstanceMoveTaskStatusResponse
	{
		public GetServiceInstanceMoveTaskStatusResponse()
		{
		}

		public GetServiceInstanceMoveTaskStatusResponse(ServiceInstanceMoveOperationResult GetServiceInstanceMoveTaskStatusResult)
		{
			this.GetServiceInstanceMoveTaskStatusResult = GetServiceInstanceMoveTaskStatusResult;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		public ServiceInstanceMoveOperationResult GetServiceInstanceMoveTaskStatusResult;
	}
}
