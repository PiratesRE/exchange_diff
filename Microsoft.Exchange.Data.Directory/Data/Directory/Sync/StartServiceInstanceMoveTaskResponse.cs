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
	[MessageContract(WrapperName = "StartServiceInstanceMoveTaskResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class StartServiceInstanceMoveTaskResponse
	{
		public StartServiceInstanceMoveTaskResponse()
		{
		}

		public StartServiceInstanceMoveTaskResponse(ServiceInstanceMoveOperationResult StartServiceInstanceMoveTaskResult)
		{
			this.StartServiceInstanceMoveTaskResult = StartServiceInstanceMoveTaskResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMoveOperationResult StartServiceInstanceMoveTaskResult;
	}
}
