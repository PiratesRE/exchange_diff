using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum InferenceClientActivityFlags : uint
	{
		None = 0U,
		OWALoggingEnabled = 1U,
		OLKLoggingEnabled = 2U
	}
}
