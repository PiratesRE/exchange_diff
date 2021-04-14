using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	internal static class EnumConverter
	{
		public static T ConvertIntegerToEnum<T>(int id) where T : struct, IConvertible
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsEnum)
			{
				throw new ArgumentException("T must be an enum.");
			}
			T t = (T)((object)Enum.ToObject(typeFromHandle, id));
			if (!Enum.IsDefined(typeFromHandle, t))
			{
				t = (T)((object)Enum.ToObject(typeFromHandle, 0));
			}
			return t;
		}

		public static int ConvertEnumToInteger<T>(T obj) where T : struct, IConvertible
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsEnum)
			{
				throw new ArgumentException("T must be an enum.");
			}
			return obj.ToInt32(null);
		}
	}
}
