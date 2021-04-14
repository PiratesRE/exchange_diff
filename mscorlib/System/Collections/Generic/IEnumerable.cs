using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	[TypeDependency("System.SZArrayHelper")]
	[__DynamicallyInvokable]
	public interface IEnumerable<out T> : IEnumerable
	{
		[__DynamicallyInvokable]
		IEnumerator<T> GetEnumerator();
	}
}
