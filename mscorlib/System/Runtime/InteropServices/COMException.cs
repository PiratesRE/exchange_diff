using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using Microsoft.Win32;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class COMException : ExternalException
	{
		[__DynamicallyInvokable]
		public COMException() : base(Environment.GetResourceString("Arg_COMException"))
		{
			base.SetErrorCode(-2147467259);
		}

		[__DynamicallyInvokable]
		public COMException(string message) : base(message)
		{
			base.SetErrorCode(-2147467259);
		}

		[__DynamicallyInvokable]
		public COMException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147467259);
		}

		[__DynamicallyInvokable]
		public COMException(string message, int errorCode) : base(message)
		{
			base.SetErrorCode(errorCode);
		}

		[SecuritySafeCritical]
		internal COMException(int hresult) : base(Win32Native.GetMessage(hresult))
		{
			base.SetErrorCode(hresult);
		}

		internal COMException(string message, int hresult, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(hresult);
		}

		protected COMException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string ToString()
		{
			string message = this.Message;
			string str = base.GetType().ToString();
			string text = str + " (0x" + base.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")";
			if (message != null && message.Length > 0)
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
