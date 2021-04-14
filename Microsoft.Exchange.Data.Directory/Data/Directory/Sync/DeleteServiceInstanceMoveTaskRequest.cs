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
	[MessageContract(WrapperName = "DeleteServiceInstanceMoveTask", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	public class DeleteServiceInstanceMoveTaskRequest
	{
		public DeleteServiceInstanceMoveTaskRequest()
		{
		}

		public DeleteServiceInstanceMoveTaskRequest(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			this.serviceInstanceMoveTask = serviceInstanceMoveTask;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMoveTask serviceInstanceMoveTask;
	}
}
