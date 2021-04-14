using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	[Flags]
	public enum IntegrityCheckExecutionFlags : uint
	{
		None = 0U,
		OnlineIntegrityCheckAssistant = 1U,
		ScheduledIntegrityCheckAssistant = 2U
	}
}
