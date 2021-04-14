using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal enum OPDetectionResult
	{
		Unknown,
		IsOpenProxy,
		NotOpenProxy,
		Pending
	}
}
