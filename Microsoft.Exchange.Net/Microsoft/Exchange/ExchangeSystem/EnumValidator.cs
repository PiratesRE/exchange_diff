using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.ExchangeSystem
{
	public static class EnumValidator
	{
		public static void AssertValid<T>(T valueToCheck) where T : struct
		{
		}

		public static void ThrowIfInvalid<T>(T valueToCheck) where T : struct
		{
			EnumValidator<T>.ThrowIfInvalid(valueToCheck);
		}

		public static void ThrowIfInvalid<T>(T valueToCheck, string paramName) where T : struct
		{
			EnumValidator<T>.ThrowIfInvalid(valueToCheck, paramName);
		}

		public static void ThrowIfInvalid<T>(T valueToCheck, T validValue) where T : struct
		{
			EnumValidator<T>.ThrowIfInvalid(valueToCheck, validValue);
		}

		public static void ThrowIfInvalid<T>(T valueToCheck, T[] validValues) where T : struct
		{
			EnumValidator<T>.ThrowIfInvalid(valueToCheck, validValues);
		}

		public static bool IsValidValue<T>(T valueToCheck) where T : struct
		{
			return EnumValidator<T>.IsValidValue(valueToCheck);
		}

		public static bool TryParse<T>(string value, EnumParseOptions options, out T result)
		{
			object obj = null;
			if (EnumValidator.TryParse(typeof(T), value, options, out obj))
			{
				result = (T)((object)obj);
				return true;
			}
			result = default(T);
			return false;
		}

		public static bool TryParse(Type enumType, string value, EnumParseOptions options, out object result)
		{
			if (!enumType.IsEnum)
			{
				throw new EnumArgumentException(SystemStrings.InvalidTypeParam(enumType));
			}
			IEnumConvert converter = EnumValidator.GetConverter(enumType);
			return converter.TryParse(value, options, out result);
		}

		public static object Parse(Type enumType, string value, EnumParseOptions options)
		{
			object result;
			if (EnumValidator.TryParse(enumType, value, options, out result))
			{
				return result;
			}
			throw new EnumArgumentException(SystemStrings.BadEnumValue(enumType, value));
		}

		private static IEnumConvert GetConverter(Type enumType)
		{
			IEnumConvert enumConvert;
			try
			{
				EnumValidator.cacheLock.EnterReadLock();
				EnumValidator.typeMap.TryGetValue(enumType, out enumConvert);
			}
			finally
			{
				try
				{
					EnumValidator.cacheLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (enumConvert == null)
			{
				Type type = EnumValidator.genericType.MakeGenericType(new Type[]
				{
					enumType
				});
				ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
				if (constructor == null)
				{
					throw new InvalidOperationException("Unsupported EnumValidator type: " + enumType.ToString());
				}
				object obj = constructor.Invoke(null);
				enumConvert = (obj as IEnumConvert);
				try
				{
					if (EnumValidator.cacheLock.TryEnterWriteLock(EnumValidator.cacheTimeout) && !EnumValidator.typeMap.ContainsKey(enumType))
					{
						EnumValidator.typeMap.Add(enumType, enumConvert);
					}
				}
				finally
				{
					try
					{
						EnumValidator.cacheLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			return enumConvert;
		}

		private static Type genericType = typeof(EnumValidator<>);

		private static Dictionary<Type, IEnumConvert> typeMap = new Dictionary<Type, IEnumConvert>();

		private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

		private static TimeSpan cacheTimeout = TimeSpan.FromSeconds(10.0);
	}
}
