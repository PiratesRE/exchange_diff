using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Settings
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SettingsCommand : EasServerCommand<SettingsRequest, SettingsResponse, SettingsStatus>
	{
		internal SettingsCommand(EasConnectionSettings easConnectionSettings) : base(Command.Settings, easConnectionSettings)
		{
		}
	}
}
