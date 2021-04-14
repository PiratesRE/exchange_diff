using System;

namespace System.Threading.Tasks
{
	internal class Shared<T>
	{
		internal Shared(T value)
		{
			this.Value = value;
		}

		internal T Value;
	}
}
