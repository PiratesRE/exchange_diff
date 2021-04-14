using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "FinalizeServiceInstanceMoveTaskResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DebuggerStepThrough]
	public class FinalizeServiceInstanceMoveTaskResponse
	{
		public FinalizeServiceInstanceMoveTaskResponse()
		{
		}

		public FinalizeServiceInstanceMoveTaskResponse(ServiceInstanceMoveOperationResult FinalizeServiceInstanceMoveTaskResult)
		{
			this.FinalizeServiceInstanceMoveTaskResult = FinalizeServiceInstanceMoveTaskResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		[XmlElement(IsNullable = true)]
		public ServiceInstanceMoveOperationResult FinalizeServiceInstanceMoveTaskResult;
	}
}
