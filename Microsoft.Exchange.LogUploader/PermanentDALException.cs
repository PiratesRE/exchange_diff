using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class PermanentDALException : Exception
	{
		public PermanentDALException(string message) : base(message)
		{
		}

		public PermanentDALException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
