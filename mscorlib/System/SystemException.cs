﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class SystemException : Exception
	{
		public SystemException() : base(Environment.GetResourceString("Arg_SystemException"))
		{
			base.SetErrorCode(-2146233087);
		}

		public SystemException(string message) : base(message)
		{
			base.SetErrorCode(-2146233087);
		}

		public SystemException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233087);
		}

		protected SystemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
