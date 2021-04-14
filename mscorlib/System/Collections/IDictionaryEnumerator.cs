using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IDictionaryEnumerator : IEnumerator
	{
		[__DynamicallyInvokable]
		object Key { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		object Value { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		DictionaryEntry Entry { [__DynamicallyInvokable] get; }
	}
}
