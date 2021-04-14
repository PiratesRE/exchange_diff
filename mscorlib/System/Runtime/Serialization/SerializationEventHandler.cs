using System;

namespace System.Runtime.Serialization
{
	[Serializable]
	internal delegate void SerializationEventHandler(StreamingContext context);
}
