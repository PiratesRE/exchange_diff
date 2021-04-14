using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum MapiServiceType : uint
	{
		Availability,
		Assistants,
		ContentIndex,
		Transport,
		Admin,
		Inference,
		ELC,
		SMS,
		UnknownServiceType,
		MaxServiceType = 8U
	}
}
