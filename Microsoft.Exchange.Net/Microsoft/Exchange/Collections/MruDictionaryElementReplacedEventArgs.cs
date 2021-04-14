using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MruDictionaryElementReplacedEventArgs<TK, TV> : EventArgs
	{
		public MruDictionaryElementReplacedEventArgs(KeyValuePair<TK, TV> oldKeyValuePair, KeyValuePair<TK, TV> newKeyValuePair)
		{
			this.OldKeyValuePair = oldKeyValuePair;
			this.NewKeyValuePair = newKeyValuePair;
		}

		public readonly KeyValuePair<TK, TV> OldKeyValuePair;

		public readonly KeyValuePair<TK, TV> NewKeyValuePair;
	}
}
