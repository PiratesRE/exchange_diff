using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MapiFxProxyPermanentException : MapiPermanentException
	{
		protected MapiFxProxyPermanentException(Exception inner) : base("MapiFxProxyPermanentException", inner.Message, inner)
		{
		}
	}
}
