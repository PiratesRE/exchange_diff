using System;

namespace Microsoft.Mapi
{
	internal enum AttachMethods
	{
		NoAttachment,
		ByValue,
		ByReference,
		ByRefResolve,
		ByRefOnly,
		EmbeddedMessage,
		Ole,
		ByWebReference
	}
}
