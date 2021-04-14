using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "QueueEduFaultinResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	public class QueueEduFaultinResponse
	{
		public QueueEduFaultinResponse()
		{
		}

		public QueueEduFaultinResponse(ExchangeFaultinStatus QueueEduFaultinResult)
		{
			this.QueueEduFaultinResult = QueueEduFaultinResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		public ExchangeFaultinStatus QueueEduFaultinResult;
	}
}
