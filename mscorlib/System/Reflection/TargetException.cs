using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public class TargetException : ApplicationException
	{
		public TargetException()
		{
			base.SetErrorCode(-2146232829);
		}

		public TargetException(string message) : base(message)
		{
			base.SetErrorCode(-2146232829);
		}

		public TargetException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146232829);
		}

		protected TargetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
