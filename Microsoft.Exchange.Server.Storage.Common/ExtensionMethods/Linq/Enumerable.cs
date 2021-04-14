using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq
{
	public static class Enumerable
	{
		public static IEnumerable<TResult> Empty<TResult>()
		{
			return Enumerable.Empty<TResult>();
		}
	}
}
