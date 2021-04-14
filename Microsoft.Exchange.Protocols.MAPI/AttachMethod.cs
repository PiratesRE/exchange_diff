using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum AttachMethod
	{
		NoAttachment,
		AttachByValue,
		AttachByReference,
		AttachByRefResolve,
		AttachByRefOnly,
		AttachEmbeddedMsg,
		AttachOle
	}
}
