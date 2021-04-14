using System;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal interface ISerializationGuard
	{
		bool StopSerializedEvents { get; }

		object SerializationLocker { get; }
	}
}
