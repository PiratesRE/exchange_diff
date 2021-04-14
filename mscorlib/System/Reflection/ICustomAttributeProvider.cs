using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public interface ICustomAttributeProvider
	{
		object[] GetCustomAttributes(Type attributeType, bool inherit);

		object[] GetCustomAttributes(bool inherit);

		bool IsDefined(Type attributeType, bool inherit);
	}
}
