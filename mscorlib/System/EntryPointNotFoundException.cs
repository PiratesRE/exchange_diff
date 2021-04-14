using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class EntryPointNotFoundException : TypeLoadException
	{
		public EntryPointNotFoundException() : base(Environment.GetResourceString("Arg_EntryPointNotFoundException"))
		{
			base.SetErrorCode(-2146233053);
		}

		public EntryPointNotFoundException(string message) : base(message)
		{
			base.SetErrorCode(-2146233053);
		}

		public EntryPointNotFoundException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233053);
		}

		protected EntryPointNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
