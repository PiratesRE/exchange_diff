using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Settings
{
	[Flags]
	public enum SettingsStatus
	{
		Success = 1,
		ProtocolError = 4098,
		AccessDenied = 4099,
		ServerUnavailable = 260,
		InvalidArguments = 4101,
		ConflictingArguments = 4102,
		DeniedByPolicy = 4103
	}
}
