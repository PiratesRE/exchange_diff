using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum UserConfigurationTypes
	{
		Stream = 1,
		XML = 2,
		Dictionary = 4
	}
}
