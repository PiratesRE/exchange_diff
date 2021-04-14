using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System
{
	[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=Neutral, PublicKeyToken=b77a5c561934e089")]
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InvalidTimeZoneException : Exception
	{
		[__DynamicallyInvokable]
		public InvalidTimeZoneException(string message) : base(message)
		{
		}

		[__DynamicallyInvokable]
		public InvalidTimeZoneException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidTimeZoneException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public InvalidTimeZoneException()
		{
		}
	}
}
