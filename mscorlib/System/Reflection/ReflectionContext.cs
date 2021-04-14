using System;

namespace System.Reflection
{
	[__DynamicallyInvokable]
	public abstract class ReflectionContext
	{
		[__DynamicallyInvokable]
		protected ReflectionContext()
		{
		}

		[__DynamicallyInvokable]
		public abstract Assembly MapAssembly(Assembly assembly);

		[__DynamicallyInvokable]
		public abstract TypeInfo MapType(TypeInfo type);

		[__DynamicallyInvokable]
		public virtual TypeInfo GetTypeForObject(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this.MapType(value.GetType().GetTypeInfo());
		}
	}
}
