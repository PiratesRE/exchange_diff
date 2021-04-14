using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ToStringHelper
	{
		public static string GetAsString(byte[] bytes, int offset, int length)
		{
			return ToStringHelper.BytesToStringHelper.GetAsString(bytes, offset, length, true);
		}

		public static string GetAsString<T>(T value)
		{
			return ToStringHelper.GenericToStringHelper<T>.Default.GetAsString(value, true);
		}

		public static void AppendAsString(byte[] bytes, int offset, int length, StringBuilder sb)
		{
			ToStringHelper.BytesToStringHelper.AppendAsString(bytes, offset, length, true, sb);
		}

		public static void AppendAsString<T>(T value, StringBuilder sb)
		{
			ToStringHelper.GenericToStringHelper<T>.Default.AppendAsString(value, true, sb);
		}

		private interface IToStringHelper<T>
		{
			string GetAsString(T value, bool firstLevel);

			void AppendAsString(T value, bool firstLevel, StringBuilder sb);
		}

		private class GenericToStringHelper<T> : ToStringHelper.IToStringHelper<T>
		{
			public static ToStringHelper.IToStringHelper<T> Default
			{
				get
				{
					if (ToStringHelper.GenericToStringHelper<T>.defaultIToStringHelper == null)
					{
						ToStringHelper.GenericToStringHelper<T>.defaultIToStringHelper = ToStringHelper.GenericToStringHelper<T>.CreateToStringHelper();
					}
					return ToStringHelper.GenericToStringHelper<T>.defaultIToStringHelper;
				}
			}

			public string GetAsString(T value, bool firstLevel)
			{
				if (value == null)
				{
					return "null";
				}
				return value.ToString();
			}

			public void AppendAsString(T value, bool firstLevel, StringBuilder sb)
			{
				if (value == null)
				{
					sb.Append("null");
					return;
				}
				sb.Append(value);
			}

			private static ToStringHelper.IToStringHelper<T> CreateToStringHelper()
			{
				if (typeof(T) == typeof(DateTime))
				{
					return (ToStringHelper.IToStringHelper<T>)new ToStringHelper.DateTimeToStringHelper();
				}
				if (typeof(T) == typeof(byte[]))
				{
					return (ToStringHelper.IToStringHelper<T>)new ToStringHelper.BytesToStringHelper();
				}
				if (typeof(T) == typeof(object))
				{
					return (ToStringHelper.IToStringHelper<T>)new ToStringHelper.ObjectToStringHelper();
				}
				if (typeof(T) == typeof(string))
				{
					return (ToStringHelper.IToStringHelper<T>)new ToStringHelper.GenericToStringHelper<string>();
				}
				if (typeof(IHasCustomToStringImplementation).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
				{
					return new ToStringHelper.GenericToStringHelper<T>();
				}
				if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
				{
					if (typeof(T).GetTypeInfo().IsGenericType)
					{
						Type[] genericTypeArguments = typeof(T).GenericTypeArguments;
						if (genericTypeArguments.Length == 2)
						{
							Type right = typeof(KeyValuePair<, >).MakeGenericType(genericTypeArguments);
							if (typeof(T) == right)
							{
								return (ToStringHelper.IToStringHelper<T>)Activator.CreateInstance(typeof(ToStringHelper.GenericKeyValuePairToStringHelper<, >).MakeGenericType(genericTypeArguments));
							}
						}
					}
					return new ToStringHelper.GenericToStringHelper<T>();
				}
				if (typeof(T).GetTypeInfo().IsArray)
				{
					typeof(IEnumerable<>).MakeGenericType(new Type[]
					{
						typeof(T).GetElementType()
					});
					return (ToStringHelper.IToStringHelper<T>)Activator.CreateInstance(typeof(ToStringHelper.GenericEnumerableToStringHelper<, >).MakeGenericType(new Type[]
					{
						typeof(T),
						typeof(T).GetElementType()
					}));
				}
				Type type = ToStringHelper.GenericToStringHelper<T>.TryGetElementStrongType(typeof(T));
				if (type != null)
				{
					typeof(IEnumerable<>).MakeGenericType(new Type[]
					{
						type
					});
					return (ToStringHelper.IToStringHelper<T>)Activator.CreateInstance(typeof(ToStringHelper.GenericEnumerableToStringHelper<, >).MakeGenericType(new Type[]
					{
						typeof(T),
						type
					}));
				}
				return (ToStringHelper.IToStringHelper<T>)Activator.CreateInstance(typeof(ToStringHelper.EnumerableToStringHelper<>).MakeGenericType(new Type[]
				{
					typeof(T)
				}));
			}

			private static Type TryGetElementStrongType(Type collectionType)
			{
				if (collectionType.GetTypeInfo().IsGenericType)
				{
					foreach (Type type in collectionType.GenericTypeArguments)
					{
						if (typeof(IEnumerable<>).MakeGenericType(new Type[]
						{
							type
						}).GetTypeInfo().IsAssignableFrom(collectionType.GetTypeInfo()))
						{
							return type;
						}
					}
				}
				foreach (Type collectionType2 in collectionType.GetTypeInfo().ImplementedInterfaces)
				{
					Type type2 = ToStringHelper.GenericToStringHelper<T>.TryGetElementStrongType(collectionType2);
					if (type2 != null)
					{
						return type2;
					}
				}
				if (collectionType.GetTypeInfo().BaseType != null && collectionType.GetTypeInfo().BaseType != typeof(object))
				{
					return ToStringHelper.GenericToStringHelper<T>.TryGetElementStrongType(collectionType.GetTypeInfo().BaseType);
				}
				return null;
			}

			private static ToStringHelper.IToStringHelper<T> defaultIToStringHelper;
		}

		private class DateTimeToStringHelper : ToStringHelper.IToStringHelper<DateTime>
		{
			public string GetAsString(DateTime value, bool firstLevel)
			{
				return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff");
			}

			public void AppendAsString(DateTime value, bool firstLevel, StringBuilder sb)
			{
				sb.Append(value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff"));
			}
		}

		private class BytesToStringHelper : ToStringHelper.IToStringHelper<byte[]>
		{
			public static string GetAsString(byte[] value, int offset, int length, bool firstLevel)
			{
				if (length != 0)
				{
					StringBuilder stringBuilder = new StringBuilder(length * 2 + length / 4);
					ToStringHelper.BytesToStringHelper.AppendAsString(value, offset, length, firstLevel, stringBuilder);
					return stringBuilder.ToString();
				}
				if (!firstLevel)
				{
					return "[]";
				}
				return string.Empty;
			}

			public static void AppendAsString(byte[] bytes, int offset, int length, bool firstLevel, StringBuilder sb)
			{
				if (!firstLevel)
				{
					sb.Append("[");
				}
				for (int i = 0; i < length; i++)
				{
					if (i != 0 && i % 4 == 0)
					{
						sb.Append(" ");
					}
					byte b = bytes[offset + i];
					sb.Append("0123456789ABCDEF"[(b & 240) >> 4]);
					sb.Append("0123456789ABCDEF"[(int)(b & 15)]);
				}
				if (!firstLevel)
				{
					sb.Append("]");
				}
			}

			public string GetAsString(byte[] value, bool firstLevel)
			{
				if (value == null)
				{
					return "null";
				}
				return ToStringHelper.BytesToStringHelper.GetAsString(value, 0, value.Length, firstLevel);
			}

			public void AppendAsString(byte[] value, bool firstLevel, StringBuilder sb)
			{
				if (value == null)
				{
					sb.Append("null");
					return;
				}
				ToStringHelper.BytesToStringHelper.AppendAsString(value, 0, value.Length, firstLevel, sb);
			}

			private const string HexDigits = "0123456789ABCDEF";
		}

		private class GenericEnumerableToStringHelper<T, E> : ToStringHelper.IToStringHelper<T> where T : IEnumerable<E>
		{
			public string GetAsString(T value, bool firstLevel)
			{
				if (value == null)
				{
					return "null";
				}
				StringBuilder stringBuilder = new StringBuilder(50);
				this.AppendAsString(value, firstLevel, stringBuilder);
				return stringBuilder.ToString();
			}

			public void AppendAsString(T value, bool firstLevel, StringBuilder sb)
			{
				if (value == null)
				{
					sb.Append("null");
					return;
				}
				if (!firstLevel)
				{
					sb.Append("{");
				}
				bool flag = true;
				foreach (E value2 in value)
				{
					if (!flag)
					{
						sb.Append(", ");
					}
					ToStringHelper.GenericToStringHelper<E>.Default.AppendAsString(value2, false, sb);
					flag = false;
				}
				if (!firstLevel)
				{
					sb.Append("}");
				}
			}
		}

		private class GenericKeyValuePairToStringHelper<K, V> : ToStringHelper.IToStringHelper<KeyValuePair<K, V>>
		{
			public string GetAsString(KeyValuePair<K, V> value, bool firstLevel)
			{
				StringBuilder stringBuilder = new StringBuilder(50);
				this.AppendAsString(value, firstLevel, stringBuilder);
				return stringBuilder.ToString();
			}

			public void AppendAsString(KeyValuePair<K, V> value, bool firstLevel, StringBuilder sb)
			{
				if (!firstLevel)
				{
					sb.Append("[");
				}
				ToStringHelper.GenericToStringHelper<K>.Default.AppendAsString(value.Key, false, sb);
				sb.Append(", ");
				ToStringHelper.GenericToStringHelper<V>.Default.AppendAsString(value.Value, false, sb);
				if (!firstLevel)
				{
					sb.Append("]");
				}
			}
		}

		private class EnumerableToStringHelper<T> : ToStringHelper.IToStringHelper<T> where T : IEnumerable
		{
			public string GetAsString(T value, bool firstLevel)
			{
				if (value == null)
				{
					return "null";
				}
				StringBuilder stringBuilder = new StringBuilder(50);
				this.AppendAsString(value, firstLevel, stringBuilder);
				return stringBuilder.ToString();
			}

			public void AppendAsString(T value, bool firstLevel, StringBuilder sb)
			{
				if (value == null)
				{
					sb.Append("null");
					return;
				}
				if (!firstLevel)
				{
					sb.Append("{");
				}
				bool flag = true;
				foreach (object value2 in value)
				{
					if (!flag)
					{
						sb.Append(", ");
					}
					ToStringHelper.GenericToStringHelper<object>.Default.AppendAsString(value2, false, sb);
					flag = false;
				}
				if (!firstLevel)
				{
					sb.Append("}");
				}
			}
		}

		private class ObjectToStringHelper : ToStringHelper.IToStringHelper<object>
		{
			public string GetAsString(object value, bool firstLevel)
			{
				if (value == null)
				{
					return "null";
				}
				if (value is string)
				{
					return (string)value;
				}
				if (value is byte[])
				{
					return ToStringHelper.GenericToStringHelper<byte[]>.Default.GetAsString((byte[])value, firstLevel);
				}
				if (value is IHasCustomToStringImplementation)
				{
					return value.ToString();
				}
				if (value is ArraySegment<byte>)
				{
					ArraySegment<byte> arraySegment = (ArraySegment<byte>)value;
					return ToStringHelper.BytesToStringHelper.GetAsString(arraySegment.Array, arraySegment.Offset, arraySegment.Count, firstLevel);
				}
				if (value is IEnumerable)
				{
					if (value is IEnumerable<int>)
					{
						return ToStringHelper.GenericToStringHelper<IEnumerable<int>>.Default.GetAsString((IEnumerable<int>)value, firstLevel);
					}
					if (value is IEnumerable<long>)
					{
						return ToStringHelper.GenericToStringHelper<IEnumerable<long>>.Default.GetAsString((IEnumerable<long>)value, firstLevel);
					}
					if (value is IEnumerable<DateTime>)
					{
						return ToStringHelper.GenericToStringHelper<IEnumerable<DateTime>>.Default.GetAsString((IEnumerable<DateTime>)value, firstLevel);
					}
					if (value is IEnumerable<Guid>)
					{
						return ToStringHelper.GenericToStringHelper<IEnumerable<Guid>>.Default.GetAsString((IEnumerable<Guid>)value, firstLevel);
					}
					return ToStringHelper.GenericToStringHelper<IEnumerable>.Default.GetAsString((IEnumerable)value, firstLevel);
				}
				else
				{
					if (value is DateTime)
					{
						return ToStringHelper.GenericToStringHelper<DateTime>.Default.GetAsString((DateTime)value, firstLevel);
					}
					if (value is DictionaryEntry)
					{
						StringBuilder stringBuilder = new StringBuilder(50);
						this.AppendAsString(value, firstLevel, stringBuilder);
						return stringBuilder.ToString();
					}
					return value.ToString();
				}
			}

			public void AppendAsString(object value, bool firstLevel, StringBuilder sb)
			{
				if (value == null)
				{
					sb.Append("null");
					return;
				}
				if (value is string)
				{
					sb.Append((string)value);
					return;
				}
				if (value is byte[])
				{
					ToStringHelper.GenericToStringHelper<byte[]>.Default.AppendAsString((byte[])value, firstLevel, sb);
					return;
				}
				if (value is IHasCustomToStringImplementation)
				{
					((IHasCustomToStringImplementation)value).AppendAsString(sb);
					return;
				}
				if (value is ArraySegment<byte>)
				{
					ArraySegment<byte> arraySegment = (ArraySegment<byte>)value;
					ToStringHelper.BytesToStringHelper.AppendAsString(arraySegment.Array, arraySegment.Offset, arraySegment.Count, firstLevel, sb);
					return;
				}
				if (value is IEnumerable)
				{
					if (value is IEnumerable<int>)
					{
						ToStringHelper.GenericToStringHelper<IEnumerable<int>>.Default.AppendAsString((IEnumerable<int>)value, firstLevel, sb);
						return;
					}
					if (value is IEnumerable<long>)
					{
						ToStringHelper.GenericToStringHelper<IEnumerable<long>>.Default.AppendAsString((IEnumerable<long>)value, firstLevel, sb);
						return;
					}
					if (value is IEnumerable<DateTime>)
					{
						ToStringHelper.GenericToStringHelper<IEnumerable<DateTime>>.Default.AppendAsString((IEnumerable<DateTime>)value, firstLevel, sb);
						return;
					}
					if (value is IEnumerable<Guid>)
					{
						ToStringHelper.GenericToStringHelper<IEnumerable<Guid>>.Default.AppendAsString((IEnumerable<Guid>)value, firstLevel, sb);
						return;
					}
					ToStringHelper.GenericToStringHelper<IEnumerable>.Default.AppendAsString((IEnumerable)value, firstLevel, sb);
					return;
				}
				else
				{
					if (value is DateTime)
					{
						ToStringHelper.GenericToStringHelper<DateTime>.Default.AppendAsString((DateTime)value, firstLevel, sb);
						return;
					}
					if (value is DictionaryEntry)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)value;
						if (!firstLevel)
						{
							sb.Append("[");
						}
						ToStringHelper.GenericToStringHelper<object>.Default.AppendAsString(dictionaryEntry.Key, false, sb);
						sb.Append(", ");
						ToStringHelper.GenericToStringHelper<object>.Default.AppendAsString(dictionaryEntry.Value, false, sb);
						if (!firstLevel)
						{
							sb.Append("]");
							return;
						}
					}
					else
					{
						sb.Append(value);
					}
					return;
				}
			}
		}
	}
}
