using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaRenderingEmbeddedReadingPaneException : OwaTransientException
	{
		public OwaRenderingEmbeddedReadingPaneException(Exception innerException) : base(string.Empty, innerException)
		{
		}
	}
}
