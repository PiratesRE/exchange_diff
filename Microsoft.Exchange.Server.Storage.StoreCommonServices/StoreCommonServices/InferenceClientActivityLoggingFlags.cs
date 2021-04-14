using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	public enum InferenceClientActivityLoggingFlags : uint
	{
		None = 0U,
		EnabledOWALogging = 1U,
		EnabledOLKLogging = 2U
	}
}
