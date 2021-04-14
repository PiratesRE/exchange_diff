using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public enum EasCommonStatus
	{
		InvalidContent = 4197,
		ServerError = 8302,
		MaximumDevicesReached = 8369,
		CompositeStatusError = 510,
		StatusOutOfRange,
		LowOrderByte = 255,
		HighOrderByte = 65280,
		TransientError = 256,
		RespondsToSyncKeyReset = 512,
		RequiresSyncKeyReset = 1024,
		RequiresFolderSync = 2048,
		PermanentError = 4096,
		BackOff = 8192
	}
}
