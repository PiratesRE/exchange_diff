using System;

namespace Microsoft.Exchange.Net
{
	internal class HttpWebRequestException : Exception
	{
		public HttpWebRequestException(Exception exception) : base("HttpWebRequest exception", exception)
		{
		}
	}
}
