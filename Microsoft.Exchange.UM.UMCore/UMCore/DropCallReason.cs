using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal enum DropCallReason
	{
		None,
		UserError,
		SystemError,
		GracefulHangup,
		OutboundFailedCall
	}
}
