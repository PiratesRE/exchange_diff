using System;
using System.Diagnostics;

namespace System.Collections
{
	[DebuggerDisplay("{value}", Name = "[{key}]", Type = "")]
	internal class KeyValuePairs
	{
		public KeyValuePairs(object key, object value)
		{
			this.value = value;
			this.key = key;
		}

		public object Key
		{
			get
			{
				return this.key;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object key;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object value;
	}
}
