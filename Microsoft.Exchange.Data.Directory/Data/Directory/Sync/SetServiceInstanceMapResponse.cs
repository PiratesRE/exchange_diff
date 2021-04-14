using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[MessageContract(WrapperName = "SetServiceInstanceMapResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class SetServiceInstanceMapResponse
	{
		public SetServiceInstanceMapResponse()
		{
		}

		public SetServiceInstanceMapResponse(ResultCode SetServiceInstanceMapResult)
		{
			this.SetServiceInstanceMapResult = SetServiceInstanceMapResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		public ResultCode SetServiceInstanceMapResult;
	}
}
