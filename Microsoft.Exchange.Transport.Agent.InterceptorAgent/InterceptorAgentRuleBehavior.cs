using System;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	[Flags]
	public enum InterceptorAgentRuleBehavior : ushort
	{
		NoOp = 0,
		PermanentReject = 1,
		TransientReject = 2,
		Drop = 4,
		Defer = 8,
		Delay = 16,
		Archive = 32,
		ArchiveHeaders = 64,
		ArchiveAndPermanentReject = 33,
		ArchiveAndTransientReject = 34,
		ArchiveAndDrop = 36,
		ArchiveHeadersAndDrop = 68,
		ArchiveHeadersAndTransientReject = 66
	}
}
