using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class DirectoryNotFoundException : IOException
	{
		[__DynamicallyInvokable]
		public DirectoryNotFoundException() : base(Environment.GetResourceString("Arg_DirectoryNotFoundException"))
		{
			base.SetErrorCode(-2147024893);
		}

		[__DynamicallyInvokable]
		public DirectoryNotFoundException(string message) : base(message)
		{
			base.SetErrorCode(-2147024893);
		}

		[__DynamicallyInvokable]
		public DirectoryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024893);
		}

		protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
