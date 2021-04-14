using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	[StructLayout(LayoutKind.Sequential)]
	internal class NullableWrapper<T> where T : struct
	{
		public override bool Equals(object obj)
		{
			return this.Value.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		public static explicit operator NullableWrapper<T>(T value)
		{
			return new NullableWrapper<T>
			{
				Value = value
			};
		}

		public static explicit operator T(NullableWrapper<T> nullable)
		{
			if (nullable == null)
			{
				throw new ArgumentNullException("nullable");
			}
			return nullable.Value;
		}

		public T Value;
	}
}
