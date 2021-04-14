using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	[TypeDependency("System.SZArrayHelper")]
	[__DynamicallyInvokable]
	public interface IReadOnlyCollection<out T> : IEnumerable<!0>, IEnumerable
	{
		[__DynamicallyInvokable]
		int Count { [__DynamicallyInvokable] get; }
	}
}
