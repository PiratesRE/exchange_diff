using System;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal interface ISessionSerializer
	{
		void SerializeCallback<TState>(TState state, SerializableCallback<TState> callback, ISerializationGuard guard, bool forceCallback, string callbackName);

		void SerializeEvent<TArgs>(object sender, TArgs args, SerializableEventHandler<TArgs> callback, ISerializationGuard guard, bool forceEvent, string eventName);
	}
}
