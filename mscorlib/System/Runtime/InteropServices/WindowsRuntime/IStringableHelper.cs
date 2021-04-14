using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal class IStringableHelper
	{
		internal static string ToString(object obj)
		{
			IStringable stringable = obj as IStringable;
			if (stringable != null)
			{
				return stringable.ToString();
			}
			return obj.ToString();
		}
	}
}
