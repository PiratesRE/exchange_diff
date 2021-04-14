using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RelocationStatusDetailsDestination : byte
	{
		NotStarted,
		InitializationStarted = 5,
		InitializationFinished = 10,
		Arriving = 75,
		Active = 80
	}
}
