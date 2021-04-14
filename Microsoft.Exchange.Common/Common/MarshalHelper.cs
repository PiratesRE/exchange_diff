using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Common
{
	internal class MarshalHelper
	{
		internal static IntPtr StringArrayToIntPtr(string[] stringArray)
		{
			int num = stringArray.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * num);
			for (int i = 0; i < num; i++)
			{
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(IntPtr)) * i, Marshal.StringToHGlobalUni(stringArray[i]));
			}
			return intPtr;
		}

		internal static void FreeIntPtrOfMarshalledObjectsArray(IntPtr marshalledArrayPtr, int numStrings)
		{
			for (int i = 0; i < numStrings; i++)
			{
				IntPtr hglobal = Marshal.ReadIntPtr(marshalledArrayPtr, Marshal.SizeOf(typeof(IntPtr)) * i);
				Marshal.FreeHGlobal(hglobal);
			}
			Marshal.FreeHGlobal(marshalledArrayPtr);
		}

		internal static IntPtr ByteArrayToIntPtr(byte[] byteArray)
		{
			int num = byteArray.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(byteArray, 0, intPtr, num);
			return intPtr;
		}
	}
}
