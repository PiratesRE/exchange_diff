using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum UserOptionsMigrationState
	{
		None = 0,
		WorkingHoursTimeZoneFixUp = 1,
		ShowInferenceUiElementsMigrated = 2
	}
}
