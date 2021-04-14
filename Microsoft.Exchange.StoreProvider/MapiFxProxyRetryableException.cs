using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MapiFxProxyRetryableException : MapiRetryableException
	{
		protected MapiFxProxyRetryableException(Exception inner) : base("MapiFxProxyTransientException", inner.Message, inner)
		{
		}
	}
}
