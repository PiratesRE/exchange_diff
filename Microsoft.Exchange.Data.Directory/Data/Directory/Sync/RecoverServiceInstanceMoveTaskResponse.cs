using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "RecoverServiceInstanceMoveTaskResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	public class RecoverServiceInstanceMoveTaskResponse
	{
		public RecoverServiceInstanceMoveTaskResponse()
		{
		}

		public RecoverServiceInstanceMoveTaskResponse(ServiceInstanceMoveOperationResult RecoverServiceInstanceMoveTaskResult)
		{
			this.RecoverServiceInstanceMoveTaskResult = RecoverServiceInstanceMoveTaskResult;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		public ServiceInstanceMoveOperationResult RecoverServiceInstanceMoveTaskResult;
	}
}
