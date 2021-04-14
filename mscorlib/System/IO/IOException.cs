using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class IOException : SystemException
	{
		[__DynamicallyInvokable]
		public IOException() : base(Environment.GetResourceString("Arg_IOException"))
		{
			base.SetErrorCode(-2146232800);
		}

		[__DynamicallyInvokable]
		public IOException(string message) : base(message)
		{
			base.SetErrorCode(-2146232800);
		}

		[__DynamicallyInvokable]
		public IOException(string message, int hresult) : base(message)
		{
			base.SetErrorCode(hresult);
		}

		internal IOException(string message, int hresult, string maybeFullPath) : base(message)
		{
			base.SetErrorCode(hresult);
			this._maybeFullPath = maybeFullPath;
		}

		[__DynamicallyInvokable]
		public IOException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146232800);
		}

		protected IOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[NonSerialized]
		private string _maybeFullPath;
	}
}
