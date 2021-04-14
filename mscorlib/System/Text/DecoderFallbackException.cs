using System;
using System.Runtime.Serialization;

namespace System.Text
{
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class DecoderFallbackException : ArgumentException
	{
		[__DynamicallyInvokable]
		public DecoderFallbackException() : base(Environment.GetResourceString("Arg_ArgumentException"))
		{
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public DecoderFallbackException(string message) : base(message)
		{
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public DecoderFallbackException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024809);
		}

		internal DecoderFallbackException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public DecoderFallbackException(string message, byte[] bytesUnknown, int index) : base(message)
		{
			this.bytesUnknown = bytesUnknown;
			this.index = index;
		}

		[__DynamicallyInvokable]
		public byte[] BytesUnknown
		{
			[__DynamicallyInvokable]
			get
			{
				return this.bytesUnknown;
			}
		}

		[__DynamicallyInvokable]
		public int Index
		{
			[__DynamicallyInvokable]
			get
			{
				return this.index;
			}
		}

		private byte[] bytesUnknown;

		private int index;
	}
}
