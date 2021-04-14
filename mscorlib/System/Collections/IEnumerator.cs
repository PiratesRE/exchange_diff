using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Guid("496B0ABF-CDEE-11d3-88E8-00902754C43A")]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IEnumerator
	{
		[__DynamicallyInvokable]
		bool MoveNext();

		[__DynamicallyInvokable]
		object Current { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		void Reset();
	}
}
