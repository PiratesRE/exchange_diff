using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "SetServiceInstanceInfoResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class SetServiceInstanceInfoResponse
	{
		public SetServiceInstanceInfoResponse()
		{
		}

		public SetServiceInstanceInfoResponse(ResultCode SetServiceInstanceInfoResult)
		{
			this.SetServiceInstanceInfoResult = SetServiceInstanceInfoResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/federatedserviceonboarding/2008/11", Order = 0)]
		public ResultCode SetServiceInstanceInfoResult;
	}
}
