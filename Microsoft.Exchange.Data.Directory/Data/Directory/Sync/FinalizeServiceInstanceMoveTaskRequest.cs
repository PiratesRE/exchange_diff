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
	[MessageContract(WrapperName = "FinalizeServiceInstanceMoveTask", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class FinalizeServiceInstanceMoveTaskRequest
	{
		public FinalizeServiceInstanceMoveTaskRequest()
		{
		}

		public FinalizeServiceInstanceMoveTaskRequest(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			this.serviceInstanceMoveTask = serviceInstanceMoveTask;
			this.lastCookie = lastCookie;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		public ServiceInstanceMoveTask serviceInstanceMoveTask;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 1)]
		[XmlElement(DataType = "base64Binary", IsNullable = true)]
		public byte[] lastCookie;
	}
}
