using System;

namespace System.Threading
{
	[__DynamicallyInvokable]
	public struct AsyncLocalValueChangedArgs<T>
	{
		[__DynamicallyInvokable]
		public T PreviousValue { [__DynamicallyInvokable] get; private set; }

		[__DynamicallyInvokable]
		public T CurrentValue { [__DynamicallyInvokable] get; private set; }

		[__DynamicallyInvokable]
		public bool ThreadContextChanged { [__DynamicallyInvokable] get; private set; }

		internal AsyncLocalValueChangedArgs(T previousValue, T currentValue, bool contextChanged)
		{
			this = default(AsyncLocalValueChangedArgs<T>);
			this.PreviousValue = previousValue;
			this.CurrentValue = currentValue;
			this.ThreadContextChanged = contextChanged;
		}
	}
}
