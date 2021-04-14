using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MruDictionaryElementRemovedEventArgs<TK, TV> : EventArgs
	{
		public MruDictionaryElementRemovedEventArgs(KeyValuePair<TK, TV> keyValuePair)
		{
			this.KeyValuePair = keyValuePair;
		}

		public readonly KeyValuePair<TK, TV> KeyValuePair;
	}
}
