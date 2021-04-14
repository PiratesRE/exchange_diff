using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[MessageContract(WrapperName = "StartServiceInstanceMoveTask", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class StartServiceInstanceMoveTaskRequest
	{
		public StartServiceInstanceMoveTaskRequest()
		{
		}

		public StartServiceInstanceMoveTaskRequest(string contextId, string oldServiceInstance, string newServiceInstance, ServiceInstanceMoveOptions options)
		{
			this.contextId = contextId;
			this.oldServiceInstance = oldServiceInstance;
			this.newServiceInstance = newServiceInstance;
			this.options = options;
		}

		[XmlElement(IsNullable = true)]
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 0)]
		public string contextId;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 1)]
		[XmlElement(IsNullable = true)]
		public string oldServiceInstance;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 2)]
		[XmlElement(IsNullable = true)]
		public string newServiceInstance;

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/serviceinstancemove/2008/11", Order = 3)]
		public ServiceInstanceMoveOptions options;
	}
}
