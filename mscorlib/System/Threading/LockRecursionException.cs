using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Threading
{
	[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class LockRecursionException : Exception
	{
		[__DynamicallyInvokable]
		public LockRecursionException()
		{
		}

		[__DynamicallyInvokable]
		public LockRecursionException(string message) : base(message)
		{
		}

		protected LockRecursionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public LockRecursionException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
