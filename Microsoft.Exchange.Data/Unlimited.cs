using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Data
{
	[TypeConverter(typeof(SimpleGenericsTypeConverter))]
	[Serializable]
	public struct Unlimited<T> : IComparable, IFormattable, IEquatable<T>, IComparable<T> where T : struct, IComparable
	{
		public Unlimited(T limitedValue)
		{
			this.limitedValue = limitedValue;
			this.isUnlimited = false;
		}

		private Unlimited(string expression)
		{
			this = default(Unlimited<T>);
			if (StringComparer.OrdinalIgnoreCase.Compare(expression, "Unlimited") == 0)
			{
				this.isUnlimited = true;
				return;
			}
			object obj;
			if (typeof(T) == typeof(ByteQuantifiedSize))
			{
				obj = ByteQuantifiedSize.Parse(expression);
			}
			else if (typeof(T) == typeof(int))
			{
				obj = int.Parse(expression);
			}
			else if (typeof(T) == typeof(uint))
			{
				obj = uint.Parse(expression);
			}
			else if (typeof(T) == typeof(TimeSpan))
			{
				obj = TimeSpan.Parse(expression);
			}
			else
			{
				if (!(typeof(T) == typeof(EnhancedTimeSpan)))
				{
					throw new InvalidOperationException(DataStrings.ExceptionParseNotSupported);
				}
				obj = EnhancedTimeSpan.Parse(expression);
			}
			this.isUnlimited = false;
			this.limitedValue = (T)((object)obj);
		}

		public static Unlimited<T> UnlimitedValue
		{
			get
			{
				return new Unlimited<T>
				{
					isUnlimited = true
				};
			}
		}

		public static string UnlimitedString
		{
			get
			{
				return "Unlimited";
			}
		}

		public bool IsUnlimited
		{
			get
			{
				return this.isUnlimited;
			}
		}

		public T Value
		{
			get
			{
				if (this.isUnlimited)
				{
					throw new InvalidOperationException(DataStrings.ExceptionNoValue);
				}
				return this.limitedValue;
			}
			set
			{
				this.limitedValue = value;
				this.isUnlimited = false;
			}
		}

		public override bool Equals(object other)
		{
			if (other is Unlimited<T>)
			{
				Unlimited<T> other2 = (Unlimited<T>)other;
				return this.Equals(other2);
			}
			return false;
		}

		public bool Equals(T other)
		{
			return !this.IsUnlimited && this.limitedValue.Equals(other);
		}

		public override int GetHashCode()
		{
			if (!this.isUnlimited)
			{
				return this.limitedValue.GetHashCode();
			}
			return 0;
		}

		public override string ToString()
		{
			if (!this.isUnlimited)
			{
				return this.limitedValue.ToString();
			}
			return "Unlimited";
		}

		public static Unlimited<ByteQuantifiedSize> Parse(string expression, ByteQuantifiedSize.Quantifier defaultUnit)
		{
			if (StringComparer.OrdinalIgnoreCase.Compare(expression, "Unlimited") == 0)
			{
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
			return ByteQuantifiedSize.Parse(expression, defaultUnit);
		}

		public static bool TryParse(string expression, ByteQuantifiedSize.Quantifier defaultUnit, out Unlimited<ByteQuantifiedSize> result)
		{
			bool result2 = false;
			result = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			if (StringComparer.OrdinalIgnoreCase.Compare(expression, "Unlimited") == 0)
			{
				return true;
			}
			ByteQuantifiedSize fromValue;
			if (ByteQuantifiedSize.TryParse(expression, defaultUnit, out fromValue))
			{
				result = fromValue;
			}
			return result2;
		}

		public static Unlimited<T> Parse(string expression)
		{
			return new Unlimited<T>(expression);
		}

		public static bool TryParse(string expression, out Unlimited<T> result)
		{
			bool flag = false;
			result = Unlimited<T>.UnlimitedValue;
			if (StringComparer.OrdinalIgnoreCase.Compare(expression, "Unlimited") == 0)
			{
				return true;
			}
			object obj = null;
			if (typeof(T) == typeof(ByteQuantifiedSize))
			{
				ByteQuantifiedSize byteQuantifiedSize;
				flag = ByteQuantifiedSize.TryParse(expression, out byteQuantifiedSize);
				obj = byteQuantifiedSize;
			}
			else if (typeof(T) == typeof(int))
			{
				int num;
				flag = int.TryParse(expression, out num);
				obj = num;
			}
			else if (typeof(T) == typeof(uint))
			{
				uint num2;
				flag = uint.TryParse(expression, out num2);
				obj = num2;
			}
			else if (typeof(T) == typeof(TimeSpan))
			{
				TimeSpan timeSpan;
				flag = TimeSpan.TryParse(expression, out timeSpan);
				obj = timeSpan;
			}
			else if (typeof(T) == typeof(EnhancedTimeSpan))
			{
				EnhancedTimeSpan enhancedTimeSpan;
				flag = EnhancedTimeSpan.TryParse(expression, out enhancedTimeSpan);
				obj = enhancedTimeSpan;
			}
			if (flag && obj != null)
			{
				result = (T)((object)obj);
			}
			return flag;
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (typeof(IFormattable).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) && !this.IsUnlimited)
			{
				return ((IFormattable)((object)this.Value)).ToString(format, formatProvider);
			}
			return this.ToString();
		}

		public int CompareTo(Unlimited<T> other)
		{
			if (this.isUnlimited)
			{
				if (!other.isUnlimited)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (!other.isUnlimited)
				{
					return Comparer<T>.Default.Compare(this.limitedValue, other.limitedValue);
				}
				return -1;
			}
		}

		int IComparable.CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			if (other is Unlimited<T>)
			{
				return this.CompareTo((Unlimited<T>)other);
			}
			throw new ArgumentException(DataStrings.ExceptionObjectInvalid);
		}

		public int CompareTo(T other)
		{
			if (!this.isUnlimited)
			{
				return Comparer<T>.Default.Compare(this.limitedValue, other);
			}
			return 1;
		}

		public bool Equals(Unlimited<T> other)
		{
			return this.isUnlimited == other.isUnlimited && (this.isUnlimited || EqualityComparer<T>.Default.Equals(this.limitedValue, other.limitedValue));
		}

		public static bool operator ==(Unlimited<T> value1, Unlimited<T> value2)
		{
			return value1.Equals(value2);
		}

		public static bool operator !=(Unlimited<T> value1, Unlimited<T> value2)
		{
			return !value1.Equals(value2);
		}

		public static bool operator >(Unlimited<T> value1, Unlimited<T> value2)
		{
			return value1.CompareTo(value2) > 0;
		}

		public static bool operator >=(Unlimited<T> value1, Unlimited<T> value2)
		{
			return value1.CompareTo(value2) >= 0;
		}

		public static bool operator <(Unlimited<T> value1, Unlimited<T> value2)
		{
			return value1.CompareTo(value2) < 0;
		}

		public static bool operator <=(Unlimited<T> value1, Unlimited<T> value2)
		{
			return value1.CompareTo(value2) <= 0;
		}

		public static Unlimited<T>operator /(Unlimited<T> value1, object value2)
		{
			return Unlimited<T>.ExecDynamicOperation(value1, value2, "op_Division");
		}

		public static Unlimited<T>operator *(Unlimited<T> value1, object value2)
		{
			return Unlimited<T>.ExecDynamicOperation(value1, value2, "op_Multiply");
		}

		public static Unlimited<T>operator +(Unlimited<T> value1, object value2)
		{
			return Unlimited<T>.ExecDynamicOperation(value1, value2, "op_Addition");
		}

		public static Unlimited<T>operator -(Unlimited<T> value1, object value2)
		{
			return Unlimited<T>.ExecDynamicOperation(value1, value2, "op_Subtraction");
		}

		private static Unlimited<T> ExecDynamicOperation(Unlimited<T> value1, object value2, string operationName)
		{
			object obj = Unlimited<T>.UnBucketT(value2);
			if (!Unlimited<T>.IsValidRightOperand(obj))
			{
				throw new InvalidOperationException(DataStrings.ExceptionInvalidOperation(operationName, (value2 == null) ? null : value2.GetType().Name));
			}
			if (value1.IsUnlimited)
			{
				return Unlimited<T>.UnlimitedValue;
			}
			T value3 = Unlimited<T>.DynamicResolveOperation(value1, obj, operationName);
			return new Unlimited<T>
			{
				Value = value3
			};
		}

		private static object UnBucketT(object value)
		{
			if (value != null && value.GetType() == typeof(Unlimited<T>))
			{
				return ((Unlimited<T>)value).Value;
			}
			return value;
		}

		private static bool IsValidRightOperand(object value)
		{
			if (value == null || value.GetType().GetTypeInfo().IsGenericType)
			{
				return false;
			}
			foreach (Type right in Unlimited<T>.ValidTypeMathOperations)
			{
				if (value.GetType() == right)
				{
					return true;
				}
			}
			return false;
		}

		private static T DynamicResolveOperation(Unlimited<T> value1, object value2, string operationName)
		{
			Type[] genericTypeArguments = value1.GetType().GetTypeInfo().GenericTypeArguments;
			if (genericTypeArguments[0] == typeof(int))
			{
				int? num = value1.Value as int?;
				int? num2 = value2 as int?;
				if (num2 == null)
				{
					num2 = new int?((int)Convert.ChangeType(value2, typeof(int)));
				}
				if (num2 == null)
				{
					throw new InvalidOperationException(DataStrings.ExceptionCannotResolveOperation(operationName, typeof(T).Name, value2.GetType().Name));
				}
				if ("op_Subtraction".Equals(operationName))
				{
					return (T)((object)(num.Value - num2.Value));
				}
				if ("op_Division".Equals(operationName))
				{
					return (T)((object)(num.Value / num2.Value));
				}
				if ("op_Multiply".Equals(operationName))
				{
					return (T)((object)(num.Value * num2.Value));
				}
				return (T)((object)(num.Value + num2.Value));
			}
			else if (genericTypeArguments[0] == typeof(uint))
			{
				uint? num3 = value1.Value as uint?;
				uint? num4 = value2 as uint?;
				if (num4 == null)
				{
					num4 = new uint?((uint)Convert.ChangeType(value2, typeof(uint)));
				}
				if (num4 == null)
				{
					throw new InvalidOperationException(DataStrings.ExceptionCannotResolveOperation(operationName, typeof(T).Name, value2.GetType().Name));
				}
				if ("op_Subtraction".Equals(operationName))
				{
					return (T)((object)(num3.Value - num4.Value));
				}
				if ("op_Division".Equals(operationName))
				{
					return (T)((object)(num3.Value / num4.Value));
				}
				if ("op_Multiply".Equals(operationName))
				{
					return (T)((object)(num3.Value * num4.Value));
				}
				return (T)((object)(num3.Value + num4.Value));
			}
			else
			{
				MethodInfo methodInfo = null;
				foreach (MethodInfo methodInfo2 in from x in genericTypeArguments[0].GetTypeInfo().DeclaredMethods
				where x.Name == operationName
				select x)
				{
					ParameterInfo[] parameters = methodInfo2.GetParameters();
					if (parameters.Length == 2 && parameters[0].ParameterType == genericTypeArguments[0] && parameters[1].ParameterType == value2.GetType())
					{
						methodInfo = methodInfo2;
						break;
					}
				}
				if (methodInfo == null)
				{
					throw new InvalidOperationException(DataStrings.ExceptionCannotResolveOperation(operationName, typeof(T).Name, value2.GetType().Name));
				}
				object[] parameters2 = new object[]
				{
					value1.Value,
					value2
				};
				T result;
				try
				{
					result = (T)((object)methodInfo.Invoke(value1.Value, parameters2));
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				return result;
			}
		}

		public static implicit operator Unlimited<T>(T fromValue)
		{
			return new Unlimited<T>(fromValue);
		}

		public static explicit operator T(Unlimited<T> fromValue)
		{
			return fromValue.Value;
		}

		private const string unlimitedString = "Unlimited";

		private T limitedValue;

		private bool isUnlimited;

		private static Type[] ValidTypeMathOperations = new Type[]
		{
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(ByteQuantifiedSize)
		};
	}
}
