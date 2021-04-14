using System;

namespace Microsoft.Exchange.Common
{
	public class BufferTooSmallException : Exception
	{
		public BufferTooSmallException(string message) : base(message)
		{
		}
	}
}
