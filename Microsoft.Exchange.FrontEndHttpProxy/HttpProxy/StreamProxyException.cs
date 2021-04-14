using System;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class StreamProxyException : Exception
	{
		public StreamProxyException(Exception innerException) : base(innerException.Message, innerException)
		{
		}
	}
}
