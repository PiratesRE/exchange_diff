﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public class InvalidFilterCriteriaException : ApplicationException
	{
		public InvalidFilterCriteriaException() : base(Environment.GetResourceString("Arg_InvalidFilterCriteriaException"))
		{
			base.SetErrorCode(-2146232831);
		}

		public InvalidFilterCriteriaException(string message) : base(message)
		{
			base.SetErrorCode(-2146232831);
		}

		public InvalidFilterCriteriaException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146232831);
		}

		protected InvalidFilterCriteriaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
