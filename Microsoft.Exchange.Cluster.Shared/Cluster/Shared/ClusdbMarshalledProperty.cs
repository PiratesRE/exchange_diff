using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class ClusdbMarshalledProperty : IDisposable
	{
		public string PropertyName { get; set; }

		public object PropertyValue { get; set; }

		public RegistryValueKind ValueKind { get; set; }

		public IntPtr PropertyValueIntPtr { get; set; }

		public int PropertyValueSize { get; set; }

		public static IntPtr FromStringArrayToIntPtr(string[] strings, out int valueSize)
		{
			valueSize = strings.Sum((string s) => s.Length + 1) * 2;
			IntPtr intPtr = Marshal.AllocHGlobal(valueSize);
			IntPtr intPtr2 = intPtr;
			foreach (string text in strings)
			{
				Marshal.Copy(text.ToCharArray(), 0, intPtr2, text.Length);
				intPtr2 = new IntPtr(intPtr2.ToInt64() + (long)(text.Length * 2));
				Marshal.WriteInt16(intPtr2, '\0');
				intPtr2 = new IntPtr(intPtr2.ToInt64() + 2L);
			}
			return intPtr;
		}

		public static string[] FromIntPtrToStringArray(IntPtr intPtr, int valueSize)
		{
			List<string> list = new List<string>(4);
			if (intPtr != IntPtr.Zero)
			{
				int i = 0;
				int num = 0;
				while (i < valueSize)
				{
					IntPtr ptr = new IntPtr(intPtr.ToInt64() + (long)i);
					string text = Marshal.PtrToStringUni(ptr);
					if (text == null)
					{
						break;
					}
					list.Add(text);
					i += (text.Length + 1) * 2;
					num++;
				}
			}
			return list.ToArray();
		}

		public static IntPtr FromByteArrayToIntPtr(byte[] bytes, out int valueSize)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			valueSize = bytes.Length;
			return intPtr;
		}

		public static ClusdbMarshalledProperty Create(string propertyName, object propertyValue, RegistryValueKind valueKind = RegistryValueKind.Unknown)
		{
			int propertyValueSize = 0;
			if (valueKind == RegistryValueKind.Unknown)
			{
				valueKind = Utils.GetValueKind(propertyValue);
			}
			IntPtr intPtr;
			if (propertyValue is string)
			{
				string text = propertyValue as string;
				intPtr = Marshal.StringToHGlobalUni(text);
				propertyValueSize = (text.Length + 1) * 2;
			}
			else if (propertyValue is string[])
			{
				string[] strings = propertyValue as string[];
				intPtr = ClusdbMarshalledProperty.FromStringArrayToIntPtr(strings, out propertyValueSize);
			}
			else if (propertyValue is IEnumerable<string>)
			{
				string[] strings2 = (propertyValue as IEnumerable<string>).ToArray<string>();
				intPtr = ClusdbMarshalledProperty.FromStringArrayToIntPtr(strings2, out propertyValueSize);
			}
			else if (propertyValue is byte[])
			{
				intPtr = ClusdbMarshalledProperty.FromByteArrayToIntPtr(propertyValue as byte[], out propertyValueSize);
			}
			else if (propertyValue is int)
			{
				intPtr = Marshal.AllocHGlobal(4);
				Marshal.WriteInt32(intPtr, Convert.ToInt32(propertyValue));
				propertyValueSize = 4;
			}
			else if (propertyValue is uint)
			{
				intPtr = Marshal.AllocHGlobal(4);
				Marshal.WriteInt32(intPtr, (int)Convert.ToUInt32(propertyValue));
				propertyValueSize = 4;
			}
			else if (propertyValue is long)
			{
				intPtr = Marshal.AllocHGlobal(8);
				Marshal.WriteInt64(intPtr, Convert.ToInt64(propertyValue));
				propertyValueSize = 8;
			}
			else
			{
				if (!(propertyValue is ulong))
				{
					return null;
				}
				intPtr = Marshal.AllocHGlobal(8);
				Marshal.WriteInt64(intPtr, (long)Convert.ToUInt64(propertyValue));
				propertyValueSize = 8;
			}
			return new ClusdbMarshalledProperty
			{
				PropertyName = propertyName,
				PropertyValue = propertyValue,
				ValueKind = valueKind,
				PropertyValueIntPtr = intPtr,
				PropertyValueSize = propertyValueSize
			};
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (this.PropertyValueIntPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.PropertyValueIntPtr);
				}
				this.isDisposed = true;
			}
		}

		private bool isDisposed;
	}
}
