using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "FilterAndGetContextRecoveryInfoResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[DebuggerStepThrough]
	public class FilterAndGetContextRecoveryInfoResponse
	{
		public FilterAndGetContextRecoveryInfoResponse()
		{
		}

		public FilterAndGetContextRecoveryInfoResponse(ContextRecoveryInfo FilterAndGetContextRecoveryInfoResult)
		{
			this.FilterAndGetContextRecoveryInfoResult = FilterAndGetContextRecoveryInfoResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public ContextRecoveryInfo FilterAndGetContextRecoveryInfoResult;
	}
}
