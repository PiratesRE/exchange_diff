using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class Unlimited<T> : IComparable, IComparable<Unlimited<T>>, IEquatable<Unlimited<T>> where T : struct, IComparable<T>, IEquatable<T>
	{
		public Unlimited()
		{
			this.value = null;
		}

		public Unlimited(T value)
		{
			this.value = new T?(value);
		}

		public bool IsUnlimited
		{
			get
			{
				return this.value == null;
			}
		}

		public T Value
		{
			get
			{
				return this.value.Value;
			}
		}

		public static bool operator !=(Unlimited<T> left, Unlimited<T> right)
		{
			return !(left == right);
		}

		public static bool operator ==(Unlimited<T> left, Unlimited<T> right)
		{
			if (object.ReferenceEquals(left, null))
			{
				return object.ReferenceEquals(right, null);
			}
			return left.Equals(right);
		}

		public static bool operator <(Unlimited<T> left, Unlimited<T> right)
		{
			return left.CompareTo(right) == -1;
		}

		public static bool operator >(Unlimited<T> left, Unlimited<T> right)
		{
			return left.CompareTo(right) == 1;
		}

		public static bool operator <=(Unlimited<T> left, Unlimited<T> right)
		{
			return left.CompareTo(right) != 1;
		}

		public static bool operator >=(Unlimited<T> left, Unlimited<T> right)
		{
			return left.CompareTo(right) != -1;
		}

		public override bool Equals(object other)
		{
			return other is Unlimited<T> && this.Equals((Unlimited<T>)other);
		}

		public bool Equals(Unlimited<T> other)
		{
			return !object.ReferenceEquals(other, null) && this.CompareTo(other) == 0;
		}

		public int CompareTo(object other)
		{
			if (object.ReferenceEquals(other, null))
			{
				throw new ArgumentException();
			}
			if (!(other is Unlimited<T>))
			{
				throw new ArgumentException();
			}
			return this.CompareTo((Unlimited<T>)other);
		}

		public int CompareTo(Unlimited<T> other)
		{
			if (object.ReferenceEquals(other, null))
			{
				throw new ArgumentException();
			}
			if (this.IsUnlimited)
			{
				if (other.IsUnlimited)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (other.IsUnlimited)
				{
					return -1;
				}
				T t = this.value.Value;
				return t.CompareTo(other.value.Value);
			}
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override string ToString()
		{
			if (this.IsUnlimited)
			{
				return "<Unlimited>";
			}
			T t = this.Value;
			return t.ToString();
		}

		private readonly T? value;

		public static Unlimited<T> UnlimitedValue = new Unlimited<T>();
	}
}
