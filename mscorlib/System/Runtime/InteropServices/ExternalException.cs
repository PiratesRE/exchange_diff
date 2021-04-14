using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public class ExternalException : SystemException
	{
		public ExternalException() : base(Environment.GetResourceString("Arg_ExternalException"))
		{
			base.SetErrorCode(-2147467259);
		}

		public ExternalException(string message) : base(message)
		{
			base.SetErrorCode(-2147467259);
		}

		public ExternalException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147467259);
		}

		public ExternalException(string message, int errorCode) : base(message)
		{
			base.SetErrorCode(errorCode);
		}

		protected ExternalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public virtual int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}

		public override string ToString()
		{
			string message = this.Message;
			string str = base.GetType().ToString();
			string text = str + " (0x" + base.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
			if (!string.IsNullOrEmpty(message))
			{
				text = text + ": " + message;
			}
			Exception innerException = base.InnerException;
			if (innerException != null)
			{
				text = text + " ---> " + innerException.ToString();
			}
			if (this.StackTrace != null)
			{
				text = text + Environment.NewLine + this.StackTrace;
			}
			return text;
		}
	}
}
