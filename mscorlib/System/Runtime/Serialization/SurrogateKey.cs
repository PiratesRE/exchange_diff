using System;

namespace System.Runtime.Serialization
{
	[Serializable]
	internal class SurrogateKey
	{
		internal SurrogateKey(Type type, StreamingContext context)
		{
			this.m_type = type;
			this.m_context = context;
		}

		public override int GetHashCode()
		{
			return this.m_type.GetHashCode();
		}

		internal Type m_type;

		internal StreamingContext m_context;
	}
}
