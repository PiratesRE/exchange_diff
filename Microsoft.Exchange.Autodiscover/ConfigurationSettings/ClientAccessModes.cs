using System;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	[Flags]
	internal enum ClientAccessModes
	{
		None = 0,
		InternalAccess = 1,
		ExternalAccess = 2
	}
}
