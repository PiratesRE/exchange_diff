using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class AmbiguousMatchException : SystemException
	{
		[__DynamicallyInvokable]
		public AmbiguousMatchException() : base(Environment.GetResourceString("RFLCT.Ambiguous"))
		{
			base.SetErrorCode(-2147475171);
		}

		[__DynamicallyInvokable]
		public AmbiguousMatchException(string message) : base(message)
		{
			base.SetErrorCode(-2147475171);
		}

		[__DynamicallyInvokable]
		public AmbiguousMatchException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147475171);
		}

		internal AmbiguousMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
