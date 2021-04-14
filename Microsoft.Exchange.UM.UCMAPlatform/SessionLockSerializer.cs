using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class SessionLockSerializer : ISessionSerializer
	{
		public void SerializeCallback<TState>(TState state, SerializableCallback<TState> callback, ISerializationGuard guard, bool forceCallback, string callbackName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "=> EventSerializer queueing {0}", new object[]
			{
				callbackName
			});
			lock (this.myLock)
			{
				lock (guard.SerializationLocker)
				{
					if (forceCallback || this.ShouldProcessCallback(guard))
					{
						callback(state);
					}
				}
			}
		}

		public void SerializeEvent<TArgs>(object sender, TArgs args, SerializableEventHandler<TArgs> callback, ISerializationGuard guard, bool forceEvent, string eventName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "=> EventSerializer queueing {0}", new object[]
			{
				eventName
			});
			lock (this.myLock)
			{
				lock (guard.SerializationLocker)
				{
					if (forceEvent || this.ShouldProcessCallback(guard))
					{
						callback(sender, args);
					}
				}
			}
		}

		private bool ShouldProcessCallback(ISerializationGuard guard)
		{
			bool result = true;
			if (guard.StopSerializedEvents)
			{
				result = false;
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Ignoring callback because the serialization guard has been stopped.", new object[0]);
			}
			return result;
		}

		private object myLock = new object();
	}
}
