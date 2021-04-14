using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	[__DynamicallyInvokable]
	[Serializable]
	public class StateMachineAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public Type StateMachineType { [__DynamicallyInvokable] get; private set; }

		[__DynamicallyInvokable]
		public StateMachineAttribute(Type stateMachineType)
		{
			this.StateMachineType = stateMachineType;
		}
	}
}
