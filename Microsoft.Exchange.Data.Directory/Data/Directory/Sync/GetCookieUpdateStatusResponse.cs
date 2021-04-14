using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[MessageContract(WrapperName = "GetCookieUpdateStatusResponse", WrapperNamespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", IsWrapped = true)]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class GetCookieUpdateStatusResponse
	{
		public GetCookieUpdateStatusResponse()
		{
		}

		public GetCookieUpdateStatusResponse(CookieUpdateStatus GetCookieUpdateStatusResult)
		{
			this.GetCookieUpdateStatusResult = GetCookieUpdateStatusResult;
		}

		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11", Order = 0)]
		public CookieUpdateStatus GetCookieUpdateStatusResult;
	}
}
