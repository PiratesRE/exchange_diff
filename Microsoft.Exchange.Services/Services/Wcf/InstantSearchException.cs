using System;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class InstantSearchException : Exception
	{
		public InstantSearchException()
		{
		}

		public InstantSearchException(string msg) : base(msg)
		{
		}
	}
}
