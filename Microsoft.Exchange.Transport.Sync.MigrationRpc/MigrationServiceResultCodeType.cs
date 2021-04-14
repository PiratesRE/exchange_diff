using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[Flags]
	internal enum MigrationServiceResultCodeType
	{
		Success = 4096,
		CommunicationPipelineError = 8192,
		TargetInvocationException = 16384,
		TransientError = 32768,
		ObjectNotHostedError = 256
	}
}
