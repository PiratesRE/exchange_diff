using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal class AmClusterPropList
	{
		public AmClusterPropList(IntPtr rawBuffer, uint bufferSize)
		{
			this.RawBuffer = rawBuffer;
			this.BufferSize = bufferSize;
		}

		public IntPtr RawBuffer { get; private set; }

		public uint BufferSize { get; private set; }

		public static IntPtr IntPtrAdd(IntPtr left, int right)
		{
			long num = left.ToInt64();
			num += (long)right;
			return new IntPtr(num);
		}

		public int ParseIntFromPropList(string propName)
		{
			uint result;
			uint num = ClusapiMethods.ResUtilFindDwordProperty(this.RawBuffer, this.BufferSize, propName, out result);
			if (num == 2U)
			{
				result = 0U;
			}
			else if (num != 0U)
			{
				throw AmExceptionHelper.ConstructClusterApiException((int)num, "ResUtilFindDwordProperty()", new object[0]);
			}
			return (int)result;
		}

		public string ParseStringFromPropList(string propName)
		{
			string empty;
			uint num = ClusapiMethods.ResUtilFindSzProperty(this.RawBuffer, this.BufferSize, propName, out empty);
			if (num == 2U)
			{
				empty = string.Empty;
			}
			else if (num != 0U)
			{
				throw AmExceptionHelper.ConstructClusterApiException((int)num, "ResUtilFindSzProperty()", new object[0]);
			}
			return empty;
		}

		public string[] ParseMultipleStringsFromPropList(string propName)
		{
			List<string> list = new List<string>(10);
			SafeHGlobalHandle safeHGlobalHandle;
			uint num2;
			uint num = ClusapiMethods.ResUtilFindMultiSzProperty(this.RawBuffer, this.BufferSize, propName, out safeHGlobalHandle, out num2);
			if (num != 2U)
			{
				if (num != 0U)
				{
					throw AmExceptionHelper.ConstructClusterApiException((int)num, "ResUtilFindMultiSzProperty()", new object[0]);
				}
				IntPtr left = safeHGlobalHandle.DangerousGetHandle();
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num2))
				{
					string text = Marshal.PtrToStringUni(AmClusterPropList.IntPtrAdd(left, num3));
					list.Add(text);
					num3 += (text.Length + 1) * 2;
				}
			}
			return list.ToArray();
		}

		public MyType Read<MyType>(string key)
		{
			MyType result = default(MyType);
			Type typeFromHandle = typeof(MyType);
			if (typeFromHandle == typeof(int))
			{
				result = (MyType)((object)this.ParseIntFromPropList(key));
			}
			else if (typeFromHandle == typeof(string))
			{
				result = (MyType)((object)this.ParseStringFromPropList(key));
			}
			else
			{
				if (!(typeFromHandle == typeof(string[])))
				{
					throw new ClusterApiException("GetCommonProperty", new NotImplementedException(string.Format("Unknown type: {0}", typeof(MyType))));
				}
				result = (MyType)((object)this.ParseMultipleStringsFromPropList(key));
			}
			return result;
		}
	}
}
