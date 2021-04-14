﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class MulticastNotSupportedException : SystemException
	{
		public MulticastNotSupportedException() : base(Environment.GetResourceString("Arg_MulticastNotSupportedException"))
		{
			base.SetErrorCode(-2146233068);
		}

		public MulticastNotSupportedException(string message) : base(message)
		{
			base.SetErrorCode(-2146233068);
		}

		public MulticastNotSupportedException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233068);
		}

		internal MulticastNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
