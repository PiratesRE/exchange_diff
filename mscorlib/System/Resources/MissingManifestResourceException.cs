using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MissingManifestResourceException : SystemException
	{
		[__DynamicallyInvokable]
		public MissingManifestResourceException() : base(Environment.GetResourceString("Arg_MissingManifestResourceException"))
		{
			base.SetErrorCode(-2146233038);
		}

		[__DynamicallyInvokable]
		public MissingManifestResourceException(string message) : base(message)
		{
			base.SetErrorCode(-2146233038);
		}

		[__DynamicallyInvokable]
		public MissingManifestResourceException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233038);
		}

		protected MissingManifestResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
