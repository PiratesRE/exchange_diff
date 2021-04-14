using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "RecoverServiceInstanceMoveTask", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	public class RecoverServiceInstanceMoveTaskRequest
	{
		public RecoverServiceInstanceMoveTaskRequest()
		{
		}

		public RecoverServiceInstanceMoveTaskRequest(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			this.serviceInstanceMoveTask = serviceInstanceMoveTask;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMoveTask serviceInstanceMoveTask;
	}
}
