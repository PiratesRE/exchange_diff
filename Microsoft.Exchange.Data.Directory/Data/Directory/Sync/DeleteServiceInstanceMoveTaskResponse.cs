using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "DeleteServiceInstanceMoveTaskResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class DeleteServiceInstanceMoveTaskResponse
	{
		public DeleteServiceInstanceMoveTaskResponse()
		{
		}

		public DeleteServiceInstanceMoveTaskResponse(ServiceInstanceMoveOperationResult DeleteServiceInstanceMoveTaskResult)
		{
			this.DeleteServiceInstanceMoveTaskResult = DeleteServiceInstanceMoveTaskResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMoveOperationResult DeleteServiceInstanceMoveTaskResult;
	}
}
