using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IDiagnosticInfoProvider
	{
		void GetDiagnosticData(long maxSize, out uint threadId, out uint requestId, out DiagnosticContextFlags flags, out byte[] data);
	}
}
