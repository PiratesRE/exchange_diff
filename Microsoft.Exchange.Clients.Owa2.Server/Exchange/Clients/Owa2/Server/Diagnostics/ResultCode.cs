using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum ResultCode
	{
		Success,
		DebugBuild,
		NoStackTrace,
		VersionMismatch,
		NoTraceComponent,
		NoMatch,
		FailedToDeConsolidate,
		FailedToDeMinify,
		FailedToDeObfuscate,
		NoFrames
	}
}
