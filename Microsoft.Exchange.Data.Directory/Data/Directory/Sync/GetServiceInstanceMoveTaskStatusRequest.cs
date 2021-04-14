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
	[MessageContract(WrapperName = "GetServiceInstanceMoveTaskStatus", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class GetServiceInstanceMoveTaskStatusRequest
	{
		public GetServiceInstanceMoveTaskStatusRequest()
		{
		}

		public GetServiceInstanceMoveTaskStatusRequest(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			this.serviceInstanceMoveTask = serviceInstanceMoveTask;
			this.lastCookie = lastCookie;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		public ServiceInstanceMoveTask serviceInstanceMoveTask;

		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 1)]
		public byte[] lastCookie;
	}
}
