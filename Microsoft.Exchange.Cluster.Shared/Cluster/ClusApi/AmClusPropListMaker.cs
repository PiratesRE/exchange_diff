using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal static class AmClusPropListMaker
	{
		public static AmClusterPropListDisposable CreatePropListString(string name, string value, out int bufferSize)
		{
			int num = 4 + AmClusPropListMaker.ClusPropValueSize(name) + AmClusPropListMaker.ClusPropValueSize(value) + 4;
			int num2 = AmClusPropListMaker.PaddingSize(num, 4);
			bufferSize = num + num2;
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize);
			string.Format("buffer is alloc'ed 0x{0:x} at 0x{1:x}", bufferSize, intPtr.ToInt64());
			int num3 = 0;
			num3 += AmClusPropListMaker.WriteHeader(intPtr, num3, 1);
			num3 += AmClusPropListMaker.WriteStringValue(intPtr, num3, 262147, name);
			num3 += AmClusPropListMaker.WriteStringValue(intPtr, num3, 65539, value);
			num3 += AmClusPropListMaker.WriteTerminator(intPtr, num3);
			ClusapiMethods.ResUtilVerifyPrivatePropertyList(intPtr, bufferSize);
			return new AmClusterPropListDisposable(new SafeHGlobalHandle(intPtr), (uint)bufferSize);
		}

		public static AmClusterPropListDisposable CreatePropListInt(string name, int value, out int bufferSize)
		{
			int num = 4 + AmClusPropListMaker.ClusPropValueSize(name) + AmClusPropListMaker.ClusPropValueSize(value) + 4;
			int num2 = AmClusPropListMaker.PaddingSize(num, 4);
			bufferSize = num + num2;
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize);
			string.Format("buffer is alloc'ed 0x{0:x} at 0x{1:x}", bufferSize, intPtr.ToInt64());
			int num3 = 0;
			num3 += AmClusPropListMaker.WriteHeader(intPtr, num3, 1);
			num3 += AmClusPropListMaker.WriteStringValue(intPtr, num3, 262147, name);
			num3 += AmClusPropListMaker.WriteIntValue(intPtr, num3, 65538, value);
			num3 += AmClusPropListMaker.WriteTerminator(intPtr, num3);
			ClusapiMethods.ResUtilVerifyPrivatePropertyList(intPtr, bufferSize);
			return new AmClusterPropListDisposable(new SafeHGlobalHandle(intPtr), (uint)bufferSize);
		}

		public static AmClusterPropListDisposable CreatePropListMultiString(string name, string[] value, out int bufferSize)
		{
			int num = 4 + AmClusPropListMaker.ClusPropValueSize(name) + AmClusPropListMaker.ClusPropValueSize(value) + 4;
			int num2 = AmClusPropListMaker.PaddingSize(num, 4);
			bufferSize = num + num2;
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize);
			string.Format("buffer is alloc'ed 0x{0:x} at 0x{1:x}", bufferSize, intPtr.ToInt64());
			int num3 = 0;
			num3 += AmClusPropListMaker.WriteHeader(intPtr, num3, 1);
			num3 += AmClusPropListMaker.WriteStringValue(intPtr, num3, 262147, name);
			num3 += AmClusPropListMaker.WriteMultiStringValue(intPtr, num3, 65541, value);
			num3 += AmClusPropListMaker.WriteTerminator(intPtr, num3);
			ClusapiMethods.ResUtilVerifyPrivatePropertyList(intPtr, bufferSize);
			throw new NotImplementedException("CreatePropListMultiString entire function may work, but has not been tested properly.");
		}

		public static AmClusterPropListDisposable DupeAndAppendPropListString(IntPtr pvCurrentList, int cbCurrentList, string name, string value, out int bufferSize)
		{
			int num = cbCurrentList + AmClusPropListMaker.ClusPropValueSize(name) + AmClusPropListMaker.ClusPropValueSize(value) + 4;
			int num2 = AmClusPropListMaker.PaddingSize(num, 4);
			bufferSize = num + num2;
			IntPtr intPtr = Marshal.AllocHGlobal(bufferSize);
			string.Format("buffer is alloc'ed 0x{0:x} at 0x{1:x}", bufferSize, intPtr.ToInt64());
			int num3 = 0;
			ClusapiMethods.memcpy(intPtr, pvCurrentList, cbCurrentList);
			int num4 = Marshal.ReadInt32(pvCurrentList);
			num3 += AmClusPropListMaker.WriteHeader(intPtr, num3, num4 + 1);
			num3 = cbCurrentList + AmClusPropListMaker.WriteStringValue(intPtr, cbCurrentList, 262147, name);
			num3 += AmClusPropListMaker.WriteStringValue(intPtr, num3, 65539, value);
			num3 += AmClusPropListMaker.WriteTerminator(intPtr, num3);
			ClusapiMethods.ResUtilVerifyPrivatePropertyList(intPtr, bufferSize);
			return new AmClusterPropListDisposable(new SafeHGlobalHandle(intPtr), (uint)bufferSize);
		}

		private static int WriteHeader(IntPtr buffer, int offset, int numberOfProperties)
		{
			Marshal.WriteInt32(buffer, offset, numberOfProperties);
			return 4;
		}

		private static int WriteTerminator(IntPtr buffer, int offset)
		{
			Marshal.WriteInt32(buffer, offset, 0);
			return 4;
		}

		private static int WriteStringValue(IntPtr buffer, int offset, int syntax, string data)
		{
			int num = 0;
			Marshal.WriteInt32(buffer, offset + num, syntax);
			num += 4;
			int num2 = (data.Length + 1) * 2;
			Marshal.WriteInt32(buffer, offset + num, num2);
			num += 4;
			IntPtr destination = (IntPtr)(buffer.ToInt64() + (long)offset + (long)num);
			Marshal.Copy(data.ToCharArray(), 0, destination, data.Length);
			num += data.Length * 2;
			Marshal.WriteInt16(buffer, offset + num, 0);
			num += 2;
			int num3 = AmClusPropListMaker.PaddingSize(num2, 4);
			for (int i = 0; i < num3; i++)
			{
				Marshal.WriteByte(buffer, offset + num, 0);
				num++;
			}
			return num;
		}

		private static int WriteMultiStringValue(IntPtr buffer, int offset, int syntax, string[] values)
		{
			int num = 0;
			Marshal.WriteInt32(buffer, offset + num, syntax);
			num += 4;
			int num2 = 0;
			foreach (string text in values)
			{
				num2 += (text.Length + 1) * 2;
			}
			num2 += 2;
			Marshal.WriteInt32(buffer, offset + num, num2);
			num += 4;
			foreach (string text2 in values)
			{
				IntPtr destination = (IntPtr)(buffer.ToInt64() + (long)offset + (long)num);
				Marshal.Copy(text2.ToCharArray(), 0, destination, text2.Length);
				num += text2.Length * 2;
				Marshal.WriteInt16(buffer, offset + num, 0);
				num += 2;
			}
			Marshal.WriteInt16(buffer, offset + num, 0);
			num += 2;
			int num3 = AmClusPropListMaker.PaddingSize(num2, 4);
			for (int k = 0; k < num3; k++)
			{
				Marshal.WriteByte(buffer, offset + num, 0);
				num++;
			}
			return num;
		}

		private static int WriteIntValue(IntPtr buffer, int offset, int syntax, int data)
		{
			int num = 0;
			Marshal.WriteInt32(buffer, offset + num, syntax);
			num += 4;
			int val = 4;
			Marshal.WriteInt32(buffer, offset + num, val);
			num += 4;
			Marshal.WriteInt32(buffer, offset + num, data);
			return num + 4;
		}

		private static int ClusPropValueSize(string data)
		{
			int num = 8;
			int num2 = (data.Length + 1) * 2;
			return num + num2;
		}

		private static int ClusPropValueSize(string[] data)
		{
			int num = 8;
			int num2 = 0;
			foreach (string text in data)
			{
				num2 += (text.Length + 1) * 2;
			}
			num2 += 2;
			return num + num2;
		}

		private static int ClusPropValueSize(int data)
		{
			int num = 8;
			int num2 = 4;
			return num + num2;
		}

		private static int PaddingSize(int dataBytes, int desiredAlignment)
		{
			return desiredAlignment - ((dataBytes - 1) % desiredAlignment + 1);
		}

		private const int ClusPropListSize = 4;

		private const int ClusPropSyntaxSize = 4;

		private const int CLUSPROP_SYNTAX_NAME = 262147;

		private const int CLUSPROP_SYNTAX_LIST_VALUE_SZ = 65539;

		private const int CLUSPROP_SYNTAX_LIST_VALUE_MULTI_SZ = 65541;

		private const int CLUSPROP_SYNTAX_LIST_VALUE_DWORD = 65538;

		private const int CLUSPROP_SYNTAX_ENDMARK = 0;
	}
}
