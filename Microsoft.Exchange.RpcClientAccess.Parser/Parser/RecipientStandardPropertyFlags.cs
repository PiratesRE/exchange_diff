using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum RecipientStandardPropertyFlags : ushort
	{
		EmailAddressPresent = 8,
		DisplayNamePresent = 16,
		TransmittableDisplayNamePresent = 32,
		StandardPropertiesInUnicode = 512,
		SimpleDisplayNamePresent = 1024
	}
}
