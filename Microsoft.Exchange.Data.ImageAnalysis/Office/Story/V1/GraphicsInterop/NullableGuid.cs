using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop
{
	[StructLayout(LayoutKind.Sequential)]
	internal class NullableGuid : NullableWrapper<Guid>
	{
		public new static explicit operator NullableGuid(Guid value)
		{
			return new NullableGuid
			{
				Value = value
			};
		}

		public static explicit operator Guid(NullableGuid nullable)
		{
			if (nullable == null)
			{
				throw new ArgumentNullException("nullable");
			}
			return nullable.Value;
		}

		public static NullableGuid FromGuid(Guid value)
		{
			return new NullableGuid
			{
				Value = value
			};
		}

		public Guid ToGuid()
		{
			return this.Value;
		}
	}
}
