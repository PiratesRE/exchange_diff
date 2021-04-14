using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class DuplicateWaitObjectException : ArgumentException
	{
		private static string DuplicateWaitObjectMessage
		{
			get
			{
				if (DuplicateWaitObjectException._duplicateWaitObjectMessage == null)
				{
					DuplicateWaitObjectException._duplicateWaitObjectMessage = Environment.GetResourceString("Arg_DuplicateWaitObjectException");
				}
				return DuplicateWaitObjectException._duplicateWaitObjectMessage;
			}
		}

		public DuplicateWaitObjectException() : base(DuplicateWaitObjectException.DuplicateWaitObjectMessage)
		{
			base.SetErrorCode(-2146233047);
		}

		public DuplicateWaitObjectException(string parameterName) : base(DuplicateWaitObjectException.DuplicateWaitObjectMessage, parameterName)
		{
			base.SetErrorCode(-2146233047);
		}

		public DuplicateWaitObjectException(string parameterName, string message) : base(message, parameterName)
		{
			base.SetErrorCode(-2146233047);
		}

		public DuplicateWaitObjectException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233047);
		}

		protected DuplicateWaitObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static volatile string _duplicateWaitObjectMessage;
	}
}
