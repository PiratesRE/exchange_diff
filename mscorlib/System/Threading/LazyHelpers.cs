using System;

namespace System.Threading
{
	internal static class LazyHelpers<T>
	{
		private static T ActivatorFactorySelector()
		{
			T result;
			try
			{
				result = (T)((object)Activator.CreateInstance(typeof(T)));
			}
			catch (MissingMethodException)
			{
				throw new MissingMemberException(Environment.GetResourceString("Lazy_CreateValue_NoParameterlessCtorForT"));
			}
			return result;
		}

		internal static Func<T> s_activatorFactorySelector = new Func<T>(LazyHelpers<T>.ActivatorFactorySelector);
	}
}
