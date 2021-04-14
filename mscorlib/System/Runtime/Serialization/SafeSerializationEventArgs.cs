using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	public sealed class SafeSerializationEventArgs : EventArgs
	{
		internal SafeSerializationEventArgs(StreamingContext streamingContext)
		{
			this.m_streamingContext = streamingContext;
		}

		public void AddSerializedState(ISafeSerializationData serializedState)
		{
			if (serializedState == null)
			{
				throw new ArgumentNullException("serializedState");
			}
			if (!serializedState.GetType().IsSerializable)
			{
				throw new ArgumentException(Environment.GetResourceString("Serialization_NonSerType", new object[]
				{
					serializedState.GetType(),
					serializedState.GetType().Assembly.FullName
				}));
			}
			this.m_serializedStates.Add(serializedState);
		}

		internal IList<object> SerializedStates
		{
			get
			{
				return this.m_serializedStates;
			}
		}

		public StreamingContext StreamingContext
		{
			get
			{
				return this.m_streamingContext;
			}
		}

		private StreamingContext m_streamingContext;

		private List<object> m_serializedStates = new List<object>();
	}
}
