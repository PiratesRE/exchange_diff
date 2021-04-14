using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal enum CallerIdStatus
	{
		Valid,
		Invalid,
		Indeterminate,
		EpdError,
		Error,
		Null
	}
}
