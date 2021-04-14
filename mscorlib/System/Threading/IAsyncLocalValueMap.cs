using System;

namespace System.Threading
{
	internal interface IAsyncLocalValueMap
	{
		bool TryGetValue(IAsyncLocal key, out object value);

		IAsyncLocalValueMap Set(IAsyncLocal key, object value, bool treatNullValueAsNonexistent);
	}
}
