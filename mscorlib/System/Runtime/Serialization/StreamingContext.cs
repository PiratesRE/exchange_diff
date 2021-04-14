using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct StreamingContext
	{
		public StreamingContext(StreamingContextStates state)
		{
			this = new StreamingContext(state, null);
		}

		public StreamingContext(StreamingContextStates state, object additional)
		{
			this.m_state = state;
			this.m_additionalContext = additional;
		}

		public object Context
		{
			get
			{
				return this.m_additionalContext;
			}
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is StreamingContext && (((StreamingContext)obj).m_additionalContext == this.m_additionalContext && ((StreamingContext)obj).m_state == this.m_state);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return (int)this.m_state;
		}

		public StreamingContextStates State
		{
			get
			{
				return this.m_state;
			}
		}

		internal object m_additionalContext;

		internal StreamingContextStates m_state;
	}
}
