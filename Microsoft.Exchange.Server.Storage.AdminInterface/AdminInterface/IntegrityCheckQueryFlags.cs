using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	[Flags]
	public enum IntegrityCheckQueryFlags : uint
	{
		None = 0U,
		OnlineIntegrityCheckAssistant = 1U,
		ScheduledIntegrityCheckAssistant = 2U,
		QueryJob = 4U
	}
}
