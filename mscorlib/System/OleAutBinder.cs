using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace System
{
	[Serializable]
	internal class OleAutBinder : DefaultBinder
	{
		[SecuritySafeCritical]
		public override object ChangeType(object value, Type type, CultureInfo cultureInfo)
		{
			System.Variant source = new System.Variant(value);
			if (cultureInfo == null)
			{
				cultureInfo = CultureInfo.CurrentCulture;
			}
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			if (!type.IsPrimitive && type.IsInstanceOfType(value))
			{
				return value;
			}
			Type type2 = value.GetType();
			if (type.IsEnum && type2.IsPrimitive)
			{
				return Enum.Parse(type, value.ToString());
			}
			if (type2 == typeof(DBNull))
			{
				if (type == typeof(DBNull))
				{
					return value;
				}
				if ((type.IsClass && type != typeof(object)) || type.IsInterface)
				{
					return null;
				}
			}
			object result;
			try
			{
				object obj = OAVariantLib.ChangeType(source, type, 16, cultureInfo).ToObject();
				result = obj;
			}
			catch (NotSupportedException)
			{
				throw new COMException(Environment.GetResourceString("Interop.COM_TypeMismatch"), -2147352571);
			}
			return result;
		}
	}
}
