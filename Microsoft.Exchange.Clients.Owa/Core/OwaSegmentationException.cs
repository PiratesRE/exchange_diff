using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaSegmentationException : OwaPermanentException
	{
		public OwaSegmentationException(string message) : base(message)
		{
		}
	}
}
