using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterRegProperty
	{
		internal static void Set<T>(string key, T oValue, AmClusterRegkeyHandle handle)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				int num;
				if (oValue is string)
				{
					string text = oValue as string;
					intPtr = Marshal.StringToHGlobalUni(text);
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.String, intPtr, (text.Length + 1) * 2);
				}
				else if (oValue is string[])
				{
					string[] array = oValue as string[];
					int num2 = 0;
					IntPtr intPtr3 = IntPtr.Zero;
					foreach (string text2 in array)
					{
						num2 += text2.Length + 1;
					}
					num2 *= 2;
					intPtr2 = Marshal.AllocHGlobal(num2);
					intPtr3 = intPtr2;
					foreach (string text3 in array)
					{
						Marshal.Copy(text3.ToCharArray(), 0, intPtr3, text3.Length);
						intPtr3 = new IntPtr(intPtr3.ToInt64() + (long)(text3.Length * 2));
						Marshal.WriteInt16(intPtr3, '\0');
						intPtr3 = new IntPtr(intPtr3.ToInt64() + 2L);
					}
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.MultiString, intPtr2, num2);
				}
				else if (oValue is byte[])
				{
					byte[] array4 = oValue as byte[];
					intPtr2 = Marshal.AllocHGlobal(array4.Length);
					Marshal.Copy(array4, 0, intPtr2, array4.Length);
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.Binary, intPtr2, array4.Length);
				}
				else if (oValue is int)
				{
					intPtr2 = Marshal.AllocHGlobal(4);
					Marshal.WriteInt32(intPtr2, Convert.ToInt32(oValue));
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.DWord, intPtr2, 4);
				}
				else if (oValue is uint)
				{
					intPtr2 = Marshal.AllocHGlobal(4);
					Marshal.WriteInt32(intPtr2, (int)Convert.ToUInt32(oValue));
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.DWord, intPtr2, 4);
				}
				else if (oValue is long)
				{
					intPtr2 = Marshal.AllocHGlobal(8);
					Marshal.WriteInt64(intPtr2, Convert.ToInt64(oValue));
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.QWord, intPtr2, 8);
				}
				else
				{
					if (!(oValue is ulong))
					{
						throw new ClusterUnsupportedRegistryTypeException(typeof(T).ToString());
					}
					intPtr2 = Marshal.AllocHGlobal(8);
					Marshal.WriteInt64(intPtr2, (long)Convert.ToUInt64(oValue));
					num = ClusapiMethods.ClusterRegSetValue(handle, key, RegistryValueKind.QWord, intPtr2, 8);
				}
				if (num != 0)
				{
					throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegSetValue()", new object[0]);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
			}
		}

		internal static T GetBestEffort<T>(string key, AmClusterRegkeyHandle handle, out bool doesKeyExist, out Exception ex)
		{
			T result = default(T);
			bool doesKeyExistTmp = false;
			ex = SharedHelper.RunClusterOperation(delegate
			{
				result = AmClusterRegProperty.Get<T>(key, handle, out doesKeyExistTmp);
			});
			doesKeyExist = doesKeyExistTmp;
			return result;
		}

		internal static T Get<T>(string valueName, AmClusterRegkeyHandle hKey, out bool doesValueExist)
		{
			object raw = AmClusterRegProperty.GetRaw(valueName, hKey, out doesValueExist);
			if (!doesValueExist)
			{
				return default(T);
			}
			return (T)((object)raw);
		}

		internal static object GetRaw(string valueName, AmClusterRegkeyHandle hKey, out bool doesValueExist)
		{
			IntPtr intPtr = IntPtr.Zero;
			int num = 1024;
			object result;
			try
			{
				intPtr = Marshal.AllocHGlobal(num);
				RegistryValueKind valueType;
				int num2 = ClusapiMethods.ClusterRegQueryValue(hKey, valueName, out valueType, intPtr, ref num);
				if (num2 == 234)
				{
					int num3 = 0;
					do
					{
						Marshal.FreeHGlobal(intPtr);
						intPtr = Marshal.AllocHGlobal(num);
						num2 = ClusapiMethods.ClusterRegQueryValue(hKey, valueName, out valueType, intPtr, ref num);
					}
					while (num2 == 234 && num3++ < 3);
				}
				if (num2 == 2 || num2 == 1018)
				{
					doesValueExist = false;
					result = null;
				}
				else
				{
					if (num2 != 0)
					{
						throw AmExceptionHelper.ConstructClusterApiException(num2, "ClusterRegQueryValue()", new object[0]);
					}
					doesValueExist = true;
					result = AmClusterRegProperty.ParseRegistryValue(valueType, intPtr, num);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return result;
		}

		internal static object ParseRegistryValue(RegistryValueKind valueType, IntPtr value, int valueSize)
		{
			switch (valueType)
			{
			case RegistryValueKind.String:
				return Marshal.PtrToStringUni(value);
			case RegistryValueKind.ExpandString:
			case (RegistryValueKind)5:
			case (RegistryValueKind)6:
				break;
			case RegistryValueKind.Binary:
			{
				byte[] array = new byte[valueSize];
				Marshal.Copy(value, array, 0, valueSize);
				return array;
			}
			case RegistryValueKind.DWord:
				return Marshal.ReadInt32(value);
			case RegistryValueKind.MultiString:
			{
				List<string> list = new List<string>(4);
				int i = 0;
				int num = 0;
				while (i < valueSize)
				{
					IntPtr ptr = new IntPtr(value.ToInt64() + (long)i);
					string text = Marshal.PtrToStringUni(ptr);
					list.Add(text);
					i += (text.Length + 1) * 2;
					num++;
				}
				return list.ToArray();
			}
			default:
				if (valueType == RegistryValueKind.QWord)
				{
					return Marshal.ReadInt64(value);
				}
				break;
			}
			throw new ClusterUnsupportedRegistryTypeException(valueType.ToString());
		}

		internal static void Delete(string valueName, AmClusterRegkeyHandle handle)
		{
			int num = ClusapiMethods.ClusterRegDeleteValue(handle, valueName);
			if (num != 2 && num != 3 && num != 1018 && num != 0)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegDeleteValue()", new object[0]);
			}
		}

		internal static string[] ReadValueNames(AmClusterRegkeyHandle handle)
		{
			List<string> list = new List<string>();
			int num = 1024;
			StringBuilder stringBuilder = new StringBuilder(num);
			int num2 = 0;
			int num6;
			for (;;)
			{
				int num3 = num;
				int num4 = 0;
				int num5 = 0;
				num6 = ClusapiMethods.ClusterRegEnumValue(handle, num2, stringBuilder, ref num3, ref num4, IntPtr.Zero, ref num5);
				if (num6 != 0)
				{
					break;
				}
				string item = stringBuilder.ToString();
				list.Add(item);
				num2++;
			}
			if (num6 != 259)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num6, "ClusterRegEnumValue()", new object[0]);
			}
			return list.ToArray();
		}
	}
}
