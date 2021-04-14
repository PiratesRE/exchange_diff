using System;
using System.Collections.Generic;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal static class IReferenceFactory
	{
		[SecuritySafeCritical]
		internal static object CreateIReference(object obj)
		{
			Type type = obj.GetType();
			if (type.IsArray)
			{
				return IReferenceFactory.CreateIReferenceArray((Array)obj);
			}
			if (type == typeof(int))
			{
				return new CLRIReferenceImpl<int>(PropertyType.Int32, (int)obj);
			}
			if (type == typeof(string))
			{
				return new CLRIReferenceImpl<string>(PropertyType.String, (string)obj);
			}
			if (type == typeof(byte))
			{
				return new CLRIReferenceImpl<byte>(PropertyType.UInt8, (byte)obj);
			}
			if (type == typeof(short))
			{
				return new CLRIReferenceImpl<short>(PropertyType.Int16, (short)obj);
			}
			if (type == typeof(ushort))
			{
				return new CLRIReferenceImpl<ushort>(PropertyType.UInt16, (ushort)obj);
			}
			if (type == typeof(uint))
			{
				return new CLRIReferenceImpl<uint>(PropertyType.UInt32, (uint)obj);
			}
			if (type == typeof(long))
			{
				return new CLRIReferenceImpl<long>(PropertyType.Int64, (long)obj);
			}
			if (type == typeof(ulong))
			{
				return new CLRIReferenceImpl<ulong>(PropertyType.UInt64, (ulong)obj);
			}
			if (type == typeof(float))
			{
				return new CLRIReferenceImpl<float>(PropertyType.Single, (float)obj);
			}
			if (type == typeof(double))
			{
				return new CLRIReferenceImpl<double>(PropertyType.Double, (double)obj);
			}
			if (type == typeof(char))
			{
				return new CLRIReferenceImpl<char>(PropertyType.Char16, (char)obj);
			}
			if (type == typeof(bool))
			{
				return new CLRIReferenceImpl<bool>(PropertyType.Boolean, (bool)obj);
			}
			if (type == typeof(Guid))
			{
				return new CLRIReferenceImpl<Guid>(PropertyType.Guid, (Guid)obj);
			}
			if (type == typeof(DateTimeOffset))
			{
				return new CLRIReferenceImpl<DateTimeOffset>(PropertyType.DateTime, (DateTimeOffset)obj);
			}
			if (type == typeof(TimeSpan))
			{
				return new CLRIReferenceImpl<TimeSpan>(PropertyType.TimeSpan, (TimeSpan)obj);
			}
			if (type == typeof(object))
			{
				return new CLRIReferenceImpl<object>(PropertyType.Inspectable, obj);
			}
			if (type == typeof(RuntimeType))
			{
				return new CLRIReferenceImpl<Type>(PropertyType.Other, (Type)obj);
			}
			PropertyType? propertyType = null;
			if (type == IReferenceFactory.s_pointType)
			{
				propertyType = new PropertyType?(PropertyType.Point);
			}
			else if (type == IReferenceFactory.s_rectType)
			{
				propertyType = new PropertyType?(PropertyType.Rect);
			}
			else if (type == IReferenceFactory.s_sizeType)
			{
				propertyType = new PropertyType?(PropertyType.Size);
			}
			else if (type.IsValueType || obj is Delegate)
			{
				propertyType = new PropertyType?(PropertyType.Other);
			}
			if (propertyType != null)
			{
				Type type2 = typeof(CLRIReferenceImpl<>).MakeGenericType(new Type[]
				{
					type
				});
				return Activator.CreateInstance(type2, new object[]
				{
					propertyType.Value,
					obj
				});
			}
			return null;
		}

		[SecuritySafeCritical]
		internal static object CreateIReferenceArray(Array obj)
		{
			Type elementType = obj.GetType().GetElementType();
			if (elementType == typeof(int))
			{
				return new CLRIReferenceArrayImpl<int>(PropertyType.Int32Array, (int[])obj);
			}
			if (elementType == typeof(string))
			{
				return new CLRIReferenceArrayImpl<string>(PropertyType.StringArray, (string[])obj);
			}
			if (elementType == typeof(byte))
			{
				return new CLRIReferenceArrayImpl<byte>(PropertyType.UInt8Array, (byte[])obj);
			}
			if (elementType == typeof(short))
			{
				return new CLRIReferenceArrayImpl<short>(PropertyType.Int16Array, (short[])obj);
			}
			if (elementType == typeof(ushort))
			{
				return new CLRIReferenceArrayImpl<ushort>(PropertyType.UInt16Array, (ushort[])obj);
			}
			if (elementType == typeof(uint))
			{
				return new CLRIReferenceArrayImpl<uint>(PropertyType.UInt32Array, (uint[])obj);
			}
			if (elementType == typeof(long))
			{
				return new CLRIReferenceArrayImpl<long>(PropertyType.Int64Array, (long[])obj);
			}
			if (elementType == typeof(ulong))
			{
				return new CLRIReferenceArrayImpl<ulong>(PropertyType.UInt64Array, (ulong[])obj);
			}
			if (elementType == typeof(float))
			{
				return new CLRIReferenceArrayImpl<float>(PropertyType.SingleArray, (float[])obj);
			}
			if (elementType == typeof(double))
			{
				return new CLRIReferenceArrayImpl<double>(PropertyType.DoubleArray, (double[])obj);
			}
			if (elementType == typeof(char))
			{
				return new CLRIReferenceArrayImpl<char>(PropertyType.Char16Array, (char[])obj);
			}
			if (elementType == typeof(bool))
			{
				return new CLRIReferenceArrayImpl<bool>(PropertyType.BooleanArray, (bool[])obj);
			}
			if (elementType == typeof(Guid))
			{
				return new CLRIReferenceArrayImpl<Guid>(PropertyType.GuidArray, (Guid[])obj);
			}
			if (elementType == typeof(DateTimeOffset))
			{
				return new CLRIReferenceArrayImpl<DateTimeOffset>(PropertyType.DateTimeArray, (DateTimeOffset[])obj);
			}
			if (elementType == typeof(TimeSpan))
			{
				return new CLRIReferenceArrayImpl<TimeSpan>(PropertyType.TimeSpanArray, (TimeSpan[])obj);
			}
			if (elementType == typeof(Type))
			{
				return new CLRIReferenceArrayImpl<Type>(PropertyType.OtherArray, (Type[])obj);
			}
			PropertyType? propertyType = null;
			if (elementType == IReferenceFactory.s_pointType)
			{
				propertyType = new PropertyType?(PropertyType.PointArray);
			}
			else if (elementType == IReferenceFactory.s_rectType)
			{
				propertyType = new PropertyType?(PropertyType.RectArray);
			}
			else if (elementType == IReferenceFactory.s_sizeType)
			{
				propertyType = new PropertyType?(PropertyType.SizeArray);
			}
			else if (elementType.IsValueType)
			{
				if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<, >))
				{
					object[] array = new object[obj.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = obj.GetValue(i);
					}
					obj = array;
				}
				else
				{
					propertyType = new PropertyType?(PropertyType.OtherArray);
				}
			}
			else if (typeof(Delegate).IsAssignableFrom(elementType))
			{
				propertyType = new PropertyType?(PropertyType.OtherArray);
			}
			if (propertyType != null)
			{
				Type type = typeof(CLRIReferenceArrayImpl<>).MakeGenericType(new Type[]
				{
					elementType
				});
				return Activator.CreateInstance(type, new object[]
				{
					propertyType.Value,
					obj
				});
			}
			return new CLRIReferenceArrayImpl<object>(PropertyType.InspectableArray, (object[])obj);
		}

		internal static readonly Type s_pointType = Type.GetType("Windows.Foundation.Point, System.Runtime.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

		internal static readonly Type s_rectType = Type.GetType("Windows.Foundation.Rect, System.Runtime.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

		internal static readonly Type s_sizeType = Type.GetType("Windows.Foundation.Size, System.Runtime.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
	}
}
